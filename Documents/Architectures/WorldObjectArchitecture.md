World Object Architecture
**Project:** FlameOfTheLost  
**Scope:** Standard profile–behaviour pairing for reusable world objects.
---
# 1. Purpose
World objects commonly use:
```text
FOTLWorldObjectProfileBehaviour<TProfile>
        ↕
FOTLWorldObjectProfile
```
The pattern separates reusable data/configuration from scene-instance behaviour and runtime state.
Use it for project-owned world objects with a stable data definition.
---
# 2. Inheritance Structure
```text
FOTLScriptableObjectProfile
└─ FOTLWorldObjectProfile
   └─ ConcreteWorldObjectProfile
FOTLMonoBehaviour
└─ FOTLWorldObjectProfileBehaviour<TProfile>
   └─ ConcreteWorldObjectController
```
Example: `HarvestableObjectProfile` ↔ `HarvestableObjectController`.
---
# 3. Base Profile Responsibility
`FOTLWorldObjectProfile` is the shared data base for world-object profiles.
It provides:
- ID, Key, Display Name, Description
- Icon
- Optional initial root transform
- Optional initial visual transform
- Optional initial icon configuration
The profile represents the reusable definition of a world-object archetype.
---
# 4. Base Behaviour Responsibility
`FOTLWorldObjectProfileBehaviour<TProfile>` connects a scene object to its profile.
It is responsible for:
- Storing the concrete profile reference
- Forwarding identity/display data
- Resolving shared visual references
- Validating the profile reference
- Applying shared initial visual configuration
Concrete controllers should inherit it instead of inheriting `FOTLMonoBehaviour` directly.
---
# 5. Concrete Profile Responsibility
A concrete profile contains stable configuration specific to one world-object type.
```text
HarvestableObjectProfile
- Maximum durability
- Accepted impacts
- Broken-state configuration
- Drop configuration
CollectibleItemProfile
- Item
- Quantity
- Interaction priority
- Collection behaviour
```
Do not store mutable runtime state in ScriptableObject profiles.
---
# 6. Concrete Controller Responsibility
A concrete controller owns behaviour and per-instance runtime state.
Typical responsibilities:
- Runtime state
- Public gameplay APIs
- Gameplay events
- Interaction logic
- Sub-behaviour coordination
- Lifecycle setup
- Controller-specific validation
Examples: `CurrentDurability`, `IsBroken`, `IsCollected`.
---
# 7. Profile vs Runtime Rule
```text
Shared by every instance using the profile
→ Profile
Changes during gameplay or differs per instance
→ Controller / runtime state
```
Examples:
```text
MaximumDurability → Profile
CurrentDurability → Controller
Quantity          → Profile
IsCollected       → Controller
```
---
# 8. Shared Profiles
Multiple scene objects may reference one profile.
```text
ConcreteWorldObjectProfile
        ↑
   ┌────┼────┐
Object A Object B Object C
```
Each instance keeps independent runtime state.
Profiles must not contain mutable scene-instance state.
---
# 9. Transform and Visual Configuration
Use profile-driven root transforms only when the transform belongs to the archetype definition.
For independently placed reusable instances:
```text
ConfigureInitialTransformAtStart = false
```
Root position should normally come from the prefab or scene.
Visual-local transform configuration may still be profile-driven.
The world-object profile owns its own icon.
This keeps world representation independent from referenced gameplay data.
Example: `ItemProfile.Icon` does not have to equal `CollectibleItemProfile.Icon`.
Concrete controllers should avoid duplicating visual setup handled by the base behaviour.
---
# 10. Lifecycle Integration
The architecture follows `FOTLMonoBehaviour`.
```text
Awake
├─ CacheInternalReferences
├─ ValidateInternalReferences
├─ BindInternalReferences
└─ InitializeInternalState
Start
├─ CacheExternalReferences
├─ ValidateExternalReferences
├─ BindExternalReferences
├─ InitializePregameplayState
└─ InitializeGameplay
```
Concrete controllers extend lifecycle methods and call `base`.
Shared world-object setup should remain in the base behaviour.
---
# 11. Validation Ownership
Base behaviour validates:
- Profile reference
- Shared world-object references
Concrete controller validates:
- Required profile-owned gameplay data
- Controller-specific references
- Required sub-behaviours
Avoid validating the same responsibility in multiple layers.
---
# 12. Brain + Sub-Behaviour Integration
The profile pairing works with the project's Brain + Sub-Behaviour architecture.
```text
ConcreteWorldObjectController
├─ SubBehaviourA
├─ SubBehaviourB
└─ SubBehaviourC
```
The controller owns the profile.
Sub-behaviours should normally access configuration through their bound controller.
Avoid duplicate profile references in sub-behaviours.
---
# 13. Typical Uses
Suitable systems include:
```text
Crafting Stations
Harvestable Objects
Tutorial Objects
Item Containers
Collectible Items
Spawn Points
Checkpoints
Other reusable environment objects
```
The concrete profile and controller remain system-specific.
---
# 14. When Not to Use It
Do not create a profile pair for every helper component.
The pattern is most useful when an object has:
- Stable project-owned data
- Reusable configuration
- Multiple instances
- Data identity or persistence relevance
- Shared world-object visual/configuration behaviour
Runtime-only helper components normally do not need their own profile.
---
# 15. Implementation Template
```text
ConcreteWorldObjectProfile : FOTLWorldObjectProfile
ConcreteWorldObjectController
    : FOTLWorldObjectProfileBehaviour<ConcreteWorldObjectProfile>
```
Responsibility summary:
```text
Definition / configuration → Profile
Runtime / instance state   → Controller
Shared world-object setup  → Base behaviour
Specialized behaviour      → Controller / sub-behaviours
```
---
# 16. Core Principle
The profile defines **what the world object is**.
The controller defines **what the current instance is doing**.
`FOTLWorldObjectProfileBehaviour<TProfile>` connects these layers and centralizes behaviour shared across world-object systems.
---
**End of Document**
