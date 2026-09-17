# Harvestable Design

## Purpose

This document is the authoritative catalogue of Harvestable Objects in **FlameOfTheLost**.

It defines:

- Harvestable gameplay identity;
- content status;
- harvesting response archetype;
- intended visual variants;
- direct drop entries;
- minimum and maximum drop quantity;
- independent drop chance.

Impact values for each response archetype are defined in [Impact Configuration](ImpactConfiguration.md).

---

## 1. Authoring Rules

### Gameplay identity vs variants

A different sprite does not automatically require a different Harvestable type.

A Harvestable may have multiple variants that share the same gameplay identity and harvesting response while differing in presentation or drop tuning.

Examples:

- **Oak Tree** has `Normal` and `Small` variants;
- **Birch Tree** has `Normal` and `Small` variants;
- **Spruce Tree** has `Normal` and `Small` variants;
- Small / Medium / Big fruit-tree sprites are presentation variants;
- different rock sprites may represent the same **Stone Deposit**;
- stump or broken sprites are states of the same tree;
- visual mineral variations do not create separate resource families.

The Normal and Small Timber Tree variants use the same drop **types** but different drop **chances**.

Create another core Harvestable type only when the object has a distinct gameplay identity, resource family, or harvesting behaviour. Variant-level tuning alone does not increase the core Harvestable count.

### Drop configuration

Each drop entry independently defines:

- Item / Collectible;
- Minimum Quantity;
- Maximum Quantity;
- Drop Chance.

Drop configuration may vary between variants of the same Harvestable.

Drops are not affected by the `HarvestImpactType` used to break the object unless a future design explicitly introduces impact-dependent loot.

### Processed resources

Normal mineral Harvestables directly drop only their raw **Chunks** form.

Metal progression:

`Chunks -> Ore -> Bar`

Crystal progression:

`Chunks -> Crystal -> Block`

`Ore`, `Bar`, `Crystal`, and `Block` belong to future processing/refining gameplay rather than normal direct harvesting.

---

## 2. Catalogue Summary

| Category | Harvestable | Variants | Status | Response Archetype | Max Durability | Primary Drop |
|---|---|---|---|---|---:|---|
| Timber Tree | Oak Tree | Normal, Small | Current | Timber Tree | 6 / 5 | Wood |
| Timber Tree | Birch Tree | Normal, Small | Planned | Timber Tree | 6 / 5 | Wood |
| Timber Tree | Spruce Tree | Normal, Small | Planned | Timber Tree | 6 / 5 | Wood |
| Fruit Tree | Apple Tree | Presentation variants | Planned | Fruit Tree | 6 | Apple |
| Fruit Tree | Cherry Tree | Presentation variants | Current | Fruit Tree | 6 | Cherry |
| Fruit Tree | Peach Tree | Presentation variants | Planned | Fruit Tree | 6 | Peach |
| Fruit Tree | Pear Tree | Presentation variants | Planned | Fruit Tree | 6 | Pear |
| Stone Resource | Stone Deposit | Visual variants | Planned | Mineral Node | 9 | Stone |
| Metal Node | Iron Node | Visual variants | Planned | Mineral Node | 9 | Iron Chunks |
| Metal Node | Copper Node | Visual variants | Planned | Mineral Node | 9 | Copper Chunks |
| Metal Node | Gold Node | Visual variants | Planned | Mineral Node | 9 | Gold Chunks |
| Crystal Node | Aquamarine Node | Visual variants | Planned | Mineral Node | 9 | Aquamarine Chunks |
| Crystal Node | Amber Node | Visual variants | Planned | Mineral Node | 9 | Amber Chunks |
| Crystal Node | Ruby Node | Visual variants | Planned | Mineral Node | 9 | Ruby Chunks |
| Crystal Node | Emerald Node | Visual variants | Planned | Mineral Node | 9 | Emerald Chunks |
| Crystal Node | Amethyst Node | Visual variants | Planned | Mineral Node | 9 | Amethyst Chunks |

**Total: 16 core Harvestable types**

The Normal / Small Timber Tree variants do **not** increase this count. `6 / 5` in the summary means `Normal / Small` Maximum Durability.

---

## 3. Timber Trees

Timber Trees use generic `Wood` and `Twig`. Species-specific wood materials are not currently required.

Oak, Birch, and Spruce each have two authored variants:

- `Normal`
- `Small`

Both variants:

- belong to the same Timber Tree species;
- use the same Timber Tree impact-response multipliers;
- drop the same Item types;
- use different Maximum Durability values;
- use different drop chances.

The Small variant represents a younger/lighter tree. It has slightly lower Maximum Durability, is more reliable for Twig, and is less reliable for substantial Wood.

### Variant Configuration

#### Normal Timber Tree

| Property | Value |
|---|---:|
| Maximum Durability | 6 |
| Slash | 1 |
| Chop | 3 |
| Pierce | 1 |
| Blunt | 1 |

Drops:

| Drop | Category | Min | Max | Chance |
|---|---|---:|---:|---:|
| Wood | Material | 1 | 2 | 100% |
| Twig | Material | 1 | 2 | 70% |

#### Small Timber Tree

| Property | Value |
|---|---:|
| Maximum Durability | 5 |
| Slash | 1 |
| Chop | 3 |
| Pierce | 1 |
| Blunt | 1 |

Drops:

| Drop | Category | Min | Max | Chance |
|---|---|---:|---:|---:|
| Wood | Material | 1 | 2 | 60% |
| Twig | Material | 1 | 2 | 100% |

These values apply consistently to Oak, Birch, and Spruce unless a species-specific exception is intentionally designed later.

### Oak Tree

**Status:** Current  
**Variants:** Normal, Small  
**Response Archetype:** Timber Tree

| Variant | Max Durability | Drop | Category | Min | Max | Chance |
|---|---:|---|---|---:|---:|---:|
| Normal | 6 | Wood | Material | 1 | 2 | 100% |
| Normal | 6 | Twig | Material | 1 | 2 | 70% |
| Small | 5 | Wood | Material | 1 | 2 | 60% |
| Small | 5 | Twig | Material | 1 | 2 | 100% |

### Birch Tree

**Status:** Planned  
**Variants:** Normal, Small  
**Response Archetype:** Timber Tree

| Variant | Max Durability | Drop | Category | Min | Max | Chance |
|---|---:|---|---|---:|---:|---:|
| Normal | 6 | Wood | Material | 1 | 2 | 100% |
| Normal | 6 | Twig | Material | 1 | 2 | 70% |
| Small | 5 | Wood | Material | 1 | 2 | 60% |
| Small | 5 | Twig | Material | 1 | 2 | 100% |

### Spruce Tree

**Status:** Planned  
**Variants:** Normal, Small  
**Response Archetype:** Timber Tree

| Variant | Max Durability | Drop | Category | Min | Max | Chance |
|---|---:|---|---|---:|---:|---:|
| Normal | 6 | Wood | Material | 1 | 2 | 100% |
| Normal | 6 | Twig | Material | 1 | 2 | 70% |
| Small | 5 | Wood | Material | 1 | 2 | 60% |
| Small | 5 | Twig | Material | 1 | 2 | 100% |

---

## 4. Fruit Trees

Fruit Trees primarily supply Consumable Items.

They may additionally drop a Twig at low probability, but they do not normally drop Wood.

The generic Small / Medium / Big fruit-tree sprites are visual variants rather than separate Harvestable types.

### Drop Configuration

| Harvestable | Drop | Category | Min | Max | Chance |
|---|---|---|---:|---:|---:|
| Apple Tree | Apple | Consumable Item | 1 | 3 | 100% |
| Apple Tree | Twig | Material | 1 | 1 | 20% |
| Cherry Tree | Cherry | Consumable Item | 1 | 3 | 100% |
| Cherry Tree | Twig | Material | 1 | 1 | 20% |
| Peach Tree | Peach | Consumable Item | 1 | 3 | 100% |
| Peach Tree | Twig | Material | 1 | 1 | 20% |
| Pear Tree | Pear | Consumable Item | 1 | 3 | 100% |
| Pear Tree | Twig | Material | 1 | 1 | 20% |

The Greenvale Cherry Tree is the first intended example of:

`Harvestable -> Consumable Item`

---

## 5. Stone Resource

### Stone Deposit

**Status:** Planned  
**Response Archetype:** Mineral Node

`Stone Deposit` represents generic harvestable rocks/boulders. Multiple rock sprites may share this gameplay profile.

| Drop | Category | Min | Max | Chance |
|---|---|---:|---:|---:|
| Stone | Material | 1 | 3 | 100% |

Stone Deposit provides a basic mineral/construction resource and can introduce blunt-oriented harvesting before valuable metal/crystal nodes.

---

## 6. Metal Nodes

Metal Nodes use the **Mineral Node** response archetype.

| Harvestable | Drop | Category | Min | Max | Chance |
|---|---|---|---:|---:|---:|
| Iron Node | Iron Chunks | Material | 1 | 3 | 100% |
| Copper Node | Copper Chunks | Material | 1 | 3 | 100% |
| Gold Node | Gold Chunks | Material | 1 | 2 | 100% |

Progression:

- `Iron Chunks -> Iron Ore -> Iron Bar`
- `Copper Chunks -> Copper Ore -> Copper Bar`
- `Gold Chunks -> Gold Ore -> Gold Bar`

Gold has a smaller baseline yield because it is intended as the rarer, higher-value metal family.

---

## 7. Crystal Nodes

Crystal Nodes use the **Mineral Node** response archetype.

| Harvestable | Drop | Category | Min | Max | Chance |
|---|---|---|---:|---:|---:|
| Aquamarine Node | Aquamarine Chunks | Material | 1 | 2 | 100% |
| Amber Node | Amber Chunks | Material | 1 | 2 | 100% |
| Ruby Node | Ruby Chunks | Material | 1 | 2 | 100% |
| Emerald Node | Emerald Chunks | Material | 1 | 2 | 100% |
| Amethyst Node | Amethyst Chunks | Material | 1 | 2 | 100% |

Progression:

- `Aquamarine Chunks -> Aquamarine Crystal -> Aquamarine Block`
- `Amber Chunks -> Amber Crystal -> Amber Block`
- `Ruby Chunks -> Ruby Crystal -> Ruby Block`
- `Emerald Chunks -> Emerald Crystal -> Emerald Block`
- `Amethyst Chunks -> Amethyst Crystal -> Amethyst Block`

---

## 8. Complete Drop Table

| Harvestable | Variant | Max Durability | Drop | Category | Min | Max | Chance |
|---|---|---:|---|---|---:|---:|---:|
| Oak Tree | Normal | 6 | Wood | Material | 1 | 2 | 100% |
| Oak Tree | Normal | 6 | Twig | Material | 1 | 2 | 70% |
| Oak Tree | Small | 5 | Wood | Material | 1 | 2 | 60% |
| Oak Tree | Small | 5 | Twig | Material | 1 | 2 | 100% |
| Birch Tree | Normal | 6 | Wood | Material | 1 | 2 | 100% |
| Birch Tree | Normal | 6 | Twig | Material | 1 | 2 | 70% |
| Birch Tree | Small | 5 | Wood | Material | 1 | 2 | 60% |
| Birch Tree | Small | 5 | Twig | Material | 1 | 2 | 100% |
| Spruce Tree | Normal | 6 | Wood | Material | 1 | 2 | 100% |
| Spruce Tree | Normal | 6 | Twig | Material | 1 | 2 | 70% |
| Spruce Tree | Small | 5 | Wood | Material | 1 | 2 | 60% |
| Spruce Tree | Small | 5 | Twig | Material | 1 | 2 | 100% |
| Apple Tree | — | 6 | Apple | Consumable Item | 1 | 3 | 100% |
| Apple Tree | — | 6 | Twig | Material | 1 | 1 | 20% |
| Cherry Tree | — | 6 | Cherry | Consumable Item | 1 | 3 | 100% |
| Cherry Tree | — | 6 | Twig | Material | 1 | 1 | 20% |
| Peach Tree | — | 6 | Peach | Consumable Item | 1 | 3 | 100% |
| Peach Tree | — | 6 | Twig | Material | 1 | 1 | 20% |
| Pear Tree | — | 6 | Pear | Consumable Item | 1 | 3 | 100% |
| Pear Tree | — | 6 | Twig | Material | 1 | 1 | 20% |
| Stone Deposit | — | 9 | Stone | Material | 1 | 3 | 100% |
| Iron Node | — | 9 | Iron Chunks | Material | 1 | 3 | 100% |
| Copper Node | — | 9 | Copper Chunks | Material | 1 | 3 | 100% |
| Gold Node | — | 9 | Gold Chunks | Material | 1 | 2 | 100% |
| Aquamarine Node | — | 9 | Aquamarine Chunks | Material | 1 | 2 | 100% |
| Amber Node | — | 9 | Amber Chunks | Material | 1 | 2 | 100% |
| Ruby Node | — | 9 | Ruby Chunks | Material | 1 | 2 | 100% |
| Emerald Node | — | 9 | Emerald Chunks | Material | 1 | 2 | 100% |
| Amethyst Node | — | 9 | Amethyst Chunks | Material | 1 | 2 | 100% |

---

## 9. Items Outside the Current Harvestable Catalogue

The existence of an Item or Collectible prefab does not make it a Harvestable drop.

The following remain outside this core Harvestable catalogue unless a later design intentionally adds an appropriate source:

- animal/enemy resources such as meat and Bone;
- fish;
- mushrooms;
- cooked or processed foods;
- field crops and other farm produce;
- Purple Orb.

Crops, bushes, vines, fungi, and similar gathering content can be added later as separate Harvestable families when their gameplay loop is designed.

---

## 10. Greenvale Border Path

Greenvale currently requires only:

| Harvestable | Variant | Max Durability | Response Archetype | Drops |
|---|---|---:|---|---|
| Oak Tree | Normal / Small | 6 / 5 | Timber Tree | Wood, Twig |
| Cherry Tree | Presentation variant as placed | 6 | Fruit Tree | Cherry, optional Twig |

Oak Tree's Normal / Small variant determines both Maximum Durability and drop chances. Both variants retain the same Timber Tree impact-response multipliers.

This provides two distinct tutorial relationships:

- `Oak Tree -> crafting Materials`
- `Cherry Tree -> Consumable Item`

Stone and mineral nodes can be introduced later when mining/blunt-oriented harvesting is taught.
