# Character Composition Architecture
## 1. Purpose
This document summarizes the current Character Composition architecture in **FlameOfTheLost**.
The Composition layer defines how a Character is assembled at the top level.
It connects Character Profiles, RootControllers, shared/specialized systems, config ownership, and automatic config caching.
The main goal is to keep the hierarchy strongly typed while preserving simple shared runtime APIs.
## 2. Character Hierarchy
```text
Character
├── Player
└── CharacterAI
    ├── Enemy
    └── Ally [future]
```
`CharacterAI` is an intermediate category for behavior shared by AI-controlled Characters.
Enemy and future Ally implementations specialize this layer.
## 3. Profile Hierarchy
```text
CharacterProfile
├── PlayerCharacterProfile
└── CharacterAIProfile
    ├── EnemyCharacterProfile
    └── AllyCharacterProfile [future]
```
Shared Character data belongs in `CharacterProfile`.
AI-wide data belongs in `CharacterAIProfile`.
Enemy-only data belongs in `EnemyCharacterProfile`.
Player-only data belongs in `PlayerCharacterProfile`.
This prevents specialized data from leaking into more general Character categories.
## 4. CharacterProfile
`CharacterProfile` is the common Profile for every Character.
It currently owns Character classification, faction, traversal type, and the following shared configs:
- `CharacterMovementConfig`, `CharacterDodgeConfig`, `CharacterVisualConfig`.
- `CharacterCombatConfig`, `StatusEffectGuardConfig`, `CharacterProgressionConfig`.
A config belongs here only when it is valid as general Character composition data.
## 5. CharacterAIProfile
`CharacterAIProfile` inherits from `CharacterProfile`.
It is intentionally empty for now.
Its current role is to establish a clear Profile boundary between general Characters and AI Characters.
AI-specific fields should be added only when genuine shared AI configuration appears.
## 6. PlayerCharacterProfile
`PlayerCharacterProfile` directly inherits from `CharacterProfile`.
It currently does not require additional Player-only fields.
It remains the extension point for future Player-specific configuration.
## 7. EnemyCharacterProfile
`EnemyCharacterProfile` inherits from `CharacterAIProfile`.
Enemy-only data belongs here.
Currently this includes `CharacterRewardDropProfile`.
Reward Drop is therefore currently owned by Enemy composition.
The Reward Drop classes keep the `Character` prefix because the feature may later be reused by other Character categories.
## 8. RootController Hierarchy
```text
CharacterRootController
├── CharacterRootController<TProfile>
│   └── PlayerRootController
└── CharacterAIRootController
    └── CharacterAIRootController<TProfile>
        └── EnemyRootController
```
Generic classes provide strongly typed Profiles.
Non-generic classes provide convenient runtime references to general Character or Character AI types.
## 9. CharacterRootController
`CharacterRootController` is the common runtime root for every Character.
It exposes `public abstract CharacterProfile Profile { get; }`.
Systems that only care about a general Character can depend on `CharacterRootController`.
The root also exposes Profile-backed identity data: `Id`, `Key`, `DisplayName`, `Icon`, `CharacterType`, `Faction`, and `TraversalType`.
## 10. Shared Runtime Composition
`CharacterRootController` caches and exposes shared Character references.
Current shared systems include:
- Rigidbody/main collider, Refresh, Input, Movement, Dodge, and Terrain Contact.
- Visual/VFX, Combat, Combat Loadout, aiming, resources, Damage, and Knockback.
- HP/FP/Stamina, Death, Status Effects, Progression, Reward Wallet, and Inventory.
The root acts as the central composition point for the Character GameObject.
## 11. Generic CharacterRootController
The generic form owns the strongly typed serialized Profile.
```csharp
CharacterRootController<TProfile>
    where TProfile : CharacterProfile
```
It exposes `TProfile TypedProfile`.
It maps the common API back to the same object using a sealed override of `Profile`.
This prevents downstream classes from making typed and untyped Profile access point to different assets.
## 12. Typed Profile Access
The hierarchy supports several access levels:
```csharp
characterRoot.Profile
playerRoot.PlayerProfile
characterAIRoot.AIProfile
enemyRoot.EnemyProfile
```
Each accessor exposes the Profile at the appropriate inheritance level.
All accessors refer to the same serialized Profile object for that Character.
## 13. PlayerRootController
`PlayerRootController` specializes `CharacterRootController<PlayerCharacterProfile>`.
It exposes `PlayerCharacterProfile PlayerProfile`.
Player-only runtime references stay here.
Current examples include `PlayerInput`, Player Carry Loadout, Player Interaction, Player Crafting, and typed `PlayerInputHandler`.
This keeps Player dependencies out of the shared Character layer.
## 14. CharacterAIRootController
Some systems need a reference to any AI Character without knowing the concrete AI subtype.
For that reason, a non-generic `CharacterAIRootController` remains a usable runtime type.
Shared code can safely test:
```csharp
CharacterRootController is CharacterAIRootController
```
This is used by systems such as target-based combat aiming.
It avoids requiring callers to know a generic Profile type argument.
## 15. Generic CharacterAIRootController
The generic AI layer provides strongly typed AI Profile ownership.
```csharp
CharacterAIRootController<TProfile>
    where TProfile : CharacterAIProfile
```
This guarantees that an AI RootController uses `CharacterAIProfile` or one of its subclasses.
Enemy therefore uses `EnemyCharacterProfile`.
A future Ally can use `AllyCharacterProfile`.
## 16. Character AI Shared Composition
AI-wide runtime references belong in `CharacterAIRootController`.
The current important shared reference is `CharacterAITargetDetectionHandler`.
Enemy-only systems should not be placed at this level.
This prevents future Allies from automatically inheriting Enemy-specific behavior.
## 17. EnemyRootController
`EnemyRootController` specializes `CharacterAIRootController<EnemyCharacterProfile>`.
It exposes `EnemyCharacterProfile EnemyProfile`.
Enemy-only runtime references belong here.
Current examples include `EnemyBrainController`, `CharacterRewardDropProvider`, and typed `EnemyInputHandler`.
## 18. Reward Drop Ownership
Reward Drop is currently Enemy-only.
```text
EnemyCharacterProfile
    └── CharacterRewardDropProfile
EnemyRootController
    └── CharacterRewardDropProvider
```
`CharacterProfile` does not own the Reward Drop Profile.
`CharacterAIRootController` does not own the Reward Drop Provider.
This prevents future Ally or other AI Characters from inheriting Enemy reward behavior.
## 19. CharacterConfiguredBehaviourBase
Configured Character systems derive from `CharacterConfiguredBehaviourBase<TConfig>`.
`TConfig` must inherit from `CharacterDataAsset`.
The base owns the serialized config reference and exposes `TConfig Config`.
It also provides `CacheConfigIfNull(TConfig config)`.
This establishes the common Profile-to-behaviour config caching pattern.
## 20. Config Caching Rule
Profiles are the composition-level source of Character configs.
The RootController reads configs from its Profile.
It supplies them to corresponding configured behaviours only when their config is currently null.
```text
Profile
    ↓
RootController
    ↓
Configured Behaviour
```
Directly assigned behaviour configs are therefore preserved.
## 21. Shared Character Config Caching
`CharacterRootController.CacheCharacterConfigs()` handles Character-wide configs.
It currently caches:
- Movement → `MovementHandler`; Dodge → `CharacterDodgeController`; Visual → `CharacterVisualController`.
- Combat → `CharacterCombatController`; Status Effect Guard → `StatusEffectController`; Progression → `CharacterProgressionController`.
This method should remain limited to data owned by `CharacterProfile`.
## 22. Specialized Config Caching
Specialized Profile data should be cached by the corresponding specialized RootController.
Example:
```text
EnemyCharacterProfile.RewardDropProfile
    ↓
EnemyRootController
    ↓
CharacterRewardDropProvider
```
Future AI-wide configs should be handled at the Character AI level.
Future Player-only configs should be handled by `PlayerRootController`.
Future Ally-only configs should be handled by `AllyRootController`.
## 23. Dodge Config Integration
Dodge now follows the same configured-system architecture as the other major Character systems.
`CharacterProfile` owns `CharacterDodgeConfig`.
`CharacterRootController` caches it into `CharacterDodgeController`.
`CharacterDodgeController` derives from `CharacterConfiguredBehaviourBase<CharacterDodgeConfig>`.
This keeps Dodge configuration centralized in the Character Profile.
## 24. Validation
`CharacterRootController` validates that a Character Profile is assigned.
`CharacterConfiguredBehaviourBase<TConfig>` validates that its required config exists.
Specialized controllers validate specialized runtime references.
Player validates Player-only components, Character AI validates AI-only dependencies such as Target Detection, and Enemy validates Enemy-only dependencies.
Validation should stay at the narrowest layer that owns the dependency.
## 25. Composition Folder Convention
RootControllers and Character Profiles are stored under `Composition`.
The name `Composition` is intentionally retained.
`Shared` already means code shared between sibling Character categories.
`Common` is already used inside several subsystems for shared internal implementations.
`Composition` specifically describes top-level Character assembly, ownership, and Profile wiring.
## 26. Folder Examples
```text
Character/Shared/Composition/
├── Behaviours/
└── Data/
Character/AI/Shared/Composition/
├── Behaviours/
└── Data/
Character/AI/Enemy/Shared/Composition/
├── Behaviours/
└── Data/
Character/Player/Composition/
├── Behaviours/
└── Data/
```
## 27. Placement Guidelines
Use `Composition/Behaviours` for top-level runtime composition classes.
Typical examples are RootControllers, generic RootController infrastructure, and composition-related Character behaviour bases.
Use `Composition/Data` for Profile and composition-level data classes such as `CharacterProfile`, `CharacterAIProfile`, `PlayerCharacterProfile`, and `EnemyCharacterProfile`.
Subsystem-specific configs should remain inside their own subsystem folders.
## 28. Design Principles
1. General data belongs to the most general valid Profile.
2. Specialized data belongs only to the specialization that owns it.
3. RootController inheritance mirrors Profile inheritance.
4. Generic RootControllers provide strongly typed Profiles.
5. Non-generic RootControllers provide shared runtime APIs.
6. Profiles are the composition-level source of Character configs.
7. RootControllers cache Profile configs into configured behaviours.
8. Automatic caching only fills null config references.
9. Player-, AI-, and Enemy-specific dependencies remain isolated.
10. Enemy-only features do not leak into Character AI.
11. Intermediate hierarchy levels may remain empty until genuine shared data appears.
12. Composition folders group classes by architectural ownership.
## 29. Future Ally Extension
The architecture is already prepared for Ally support.
Expected Profile inheritance:
```text
CharacterAIProfile
    ↓
AllyCharacterProfile
```
Expected runtime inheritance:
```text
CharacterAIRootController<AllyCharacterProfile>
    ↓
AllyRootController
```
AI-wide systems remain reusable through `CharacterAIRootController`.
Ally-only data and references remain isolated at the Ally level.
No Enemy-specific assumptions need to be inherited.
## 30. Summary
The Character Composition system now uses layered ownership.
Profiles define configuration ownership; RootControllers define runtime composition ownership.
Configured behaviours receive configs through the appropriate RootController.
Generic layers provide strong typing while non-generic layers preserve convenient shared references.
The resulting architecture is centralized, extensible, and consistent with the broader Character system design in **FlameOfTheLost**.
