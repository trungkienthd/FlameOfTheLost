# Scenery Object Catalogue

## 1. Overview

This document catalogues the Scenery Object families currently available for
use in FlameOfTheLost environments.

Scenery Objects are non-interactable Environment Objects used for visual
composition and environmental storytelling.

For gameplay rules and placement principles, see:

`SceneryObjectDesign.md`

For the exact source-sprite names, see:

`OutdoorDecorSpriteMapping.md`

---

## 2. Current Scenery Families

| Category | Current Content | Interaction | Collision |
|---|---|---|---|
| Ground Flora | Leaves, sprout, carrot top | None | None |
| Wildflowers | 3 flowering plant variants | None | None |
| Wetland Flora | Lily Pad variants | None | None |
| Mushrooms | Cap Mushrooms and Brown Stem Mushrooms | None | None |
| Shrubs | Round Small and Dense Large | None | None by default |
| Fallen Wood | Logs and Broken Stumps | None | None by default |

---

## 3. Ground Flora

Ground Flora contains small vegetation intended for common use throughout
natural environments.

### Current Objects

- `GroundFloraLeaf_1`
- `GroundFloraLeaf_2`
- `GroundFloraLeaf_3`
- `GroundFloraSprout`
- `GroundFloraCarrotTop`

Typical placement:

- grass fields;
- path edges;
- beneath trees;
- around shrubs;
- near terrain boundaries.

Ground Flora should form the most frequently used Scenery family.

---

## 4. Wildflowers

Wildflowers provide stronger colour accents than ordinary Ground Flora.

### Current Objects

- `Wildflower_1`
- `Wildflower_2`
- `Wildflower_3`

Wildflowers should generally be used more sparingly than ordinary Ground Flora.

---

## 5. Wetland Flora

Wetland Flora currently consists of decorative Lily Pad variants.

### Current Objects

- `WetlandFloraLilyPadSingle`
- `WetlandFloraLilyPadDouble`
- `WetlandFloraLilyPadFlowering`
- `WetlandFloraLilyPadClusterFlowering`

Typical placement:

- ponds;
- shallow water;
- wetland areas;
- calm river edges.

Harvestable Reed Patches are not part of this category.

---

## 6. Mushrooms

Decorative Mushrooms are divided into two initial visual families.

### Cap Mushrooms

- `MushroomRedCapLarge`
- `MushroomPurpleCapLarge`
- `MushroomBlueCapLarge`
- `MushroomRedCapSmall`
- `MushroomPurpleCapSmall`
- `MushroomBlueCapSmall`

### Brown Stem Mushrooms

- `Mushroom_BrownStemLarge`
- `Mushroom_BrownStemSmall`

Typical placement:

- beneath trees;
- beside Fallen Wood;
- damp areas;
- shaded vegetation;
- forest edges.

They currently have no gameplay functionality.

---

## 7. Shrubs

Shrubs are larger visual fillers used to create variation in vegetation
density.

### Current Objects

- `ShrubRoundSmall`
- `ShrubDenseLarge`

Typical placement:

- terrain edges;
- forest boundaries;
- unused corners;
- areas transitioning between open terrain and dense vegetation.

---

## 8. Fallen Wood

Fallen Wood includes non-interactable logs and broken stumps.

### Logs

- `FallenWoodLogPlain`
- `FallenWoodLogSprouting_01`
- `FallenWoodLogSprouting_02`
- `FallenWoodLogPurpleFungus`
- `FallenWoodLogRedFungus`
- `FallenWoodLogBlueFungus`

### Broken Stumps

- `FallenWoodBrokenStumpPlain`
- `FallenWoodBrokenStumpSprouting`
- `FallenWoodBrokenStumpHollow`

These objects are primarily used as larger visual anchors and for
environmental storytelling.

---

## 9. Reserved / Excluded Sprites

Sprites from `CFRPG16_Outdoor_Decor` that are already assigned to the
Harvestable system are not Scenery Objects.

This currently includes Stone variants such as:

- Stone Small variants;
- Stone Normal;
- Stone Large;
- Water Stone variants.

Their existing Harvestable naming and gameplay role should be preserved.

Identical Harvestable sprites should not also be used as non-interactable
Scenery because doing so would weaken gameplay readability.

### Future Gameplay Resources

Any Scenery sprite later promoted to an interactable, Collectible, Harvestable,
or other gameplay-oriented Environment Object should be reconsidered before
continuing to use an identical non-interactable version.
