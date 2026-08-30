# Brain Behaviour and Sub-Behaviour Architecture

## Purpose
Use this pattern when one `FOTLMonoBehaviour` owns a coherent system but has become too large or contains several distinct responsibilities.

> The **Brain Behaviour** owns and coordinates the system. **Sub-Behaviours** implement focused responsibilities owned by that Brain Behaviour.

These are architectural roles only. Concrete classes may use names such as `Controller`, `Handler`, `Manager`, or `Display` as appropriate.

---

## Structure
```text
Brain Behaviour
├── Sub-Behaviour A
├── Sub-Behaviour B
└── Independent Behaviour(s)
```

A Behaviour may be both a Sub-Behaviour of a higher-level Brain and the Brain of lower-level Sub-Behaviours. The role is contextual, not tied to a class name.

---

## Brain Behaviour
The Brain Behaviour is the authoritative owner and stable entry point of the system. It normally owns:
- shared runtime state;
- public APIs;
- high-level decisions and coordination;
- important events;
- Sub-Behaviour references;
- binding and unbinding.

External systems should normally communicate with the Brain Behaviour rather than directly with its Sub-Behaviours.

---

## Sub-Behaviour
A Sub-Behaviour is a `FOTLMonoBehaviour` whose responsibility belongs explicitly to one Brain Behaviour.

Use one when the responsibility:
- belongs clearly to one Brain;
- contains meaningful logic, state, references, or events;
- strongly depends on the Brain;
- should share its ownership lifecycle;
- is distinct enough to justify a separate component.

Binding should be explicit:
```text
Brain caches Sub-Behaviour
→ validates it
→ binds it
→ Sub-Behaviour receives Brain reference
```

A true Sub-Behaviour should normally not search for its Brain with `GetComponent`, `GetComponentInParent`, or scene lookup.

---

## Shared FOTL Base
Use:
```csharp
FOTLSubBehaviour<TBrainBehaviour>
```
where `TBrainBehaviour : FOTLMonoBehaviour`.

The base provides:
- Brain Behaviour reference;
- `IsBound`;
- `Bind(...)` / `Unbind(...)`;
- `EnsureBound()`;
- `OnBound(...)`;
- `OnUnbinding(...)`;
- `OnUnbound(...)`;
- cleanup through `UnbindInternalReferences()`.

Example:
```csharp
public abstract class PlayerCraftingSubHandlerBase
    : FOTLSubBehaviour<PlayerCraftingController>
{
    protected PlayerCraftingController CraftingController
        => this.BrainBehaviour;
}
```

Keep a system-specific Sub-Behaviour base only when it adds useful shared accessors or logic. Otherwise, concrete Sub-Behaviours may inherit `FOTLSubBehaviour<TBrainBehaviour>` directly.

---

## Binding Semantics
### `Bind`
`Bind(brainBehaviour)`:
1. rejects `null`;
2. does nothing when already bound to the same Brain;
3. cleanly unbinds the previous Brain when replacing it;
4. stores the new Brain reference;
5. calls `OnBound(...)`.

### `Unbind`
`Unbind(brainBehaviour)` only releases the relationship when the supplied instance is the currently bound Brain. This prevents an old Brain from clearing a newer binding.

### `EnsureBound`
Use when an API requires Brain access:
```csharp
if (!this.EnsureBound()) return;
```
Do not call it mechanically in methods that do not require the Brain.

---

## Binding Hooks
```csharp
OnBound(TBrainBehaviour brainBehaviour)
OnUnbinding(TBrainBehaviour brainBehaviour)
OnUnbound(TBrainBehaviour brainBehaviour)
```

Use them as follows:
- `OnBound`: subscribe to events or initialize Brain-dependent state.
- `OnUnbinding`: unsubscribe and perform cleanup while the Brain is still available.
- `OnUnbound`: reset local state after the Brain reference is cleared.

`OnUnbinding` is normally the main cleanup hook because event unsubscription often requires the old Brain instance.

---

## Lifecycle
Brain Behaviours use the normal `FOTLMonoBehaviour` lifecycle:
```text
CacheInternalReferences
→ ValidateInternalReferences
→ BindInternalReferences
→ InitializeInternalState
```

Typical binding:
```csharp
protected override void BindInternalReferences()
{
    base.BindInternalReferences();
    this.SubBehaviourA?.Bind(this);
    this.SubBehaviourB?.Bind(this);
}
```

Typical unbinding:
```csharp
protected override void UnbindInternalReferences()
{
    this.SubBehaviourA?.Unbind(this);
    this.SubBehaviourB?.Unbind(this);
    base.UnbindInternalReferences();
}
```

`FOTLSubBehaviour<TBrainBehaviour>` also clears its own active binding during `UnbindInternalReferences()`.

Keep Brain-side `Unbind(this)` calls for explicit ownership and deterministic teardown order.

---

## Nested Ownership
A Sub-Behaviour may itself own smaller Sub-Behaviours.
```text
Brain A
└── Behaviour B
    ├── Sub-Behaviour of Brain A
    └── Brain of Behaviour C and D
```
Each relationship is independent and should use its own typed binding.

---

## Independent Behaviours
A related component is not automatically a Sub-Behaviour. Keep it independent when it owns a separate Unity/component responsibility, such as:
- trigger receiver;
- hitbox;
- collision sensor;
- prompt trigger;
- animation event relay.

> Ownership and responsibility matter more than Transform hierarchy.

---

## State and Data Ownership
Shared authoritative state should remain on the Brain Behaviour:
```text
Profile / Shared Data
        ↓
Brain Behaviour
        ↓
Sub-Behaviours
```

Sub-Behaviours should not duplicate Brain state or shared Profile references unless necessary. They may own implementation-specific state such as cached visuals, temporary UI state, or component snapshots.

---

## Communication
Prefer:
- **Brain → Sub-Behaviour:** direct calls for delegated work;
- **Sub-Behaviour → Brain:** APIs through the bound reference;
- **Brain events:** when several Sub-Behaviours react independently;
- **External system → Brain:** normal public entry point.

Avoid exposing Sub-Behaviours publicly unless another system genuinely needs that specific component.

---

## References and Components
Use `[RequireComponent]` only when the required Sub-Behaviour must exist on the same GameObject.

Use serialized references and lifecycle caching when Sub-Behaviours may exist on child GameObjects.

The Brain remains responsible for determining which Sub-Behaviours it owns.

---

## Naming
Architecture documentation should use:
```text
Brain Behaviour
Sub-Behaviour
FOTLSubBehaviour<TBrainBehaviour>
```

Concrete names should follow each system's responsibility, for example:
```text
<System>Controller
<System>Handler
<System>Display
<System>Manager
<System>SubHandlerBase
```

Do not require `Controller` for Brains or `Handler` for Sub-Behaviours.

---

## Quick Rule
| Role | Main Question |
|---|---|
| **Brain Behaviour** | What should the system do, and what state does it own? |
| **Sub-Behaviour** | How is one Brain-owned responsibility implemented? |
| **Independent Behaviour** | Does this component own a separate Unity/component responsibility? |

---

## Guiding Principle
> **Use Brain Behaviour + Sub-Behaviour to express ownership and responsibility, not component proximity or naming convention.**

Keep the Brain authoritative, keep Sub-Behaviours focused, and centralize their common binding lifecycle in `FOTLSubBehaviour<TBrainBehaviour>`.
