# Territorial Enemy Design

## Purpose

This document defines the shared gameplay and authoring model for the current **Territorial Enemy** type in **FlameOfTheLost**.

Territorial Enemy is currently the only authored Enemy AI type.

Concrete Feralshroom and Aquabara variants are listed in `TerritorialEnemyCatalogue.md`.

---

## 1. Territorial Variant Data

`TerritorialEnemyVariantProfile` extends `EnemyVariantProfile`.

A valid Territorial Enemy variant currently requires:

1. valid base Enemy variant data;
2. a `DefaultRoamingConfig`;
3. an `AttackConfig`.

The inherited variant data also provides:

- base `EnemyProfile`;
- initial inventory entries.

Conceptually:

```text
Territorial Enemy Variant
├── Enemy identity
├── Initial inventory
├── Default roaming configuration
└── Attack configuration
```

---

## 2. Runtime Configuration

`TerritorialEnemyBrainController` receives:

- roaming config;
- attack config;
- home-area entry.

The roaming and attack configs come from authored data.

The home area is placement/runtime-specific and is not a property of the Enemy species itself.

This distinction is important:

```text
Variant Profile
→ default roaming behaviour
→ attack behaviour

Spawn / placement context
→ home area
```

---

## 3. Territorial States

Current state enum:

1. `Roaming`
2. `Alert`
3. `ReadyToAttack`
4. `PreAttack`
5. `Attacking`
6. `Recovering`
7. `Dead`

Current implemented runtime flow primarily uses:

```text
Roaming
→ Alert
→ ReadyToAttack
→ PreAttack
→ Attacking
→ ReadyToAttack / Alert / Roaming
```

On death:

```text
Any active state
→ Dead
```

### Recovering

`Recovering` exists in the state enum but the current Territorial brain does not actively enter or process it.

Treat it as reserved/unused until implementation establishes recovery-state behaviour.

---

## 4. Target-Driven State Selection

The Territorial brain uses the shared AI Character target-detection system.

Current decision priority outside PreAttack/Attacking/Dead:

```text
AttackRange target exists
→ ReadyToAttack

else Alert target exists
→ Alert

else
→ Roaming
```

### Alert

In Alert state the Enemy:

- stops moving;
- faces its current target.

### ReadyToAttack

The Enemy:

1. faces the target;
2. measures distance;
3. moves toward the target while farther than `IdealAttackRange`;
4. stops when sufficiently close;
5. waits for attack cooldown;
6. enters PreAttack.

---

## 5. PreAttack and Attacking

### PreAttack

During PreAttack:

- movement input is cleared;
- the Enemy continues facing the target;
- the brain waits for `PreAttackDuration`.

When the duration expires:

```text
RequestNormalAttack(NormalAttackSide)
```

The brain then:

- starts the attack-decision cooldown;
- starts the attacking-state timer;
- enters `Attacking`.

### Attacking

During Attacking:

- movement remains cleared;
- the Enemy continues facing a valid target.

When the state timer expires:

```text
AttackRange target
→ ReadyToAttack

else Alert target
→ Alert

else
→ Roaming
```

---

## 6. Attack Configuration

`TerritorialEnemyAttackConfig` currently defines:

| Property | Purpose |
|---|---|
| Ideal Attack Range | preferred distance before beginning attack |
| Pre-Attack Duration | telegraph/wind-up delay before normal attack request |
| Attack Decision Cooldown | minimum delay before another attack decision |
| Attacking State Duration | time brain remains in Attacking state |
| Normal Attack Side | loadout side used for normal attack request |

The Territorial brain decides **when and from where** to attack.

The equipped Weapon/Skill decides **what the attack actually does**.

Boundary:

```text
Territorial Attack Config
→ range/timing/input decision

Weapon + Normal Skill
→ attack gameplay
```

---

## 7. Roaming Configuration

`TerritorialEnemyRoamingConfig` defines:

- minimum/maximum roam walk duration;
- minimum/maximum roam idle duration.

Roaming alternates between:

```text
Idle
↔
Walk
```

Each new phase receives a randomized duration within the configured range.

During a walking phase, the Enemy moves toward a random point inside its home area.

---

## 8. Home Area

`TerritorialEnemyHomeAreaEntry` represents a rectangular territory using:

- bottom-left position;
- top-right position;
- return reach distance.

The struct can:

- normalize min/max bounds;
- validate non-zero area;
- test whether a position is inside;
- generate a random point inside.

### Leaving the Territory

If the Enemy is outside its home area:

1. a random return point inside the area is chosen;
2. the Enemy moves toward it;
3. once within `ReturnReachDistance`, the return target is cleared;
4. the Enemy begins an idle roaming phase.

The home area is therefore a movement constraint, not merely a spawn point.

---

## 9. Initialization and Lifecycle

### Initialization

The Territorial brain initializes:

- roaming runtime state;
- attack runtime state;
- AI state = `Roaming`.

During gameplay initialization it attempts to quick-equip a Weapon from its available loadout.

### Refresh

On brain refresh:

- roaming runtime resets;
- attack runtime resets;
- state returns to `Roaming`;
- movement input is cleared.

### Death

On death-disable:

- state becomes `Dead`;
- movement input is cleared;
- roaming runtime resets;
- attack runtime resets.

---

## 10. Variant Combat Loadout

Territorial variants can use inherited initial inventory to establish their combat identity.

Current examples:

```text
Feralshroom Basher
→ Feralshroom Hardened Cap
→ Cap Bash

Feralshroom Toxic
→ Feralshroom Toxic Sac
→ Toxic Puff

Aquabara Pond / Ocean
→ Aquabara Water Sac
→ Water Glob
```

Territorial documentation should reference these Weapon/Skill identities without duplicating detailed Skill configuration already owned by the Weapon/Skill GameContent docs.

---

## 11. Authoring Rules

1. Every Territorial variant must reference a base Enemy profile.
2. Every Territorial variant must define a default roaming config.
3. Every Territorial variant must define an attack config.
4. Initial inventory should provide the combat identity needed by the variant.
5. Home area belongs to placement/runtime configuration, not species identity.
6. `IdealAttackRange` should be authored relative to the actual attack behaviour.
7. `PreAttackDuration` should represent the desired telegraph/wind-up window.
8. `AttackDecisionCooldown` controls decision frequency, not Skill execution internals.
9. `AttackingStateDuration` is brain-state timing and should not be treated as the Skill's own lifetime.
10. Do not duplicate Weapon/Skill mechanics inside Territorial configs.
11. Keep generic Character systems generic unless a Territorial variant truly overrides them.
12. Treat `Recovering` as unused until the brain implements it.
