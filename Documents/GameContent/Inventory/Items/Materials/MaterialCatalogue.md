# Material Catalogue

## Purpose

This document defines the current authored **Material Item** catalogue for **FlameOfTheLost**.

The current data set contains **29 Material Item profiles**. Materials are inventory resources used by crafting, processing, harvesting, loot, and other gameplay systems. Their inventory identity is separate from the system that provides them.

The Material profile assets are currently stored in one flat folder with no subfolders.

---

## 1. Naming and Data Rules

Material Item asset naming:

`<ItemID>_Mtr_<MaterialName>`

Examples:

- `75001_Mtr_Wood`
- `75102_Mtr_IronOre`
- `75208_Mtr_RubyCrystal`

Material identity prefix:

`Mtr_`

Current Material Item IDs use three visible groups:

| ID Range | Current Role |
|---|---|
| `750xx` | General / basic Materials |
| `751xx` | Metal Materials |
| `752xx` | Crystal Materials |

The current catalogue intentionally does not require subfolders. The ID and `Mtr_` identity are sufficient to distinguish the authored Material profiles at the present scale.

---

## 2. General Materials

| Item ID | Material | Current Role / Source |
|---:|---|---|
| 75001 | Wood | Timber resource; direct Harvestable drop |
| 75002 | Bone | Current Material identity; acquisition/use not defined in this document |
| 75003 | Twig | Timber resource; direct Harvestable drop |
| 75006 | Purple Orb | Current Material identity; acquisition/use not defined in this document |
| 75007 | Stone | Stone resource; direct Harvestable drop |

### Timber Relationship

Current Timber Trees can provide:

- `Wood`
- `Twig`

Their exact quantities and chances belong to the Harvesting documentation rather than this inventory catalogue.

### Stone Relationship

Stone-like Harvestables provide:

- `Stone`

Exact Harvestable variants, durability, and drop quantities remain defined by the Harvesting documentation.

---

## 3. Metal Materials

Each current metal family contains three Material stages:

`Chunks -> Ore -> Bar`

| Item ID | Material | Stage | Normal Direct Harvestable Drop |
|---:|---|---|---|
| 75101 | Iron Chunks | Chunks | Yes |
| 75102 | Iron Ore | Ore | Yes, optional |
| 75103 | Iron Bar | Bar | No |
| 75104 | Copper Chunks | Chunks | Yes |
| 75105 | Copper Ore | Ore | Yes, optional |
| 75106 | Copper Bar | Bar | No |
| 75107 | Gold Chunks | Chunks | Yes |
| 75108 | Gold Ore | Ore | Yes, optional |
| 75109 | Gold Bar | Bar | No |

### Metal Progression Rule

Current mineral progression is:

`Metal Node -> Chunks + optional Ore -> processing -> Bar`

Rules:

1. Metal Nodes guarantee their corresponding `Chunks`.
2. Metal Nodes may additionally drop their corresponding `Ore`.
3. `Bar` is a processed Material and must not be a normal direct Harvestable drop.
4. Exact node variants, quantities, drop chances, durability, and impact responses belong to the Harvesting documentation.

---

## 4. Crystal Materials

Each current crystal family contains three Material stages:

`Chunks -> Crystal -> Block`

| Item ID | Material | Stage | Normal Direct Harvestable Drop |
|---:|---|---|---|
| 75201 | Aquamarine Chunks | Chunks | Yes |
| 75202 | Aquamarine Crystal | Crystal | Yes, optional |
| 75203 | Aquamarine Block | Block | No |
| 75204 | Amber Chunks | Chunks | Yes |
| 75205 | Amber Crystal | Crystal | Yes, optional |
| 75206 | Amber Block | Block | No |
| 75207 | Ruby Chunks | Chunks | Yes |
| 75208 | Ruby Crystal | Crystal | Yes, optional |
| 75209 | Ruby Block | Block | No |
| 75210 | Emerald Chunks | Chunks | Yes |
| 75211 | Emerald Crystal | Crystal | Yes, optional |
| 75212 | Emerald Block | Block | No |
| 75213 | Amethyst Chunks | Chunks | Yes |
| 75214 | Amethyst Crystal | Crystal | Yes, optional |
| 75215 | Amethyst Block | Block | No |

### Crystal Progression Rule

Current mineral progression is:

`Crystal Node -> Chunks + optional Crystal -> processing -> Block`

Rules:

1. Crystal Nodes guarantee their corresponding `Chunks`.
2. Crystal Nodes may additionally drop their corresponding `Crystal`.
3. `Block` is a processed Material and must not be a normal direct Harvestable drop.
4. Exact node variants, quantities, drop chances, durability, and impact responses belong to the Harvesting documentation.

---

## 5. Catalogue Summary

| Family | Material Profiles |
|---|---:|
| General / Basic | 5 |
| Metals | 9 |
| Crystals | 15 |
| **Total** | **29** |

Current mineral families:

### Metals

- Iron
- Copper
- Gold

### Crystals

- Aquamarine
- Amber
- Ruby
- Emerald
- Amethyst

Each mineral family currently has exactly three inventory Material stages.

---

## 6. Inventory vs World-Source Boundary

A Material Item defines the inventory resource itself. It does not define the object or system that supplies it.

For example:

`Iron Node -> Iron Chunks / Iron Ore`

The Harvestable Object owns the world-side durability, impact response, variant, and drop configuration. The Material Item remains the inventory identity received from that drop.

This distinction also allows Materials to come from other systems later, such as:

- crafting or processing;
- enemies;
- containers;
- vendors;
- quests;
- scripted rewards.

A new source does not require a duplicate Material Item identity.

---

## 7. Authoring Rules

1. Do not create separate Material profiles for different acquisition sources.
2. Keep the `Mtr_` identity consistent across data and related content.
3. Use the existing Material Item when another system needs the same resource.
4. Harvesting-specific values belong in the Harvesting documents, not in this catalogue.
5. Processed metal `Bar` items are not normal direct Harvestable drops.
6. Processed crystal `Block` items are not normal direct Harvestable drops.
7. Do not assign an acquisition, crafting, or progression role to a Material until that role is actually designed or authored.
8. Add a separate Material design/configuration document only when the Material system gains enough shared gameplay rules to justify one.

---

## 8. Current Documentation Boundary

This file is intentionally the only Material-specific GameContent document for now.

A separate `README.md` is not needed while the Materials folder contains only this single authoritative document.

A separate configuration or gameplay-design document should be added later only if Materials gain substantial shared systems such as:

- processing recipes and conversion ratios;
- rarity or quality tiers;
- sell/value economy;
- crafting-category rules;
- material-specific gameplay properties;
- large-scale source/progression matrices.

Until then, this catalogue should remain the concise authority for Material identities and their current high-level progression relationships.
