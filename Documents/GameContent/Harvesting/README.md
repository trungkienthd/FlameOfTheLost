# Harvesting Content

This folder documents the gameplay content that belongs to the harvesting loop in **FlameOfTheLost**.

It is intentionally separated from `Architectures/` and `Conventions/`:

- `Architectures/` explains how systems are structured in code.
- `Conventions/` explains naming and ID rules.
- `GameContent/` records the actual authored game content and intended gameplay relationships.

## Scope

For the current game scope, harvestable objects can drop two item categories:

1. **Materials** — crafting resources such as Twig and Wood.
2. **Consumable Items** — usable produce such as Cherry, Apple, and Peach.

The drop system may support additional item categories later. This documentation only describes the two categories currently planned.

## Documents

- [Harvestables](Harvestables.md) — harvestable object catalogue and drop tables.
- [Materials](Materials.md) — material catalogue and intended sources.
- [Consumables](Consumables.md) — consumable item catalogue and intended sources.

## Content Status

The documents distinguish between:

- **Current** — already used or explicitly established in the project.
- **Planned** — supported by the available sprite set and reserved as game content, but not necessarily implemented as a ScriptableObject yet.
- **Working name** — a readable content name chosen from the sprite appearance; it can be renamed later without changing the harvesting model.

## Design Rule

A sprite being available does not require every visual variation to become a separate gameplay item.

For example:

- Small and large Oak Tree sprites are variants of the same **Oak Tree** harvestable type.
- Different growth stages of a crop are states of one crop, not separate harvestables.
- Tree species can share generic `Twig` and `Wood` materials unless a future recipe requires species-specific wood.
