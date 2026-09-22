# Greenvale Border Path Design

## 1. Overview

**Greenvale Border Path** is an introductory map in FlameOfTheLost.

The map is divided into three sections. Each section is documented separately
so that gameplay teaching, world content, and visual composition can be refined
without making the overall map document overly large.

The current design work focuses on **Section 1**.

---

## 2. Section Structure

```text
Greenvale Border Path
├── Section 1
│   └── Initial gameplay introduction
├── Section 2
│   └── Reserved for future detailed documentation
└── Section 3
    └── Reserved for future detailed documentation
```

---

## 3. Shared Map Design Principles

### 3.1 Progressive Gameplay Introduction

Greenvale Border Path should introduce mechanics gradually.

A section should avoid presenting too many new gameplay object types at once.
The environment may become visually richer through Scenery Objects without
necessarily adding new mechanics.

### 3.2 Gameplay Readability

Gameplay-relevant World Objects must remain easy to identify.

Scenery should support the environment without obscuring or visually competing
with Spawn Points, Item Containers, Death Triggers, Checkpoints, Harvestables,
Collectibles, Enemies, important paths, or transitions.

### 3.3 Section Identity

Each section should have a clear gameplay purpose.

The visual density, scenery composition, and World Object selection should
support that purpose rather than simply filling available space.

---

## 4. Section Documentation

### Section 1

Detailed in:

`Sections/Section1Design.md`

Current focus:

- Player Default Spawn Point;
- first Item Container;
- introduction to obtaining an Item from an Item Container;
- Wayside Pit;
- Death Trigger;
- death and respawn;
- lightweight Scenery Objects.

### Section 2

Reserved for future detailed documentation.

See:

`Sections/Section2Design.md`

### Section 3

Reserved for future detailed documentation.

See:

`Sections/Section3Design.md`

---

## 5. Images

Section-specific map images are stored under:

`Images/`

Images should be treated as design references rather than strict tile-by-tile
implementation requirements unless explicitly stated otherwise.
