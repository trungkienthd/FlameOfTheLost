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

### Gameplay identity vs visual variants

A different sprite does not automatically require a different Harvestable profile.

Examples:

- large and small Oak sprites are variants of **Oak Tree**;
- Small / Medium / Big fruit-tree sprites are presentation variants;
- different rock sprites may represent the same **Stone Deposit**;
- stump or broken sprites are states of the same tree;
- visual mineral variations do not create separate resource families.

Create another Harvestable type only when the object has a distinct gameplay identity, drop table, or harvesting behaviour.

### Drop configuration

Each drop entry independently defines:

- Item / Collectible;
- Minimum Quantity;
- Maximum Quantity;
- Drop Chance.

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

| Category | Harvestable | Status | Response Archetype | Primary Drop |
|---|---|---|---|---|
| Timber Tree | Oak Tree | Current | Timber Tree | Wood |
| Timber Tree | Birch Tree | Planned | Timber Tree | Wood |
| Timber Tree | Spruce Tree | Planned | Timber Tree | Wood |
| Fruit Tree | Apple Tree | Planned | Fruit Tree | Apple |
| Fruit Tree | Cherry Tree | Current | Fruit Tree | Cherry |
| Fruit Tree | Peach Tree | Planned | Fruit Tree | Peach |
| Fruit Tree | Pear Tree | Planned | Fruit Tree | Pear |
| Stone Resource | Stone Deposit | Planned | Mineral Node | Stone |
| Metal Node | Iron Node | Planned | Mineral Node | Iron Chunks |
| Metal Node | Copper Node | Planned | Mineral Node | Copper Chunks |
| Metal Node | Gold Node | Planned | Mineral Node | Gold Chunks |
| Crystal Node | Aquamarine Node | Planned | Mineral Node | Aquamarine Chunks |
| Crystal Node | Amber Node | Planned | Mineral Node | Amber Chunks |
| Crystal Node | Ruby Node | Planned | Mineral Node | Ruby Chunks |
| Crystal Node | Emerald Node | Planned | Mineral Node | Emerald Chunks |
| Crystal Node | Amethyst Node | Planned | Mineral Node | Amethyst Chunks |

**Total: 16 core Harvestable types**

---

## 3. Timber Trees

Timber Trees use generic `Wood` and `Twig`. Species-specific wood materials are not currently required.

### Shared visual rule

Each species may use large/small variants and a broken/stump state without creating additional Harvestable types.

### Drop Configuration

| Harvestable | Drop | Category | Min | Max | Chance |
|---|---|---|---:|---:|---:|
| Oak Tree | Wood | Material | 1 | 2 | 100% |
| Oak Tree | Twig | Material | 1 | 2 | 70% |
| Birch Tree | Wood | Material | 1 | 2 | 100% |
| Birch Tree | Twig | Material | 1 | 2 | 70% |
| Spruce Tree | Wood | Material | 1 | 2 | 100% |
| Spruce Tree | Twig | Material | 1 | 2 | 70% |

Wood is the primary guaranteed timber resource. Twig is a common secondary branch resource.

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

| Harvestable | Drop | Category | Min | Max | Chance |
|---|---|---|---:|---:|---:|
| Oak Tree | Wood | Material | 1 | 2 | 100% |
| Oak Tree | Twig | Material | 1 | 2 | 70% |
| Birch Tree | Wood | Material | 1 | 2 | 100% |
| Birch Tree | Twig | Material | 1 | 2 | 70% |
| Spruce Tree | Wood | Material | 1 | 2 | 100% |
| Spruce Tree | Twig | Material | 1 | 2 | 70% |
| Apple Tree | Apple | Consumable Item | 1 | 3 | 100% |
| Apple Tree | Twig | Material | 1 | 1 | 20% |
| Cherry Tree | Cherry | Consumable Item | 1 | 3 | 100% |
| Cherry Tree | Twig | Material | 1 | 1 | 20% |
| Peach Tree | Peach | Consumable Item | 1 | 3 | 100% |
| Peach Tree | Twig | Material | 1 | 1 | 20% |
| Pear Tree | Pear | Consumable Item | 1 | 3 | 100% |
| Pear Tree | Twig | Material | 1 | 1 | 20% |
| Stone Deposit | Stone | Material | 1 | 3 | 100% |
| Iron Node | Iron Chunks | Material | 1 | 3 | 100% |
| Copper Node | Copper Chunks | Material | 1 | 3 | 100% |
| Gold Node | Gold Chunks | Material | 1 | 2 | 100% |
| Aquamarine Node | Aquamarine Chunks | Material | 1 | 2 | 100% |
| Amber Node | Amber Chunks | Material | 1 | 2 | 100% |
| Ruby Node | Ruby Chunks | Material | 1 | 2 | 100% |
| Emerald Node | Emerald Chunks | Material | 1 | 2 | 100% |
| Amethyst Node | Amethyst Chunks | Material | 1 | 2 | 100% |

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

| Harvestable | Response Archetype | Drops |
|---|---|---|
| Oak Tree | Timber Tree | Wood, Twig |
| Cherry Tree | Fruit Tree | Cherry, optional Twig |

This provides two distinct tutorial relationships:

- `Oak Tree -> crafting Materials`
- `Cherry Tree -> Consumable Item`

Stone and mineral nodes can be introduced later when mining/blunt-oriented harvesting is taught.
