# Enemy Design

## Purpose

This document defines the shared **Enemy** content architecture for **FlameOfTheLost**.

Enemy-specific AI types, such as Territorial Enemy, are documented in their own subfolders.

Current Enemy AI type:

- Territorial Enemy

Future Enemy AI types should be added alongside Territorial rather than folded into the shared Enemy layer.

---

## 1. Enemy Architecture

Current inheritance:

```text
CharacterAIProfile
└── EnemyProfile

CharacterAIRootController<EnemyProfile>
└── EnemyRootController

CharacterAIVariantProfile
└── EnemyVariantProfile
    └── TerritorialEnemyVariantProfile

CharacterBehaviourControllerBase
└── EnemyBrainController
    └── TerritorialEnemyBrainController
```

The shared Enemy layer intentionally does not define one universal AI state machine.

`EnemyBrainController` is an abstract common brain base. Concrete Enemy types provide their own decision/state logic.

---

## 2. EnemyProfile

`EnemyProfile` extends the shared AI Character profile with Enemy reward configuration.

Current Enemy-specific field:

```text
CharacterRewardDropProfile
```

The base Enemy identity therefore owns the reward-drop profile shared by its variants unless later content explicitly changes that architecture.

Enemy-specific AI-type behaviour does not belong directly in `EnemyProfile`.

---

## 3. EnemyRootController

`EnemyRootController` is the Enemy-specific root controller.

Current Enemy-only references:

- `EnemyBrainController`
- `CharacterRewardDropProvider`

It also exposes the inherited input handler as:

`EnemyInputHandler`

### Configuration

During internal reference caching, the Enemy root:

1. resolves its Enemy brain if missing;
2. resolves its reward-drop provider if missing;
3. caches the Enemy Profile reward configuration into the reward-drop provider when appropriate.

### Validation

Current expectations:

- the root input handler must be an `EnemyInputHandler`;
- an Enemy brain should be assigned;
- a reward-drop provider must be present.

This keeps base Enemy composition independent from the concrete Enemy AI type.

---

## 4. EnemyBrainController

`EnemyBrainController` is the shared abstract base for concrete Enemy brains.

It provides access to:

- visual controller;
- target-detection handler;
- Enemy input handler;
- current target;
- current Alert target;
- current AttackRange target.

Target information comes from the shared AI Character target-detection system.

The Enemy brain does not maintain a second independent target database.

---

## 5. Enemy Brain Lifecycle

The shared brain participates in Character refresh and death-disable lifecycles.

### Refresh

Refresh priority:

`90`

On refresh:

- death-disabled state is cleared;
- all detected targets are cleared;
- movement input is cleared;
- the concrete brain receives `OnBrainRefreshed`.

### Death Disable

Death-disable priority:

`25`

On Character death:

- the brain becomes death-disabled;
- all detected targets are cleared;
- movement input is cleared;
- the concrete brain receives `OnBrainDeathDisabled`.

Concrete Enemy brains should extend these hooks for type-specific state cleanup rather than duplicating the common lifecycle behaviour.

---

## 6. EnemyVariantProfile

`EnemyVariantProfile` specializes `CharacterAIVariantProfile` by requiring its base Character reference to be an:

`EnemyProfile`

Shared variant content inherited from AI Character:

- base Character profile reference;
- initial inventory entries.

Concrete Enemy-type variants may then add AI-type-specific configuration.

Example:

```text
TerritorialEnemyVariantProfile
→ EnemyProfile
→ Initial Inventory
→ Default Roaming Config
→ Attack Config
```

---

## 7. Generic Character Systems

Enemy Characters still use the common Character architecture.

Systems such as the following should remain generic unless an Enemy requires genuinely custom data or behaviour:

- general Combat configuration;
- Dodge;
- Status Effect Guard;
- other ordinary Character composition/configuration.

The Enemy documentation should not repeat generic Character configuration simply because Enemy prefabs reference those systems.

Document Enemy-specific differences only.

---

## 8. Current Enemy Identities

Current authored Enemy identities:

- Feralshroom
- Aquabara

Each has multiple Territorial variants.

The base Enemy identity owns shared Character-level data such as its common movement/progression/reward configuration.

Variant-specific AI and initial loadout belong to the Territorial variant profile.

See:

- `EnemyCatalogue.md`
- `Territorial/TerritorialEnemyDesign.md`
- `Territorial/TerritorialEnemyCatalogue.md`

---

## 9. Future Enemy Types

New Enemy behaviour families should be represented as sibling folders beneath `Enemies`.

Example:

```text
Enemies
├── Territorial
├── <FutureEnemyTypeA>
├── <FutureEnemyTypeB>
└── ...
```

A new Enemy type should only be introduced when it requires genuinely different AI behaviour or configuration—not merely because an Enemy has different stats, weapons, or tuning.

---

## 10. Authoring Rules

1. Keep shared AI Character behaviour in the AI Character layer.
2. Keep shared Enemy composition/lifecycle behaviour in the Enemy layer.
3. Put state machines and AI decisions in concrete Enemy-type brains.
4. Base Enemy profiles should not absorb Territorial-specific data.
5. Variants should reference a shared Enemy identity rather than duplicate that identity.
6. Initial inventory belongs to variant data.
7. Reward configuration currently belongs to `EnemyProfile`.
8. Generic Character systems remain generic unless an Enemy genuinely overrides them.
9. Concrete Enemy docs should describe meaningful differences rather than mirror every serialized field.
