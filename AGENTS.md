# Repository Engineering Guide

## 1. Project overview

`FlameOfTheLost` is a Unity 6 2D game project. The current editor version is `6000.3.21f1` (`ProjectSettings/ProjectVersion.txt`). Project-owned C# code uses the `FlameOfTheLost` namespace and is primarily component-oriented: ScriptableObject assets hold authoring data, MonoBehaviours coordinate scene/runtime behavior, and small serializable types carry configuration or transient operation data.

Before changing a subsystem, inspect its neighboring scripts, profiles, prefabs/data assets, and any matching document under `Documents/`. The codebase is evolving; repeated current patterns take precedence over older or isolated examples.

## 2. Repository and Unity structure

- `Assets/Resources/Scripts/Foundation/`: shared bases, extensions, collider records, and serializable dictionaries.
- `Assets/Resources/Scripts/Gameplay/`: character, combat, inventory, level, spawning, environment, and shared gameplay code.
- `Assets/Resources/Scripts/UI/`: HUD, menus, interaction panels, and shared UI components.
- `Assets/Resources/Scripts/Editor/`: editor-only tooling. Do not introduce `UnityEditor` dependencies into runtime folders.
- `Assets/Resources/Scripts/Tests/`: test location; currently no committed C# test suite.
- `Assets/Resources/Data/`: ScriptableObject assets, grouped by gameplay domain. Other project assets and prefabs are also under `Assets/Resources/`.
- `Documents/Architectures/` and `Documents/Conventions/`: project-specific architectural and naming rules.
- `Packages/` and `ProjectSettings/`: Unity dependency and project configuration.

Treat `Library/`, `Logs/`, `Temp/`, `obj/`, `ProfilerCaptures/`, generated solution/project files, and other Unity output as generated. Preserve `.meta` files and move/rename Unity assets with their metadata (preferably through Unity); never edit or rename a `.meta` file independently.

Within a feature, follow the existing role folders such as `Behaviours`, `Data`, `Entries`, `Records`, `Contexts`, `Runtime`, `Interfaces`, `Categories`, and `Utilities`. Keep a new type close to the subsystem that owns it rather than creating a new cross-project abstraction prematurely.

## 3. Core architecture

- Prefer profile-backed configuration plus component-owned runtime state. Profiles define reusable/archetypal content; controllers and handlers hold per-instance mutable state.
- Controllers are the stable public entry points and authoritative owners of a system's shared state and events. Handlers implement a focused responsibility. Managers coordinate wider UI/application groupings; utilities are stateless helpers.
- Character components commonly derive from `CharacterBehaviourControllerBase`, which caches the parent `CharacterRootController`. Use the root's exposed component graph instead of repeating broad hierarchy searches.
- Use events for independent reactions to state changes and direct calls when sequencing or a return value matters. Subscribe and unsubscribe symmetrically.
- Preserve subsystem boundaries. A related component with its own collision, trigger, animation-event, or other Unity boundary can remain independent rather than becoming a sub-handler.

## 4. `FOTLMonoBehaviour` lifecycle

Use `FOTLMonoBehaviour` for project components that need the repository lifecycle. Its order is:

```text
Awake:
  CacheInternalReferences
  ValidateInternalReferences
  BindInternalReferences
  InitializeInternalState

Start:
  CacheExternalReferences
  ValidateExternalReferences
  BindExternalReferences
  InitializePregameplayState
  InitializeGameplay

OnDestroy:
  CleanupStates
  UnbindInternalReferences
```

- Override the narrow hook that matches the work; do not add parallel `Awake`, `Start`, or `OnDestroy` logic when a lifecycle hook is suitable.
- When overriding lifecycle hooks, call `base` in the established order. Setup hooks normally call `base` first. Cleanup/unbinding overrides normally release derived state/references first, then call `base`.
- `Awake` is for references within the component/object hierarchy, binding owned collaborators, and local state. `Start` is for dependencies outside that ownership boundary, resource lookup, and state that must wait until other objects have awakened.
- `InitializePregameplayState` prepares initial presentation/state before play; `InitializeGameplay` starts active behavior. Use `CleanupStates` for ending sessions/resetting transient state and `UnbindInternalReferences` for event and Brain bindings.
- Some legacy/plain `MonoBehaviour` components manage Unity messages directly. Match the active base class; do not partially adopt the FOTL lifecycle without migrating the whole component safely.

## 5. Profiles and ScriptableObjects

- `FOTLScriptableObjectProfile` supplies serialized identity (`Id`, `Key`, `DisplayName`, `Description`) and `Icon` through `IIdentityProvider` and `IIconProvider`.
- `FOTLWorldObjectProfile` adds initial root/visual transform configuration and optional initial icon configuration.
- `FOTLWorldObjectProfileBehaviour<TProfile>` owns a serialized profile reference, forwards identity/icon values, caches conventional `Visual` references, validates the profile, and applies configured initial visual state.
- Not every current profile derives from `FOTLScriptableObjectProfile`: `ItemProfile`, `SkillProfile`, `StatusEffectProfile`, and `CharacterIdentityProfile` currently derive directly from `ScriptableObject` and implement the applicable provider interfaces. Follow the inheritance convention of the subsystem being edited; do not perform unrelated profile migrations.
- Keep authoring configuration serialized on profiles/config assets. Keep mutable instance state on controllers or explicit `Runtime*` objects. Do not mutate shared profile assets during gameplay.
- Expose serialized collections as read-only views where the surrounding code does so. Use `OnValidate` only for authoring-time invariants and guard editor-only implementations with `#if UNITY_EDITOR` when appropriate.
- Match neighboring `CreateAssetMenu` paths and filename prefixes; the project has feature-specific asset abbreviations rather than one universal profile filename format.

## 6. Brain Controller and Sub-Handler pattern

Use `Documents/Architectures/BrainControllerSubHandlerArchitecture.md` when splitting a component with several Brain-owned responsibilities.

- The Brain (`<System>Controller`) owns public API, authoritative shared runtime state, important events, handler references, and high-level decisions.
- A true sub-handler (`<System><Responsibility>Handler`) receives its Brain explicitly through `Bind(this)`; it should not find its Brain with `GetComponent`.
- When multiple handlers share a Brain, use a `<System>SubHandlerBase` consistent with current bases: private Brain reference, protected access (and optional profile forwarding), `IsBound`, guarded `Bind`/`Unbind`, `EnsureBound`, and binding hooks such as `OnBound`, `OnUnbinding`, and `OnUnbound`.
- The Brain caches and validates handler components, binds them in `BindInternalReferences`, and unbinds them in `UnbindInternalReferences`. Handlers must remove event subscriptions while unbinding.
- External callers normally call the Brain, not its sub-handlers. Do not duplicate Brain-owned profile or state in handlers; handler-local implementation state is acceptable.
- Use `[RequireComponent]` only for genuinely required components on the same GameObject. Transform proximity alone does not establish Brain ownership.

## 7. Reference, binding, validation, and initialization rules

- Serialized references are the authoring surface. In cache hooks, retain assigned values and fill missing references with the narrowest appropriate lookup (`GetComponent`, `GetComponentInChildren(..., true)`, `GetComponentInParent`, or a known child name).
- Internal references belong to the component/root/sub-handler ownership graph. External references cross that boundary or depend on another object's `Awake` having run.
- Validation reports missing required dependencies with `Debug.LogError(..., this)`; optional/degraded behavior may use a warning. Validate after caching, not before.
- Binding means establishing ownership or event subscriptions, not locating references. Keep binding idempotent, reject null, handle replacement deliberately, and unbind symmetrically.
- Initialize runtime fields only after their dependencies have been cached, validated, and bound. Preserve intentional inspector-seeded/debug runtime values where the subsystem explicitly supports them.
- Prefer null-safe failure and `Try*` APIs for expected gameplay rejection. Do not silently hide a required-reference configuration error.

## 8. Naming and code style

- Use 4-space indentation, braces on new lines, PascalCase for types/methods/properties/events, `_camelCase` for private fields (including serialized fields), and the `FlameOfTheLost` namespace.
- Current code commonly exposes private backing fields through properties and qualifies instance access with `this.`; match the surrounding file.
- Use role-bearing suffixes consistently: `Controller`, `Handler`, `SubHandlerBase`, `Profile`, `Config`, `Provider`, `Manager`, and `Utility`.
- Place enums in `Categories`, interfaces in `Interfaces`, ScriptableObjects in `Data`, and serializable supporting types in their established role folders.
- `Documents/Conventions/AssetNamingConvention.md` requires source prefixes for third-party assets (for example `BGE118_`, `CFRPG16_`, `SFUI_`, `CPRIPA_`, or `BAPQ3UI_`) and specifies `FTL_` for general project-created assets. Current ScriptableObject data assets instead consistently use subsystem abbreviations such as `CId_`, `WP_`, `SKM_`, `SKR_`, `SE_`, `Trn_`, and `TutPTrg_`, matching their `CreateAssetMenu` defaults. Follow the current asset family for Data assets; do not mass-rename existing content to reconcile the document. Preserve package-owned names under `Packages/` and do not edit package content directly; see `PackageNamingConvention.md`.

## 9. Interfaces and data structure conventions

- Use interfaces to decouple capability suppliers and consumers: `I...Provider` exposes data/behavior, while `I...Consumer` receives or applies a capability. Inspector-configured interface references are commonly serialized as `MonoBehaviour` and cast through a typed property; validate that the assigned behavior implements the interface.
- Prefer capability interfaces over concrete dependencies when multiple implementations already exist (for example aiming, facing, identity/icon, crafting recipes, and status-effect consumers).
- `Entry` is generally a serializable authored element in a profile/list or a mutable collection item. `Record` is a compact result, snapshot, reservation, or grouped value - often a `readonly struct`, though collider configuration records are serializable classes. `Context` carries data for one operation/event and is commonly immutable. Follow the local representation rather than assuming a suffix mandates class versus struct.
- `Runtime*` types combine profile references with per-instance/transient state. Their runtime IDs are instance-local and are distinct from stable profile Data IDs.
- Return `IReadOnlyList`/`IReadOnlyDictionary` or empty collections from read APIs where callers should not own storage. Use repository serializable dictionary subclasses for inspector-visible maps.

## 10. Data ID conventions

The intended allocation rules are under `Documents/Conventions/DataIDConvention/`. Before assigning an ID, read `DataIDConvention.md` and the relevant category document, then inspect the serialized `_id` values in the current assets. Do not assume the documents and assets are synchronized.

- IDs are integer identifiers owned by project ScriptableObject content, not independent runtime MonoBehaviour instance IDs.
- Keep assigned IDs stable and unique; never reuse an ID for unrelated content.
- `00000` means none/unassigned/invalid and is not valid content; `-1` is runtime-only invalid.
- The documented major ranges are `1xxxx` characters, `2xxxx` spawning, `3xxxx` environment, `4xxxx` tutorial, `5xxxx` status effects, `7xxxx` items, and `8xxxx` skills; category documents define narrower ranges.
- Current data contains known departures from those documents: character identity assets use four-digit `1000`/`1001` values; terrain, skill, and consumable status effects currently occupy `510xx`, `52xxx`, and `530xx`; and at least one projectile uses `731xx`. Never silently renumber these referenced assets or choose a new ID from documentation alone. Verify uniqueness across `Assets/Resources/Data`, preserve existing IDs, and surface a documentation/allocation conflict before adding content in an ambiguous range.
- Runtime IDs such as `RuntimeStatusEffect.RuntimeID` are separate counters and must not be treated as profile IDs.

## 11. Unity-specific implementation rules

- Use the project's current Unity version and APIs. Do not hand-edit scene, prefab, or asset YAML unless the task specifically requires it and references can be preserved safely.
- Preserve serialized field names during refactors or add `FormerlySerializedAs` when a rename is intentional; otherwise existing scene/prefab/asset data can be lost.
- Keep runtime code free of editor-only APIs. Keep package dependencies in `Packages/manifest.json`; do not copy or modify cached/generated package contents.
- Avoid broad `Resources.Load` additions when a serialized reference or existing profile relationship fits the subsystem. When an established resource path is used, verify the asset lives beneath `Assets/Resources/` at that exact path.
- Do not rename assets merely to normalize old naming. Follow the documented prefix registry for new assets and preserve working references.

## 12. Code modification guidelines

- Scope changes to the requested feature and preserve unrelated user changes. Inspect dependent controllers, handlers, interfaces, profiles, and serialized assets before changing public APIs or fields.
- Extend the current subsystem's pattern instead of introducing a second lifecycle, identity store, service locator, or duplicated state owner.
- When adding a Brain responsibility, decide explicitly whether it belongs in the Brain, a bound sub-handler, or an independent Unity behavior.
- Keep runtime state out of reusable ScriptableObject profiles. When copying authored entries into mutable runtime collections, create explicit runtime copies where the subsystem expects isolation.
- Maintain symmetric event/binding cleanup and stable serialization. Check affected prefabs/scenes when adding required components or serialized fields.
- Do not modify source prefixes, Data IDs, package files, or asset metadata as incidental cleanup.

## 13. Completion checks

Before considering a task complete:

1. Review the diff and confirm only intended files changed; check for accidental `.meta`, scene, prefab, project setting, or generated-file changes.
2. Re-read representative base and neighboring types to ensure lifecycle order, Brain ownership, interface contracts, naming, and profile/runtime separation remain consistent.
3. Run `dotnet build Assembly-CSharp.csproj` for runtime C# changes and `dotnet build Assembly-CSharp-Editor.csproj` for editor changes. Treat Unity-generated project files as checks, not hand-maintained sources.
4. Run relevant tests through Unity Test Runner. For suitable new pure logic, add clear `*Tests.cs` Edit Mode tests under `Assets/Resources/Scripts/Tests`; the repository currently has no committed suite, so do not claim test coverage that does not exist.
5. For scene/prefab/serialization changes, open the project in Unity `6000.3.21f1`, allow import/compilation, inspect the Console, and exercise the affected flow. Verify required serialized references and `Resources` paths.
6. For new/changed ScriptableObject content, validate identity fields, Data ID range/uniqueness, `CreateAssetMenu` conventions, and profile references.
7. Report what was validated and any checks that could not be run.
