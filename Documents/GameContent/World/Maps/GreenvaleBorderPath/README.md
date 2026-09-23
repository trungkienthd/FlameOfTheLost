# Greenvale Border Path — Map Design

## 1. Purpose

This folder contains the map-design documentation for **Greenvale Border Path**.

Greenvale Border Path is structured as a multi-section introductory map. The
documentation is separated by section so that each part of the map can evolve
independently while remaining part of one coherent map design.

The current detailed documentation covers **Section 1** and **Section 2**.

Section 3 remains reserved for future detailed design.

---

## 2. Document Structure

- `GreenvaleBorderPathDesign.md` — map-level purpose, progression, section structure, and shared design principles.
- `Sections/Section1Design.md` — detailed design for Section 1.
- `Sections/Section2Design.md` — detailed design for Section 2.
- `Sections/Section3Design.md` — reserved for future Section 3 documentation.
- `Images/` — map references and design sketches used by the Markdown documents.

---

## 3. Current Section Status

| Section | Documentation Status | Current Focus |
|---|---|---|
| Section 1 | Active | Spawn, first Item Container, death, respawn, Scenery Objects |
| Section 2 | Active | Harvesting, Materials, Bow / Arrows, Crafting, first enemy, Scenery Objects |
| Section 3 | Reserved | Future detailed documentation |

---

## 4. Current Tutorial Progression

```text
Section 1
Spawn
→ First Item Container
→ Worn Short Sword
→ Death Trigger
→ Respawn

Section 2
Harvest Oak
→ Obtain Wood / Twig
→ Loose Twig Collectible
→ Obtain Bow / Arrows
→ Use Makeshift Worktable
→ Craft Arrows
→ Fight first Feralshroom Basher

Section 3
Reserved for future detailed design
```

---

## 5. Documentation Rule

Section-specific gameplay content should be documented in the corresponding
section file rather than duplicated in the map-level document.

Each section document should describe, as applicable:

- section purpose;
- player-learning goals;
- important locations;
- World Objects and gameplay content;
- Harvestables;
- Collectibles;
- Item Containers;
- Crafting Stations;
- Enemies;
- pathing and spatial structure;
- scenery and visual composition;
- transitions to adjacent sections;
- implementation notes;
- supporting images or sketches.

---

## 6. Image Convention

Section images are stored under:

`Images/`

Current naming:

```text
Section1_CurrentImplementation.png
Section1_SceneryPlacementSketch.png
Section2_CurrentImplementation.png
Section2_SceneryPlacementSketch.png
```

Section documents reference these images with relative Markdown paths.

The implementation images represent the current Unity state.

The scenery-placement sketches communicate design intent and should not be
treated as strict tile-by-tile placement specifications.
