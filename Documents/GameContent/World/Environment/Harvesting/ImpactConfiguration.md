# Harvesting Impact Configuration

## Purpose

This document defines how skills produce harvest damage and how Harvestable Objects respond to `HarvestImpactType`.

The model is:

`Skill -> Harvest Impact Type + Base Harvest Damage -> Harvestable Response Multiplier -> Applied Durability Damage`

Combat damage and harvest damage remain separate concerns.

---

## 1. Harvest Impact Types

| Impact Type | Meaning | Typical Sources |
|---|---|---|
| `Slash` | Sweeping cutting contact | Sword slash |
| `Chop` | Heavy focused cutting contact | Axe chop |
| `Pierce` | Point/projectile penetration | Stab, arrows |
| `Blunt` | Heavy non-cutting force | Dig/shovel-like impact |

`None` means the skill does not participate in harvesting.

Impact types are exclusive classifications, not combinable flags.

---

## 2. Standard `HarvestDamageEntry`

Current harvesting-capable player skills should normally use:

| Property | Standard Value |
|---|---:|
| Is Enabled | `true` |
| Combat Damage Multiplier | `0` |
| Flat Harvest Damage | `1` |
| Minimum Harvest Damage | `1` |
| Trigger Self On Hit Status Effects | `false` |

Only `Impact Type` normally differs between skills.

### Combat Damage Multiplier

Use `0` by default.

Combat strength should not automatically increase harvesting strength.

### Flat Harvest Damage

Use `1` by default.

One successful hit contributes one unit of base harvest damage before the target response multiplier is applied.

For multi-hit or multi-projectile skills, keep `1` per individual hit.

### Minimum Harvest Damage

Use `1` by default.

This minimum applies to the skill-side base calculation and does **not** override a target multiplier of `0`.

Example:

`Base Harvest Damage 1 x Target Multiplier 0 = 0 final durability damage`

### Trigger Self On Hit Status Effects

Use `false` by default.

Harvestable Objects should not normally trigger combat-oriented self `On Hit` effects.

---

## 3. Current Skill Mapping

### Player Skills

| Skill ID | Skill | Harvesting | Impact Type |
|---:|---|---|---|
| 81001 | Slash | Enabled | `Slash` |
| 81002 | Dig | Enabled | `Blunt` |
| 81101 | Stab | Enabled | `Pierce` |
| 81102 | Chop | Enabled | `Chop` |
| 81201 | Hemorrhage Slash | Enabled | `Slash` |
| 81202 | Lightning Stab | Enabled | `Pierce` |
| 82001 | Shot | Enabled | `Pierce` |
| 82101 | Quick Shot | Enabled | `Pierce` |
| 82201 | Double Shot | Enabled | `Pierce` |
| 82202 | Tripple Shot | Enabled | `Pierce` |

Each projectile from Double Shot / Tripple Shot resolves independently.

### Enemy Skills

Enemy-only attacks currently have no harvesting role.

| Skill ID | Skill | Harvesting | Impact Type |
|---:|---|---|---|
| 81011 | Feralshroom Cap Bash | Disabled | `None` |
| 82011 | Feralshroom Toxic Puff | Disabled | `None` |
| 82012 | Aquabara Water Glob | Disabled | `None` |

Keep `Impact Type = None` when harvesting is disabled.

---

## 4. Response Tiers

With standard base harvest damage `1`, use integer target multipliers:

| Multiplier | Meaning | Damage per standard hit |
|---:|---|---:|
| `0` | No durability damage | 0 |
| `1` | Weak / fallback | 1 |
| `2` | Effective | 2 |
| `3` | Preferred | 3 |

Use:

`Default Impact Damage Multiplier = 0`

and explicitly configure the current impact types.

Avoid fractional multipliers while the standard base damage remains `1`.

---

## 5. Harvestable Response Archetypes

### Timber Tree

| Property | Value |
|---|---:|
| Maximum Durability | 6 |
| Slash | 1 |
| Chop | 3 |
| Pierce | 1 |
| Blunt | 1 |

Expected standard hits to break:

- Chop: 2
- Slash: 6
- Pierce: 6
- Blunt: 6

Used by:

- Oak Tree — Normal and Small
- Birch Tree — Normal and Small
- Spruce Tree — Normal and Small

Normal and Small variants use the same durability and impact response. Their difference is limited to drop chances, defined in `HarvestableDesign.md`.

### Fruit Tree

| Property | Value |
|---|---:|
| Maximum Durability | 6 |
| Slash | 2 |
| Chop | 2 |
| Pierce | 1 |
| Blunt | 1 |

Expected standard hits to break:

- Slash / Chop: 3
- Pierce / Blunt: 6

Used by:

- Apple Tree
- Cherry Tree
- Peach Tree
- Pear Tree

### Mineral Node

| Property | Value |
|---|---:|
| Maximum Durability | 9 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 3 |

Expected standard hits to break:

- Blunt: 3
- Chop: 9
- Slash / Pierce: no durability damage

Used by:

- Stone Deposit
- Iron Node
- Copper Node
- Gold Node
- Aquamarine Node
- Amber Node
- Ruby Node
- Emerald Node
- Amethyst Node

Stone Deposit currently shares the Mineral Node response archetype. It can receive a separate lighter Stone archetype later if playtesting shows that it should break faster than ore/crystal nodes.

---

## 6. Authoring Rules

### Skill authoring

1. Enable harvesting only when the skill should interact with Harvestables.
2. Choose the impact type from the physical action that reaches the target.
3. Use `Combat Damage Multiplier = 0` by default.
4. Use `Flat Harvest Damage = 1` by default.
5. Use `Minimum Harvest Damage = 1` by default.
6. Use `Trigger Self On Hit Status Effects = false` by default.
7. Keep multi-hit/multi-projectile skills at one base harvest-damage unit per hit.
8. Deviate only for an explicitly harvesting-specific skill.

### Harvestable authoring

1. Choose the closest response archetype.
2. Use `Default Impact Damage Multiplier = 0`.
3. Explicitly configure `Slash`, `Chop`, `Pierce`, and `Blunt`.
4. Use multiplier `0` when contact should cause no durability damage.
5. Balance durability and target response before changing skill-side harvest damage.
6. Keep drops independent from impact type unless intentionally redesigned.
7. Enemy-only skills should remain harvest-disabled unless their gameplay role changes.

---

## 7. Code Consistency

`HarvestImpactType` is treated as one exclusive value per hit.

If the enum still uses `[Flags]` while values are serialized as:

- `None = 0`
- `Slash = 1`
- `Chop = 2`
- `Pierce = 3`
- `Blunt = 4`

remove `[Flags]`.

`Pierce = 3` overlaps the binary values of `Slash` and `Chop`, so this layout is not a valid independent bit-flag definition.
