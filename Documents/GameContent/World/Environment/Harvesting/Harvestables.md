# Harvestables

## Purpose

This document defines the harvestable content currently available from the project's environment and resource sprite sets, the items each harvestable is intended to produce, and the recommended harvesting-impact configuration.

For now, harvestable drops are limited to:

- **Material**
- **Consumable Item**

Drop quantities and probabilities are baseline design values and can be tuned during balancing.

Impact configuration follows [Impact Configuration](ImpactConfiguration.md).

Unless a specific entry says otherwise:

- `Default Impact Damage Multiplier = 0`
- all four current impact types are authored explicitly;
- drops are determined by the harvestable, not by the impact type used to break it.

---

## 1. Timber Trees

### Shared Harvest Configuration

Applies to Oak, Birch, and Spruce trees unless overridden later.

| Property | Value |
|---|---:|
| Maximum Durability | 6 |
| Slash | 1 |
| Chop | 3 |
| Pierce | 1 |
| Blunt | 1 |

`Chop` is the preferred timber-harvesting impact. Other player attacks can still break a normal tree, but substantially more slowly.

### Oak Tree

**Status:** Current

Visual variants:

- Oak Tree
- Small Oak Tree

Drops:

| Drop | Category | Quantity | Chance | Notes |
|---|---|---:|---:|---|
| Twig | Material | 1–2 | 100% | Primary basic tree resource |
| Wood | Material | 1 | 30% | Less-common substantial timber |

The current Section 2 Oak Tree should use the shared Timber Tree harvest configuration and this drop table.

### Birch Tree

**Status:** Planned

Visual variants:

- Birch Tree
- Small Birch Tree

Drops:

| Drop | Category | Quantity | Chance | Notes |
|---|---|---:|---:|---|
| Twig | Material | 1–2 | 100% | Shared generic small branch resource |
| Wood | Material | 1 | 30% | Shared generic timber resource |

Birch does not require a separate `BirchWood` material at the current game scale.

### Spruce Tree

**Status:** Planned

Visual variants:

- Spruce Tree
- Small Spruce Tree

Drops:

| Drop | Category | Quantity | Chance | Notes |
|---|---|---:|---:|---|
| Twig | Material | 1–2 | 100% | Shared generic small branch resource |
| Wood | Material | 1 | 30% | Shared generic timber resource |

Spruce does not require a separate `SpruceWood` material at the current game scale.

---

## 2. Fruit Trees

Fruit-tree harvesting is primarily intended to teach and supply **Consumable Items**.

The generic `Small Fruit Tree`, `Medium Fruit Tree`, and `Big Fruit Tree` sprites are size/growth presentation variants rather than separate drop tables.

### Shared Harvest Configuration

| Property | Value |
|---|---:|
| Maximum Durability | 6 |
| Slash | 2 |
| Chop | 2 |
| Pierce | 1 |
| Blunt | 1 |

Fruit trees deliberately do not use the timber-tree `Chop = 3` response. This keeps gathering fruit mechanically distinct from chopping a tree for Wood.

### Apple Tree

**Status:** Planned

Drops:

| Drop | Category | Quantity | Chance | Notes |
|---|---|---:|---:|---|
| Apple | Consumable Item | 1–3 | 100% | Primary produce |
| Twig | Material | 1 | 20% | Optional secondary drop |

### Cherry Tree

**Status:** Current / Section 3

Drops:

| Drop | Category | Quantity | Chance | Notes |
|---|---|---:|---:|---|
| Cherry | Consumable Item | 1–3 | 100% | Introduces consumable harvesting |
| Twig | Material | 1 | 20% | Optional secondary drop |

The Cherry Tree in Greenvale Border Path Section 3 should be the first clear **Harvestable -> Consumable Item** tutorial example.

It should use the shared Fruit Tree impact configuration.

### Peach Tree

**Status:** Planned

Drops:

| Drop | Category | Quantity | Chance | Notes |
|---|---|---:|---:|---|
| Peach | Consumable Item | 1–3 | 100% | Primary produce |
| Twig | Material | 1 | 20% | Optional secondary drop |

Fruit trees do not normally drop `Wood`.

---

## 3. Berry Bushes

The available sprite sheet contains two berry families.

### Shared Harvest Configuration

| Property | Value |
|---|---:|
| Maximum Durability | 4 |
| Slash | 2 |
| Chop | 1 |
| Pierce | 1 |
| Blunt | 1 |

### Red Berry Bush

**Status:** Planned

Drops:

| Drop | Category | Quantity | Chance |
|---|---|---:|---:|
| Red Berry | Consumable Item | 1–3 | 100% |

### Purple Berry Bush

**Status:** Planned

Drops:

| Drop | Category | Quantity | Chance |
|---|---|---:|---:|
| Purple Berry | Consumable Item | 1–3 | 100% |

The final lore/gameplay names of the berries may replace the colour-based working names later.

---

## 4. Crops

Each crop sprite set contains several growth stages. Growth stages are visual/runtime states of one harvestable type and do not create separate content entries.

Crops use three response archetypes.

### Soft Crop

Intended for above-ground produce and stalk crops.

| Property | Value |
|---|---:|
| Maximum Durability | 2 |
| Slash | 2 |
| Chop | 1 |
| Pierce | 1 |
| Blunt | 1 |

Applies to:

- Wheat
- Tomato Plant
- Eggplant Crop
- Corn Crop
- Cucumber Crop
- Chili Crop
- Red Pepper Crop
- Orange Pepper Crop
- Green Pepper Crop

### Root Crop

`Dig` / `Blunt` is preferred.

| Property | Value |
|---|---:|
| Maximum Durability | 2 |
| Slash | 1 |
| Chop | 1 |
| Pierce | 1 |
| Blunt | 2 |

Applies to:

- Carrot Crop
- Parsnip Crop

### Heavy Crop

Dense produce favors a heavier cutting action.

| Property | Value |
|---|---:|
| Maximum Durability | 4 |
| Slash | 1 |
| Chop | 2 |
| Pierce | 1 |
| Blunt | 1 |

Applies to:

- Pumpkin Crop
- Cabbage Crop
- Blue Squash Crop

### Crop Catalogue

| Harvestable | Status | Primary Drop | Category | Baseline Yield | Harvest Archetype |
|---|---|---|---|---:|---|
| Wheat | Planned | Wheat | Consumable Item | 1–3 | Soft Crop |
| Tomato Plant | Planned | Tomato | Consumable Item | 1–3 | Soft Crop |
| Carrot Crop | Planned | Carrot | Consumable Item | 1–3 | Root Crop |
| Eggplant Crop | Planned | Eggplant | Consumable Item | 1–3 | Soft Crop |
| Corn Crop | Planned | Corn | Consumable Item | 1–3 | Soft Crop |
| Pumpkin Crop | Planned | Pumpkin | Consumable Item | 1 | Heavy Crop |
| Parsnip Crop | Planned | Parsnip | Consumable Item | 1–3 | Root Crop |
| Cabbage Crop | Planned | Cabbage | Consumable Item | 1 | Heavy Crop |
| Cucumber Crop | Planned | Cucumber | Consumable Item | 1–3 | Soft Crop |
| Chili Crop | Planned | Chili Pepper | Consumable Item | 1–3 | Soft Crop |
| Red Pepper Crop | Planned | Red Pepper | Consumable Item | 1–3 | Soft Crop |
| Orange Pepper Crop | Planned | Orange Pepper | Consumable Item | 1–3 | Soft Crop |
| Green Pepper Crop | Planned | Green Pepper | Consumable Item | 1–3 | Soft Crop |
| Blue Squash Crop | Planned / working name | Blue Squash | Consumable Item | 1 | Heavy Crop |

`Blue Squash` is a working name based on the final dark teal round crop sprite and should be renamed if the art pack provides an authoritative name.

---

## 5. Mineral Nodes

The supplied mineral sprites support eight visual mineral families.

The current material data separates them into:

- **Metal families:** Iron, Copper, Gold
- **Crystal families:** Aquamarine, Amber, Ruby, Emerald, Amethyst

The intended direct harvest output is the raw **Chunks** form of each family.

Processed forms such as `Ore`, `Bar`, `Crystal`, and `Block` are not intended to drop directly from mineral nodes unless a future design explicitly says otherwise.

### Shared Harvest Configuration

| Property | Value |
|---|---:|
| Maximum Durability | 9 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 3 |

`Blunt` is the preferred mineral-harvesting impact. `Chop` can slowly chip a node. Slashing attacks, stabs, and arrows can contact the object but do no durability damage.

### Metal Nodes

| Harvestable | Status | Direct Drop | Category | Baseline Yield |
|---|---|---|---|---:|
| Iron Node | Planned | Iron Chunks | Material | 1–3 |
| Copper Node | Planned | Copper Chunks | Material | 1–3 |
| Gold Node | Planned | Gold Chunks | Material | 1–2 |

Intended material progression:

`Chunks -> Ore -> Bar`

The conversion between these forms belongs to a future processing/refining system rather than the harvesting system itself.

### Crystal Nodes

| Harvestable | Status | Direct Drop | Category | Baseline Yield |
|---|---|---|---|---:|
| Aquamarine Node | Planned | Aquamarine Chunks | Material | 1–2 |
| Amber Node | Planned | Amber Chunks | Material | 1–2 |
| Ruby Node | Planned | Ruby Chunks | Material | 1–2 |
| Emerald Node | Planned | Emerald Chunks | Material | 1–2 |
| Amethyst Node | Planned | Amethyst Chunks | Material | 1–2 |

Intended material progression:

`Chunks -> Crystal -> Block`

The conversion between these forms belongs to a future processing/refining system rather than the harvesting system itself.

---

## 6. Material Data Without an Assigned Harvestable

Some material data exists without a defined harvesting source.

Current examples:

- Bone
- Stone
- Purple Orb

These should not be assigned to harvestables simply because a plausible source could be invented.

For example:

- Bone may later come from enemies, remains, containers, or another system.
- Stone may later come from rocks, boulders, mining nodes, or other world objects.
- Purple Orb currently has no established source or gameplay role.

The source should be documented only when its actual gameplay relationship is designed.

---

## 7. Environmental Resources Without a Defined Drop

Some environmental or resource sprites should **not** automatically become harvestables or gameplay materials.

Current unresolved examples include:

- an unidentified feather / plant-like resource sprite;
- an unidentified bark / wood-like resource sprite;
- reeds or decorative plant elements;
- leaves or needle particles.

A world object should become harvestable only when:

1. it has a gameplay reason to be harvested;
2. it has a meaningful item drop;
3. the item participates in crafting, consumption, progression, economy, or another intentional system.

Therefore a `Reed Patch`, for example, should remain decorative until a Reed-derived item is actually needed.

---

## 8. Current Greenvale Harvesting Set

For the first Greenvale Border Path map, the intended harvestable catalogue is deliberately small:

| Harvestable | Durability | Slash | Chop | Pierce | Blunt | Drop(s) |
|---|---:|---:|---:|---:|---:|---|
| Oak Tree | 6 | 1 | 3 | 1 | 1 | Twig, Wood |
| Cherry Tree | 6 | 2 | 2 | 1 | 1 | Cherry, optional Twig |

This keeps the first harvesting tutorials readable while demonstrating the new impact-response system:

- Oak Tree teaches that `Chop` is the preferred timber impact.
- Cherry Tree accepts the same physical attacks but does not behave like a timber resource.
- Arrow hits remain possible but intentionally inefficient.
- Enemy-only skills remain unable to harvest either object.
