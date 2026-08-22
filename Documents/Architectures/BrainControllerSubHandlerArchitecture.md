# Brain Controller and Sub-Handler Architecture

## Purpose

Use this pattern when one `FOTLMonoBehaviour` becomes too large or handles several clear responsibilities.

> The **Brain Controller** owns the system. **Sub-Handlers** implement focused parts of it.

---

## Structure

```text
Brain Controller
├── Sub-Handler A
├── Sub-Handler B
└── Independent Behaviours
```

### Brain Controller

Owns:

- main runtime state;
- public API;
- high-level decisions;
- important events;
- Sub-Handler references;
- binding/unbinding.

It should coordinate responsibilities, not implement every detail.

### Sub-Handler

Use when a responsibility:

- belongs clearly to one Brain;
- has meaningful logic, state, references, or event handling;
- depends strongly on the Brain;
- should share the Brain's lifecycle.

Sub-Handlers should receive the Brain through explicit binding.

```text
Brain finds Handler
→ Brain binds Handler
→ Handler receives Brain
```

Avoid having true Sub-Handlers search for their Brain with `GetComponent`.

---

## Independent Behaviours

A related behaviour is not automatically a Sub-Handler.

Keep it independent when it has its own Unity/component responsibility, such as:

- trigger receiver;
- hitbox;
- collision sensor;
- prompt trigger;
- animation event relay.

Example:

```text
Harvestable Object
├── HarvestableObjectController
├── HarvestableBrokenStateHandler
├── HarvestableDropHandler
└── DamageReceiver
    └── HarvestableDamageReceivingHandler
```

`HarvestableDamageReceivingHandler` remains independent because it belongs to the dedicated trigger object.

> Ownership and responsibility matter more than Transform hierarchy.

---

## Shared Base

When several handlers belong to one Brain, use a shared base such as:

```text
HarvestableObjectSubHandlerBase
PlayerInteractionSubHandlerBase
StatusEffectSubHandlerBase
```

It should normally provide:

- Brain reference;
- `Bind` / `Unbind`;
- `IsBound` / `EnsureBound`;
- optional Profile access;
- binding hooks.

---

## State and Profile Ownership

The Brain should remain the authoritative owner of shared runtime state.

```text
Profile
  ↓
Brain
  ↓
Sub-Handlers
```

Sub-Handlers should not duplicate Brain state or Profile references unless necessary.

They may own implementation-only state such as cached visuals, collider snapshots, or temporary UI state.

---

## Communication

External systems should normally call the Brain, not its Sub-Handlers.

Use:

- direct calls when order/results matter;
- events when handlers independently react to Brain state changes.

---

## Lifecycle

Bind handlers through the existing lifecycle:

```text
CacheInternalReferences
→ ValidateInternalReferences
→ BindInternalReferences
→ InitializeInternalState
```

Use `[RequireComponent]` only for required components on the same GameObject.

---

## Quick Rule

| Type | Main Question |
|---|---|
| **Brain** | What should happen? |
| **Sub-Handler** | How is one Brain-owned responsibility performed? |
| **Independent Behaviour** | Does this component have its own Unity/component boundary? |

---

## Naming

```text
<System>Controller
<System><Responsibility>Handler
<System>SubHandlerBase
```

Example:

```text
HarvestableObjectController
HarvestableBrokenStateHandler
HarvestableDropHandler
HarvestableObjectSubHandlerBase
```

---

## Guiding Principle

> **Use Brain + Sub-Handler to express ownership and responsibility, not just component proximity.**

Keep the Brain as the stable public entry point and split only responsibilities that are large enough to justify their own handler.
