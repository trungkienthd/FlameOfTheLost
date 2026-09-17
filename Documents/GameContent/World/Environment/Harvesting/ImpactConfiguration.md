# Harvesting Impact Configuration

## Purpose

This document defines how combat skills interact with `HarvestableObjectProfile` data after the harvesting impact-response redesign.

The intended model is:

`Skill -> Harvest Impact Type + Base Harvest Damage -> Harvestable Impact Multiplier -> Applied Durability Damage`

Combat damage and harvest damage remain separate concerns.

---

## 1. Harvest Impact Types

The current harvesting vocabulary contains four gameplay impact types:

| Impact Type | Meaning | Typical Sources |
|---|---|---|
| `Slash` | Sweeping cutting contact | Sword slash skills |
| `Chop` | Heavy focused cutting contact | Axe chop skills |
| `Pierce` | Point or projectile penetration | Stabs and arrows |
| `Blunt` | Heavy non-cutting force | Digging / shovel-like impacts |

`None` means that the skill does not participate in harvesting.

These impact types are intended to be **exclusive classifications**, not combinable flags. A skill supplies one impact type per harvest hit.

---

## 2. `HarvestDamageEntry` Configuration

Every `SkillProfile` contains a `HarvestDamageEntry`. The entry defines whether the skill participates in harvesting, what physical impact it produces, how much base harvest damage it contributes, and whether a successful harvesting hit can trigger self-targeted On Hit status effects.

### 2.1 Enablement and Impact

#### `Is Enabled`

Use:

- `true` for player skills that are intended to interact with harvestables;
- `false` for skills that should never produce harvest damage.

Runtime enablement also requires `Impact Type != None`. Therefore, an entry is effectively disabled whenever its impact type is `None`.

#### `Impact Type`

Each harvesting-capable skill supplies one exclusive physical impact:

- `Slash`
- `Chop`
- `Pierce`
- `Blunt`

`None` means no harvesting impact.

The impact should describe the **physical action that reaches the harvestable**, not the combat theme or status effect of the skill.

Examples:

- Hemorrhage Slash -> `Slash`
- Lightning Stab -> `Pierce`
- Arrow skills -> `Pierce`

---

### 2.2 Base Harvest Damage

The current skill-side harvesting model should remain deliberately simple.

Recommended defaults:

- `Combat Damage Multiplier = 0`
- `Flat Harvest Damage = 1`
- `Minimum Harvest Damage = 1`

This makes one successful physical hit equal to one standardized unit of base harvest damage before the target's own impact response is applied.

#### `Combat Damage Multiplier`

Current convention:

`Combat Damage Multiplier = 0`

Combat damage should not currently increase harvesting strength.

This keeps combat balance and harvesting balance independent. A high-damage combat skill such as Hemorrhage Slash or Lightning Stab should not automatically become a superior harvesting tool merely because its normal combat damage is larger.

The field remains available for future exceptional skills if harvesting is later intended to scale partly from combat damage.

#### `Flat Harvest Damage`

Current convention:

`Flat Harvest Damage = 1`

One successful skill hit contributes one base harvest-damage unit.

This value should normally remain the same across the current player skill set. Harvesting effectiveness is primarily authored on the target through `HarvestImpactResponseEntry`.

Multi-projectile skills also remain at `1` per projectile. `Double Shot` and `Tripple Shot` already produce multiple independent projectile hits, so increasing their flat harvest damage would count projectile quantity twice.

#### `Minimum Harvest Damage`

Current convention:

`Minimum Harvest Damage = 1`

This minimum belongs to the **skill-side base harvest-damage calculation**. It must not force the final target durability damage to be at least one after the harvestable's response multiplier is applied.

Conceptually:

```text
Combat-derived harvest contribution
        +
Flat Harvest Damage
        ↓
Apply Minimum Harvest Damage
        ↓
Base Harvest Damage
        ↓
Harvestable Impact Multiplier
        ↓
Final Durability Damage
```

For example:

```text
Base Harvest Damage = 1
Mineral Node Pierce Multiplier = 0

1 × 0 = 0 final durability damage
```

This preserves the harvestable's authority to reject an otherwise valid physical hit.

---

### 2.3 Successful Hit Behaviour

#### `Trigger Self On Hit Status Effects`

Current convention:

`Trigger Self On Hit Status Effects = false`

Harvesting a world object should not normally count as a combat hit for self-targeted `On Hit` effects.

Keeping this disabled prevents harvestables from becoming unintended triggers for mechanics such as:

- self buffs;
- resource recovery;
- stack-building effects;
- combat procs;
- other self-applied effects intended to require a combat target.

The field remains available for a future skill whose design intentionally treats a successful harvesting hit as a valid self-effect trigger.

---

## 3. Player Skill Configuration

All current harvesting-capable player skills should therefore use:

- `Is Enabled = true`
- `Combat Damage Multiplier = 0`
- `Flat Harvest Damage = 1`
- `Minimum Harvest Damage = 1`
- `Trigger Self On Hit Status Effects = false`

For the current skill set, only `Impact Type` normally varies between skills.

| Skill ID | Skill | Harvesting | Impact Type | Notes |
|---:|---|---|---|---|
| 81001 | Slash | Enabled | `Slash` | Current mapping is correct |
| 81002 | Dig | Enabled | `Blunt` | Current mapping is correct; `Blunt` represents the physical digging impact |
| 81101 | Stab | Enabled | `Pierce` | Change from `None` |
| 81102 | Chop | Enabled | `Chop` | Current mapping is correct |
| 81201 | Hemorrhage Slash | Enabled | `Slash` | Change from `Chop`; harvesting uses physical motion, not combat/status-effect theme |
| 81202 | Lightning Stab | Enabled | `Pierce` | Change from `None`; lightning does not create a separate harvesting impact |
| 82001 | Shot | Enabled | `Pierce` | Change from `None`; arrow contact is piercing |
| 82101 | Quick Shot | Enabled | `Pierce` | Change from `None` |
| 82201 | Double Shot | Enabled | `Pierce` | Change from `None`; each projectile resolves independently |
| 82202 | Tripple Shot | Enabled | `Pierce` | Change from `None`; each projectile resolves independently |

### Multi-projectile skills

`Double Shot` and `Tripple Shot` should not receive a larger harvest-damage value per projectile.

Each projectile already creates its own hit. If two or three arrows connect with the same harvestable, the skill naturally produces two or three weak `Pierce` harvest hits.

---

## 4. Enemy Skill Configuration

Enemy-only attacks currently have no harvesting role and should remain completely disabled for harvesting.

| Skill ID | Skill | Harvesting | Impact Type |
|---:|---|---|---|
| 81011 | Feralshroom Cap Bash | Disabled | `None` |
| 82011 | Feralshroom Toxic Puff | Disabled | `None` |
| 82012 | Aquabara Water Glob | Disabled | `None` |

Do not assign `Blunt`, `Pierce`, or another nominal type while `Is Enabled = false`. Keeping the serialized impact at `None` makes the data intent explicit and avoids accidental activation later.

Enemy attacks may still collide with harvestable world objects through normal physics; they simply do not create harvest damage.

---

## 5. Harvestable Response Tiers

With the current base harvest damage of `1`, the recommended response values are:

| Multiplier | Tier | Result per hit |
|---:|---|---:|
| `0` | Immune | 0 durability damage |
| `1` | Weak | 1 durability damage |
| `2` | Effective | 2 durability damage |
| `3` | Preferred | 3 durability damage |

Avoid fractional values such as `0.25` or `0.5` while base harvest damage remains `1`. The runtime rounds the multiplied result to an integer, so fractional values can resolve to zero and are harder to reason about during content authoring.

Recommended profile default:

`Default Impact Damage Multiplier = 0`

Then add explicit entries for the four current impact types. This makes a harvestable's behavior readable directly from its profile and prevents unknown future impact types from gaining an unintended fallback effect.

---

## 6. Recommended Response Archetypes

### Timber Tree

Intended interaction: an axe is clearly best, but ordinary player attacks and arrows can still make slow progress.

- Durability: `6`
- Slash: `1`
- Chop: `3`
- Pierce: `1`
- Blunt: `1`

Result:

- Chop: 2 hits
- Slash: 6 hits
- Stab: 6 hits
- Arrow: 6 projectile hits
- Dig: 6 hits

### Fruit Tree

Intended interaction: cutting branches is effective, while all physical player attacks remain usable.

- Durability: `6`
- Slash: `2`
- Chop: `2`
- Pierce: `1`
- Blunt: `1`

Result:

- Slash / Chop: 3 hits
- Pierce / Blunt: 6 hits

This keeps fruit harvesting distinct from timber harvesting: `Chop` is no longer uniquely dominant.

### Berry Bush

Intended interaction: light cutting is preferred; heavier actions work but are inefficient.

- Durability: `4`
- Slash: `2`
- Chop: `1`
- Pierce: `1`
- Blunt: `1`

### Soft Crop

Examples: Wheat, Tomato, Eggplant, Corn, Cucumber, Chili, peppers.

- Durability: `2`
- Slash: `2`
- Chop: `1`
- Pierce: `1`
- Blunt: `1`

A slash harvests the crop immediately, while other physical attacks require two hits.

### Root Crop

Examples: Carrot, Parsnip.

`Dig` is intentionally favored here.

- Durability: `2`
- Slash: `1`
- Chop: `1`
- Pierce: `1`
- Blunt: `2`

### Heavy Crop

Examples: Pumpkin, Cabbage, Blue Squash.

- Durability: `4`
- Slash: `1`
- Chop: `2`
- Pierce: `1`
- Blunt: `1`

### Mineral Node

Intended interaction: blunt force is the primary harvesting method. Chopping can slowly chip the node; slashing and piercing do no durability damage.

- Durability: `9`
- Slash: `0`
- Chop: `1`
- Pierce: `0`
- Blunt: `3`

Result:

- Dig / Blunt: 3 hits
- Chop: 9 hits
- Slash, Stab, arrows: valid contact but 0 durability damage

---

## 7. Greenvale Initial Configuration

The tutorial map currently needs only two harvestables.

### Oak Tree

Use the **Timber Tree** archetype:

- Maximum Durability: `6`
- Default Impact Damage Multiplier: `0`
- Slash: `1`
- Chop: `3`
- Pierce: `1`
- Blunt: `1`

This establishes the intended rule immediately: the player *can* attack the tree with anything, but `Chop` is visibly superior.

### Cherry Tree

Use the **Fruit Tree** archetype:

- Maximum Durability: `6`
- Default Impact Damage Multiplier: `0`
- Slash: `2`
- Chop: `2`
- Pierce: `1`
- Blunt: `1`

This prevents the Cherry Tree from feeling like another timber node while preserving the universal attack-driven harvesting model.

---

## 8. Authoring Rules

### Skill authoring

When adding or updating a harvesting-capable skill:

1. Enable harvesting only when the skill is intended to interact with harvestables.
2. Assign the impact type from the physical action that reaches the target.
3. Use `Combat Damage Multiplier = 0` by default.
4. Use `Flat Harvest Damage = 1` by default.
5. Use `Minimum Harvest Damage = 1` by default.
6. Use `Trigger Self On Hit Status Effects = false` by default.
7. Keep multi-hit and multi-projectile skills at `1` flat harvest damage per individual hit.
8. Deviate from these defaults only when a skill has an explicit harvesting-specific gameplay role.

### Harvestable authoring

When adding a new harvestable:

1. Choose the closest response archetype.
2. Set `Default Impact Damage Multiplier` to `0`.
3. Add explicit response entries for `Slash`, `Chop`, `Pierce`, and `Blunt`.
4. Use `0` when the impact should physically connect but cause no durability damage.
5. Tune durability and impact responses before changing skill-level harvest damage.
6. Do not use combat damage as the default balancing input for harvesting.
7. Keep drops independent from the impact that broke the object unless a future design explicitly introduces impact-dependent loot.
8. Do not make enemy-only skills harvest-capable merely because their animation resembles a harvesting impact.

---

## 9. Code Consistency Note

`HarvestImpactType` is currently used as a single exclusive value by the redesigned response system.

If the enum still carries the `[Flags]` attribute while its members are serialized as `None = 0`, `Slash = 1`, `Chop = 2`, `Pierce = 3`, and `Blunt = 4`, the attribute should be removed. `Pierce = 3` is not a valid independent bit flag because it overlaps the binary values of `Slash` and `Chop`.

The content design in this document assumes a normal, non-flags enum with one impact type per hit.
