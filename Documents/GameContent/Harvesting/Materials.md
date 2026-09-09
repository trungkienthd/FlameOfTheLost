# Materials

## Purpose

This document defines the current material catalogue for **FlameOfTheLost** and the harvestables that supply each material.

Asset naming should follow the existing item convention:

`Mtr_<MaterialName>`

Materials are non-consumable crafting resources. They may later participate in crafting, upgrading, trading, quest requirements, or other systems.

---

## 1. Natural Materials

### Twig

**Status:** Current

Suggested asset:

`Mtr_Twig`

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

This replaces the old semantic use of `ReedStalk` / `PlantFibre` for the existing early Arrow recipe.

### Wood

**Status:** Current

Asset:

`Mtr_Wood`

Primary sources:

- Oak Tree
- Birch Tree
- Spruce Tree

Gameplay role:

- More substantial timber than Twig.
- Less frequent per tree harvest.
- Suitable for construction, weapon components, crafting stations, and efficient ammunition crafting.

Planned alternate Arrow recipe:

`1 Wood -> 6 Arrows`

This allows Wood to function as a higher-value alternative to Twig without removing the basic Twig recipe.

---

## 2. Mineral Materials

The ore sprite set supports eight mineral families.

These materials are **planned**. They are documented now so the available visual content has a clear gameplay destination; they do not all need to be introduced during Greenvale.

### Iron

Suggested asset:

`Mtr_Iron`

Source:

- Iron Node

Role:

- Common structural metal.
- Candidate for weapons, tools, armour, and upgrades.

### Copper

Suggested asset:

`Mtr_Copper`

Source:

- Copper Node

Role:

- Early metal and component material.
- Candidate for simple equipment or utility recipes.

### Gold

Suggested asset:

`Mtr_Gold`

Source:

- Gold Node

Role:

- Rare crafting/value material.
- Distinct from the Lost Embers currency.

### Emerald

Suggested asset:

`Mtr_Emerald`

Source:

- Emerald Node

Role:

- Green crystal/gem material.
- Future magical, nature, or poison-related recipes are possible.

### Aquamarine

Suggested asset:

`Mtr_Aquamarine`

Source:

- Aquamarine Node

Role:

- Cyan/blue crystal material.
- Future water or magical recipes are possible.

### Amber

Suggested asset:

`Mtr_Amber`

Source:

- Amber Node

Role:

- Orange mineral/gem material.
- Future elemental or specialist recipes are possible.

### Ruby

Suggested asset:

`Mtr_Ruby`

Source:

- Ruby Node

Role:

- Red crystal/gem material.
- Future fire or offensive recipes are possible.

### Amethyst

Suggested asset:

`Mtr_Amethyst`

Source:

- Amethyst Node

Role:

- Purple crystal/gem material.
- Future arcane/status-effect recipes are possible.

---

## 3. Material Catalogue

| Material | Suggested Asset | Status | Primary Harvestable |
|---|---|---|---|
| Twig | `Mtr_Twig` | Current | Timber Trees |
| Wood | `Mtr_Wood` | Current | Timber Trees |
| Iron | `Mtr_Iron` | Planned | Iron Node |
| Copper | `Mtr_Copper` | Planned | Copper Node |
| Gold | `Mtr_Gold` | Planned | Gold Node |
| Emerald | `Mtr_Emerald` | Planned | Emerald Node |
| Aquamarine | `Mtr_Aquamarine` | Planned | Aquamarine Node |
| Amber | `Mtr_Amber` | Planned | Amber Node |
| Ruby | `Mtr_Ruby` | Planned | Ruby Node |
| Amethyst | `Mtr_Amethyst` | Planned | Amethyst Node |

---

## 4. Materials Deliberately Not Added Yet

The following should not be created only because an environmental sprite could support them:

- Reed Stalk
- Plant Fibre
- Oak Wood
- Birch Wood
- Spruce Wood
- Bark
- Leaves
- Spruce Needles

These can be added later when a recipe or gameplay loop actually requires them.

Keeping the material catalogue compact avoids inventory clutter and makes each new material mechanically meaningful.
