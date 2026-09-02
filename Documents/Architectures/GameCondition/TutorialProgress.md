# Tutorial Progress
## Purpose
`TutorialProgressController` converts tutorial-relevant gameplay state and events into shared Game Conditions.
It does not directly control Tutorial Prompt Triggers.
Flow:
```text
Gameplay State / Events
        ↓
TutorialProgressController
        ↓
GameConditionController
```
Consumers evaluate the stored conditions independently.
## Current Conditions
### Equipment
```text
PlayerHasMultipleWeapons
PlayerReadyToCycleWeapons
```
### Combat
```text
PlayerHitByFirstEnemy
FirstEnemyDefeated
```
## Main References
Tutorial Progress uses:
```text
GameConditionController
Player CharacterRootController
Preferred EnemyGroupSpawnerController
```
The preferred enemy spawner is a configuration preference.
Runtime first-enemy references are resolved separately.
## First Enemy Model
The controller stores:
```text
FirstEnemySpawner
FirstEnemySpawnEntry
FirstEnemy
```
`FirstEnemySpawner` is the selected general `EnemyGroupSpawnerController`.
`FirstEnemySpawnEntry` is the first valid entry of that spawner.
`FirstEnemy` is the current runtime `CharacterRootController` produced from that entry.
The spawn entry identifies the tutorial's first enemy slot.
The character reference represents only its current runtime instance.
## First Enemy Source Resolution
Resolution order:
```text
PreferredFirstEnemySpawner
        ↓
Valid first spawn entry?
   yes → use it
   no  → search other EnemyGroupSpawnerController instances
        ↓
Use first spawner with a valid first entry
```
The preferred reference should normally be configured explicitly.
Fallback search is recovery behavior.
## Spawner Abstraction
Tutorial Progress depends on:
```text
EnemyGroupSpawnerController
EnemySpawnEntry
```
It does not depend on territorial-specific spawner or spawn-entry types.
This keeps tutorial tracking independent from a specific enemy AI implementation.
## Existing Enemy Binding
Initial enemies may spawn before Tutorial Progress subscribes to spawn events.
The controller therefore uses both:
```text
EnemySpawned event
TryGetFirstSpawnedEnemy(...)
```
Binding flow:
```text
Subscribe to EnemySpawned
        ↓
TryBindExistingFirstEnemy
        ↓
Bind existing runtime enemy if already spawned
```
This avoids relying on Unity `Start()` ordering.
## Spawn Event Handling
A spawned character is accepted only when:
```text
SpawnEntry == FirstEnemySpawnEntry
Character != null
FirstEnemy == null
```
Enemies from other spawn entries are ignored.
Additional enemies from the same entry are ignored while a current `FirstEnemy` is bound.
## First Enemy Death
When `FirstEnemy` is bound, Tutorial Progress subscribes to:
```text
FirstEnemy.DeathHandler.OnTechnicalDeath
```
On technical death:
```text
FirstEnemyDefeated = true
```
The runtime first-enemy reference is then unbound.
The condition remains stored in `GameConditionController`.
## Player Hit By First Enemy
Tutorial Progress subscribes to:
```text
Player.DamageReceivingHandler.OnDamageApplied
```
The damage context must satisfy:
```text
Attacker == FirstEnemy
Victim == Player
DamageAmount > 0
```
When valid:
```text
PlayerHitByFirstEnemy = true
```
This tracks successful applied damage rather than attack attempts.
## `PlayerHasMultipleWeapons`
Derived from:
```text
Player.InventoryController.AllWeaponEntries
```
The condition is true when:
```text
weapon entry count >= 2
```
Tutorial Progress listens to:
```text
InventoryController.OnItemAdded
InventoryController.OnItemRemoved
```
Only `WeaponProfile` additions/removals refresh this condition.
## `PlayerReadyToCycleWeapons`
Derived from:
```text
CombatLoadoutController.EquippedWeaponEntries
CombatLoadoutController.ActiveEquippedWeaponEntry
```
The condition is true when:
```text
equippedWeaponCount >= 2
AND activeWeaponEntry != null
AND activeWeaponIsEquipped
```
Tutorial Progress listens to:
```text
OnEquippedWeaponEntriesChanged
OnActiveEquippedWeaponChanged
```
Either event refreshes this condition.
## Initial Refresh
After external references are bound, Tutorial Progress immediately refreshes:
```text
PlayerHasMultipleWeapons
PlayerReadyToCycleWeapons
```
This synchronizes conditions with player state that may already exist before subscription.
## Reset Behavior
`ResetTutorialProgress()` performs:
```text
GameConditionController.ResetConditions()
        ↓
Refresh PlayerHasMultipleWeapons
        ↓
Refresh PlayerReadyToCycleWeapons
```
Equipment conditions are recalculated because they are derived from current state.
Combat conditions are event-driven and are not immediately recomputed.
## Event Summary
```text
Player
├── DamageReceivingHandler.OnDamageApplied
├── InventoryController.OnItemAdded
├── InventoryController.OnItemRemoved
├── CombatLoadoutController.OnEquippedWeaponEntriesChanged
└── CombatLoadoutController.OnActiveEquippedWeaponChanged
First Enemy Spawner
└── EnemySpawned
First Enemy
└── DeathHandler.OnTechnicalDeath
```
## Condition Update Summary
```text
Weapon added/removed
    → PlayerHasMultipleWeapons
Equipped weapons changed
    → PlayerReadyToCycleWeapons
Active weapon changed
    → PlayerReadyToCycleWeapons
First enemy damages player
    → PlayerHitByFirstEnemy
First enemy technically dies
    → FirstEnemyDefeated
```
## FOTL Lifecycle Usage
### `InitializeInternalState`
Clears:
```text
FirstEnemy
FirstEnemySpawner
FirstEnemySpawnEntry
```
### `CacheExternalReferences`
Resolves:
```text
GameConditionController
Player
First Enemy Source
```
### `ValidateExternalReferences`
Validates the required player systems and resolved first-enemy source.
### `BindExternalReferences`
Subscribes to gameplay events.
Attempts to bind an already-spawned first enemy.
Refreshes equipment conditions.
### `UnbindExternalReferences`
Removes subscriptions and unbinds the current first enemy.
## Design Rules
- Observe authoritative Inventory, Combat Loadout, Damage, Death, and Enemy Spawning systems.
- Store shared tutorial facts in `GameConditionController`.
- Do not directly decide which Tutorial Prompt becomes visible.
- Keep runtime `FirstEnemy` identity separate from `FirstEnemyDefeated` progress state.
- Depend on general `EnemyGroupSpawnerController` / `EnemySpawnEntry` contracts.
## Current Boundaries
Tutorial Progress currently tracks only the implemented Equipment and Combat tutorial conditions.
It does not currently implement:
```text
Achievements
Milestones
Quest progression
Persistent save-game progression
Numeric counters
```
Those may reuse Game Conditions later but are outside the current controller.
## Summary
```text
Inventory state
    → PlayerHasMultipleWeapons
Combat loadout state
    → PlayerReadyToCycleWeapons
Successful first-enemy damage
    → PlayerHitByFirstEnemy
First-enemy technical death
    → FirstEnemyDefeated
```
`TutorialProgressController` is the observer and condition producer.
`GameConditionController` remains the owner of shared runtime condition state.
