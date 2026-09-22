# AI Character Design

## Purpose

This document defines the current shared **AI Character** content and gameplay architecture for **FlameOfTheLost**.

An AI Character is a non-player-controlled character built on the common Character architecture and extended with AI-specific data, targeting, aiming, and runtime behaviour.

Current and planned high-level AI Character groups:

- Enemy
- Ally
- Neutral — possible future group; not currently implemented as a targeting faction behaviour

This document covers only behaviour that is genuinely shared at the AI Character layer. Enemy-specific behaviour belongs under the Enemy documentation.

---

## 1. Architecture

Current inheritance:

```text
CharacterProfile
└── CharacterAIProfile
    └── EnemyProfile
        └── enemy-specific authored data

CharacterRootController
└── CharacterAIRootController
    ├── CharacterAIRootController<TProfile>
    └── EnemyRootController

CharacterDataAsset
└── CharacterAIVariantProfile
    └── EnemyVariantProfile
        └── TerritorialEnemyVariantProfile
```

`CharacterAIProfile` currently introduces no additional serialized fields beyond `CharacterProfile`. Its purpose is to establish the common profile type for AI-controlled characters.

`CharacterAIRootController` adds the shared AI target-detection reference.

The generic `CharacterAIRootController<TProfile>` binds a strongly typed `CharacterAIProfile` subtype to both:

- the generic Character `Profile`;
- the AI-specific `CharacterAIProfile`.

---

## 2. AI Character Variant Data

`CharacterAIVariantProfile` represents authored data that can vary between AI variants while keeping the same underlying AI Character identity.

Current shared variant responsibilities:

1. reference the base `CharacterAIProfile`;
2. define initial inventory entries;
3. expose a basic validity check.

Current shared validity rule:

```text
CharacterProfile != null
```

Enemy variants extend this base rather than duplicating the shared initial-inventory concept.

---

## 3. Target Detection

Every current `CharacterAIRootController` may reference a:

`CharacterAITargetDetectionHandler`

The root automatically searches its children for this handler when the reference is not already assigned.

The handler maintains two runtime target sets:

- Alert targets
- Attack-range targets

Current range types:

```text
Alert
AttackRange
```

### Target Priority

The handler resolves the current target using this priority:

```text
nearest valid AttackRange target
    ↓ fallback
nearest valid Alert target
```

Within each range set, the nearest valid target is selected by squared distance.

The target is resolved dynamically rather than stored as one permanently selected target.

---

## 4. Target Registration

Target-detection trigger objects use trigger colliders.

On trigger enter:

```text
CharacterRootController
→ validate target
→ register in Alert or AttackRange set
```

On trigger exit:

```text
CharacterRootController
→ unregister from corresponding set
```

The trigger controller forces its assigned `Collider2D` to operate as a trigger.

### Current Generalization Limitation

Although `CharacterAITargetDetectionHandler` and `CharacterAIRootController` are general AI Character systems, the current `CharacterAITargetDetectionTriggerController` obtains its handler only when its root is an:

`EnemyRootController`

Therefore the trigger component is **not yet fully generic for Ally or other future AI Character root types**.

This is a current implementation boundary, not a desired conceptual restriction.

If Allies later use the same detection-trigger system, this lookup should be generalized to `CharacterAIRootController` rather than documenting Enemy-only behaviour as a universal AI rule.

---

## 5. Faction-Based Target Validation

Target validity is currently determined through:

`CharacterAITargetValidationUtility.CanTarget(selfFaction, targetFaction)`

Current authored rules:

| Self Faction | Valid Target Factions |
|---|---|
| Enemy | Player, Ally |
| Ally | Enemy |
| Player | Enemy |
| Other / unsupported | None |

Characters cannot target another character with the same faction.

A character cannot target itself.

### Neutral Characters

A Neutral AI Character is not currently represented by explicit targeting behaviour in this utility.

Until Neutral faction rules are actually implemented, Neutral should remain a future design space rather than being assigned inferred targeting semantics in GameContent documentation.

---

## 6. Detection Lifecycle

`CharacterAITargetDetectionHandler` participates in Character refresh and death-disable lifecycles.

### Refresh

On refresh:

- death-disable state is cleared;
- all detected targets are cleared.

Refresh priority:

`85`

### Character Death

On death-disable:

- the handler becomes death-disabled;
- all detected targets are cleared.

Death-disable priority:

`5`

While death-disabled:

- no new targets are registered;
- target resolution returns no target.

---

## 7. AI Combat Aiming

`TargetCombatAimingProvider` is the shared targeting-based combat aiming provider for AI Characters.

It obtains the current target through `CharacterAIRootController.TargetDetectionHandler`.

When a valid target exists, aim direction is:

```text
normalize(target position - owner position)
```

If no target exists, or both positions are effectively identical, no aim direction is supplied.

This creates a clean boundary:

```text
Target Detection
→ decides which target is current

TargetCombatAimingProvider
→ converts current target position into combat aim direction
```

The aiming provider does not choose targets itself.

---

## 8. Shared vs Specialized Responsibilities

### AI Character Layer

The AI Character layer owns shared concepts such as:

- AI Character profile typing;
- variant base data;
- initial variant inventory;
- target detection;
- target-range classification;
- target validation;
- nearest-target selection;
- target-based combat aiming.

### Enemy Layer

Enemy-specific systems own:

- enemy brain abstraction;
- enemy input;
- reward drops;
- death/refresh brain handling;
- concrete enemy families and variants.

### Enemy-Type Layer

A specific Enemy type, such as Territorial Enemy, owns:

- its AI states;
- movement/roaming rules;
- attack-decision rules;
- type-specific configuration profiles.

---

## 9. Authoring Rules

1. Put behaviour in the highest layer where it is genuinely shared.
2. Do not duplicate generic Character configuration in AI Character docs unless AI Characters change its meaning.
3. `CharacterAIVariantProfile` should hold variant-level AI Character content, not base Character identity.
4. Initial inventory is shared AI-variant data and may be specialized by Enemy variants.
5. Target selection must respect faction validation.
6. AttackRange targets take priority over Alert targets.
7. Target-based aiming should consume the resolved target rather than maintain a second independent targeting state.
8. Do not assign Neutral targeting behaviour until it exists in code/data.
9. Treat the current Enemy-only lookup in `CharacterAITargetDetectionTriggerController` as an implementation limitation to revisit when Ally AI uses the same system.
