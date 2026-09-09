# Harvestables

## Purpose

This document defines the harvestable content currently available from the project's environment sprite set and the items each harvestable is intended to produce.

For now, harvestable drops are limited to:

- **Material**
- **Consumable Item**

Drop quantities and probabilities are baseline design values and can be tuned during balancing.

---

## 1. Timber Trees

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

The current Section 2 Oak Tree should use this table.

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

The Cherry Tree in Greenvale Border Path Section 3 should be the first clear **Harvestable → Consumable Item** tutorial example.

### Peach Tree

**Status:** Planned

Drops:

| Drop | Category | Quantity | Chance | Notes |
|---|---|---:|---:|---|
| Peach | Consumable Item | 1–3 | 100% | Primary produce |
| Twig | Material | 1 | 20% | Optional secondary drop |

Fruit trees do not normally drop `Wood`; harvesting fruit should remain mechanically distinct from chopping timber trees.

---

## 3. Berry Bushes

The available sprite sheet contains two berry families.

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

The following crop families are represented by the supplied sprites.

| Harvestable | Status | Primary Drop | Category | Baseline Yield |
|---|---|---|---|---:|
| Wheat | Planned | Wheat | Consumable Item | 1–3 |
| Tomato Plant | Planned | Tomato | Consumable Item | 1–3 |
| Carrot Crop | Planned | Carrot | Consumable Item | 1–3 |
| Eggplant Crop | Planned | Eggplant | Consumable Item | 1–3 |
| Corn Crop | Planned | Corn | Consumable Item | 1–3 |
| Pumpkin Crop | Planned | Pumpkin | Consumable Item | 1 |
| Parsnip Crop | Planned | Parsnip | Consumable Item | 1–3 |
| Cabbage Crop | Planned | Cabbage | Consumable Item | 1 |
| Cucumber Crop | Planned | Cucumber | Consumable Item | 1–3 |
| Chili Crop | Planned | Chili Pepper | Consumable Item | 1–3 |
| Red Pepper Crop | Planned | Red Pepper | Consumable Item | 1–3 |
| Orange Pepper Crop | Planned | Orange Pepper | Consumable Item | 1–3 |
| Green Pepper Crop | Planned | Green Pepper | Consumable Item | 1–3 |
| Blue Squash Crop | Planned / working name | Blue Squash | Consumable Item | 1 |

`Blue Squash` is a working name based on the final dark teal round crop sprite and should be renamed if the art pack provides an authoritative name.

---

## 5. Mineral Nodes

The supplied ore sprites contain eight visual mineral families. Each family includes multiple node/fragment presentations, but those are treated as visual variations of one harvestable family.

| Harvestable | Status | Drop | Category | Baseline Yield |
|---|---|---|---|---:|
| Iron Node | Planned | Iron | Material | 1–3 |
| Copper Node | Planned | Copper | Material | 1–3 |
| Gold Node | Planned | Gold | Material | 1–2 |
| Emerald Node | Planned | Emerald | Material | 1–2 |
| Aquamarine Node | Planned | Aquamarine | Material | 1–2 |
| Amber Node | Planned | Amber | Material | 1–2 |
| Ruby Node | Planned | Ruby | Material | 1–2 |
| Amethyst Node | Planned | Amethyst | Material | 1–2 |

These mineral names are the current gameplay names selected to match the available colour families. They can be replaced by setting-specific fantasy names later without changing the harvesting categories.

---

## 6. Environmental Plants Without a Defined Drop

Some environmental sprites, such as reeds or decorative leaf/needle particles, should **not** automatically become harvestables.

A world object should become harvestable only when:

1. it has a gameplay reason to be harvested;
2. it has a meaningful item drop;
3. the item participates in crafting, consumption, progression, or economy.

Therefore a `Reed Patch` should remain decorative until a Reed-derived material or consumable is actually needed.

---

## 7. Current Greenvale Harvesting Set

For the first Greenvale Border Path map, the intended harvestable catalogue is deliberately small:

| Harvestable | Drop(s) |
|---|---|
| Oak Tree | Twig, Wood |
| Cherry Tree | Cherry, optional Twig |

This keeps the first harvesting tutorials readable before the broader material and farming catalogue is introduced.
