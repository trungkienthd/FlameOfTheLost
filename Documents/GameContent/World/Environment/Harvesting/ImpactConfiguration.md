# Harvesting Impact Configuration

## Purpose

This document defines how skills produce harvest damage and how each Harvestable responds to `HarvestImpactType`.

The model is:

`Skill -> Harvest Impact Type + Base Harvest Damage -> Harvestable Multiplier -> Applied Durability Damage`

---

## 1. Harvest Impact Types

| Impact Type | Meaning |
|---|---|
| `Slash` | Sweeping cutting contact |
| `Chop` | Heavy focused cutting contact |
| `Pierce` | Point / projectile penetration |
| `Blunt` | Heavy non-cutting force |
| `None` | No harvesting participation |

Impact types are exclusive classifications.

---

## 2. Standard HarvestDamageEntry

Current harvesting-capable player skills normally use:

| Property | Value |
|---|---:|
| Is Enabled | `true` |
| Combat Damage Multiplier | `0` |
| Flat Harvest Damage | `1` |
| Minimum Harvest Damage | `1` |
| Trigger Self On Hit Status Effects | `false` |

`Minimum Harvest Damage` applies before the target multiplier, so a target multiplier of `0` still produces `0` final durability damage.

---

## 3. Current Skill Mapping

| Skill | Impact Type |
|---|---|
| Slash | `Slash` |
| Dig | `Blunt` |
| Stab | `Pierce` |
| Chop | `Chop` |
| Hemorrhage Slash | `Slash` |
| Lightning Stab | `Pierce` |
| Shot | `Pierce` |
| Quick Shot | `Pierce` |
| Double Shot | `Pierce` |
| Tripple Shot | `Pierce` |

Enemy-only skills remain harvesting-disabled with `Impact Type = None`.

---

## 4. Response Tiers

| Multiplier | Meaning |
|---:|---|
| `0` | No durability damage |
| `1` | Weak / fallback |
| `2` | Effective |
| `3` | Preferred |

Recommended profile default:

`Default Impact Damage Multiplier = 0`

Explicitly configure `Slash`, `Chop`, `Pierce`, and `Blunt`.

---

## 5. Complete Harvestable Response Configuration

### Timber Trees

| Harvestable | Variant | Max Durability | Slash | Chop | Pierce | Blunt |
|---|---|---:|---:|---:|---:|---:|
| Oak Tree | Normal | 7 | 1 | 3 | 1 | 1 |
| Oak Tree | Small | 5 | 1 | 3 | 1 | 1 |
| Birch Tree | Normal | 6 | 1 | 3 | 1 | 1 |
| Birch Tree | Small | 4 | 1 | 3 | 1 | 1 |
| Spruce Tree | Normal | 8 | 1 | 3 | 1 | 1 |
| Spruce Tree | Small | 6 | 1 | 3 | 1 | 1 |

### Fruit Trees

| Harvestable | Max Durability | Slash | Chop | Pierce | Blunt |
|---|---:|---:|---:|---:|---:|
| Apple Tree | 7 | 2 | 3 | 1 | 1 |
| Cherry Tree | 5 | 3 | 2 | 1 | 1 |
| Peach Tree | 6 | 2 | 3 | 1 | 1 |
| Pear Tree | 7 | 2 | 2 | 1 | 1 |

### Stone Resources

All seven Stone-like Harvestables use the same physical response pattern but have individually authored durability:

| Harvestable | Environment | Max Durability | Slash | Chop | Pierce | Blunt |
|---|---|---:|---:|---:|---:|---:|
| Stone Large | Land | 10 | 0 | 1 | 0 | 3 |
| Stone Normal | Land | 7 | 0 | 1 | 0 | 3 |
| Stone Small 0 | Land | 4 | 0 | 1 | 0 | 3 |
| Stone Small 1 | Land | 5 | 0 | 1 | 0 | 3 |
| Stone Water Large | Water | 9 | 0 | 1 | 0 | 3 |
| Stone Water Normal | Water | 6 | 0 | 1 | 0 | 3 |
| Stone Water Small | Water | 4 | 0 | 1 | 0 | 3 |

### Metal Nodes

| Harvestable | Max Durability | Slash | Chop | Pierce | Blunt |
|---|---:|---:|---:|---:|---:|
| Copper Node | 8 | 0 | 1 | 0 | 3 |
| Iron Node | 10 | 0 | 1 | 0 | 3 |
| Gold Node | 9 | 0 | 1 | 0 | 2 |

### Crystal Nodes

| Harvestable | Max Durability | Slash | Chop | Pierce | Blunt |
|---|---:|---:|---:|---:|---:|
| Aquamarine Node | 8 | 0 | 1 | 0 | 3 |
| Amber Node | 7 | 0 | 1 | 1 | 3 |
| Ruby Node | 10 | 0 | 1 | 0 | 2 |
| Emerald Node | 9 | 0 | 0 | 1 | 3 |
| Amethyst Node | 8 | 0 | 1 | 1 | 3 |

---

## 6. Expected Standard Hits to Break

With base harvest damage `1`:

| Harvestable | Variant | Preferred / Effective Result |
|---|---|---|
| Oak Tree | Normal | Chop: 3 hits |
| Oak Tree | Small | Chop: 2 hits |
| Birch Tree | Normal | Chop: 2 hits |
| Birch Tree | Small | Chop: 2 hits |
| Spruce Tree | Normal | Chop: 3 hits |
| Spruce Tree | Small | Chop: 2 hits |
| Apple Tree | — | Chop: 3 hits |
| Cherry Tree | — | Slash: 2 hits |
| Peach Tree | — | Chop: 2 hits |
| Pear Tree | — | Slash / Chop: 4 hits |
| Stone Large | Land | Blunt: 4 hits |
| Stone Normal | Land | Blunt: 3 hits |
| Stone Small 0 | Land | Blunt: 2 hits |
| Stone Small 1 | Land | Blunt: 2 hits |
| Stone Water Large | Water | Blunt: 3 hits |
| Stone Water Normal | Water | Blunt: 2 hits |
| Stone Water Small | Water | Blunt: 2 hits |
| Copper Node | — | Blunt: 3 hits |
| Iron Node | — | Blunt: 4 hits |
| Gold Node | — | Blunt: 5 hits |
| Aquamarine Node | — | Blunt: 3 hits |
| Amber Node | — | Blunt: 3 hits |
| Ruby Node | — | Blunt: 5 hits |
| Emerald Node | — | Blunt: 3 hits |
| Amethyst Node | — | Blunt: 3 hits |

---

## 7. Physical Harvesting Language

Broad rules remain predictable:

- Timber Trees favour `Chop`.
- Fruit Trees favour `Slash` and/or `Chop`.
- All seven Stone-like Harvestables and all Metal Nodes favour `Blunt`.
- Crystal Nodes favour `Blunt`.
- Some Crystal Nodes allow inefficient `Pierce` or `Chop` fallback.
- `Slash` does not damage current Stone / Metal / Crystal resources.

This keeps the system readable while still allowing individual Harvestables to feel different.

---

## 8. Authoring Rules

### Skill authoring

1. Enable harvesting only for intended skills.
2. Assign impact type from the physical action.
3. Use `Combat Damage Multiplier = 0` by default.
4. Use `Flat Harvest Damage = 1` by default.
5. Use `Minimum Harvest Damage = 1` by default.
6. Use `Trigger Self On Hit Status Effects = false` by default.
7. Multi-hit / multi-projectile skills remain one base harvest-damage unit per hit.

### Harvestable authoring

1. Use `Default Impact Damage Multiplier = 0`.
2. Explicitly configure all four current impact types.
3. Use `0` when the impact should cause no durability damage.
4. Tune durability and target multipliers before changing skill-side harvest damage.
5. Keep drops independent from impact type unless intentionally redesigned.
