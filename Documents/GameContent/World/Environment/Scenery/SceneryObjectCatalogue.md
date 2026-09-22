# Scenery Object Catalogue

## 1. Overview

This document catalogues the Scenery Object families currently available for
use in FlameOfTheLost environments.

Scenery Objects are non-interactable Environment Objects used for visual
composition and environmental storytelling.

For gameplay rules and placement principles, see:

`SceneryObjectDesign.md`

---

## 2. Current Scenery Families

| Category | Current Use | Interaction | Collision |
|---|---|---|---|
| Ground Flora | Grass, sprouts, small vegetation | None | None |
| Wildflowers | Coloured decorative flowers | None | None |
| Wetland Flora | Lily pads and aquatic vegetation | None | None |
| Natural Debris | Branches and minor natural debris | None | None |
| Mushrooms | Decorative forest fungi | None | None |
| Shrubs | Medium-sized vegetation | None | None by default |
| Fallen Wood | Logs and decaying wood | None | None by default |

---

## 3. Ground Flora

Ground Flora contains small vegetation intended for common use throughout
natural environments.

Typical placement:

- grass fields;
- path edges;
- beneath trees;
- around shrubs;
- near terrain boundaries.

Ground Flora should form the most frequently used Scenery family.

---

## 4. Wildflowers

Wildflowers provide stronger colour accents than Ground Flora.

Current visual groups may include:

- Red;
- Purple;
- Blue;
- Yellow;
- Mixed / neutral flower variants.

Wildflowers should generally be used more sparingly than ordinary Ground Flora.

---

## 5. Wetland Flora

Wetland Flora currently includes decorative aquatic vegetation such as
lily pads.

Typical placement:

- ponds;
- shallow water;
- wetland areas;
- calm river edges.

Harvestable Reed Patches are not part of this category.

---

## 6. Natural Debris

Natural Debris consists of small non-interactable environmental fragments.

Examples:

- branches;
- twigs;
- woody fragments.

Stone-like sprites should only be included when they are sufficiently
distinguishable from Harvestable Stones.

---

## 7. Mushrooms

Decorative Mushroom sprites may be used underneath trees, near fallen wood,
or in damp and shaded environmental regions.

They currently have no gameplay functionality.

---

## 8. Shrubs

Shrubs are larger visual fillers used to create variation in vegetation
density.

Typical placement:

- terrain edges;
- forest boundaries;
- unused corners;
- areas transitioning between open terrain and dense vegetation.

---

## 9. Fallen Wood

Fallen Wood includes non-interactable logs and related woody scenery.

Variants may visually contain:

- plain bark;
- sprouts;
- mushrooms;
- flowers or coloured natural growth.

These objects are primarily used as larger visual anchors and for
environmental storytelling.

---

## 10. Reserved / Excluded Sprites

The following sprite types should not currently be treated as Scenery Objects:

### Harvestable Stones

Stone sprites already used by the Harvestable system remain gameplay assets.

They should not also appear as visually identical non-interactable scenery.

### Future Gameplay Resources

Any sprite promoted to an interactable, Collectible, Harvestable, or other
gameplay-oriented Environment Object should be reconsidered before continuing
to use identical scenery variants.