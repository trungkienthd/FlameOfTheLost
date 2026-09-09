# Harvesting Content

This folder documents the gameplay content that belongs to the harvesting loop in **FlameOfTheLost**.

It is intentionally separated from `Architectures/` and `Conventions/`:

- `Architectures/` explains how systems are structured in code.
- `Conventions/` explains naming and ID rules.
- `GameContent/` records the actual authored game content and intended gameplay relationships.

## Scope

For the current game scope, harvestable objects can drop two item categories:

1. **Materials** — crafting and resource items such as Twig, Wood, metal chunks, and crystal chunks.
2. **Consumable Items** — usable produce such as Cherry, Apple, and Peach.

The drop system may support additional item categories later. This documentation only describes the two categories currently planned.

Material data may also exist without a direct harvestable source. For example, a material can be reserved for later processing, enemy drops, containers, progression systems, or other gameplay.

## Documents

- [Harvestables](Harvestables.md) — harvestable object catalogue and drop tables.
- [Materials](Materials.md) — material catalogue, intended sources, and material-family progression.
- [Consumables](Consumables.md) — consumable item catalogue and intended sources.

## Content Status

The documents distinguish between:

- **Current** — already used or explicitly established in active gameplay.
- **Data Created** — a corresponding data asset exists, but the content may not yet be obtainable, processed, or used in gameplay.
- **Planned** — defined as intended game content, but its data or gameplay implementation has not yet been completed.
- **Working name** — a readable content name chosen from the sprite appearance; it can be renamed later without changing the underlying gameplay model.

These statuses describe game-content readiness rather than naming or Data ID conventions.

## Design Rules

A sprite being available does not require every visual variation to become a separate gameplay item.

For example:

- Small and large Oak Tree sprites are variants of the same **Oak Tree** harvestable type.
- Different growth stages of a crop are states of one crop, not separate harvestables.
- Tree species can share generic `Twig` and `Wood` materials unless a future recipe requires species-specific wood.
- A mineral family can contain multiple material forms without every form being a direct harvest drop.
- A material can exist as data before its world source or gameplay role is finalized.

Harvested and processed resources should remain conceptually distinct.

Current intended examples:

- Metal nodes: `Chunks -> Ore -> Bar`
- Crystal nodes: `Chunks -> Crystal -> Block`

Only the raw `Chunks` form is currently intended as the direct mineral-node drop. Later forms belong to future processing/refining gameplay unless explicitly designed otherwise.
