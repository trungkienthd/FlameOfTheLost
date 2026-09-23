# Greenvale Border Path Design

## 1. Overview

**Greenvale Border Path** is an introductory map in FlameOfTheLost.

The map is divided into three sections. Each section is documented separately
so that gameplay teaching, world content, and visual composition can be refined
without making the overall map document overly large.

The current detailed design covers **Section 1** and **Section 2**.

Section 3 remains reserved for future detailed documentation.

---

## 2. Section Structure

```text
Greenvale Border Path
├── Section 1
│   └── Spawn, first Item Container, death / respawn, basic Scenery
├── Section 2
│   └── Harvesting, Materials, Bow / Arrows, Crafting, first enemy
└── Section 3
    └── Reserved for future detailed documentation
```

The sections intentionally increase gameplay complexity gradually.

---

## 3. Shared Map Design Principles

### 3.1 Progressive Gameplay Introduction

Greenvale Border Path should introduce mechanics gradually.

A section should avoid presenting too many new gameplay object types at once.
The environment may become visually richer through Scenery Objects without
necessarily adding new mechanics.

Current progression:

```text
Section 1
Spawn / Item Container / Death / Respawn
    ↓
Section 2
Harvest / Materials / Bow / Arrows / Craft / First Enemy
    ↓
Section 3
Future expanded Greenvale systems
```

### 3.2 Gameplay Readability

Gameplay-relevant World Objects must remain easy to identify.

Scenery should support the environment without obscuring or visually competing
with:

- Spawn Points;
- Item Containers;
- Death Triggers;
- Crafting Stations;
- Harvestables;
- Collectibles;
- Enemies;
- Checkpoints;
- important paths and transitions.

### 3.3 Section Identity

Each section should have a clear gameplay purpose.

The visual density, scenery composition, and World Object selection should
support that purpose rather than simply filling available space.

### 3.4 Controlled Tutorial Expansion

A mechanic should normally be introduced in a section where the player has
enough context to understand and use it.

Current examples:

- Section 1 does not introduce Harvestables or Enemies;
- Section 2 introduces timber harvesting but not mining-oriented Harvestables;
- Section 2 introduces only one enemy;
- Consumables and Checkpoints remain outside the current Section 2 scope.

---

## 4. Section Documentation

### Section 1

Detailed in:

`Sections/Section1Design.md`

Current focus:

- Player Default Spawn Point;
- first Item Container;
- Worn Short Sword;
- Wayside Pit;
- Death Trigger;
- death and respawn;
- lightweight Scenery Objects.

### Section 2

Detailed in:

`Sections/Section2Design.md`

Current focus:

- Oak Tree harvesting;
- Wood and Twig Materials;
- loose Twig Collectible;
- Bow acquisition;
- Arrow support supply;
- Makeshift Worktable;
- Arrow crafting;
- first Feralshroom Basher encounter;
- expanded Scenery composition.

### Section 3

Reserved for future detailed documentation.

See:

`Sections/Section3Design.md`

Section 3 is expected to expand the Greenvale tutorial beyond the systems
introduced in Sections 1 and 2.

---

## 5. Section-to-Section Progression

### Section 1 → Section 2

The player transitions from basic world interaction into resource acquisition
and combat preparation.

Expected learned concepts before entering Section 2:

- movement;
- spawning / respawning;
- Item Container interaction;
- first weapon acquisition;
- basic death behaviour.

### Section 2 → Section 3

The player should leave Section 2 having learned:

- how Harvestable Objects provide Materials;
- how loose Collectibles differ from Scenery;
- how to acquire and use the Bow;
- how Arrows are obtained;
- how to use the first Crafting Station;
- how to craft Arrows;
- how to engage the first enemy.

This creates room for Section 3 to introduce additional world systems without
overloading earlier sections.

---

## 6. Images

Section-specific map images are stored under:

`Images/`

Current image set:

- `Section1_CurrentImplementation.png`
- `Section1_SceneryPlacementSketch.png`
- `Section2_CurrentImplementation.png`
- `Section2_SceneryPlacementSketch.png`

Images should be treated as design references rather than strict tile-by-tile
implementation requirements unless explicitly stated otherwise.
