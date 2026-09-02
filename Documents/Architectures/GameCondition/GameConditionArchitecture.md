# Game Condition Architecture
## Purpose
The Game Condition system stores reusable boolean facts about current game state.
Each condition is identified by a `GameConditionKey`.
The system is independent from the feature that consumes a condition.
Current implemented condition producers belong to Tutorial Progress.
## Current Scope
The implementation supports:
- Boolean condition values.
- Configured initial values.
- Runtime state changes.
- Required `true` or `false` values.
- `All` requirement combination.
- `Any` requirement combination.
- Condition change notifications.
- Reset to initial values.
It does not currently support numeric progress, counters, ranges, or comparison operators.
## Main Types
### `GameConditionKey`
Identifies a shared game condition.
`None` is the invalid/default key.
Current keys:
```text
Tutorial: Equipment
100  PlayerHasMultipleWeapons
101  PlayerReadyToCycleWeapons
Tutorial: Combat
200  PlayerHitByFirstEnemy
201  FirstEnemyDefeated
```
These numbers organize the enum only.
They are not Data IDs.
### `GameConditionController`
Owns the authoritative runtime condition states.
Main APIs:
```csharp
GetConditionState(...)
SetConditionState(...)
SatisfyCondition(...)
UnsatisfyCondition(...)
ResetConditions()
```
`SatisfyCondition(key)` sets the condition to `true`.
`UnsatisfyCondition(key)` sets the condition to `false`.
`SetConditionState` creates a runtime entry if a valid key does not exist yet.
No change event is raised when an existing condition receives the same value.
### `GameConditionInitialStateEntry`
Defines the initial value of a condition:
```text
ConditionKey
InitialValue
```
During `InitializeInternalState()`, the controller rebuilds runtime state from these entries.
Entries using `GameConditionKey.None` are ignored.
### `GameConditionStateEntry`
Represents one runtime condition:
```text
ConditionKey
Value
```
Other systems should change state through `GameConditionController`, not by editing runtime entries directly.
### `GameConditionRequirementEntry`
Describes what value a consumer expects:
```text
ConditionKey
RequiredValue
```
Examples:
```text
PlayerHitByFirstEnemy == true
FirstEnemyDefeated == false
```
Requirements do not own or initialize runtime condition state.
### `GameConditionCombinationType`
Currently supports:
```text
All
Any
```
`All` means every valid requirement must match.
```text
A == requiredA
AND
B == requiredB
```
`Any` means at least one valid requirement must match.
```text
A == requiredA
OR
B == requiredB
```
Null requirements and `GameConditionKey.None` entries are ignored.
An empty requirement list is treated as satisfied.
### `GameConditionEvaluator`
A static, stateless evaluator.
Main API:
```csharp
AreConditionsSatisfied(
    requirements,
    combinationType,
    conditionController)
```
Evaluation flow:
```text
Consumer Requirements
        ↓
GameConditionEvaluator
        ↓
GameConditionController runtime values
        ↓
true / false
```
The evaluator reads state only.
It does not change conditions.
It does not know which feature requested evaluation.
## Condition Change Event
`GameConditionController` exposes:
```csharp
ConditionStateChanged(GameConditionKey key, bool value)
```
The event is raised when:
- A new runtime condition is created.
- An existing condition changes value.
- `ResetConditions()` republishes initialized values.
Consumers can subscribe when they need to refresh after relevant state changes.
## Responsibility Separation
The intended flow is:
```text
Gameplay State / Events
        ↓
Feature-specific Progress Tracker
        ↓
GameConditionController
        ↓
GameConditionEvaluator
        ↓
Condition Consumer
```
A producer decides when a fact changes.
`GameConditionController` stores the fact.
A consumer declares its requirements.
`GameConditionEvaluator` checks whether those requirements are satisfied.
## Current Condition Domain
### Tutorial Progress
Tutorial Progress is currently the implemented producer domain.
Current groups:
```text
Tutorial Progress
├── Equipment
│   ├── PlayerHasMultipleWeapons
│   └── PlayerReadyToCycleWeapons
└── Combat
    ├── PlayerHitByFirstEnemy
    └── FirstEnemyDefeated
```
See:
```text
Architectures/GameCondition/TutorialProgress.md
```
## Design Rules
### Conditions Represent Facts
Prefer:
```text
PlayerHasMultipleWeapons
FirstEnemyDefeated
```
Avoid consumer-specific state such as:
```text
ShowLoadoutPrompt
HideDodgePrompt
UnlockAchievementA
```
A shared fact may be reused by multiple consumers.
### Producers Do Not Control Consumers
A progress tracker may set:
```text
FirstEnemyDefeated = true
```
It should not decide which prompt, milestone, achievement, quest, or unlock reacts to it.
### Requirements Do Not Own State
Initial values belong to `GameConditionController`.
Runtime values belong to `GameConditionController`.
`GameConditionRequirementEntry` only defines expected values.
## Extension Direction
The same boolean condition layer may later be reused by:
```text
Tutorial Progress
Milestones
Achievements
Quest Progress
World Progress
Unlock Conditions
```
Only Tutorial Progress is implemented at present.
If future systems require numeric or accumulated progress, design that separately rather than forcing it into the current boolean model.
## Summary
```text
What happened?
    → condition producer
What is true now?
    → GameConditionController
What is required?
    → GameConditionRequirementEntry
How are requirements combined?
    → GameConditionCombinationType
Are they satisfied?
    → GameConditionEvaluator
```
The current implementation is intentionally small, shared, and boolean-focused.
