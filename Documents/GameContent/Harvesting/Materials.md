# Materials

## Purpose

This document defines the current material catalogue for **FlameOfTheLost**, including both materials already used in gameplay and material data prepared for later harvesting, processing, crafting, upgrading, trading, quests, or economy systems.

Materials are non-consumable item resources.

This document focuses on **game content** rather than naming or Data ID conventions. Those conventions are documented separately under `Documents/Conventions/`.

---

## 1. Natural / General Materials

### Wood

**Status:** Current

Primary sources:

- Oak Tree
- Birch Tree
- Spruce Tree

Gameplay role:

- Substantial timber resource.
- Suitable for construction, weapon components, crafting stations, and ammunition crafting.

Planned alternate Arrow recipe:

`1 Wood -> 6 Arrows`

This allows Wood to function as a higher-value alternative to Twig.

### Bone

**Status:** Data Created

Source:

- Not defined yet.

Gameplay role:

- Not defined yet.

Bone has a material data asset, but it should not be assigned to a harvestable or enemy drop until its gameplay source is intentionally designed.

### Twig

**Status:** Current

Primary sources:

- Oak Tree
- Birch Tree
- Spruce Tree

Secondary sources:

- Apple Tree
- Cherry Tree
- Peach Tree

Gameplay role:

- Very common early-game crafting material.
- Represents small branches rather than processed timber.
- Suitable for simple ammunition and primitive recipes.

Current Arrow recipe:

`1 Twig -> 3 Arrows`

Twig replaces the earlier semantic use of `ReedStalk` / `PlantFibre` for the basic Arrow recipe.

### Purple Orb

**Status:** Data Created

Source:

- Not defined yet.

Gameplay role:

- Not defined yet.

The sprite and material data exist, but its actual gameplay identity should remain undefined until it is connected to a system, world source, recipe, progression mechanic, or other content.

### Stone

**Status:** Data Created

Source:

- Not defined yet.

Gameplay role:

- Candidate generic mineral/construction resource.
- No harvestable source has been established yet.

Stone should not automatically be assigned to a rock or mining node until that gameplay relationship is explicitly designed.

---

## 2. Metal Materials

Metal resources are represented by three families:

- Iron
- Copper
- Gold

Each family contains three distinct material forms:

`Chunks -> Ore -> Bar`

The current intended interpretation is:

- **Chunks** — raw material obtained from a mineral node.
- **Ore** — processed or consolidated intermediate material.
- **Bar** — refined material intended for later crafting, equipment, upgrades, or economy systems.

The processing system that converts between these forms has not yet been implemented.

### Iron

**Status:** Data Created

Materials:

- Iron Chunks
- Iron Ore
- Iron Bar

Primary harvestable:

- Iron Node

Current intended direct harvest drop:

- Iron Chunks

Role:

- Common structural metal family.
- Candidate for weapons, tools, armour, and upgrades.

### Copper

**Status:** Data Created

Materials:

- Copper Chunks
- Copper Ore
- Copper Bar

Primary harvestable:

- Copper Node

Current intended direct harvest drop:

- Copper Chunks

Role:

- Early metal and component family.
- Candidate for simple equipment and utility recipes.

### Gold

**Status:** Data Created

Materials:

- Gold Chunks
- Gold Ore
- Gold Bar

Primary harvestable:

- Gold Node

Current intended direct harvest drop:

- Gold Chunks

Role:

- Rare crafting/value material family.
- Distinct from the Lost Embers currency.

---

## 3. Crystal Materials

Crystal resources are represented by five families:

- Aquamarine
- Amber
- Ruby
- Emerald
- Amethyst

Each family contains three distinct material forms:

`Chunks -> Crystal -> Block`

The current intended interpretation is:

- **Chunks** — raw crystal fragments obtained from a mineral node.
- **Crystal** — processed or consolidated crystal material.
- **Block** — refined high-density material intended for later crafting, upgrading, magical systems, trading, or specialist recipes.

The processing system that converts between these forms has not yet been implemented.

### Aquamarine

**Status:** Data Created

Materials:

- Aquamarine Chunks
- Aquamarine Crystal
- Aquamarine Block

Primary harvestable:

- Aquamarine Node

Current intended direct harvest drop:

- Aquamarine Chunks

Role:

- Cyan/blue crystal family.
- Future water or magical recipes are possible.

### Amber

**Status:** Data Created

Materials:

- Amber Chunks
- Amber Crystal
- Amber Block

Primary harvestable:

- Amber Node

Current intended direct harvest drop:

- Amber Chunks

Role:

- Orange crystal family.
- Future elemental or specialist recipes are possible.

### Ruby

**Status:** Data Created

Materials:

- Ruby Chunks
- Ruby Crystal
- Ruby Block

Primary harvestable:

- Ruby Node

Current intended direct harvest drop:

- Ruby Chunks

Role:

- Red crystal family.
- Future fire or offensive recipes are possible.

### Emerald

**Status:** Data Created

Materials:

- Emerald Chunks
- Emerald Crystal
- Emerald Block

Primary harvestable:

- Emerald Node

Current intended direct harvest drop:

- Emerald Chunks

Role:

- Green crystal family.
- Future magical, nature, or poison-related recipes are possible.

### Amethyst

**Status:** Data Created

Materials:

- Amethyst Chunks
- Amethyst Crystal
- Amethyst Block

Primary harvestable:

- Amethyst Node

Current intended direct harvest drop:

- Amethyst Chunks

Role:

- Purple crystal family.
- Future arcane or status-effect recipes are possible.

---

## 4. Material Catalogue

| Material | Status | Primary Source | Direct Harvest Drop? |
|---|---|---|---|
| Wood | Current | Timber Trees | Yes |
| Bone | Data Created | Not defined | Not defined |
| Twig | Current | Timber Trees | Yes |
| Purple Orb | Data Created | Not defined | Not defined |
| Stone | Data Created | Not defined | Not defined |
| Iron Chunks | Data Created | Iron Node | Yes |
| Iron Ore | Data Created | Processing | No |
| Iron Bar | Data Created | Processing | No |
| Copper Chunks | Data Created | Copper Node | Yes |
| Copper Ore | Data Created | Processing | No |
| Copper Bar | Data Created | Processing | No |
| Gold Chunks | Data Created | Gold Node | Yes |
| Gold Ore | Data Created | Processing | No |
| Gold Bar | Data Created | Processing | No |
| Aquamarine Chunks | Data Created | Aquamarine Node | Yes |
| Aquamarine Crystal | Data Created | Processing | No |
| Aquamarine Block | Data Created | Processing | No |
| Amber Chunks | Data Created | Amber Node | Yes |
| Amber Crystal | Data Created | Processing | No |
| Amber Block | Data Created | Processing | No |
| Ruby Chunks | Data Created | Ruby Node | Yes |
| Ruby Crystal | Data Created | Processing | No |
| Ruby Block | Data Created | Processing | No |
| Emerald Chunks | Data Created | Emerald Node | Yes |
| Emerald Crystal | Data Created | Processing | No |
| Emerald Block | Data Created | Processing | No |
| Amethyst Chunks | Data Created | Amethyst Node | Yes |
| Amethyst Crystal | Data Created | Processing | No |
| Amethyst Block | Data Created | Processing | No |

---

## 5. Materials Deliberately Not Defined Yet

The following should not be created only because an environmental or resource sprite could visually support them:

- Reed Stalk
- Plant Fibre
- Oak Wood
- Birch Wood
- Spruce Wood
- Bark
- Leaves
- Spruce Needles
- Unidentified feather / plant-like resource
- Unidentified bark / wood-like resource

These can be added later when a recipe, harvesting loop, enemy drop, progression system, or economy role actually requires them.

Keeping the material catalogue intentional avoids inventory clutter and prevents visual assets from defining gameplay content before the design is established.
