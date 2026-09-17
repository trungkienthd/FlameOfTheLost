# Harvesting Content

This folder contains the **game-content design** for Harvestable Objects in **FlameOfTheLost**.

Architecture, runtime implementation, naming conventions, and global Item catalogues belong in their own documentation areas. This folder only records the authored harvesting relationships needed to configure game content.

## Documents

- [Harvestable Design](HarvestableDesign.md) — authoritative Harvestable catalogue, status, archetype, visual-variant rules, and drop configuration.
- [Impact Configuration](ImpactConfiguration.md) — harvesting impact model, skill-side `HarvestDamageEntry` configuration, response tiers, and reusable Harvestable response archetypes.

## Scope

Current Harvestable drops use two Item categories:

- **Material**
- **Consumable Item**

The global definitions of those Items should remain under their Item-specific GameContent documentation. This folder only records which Items are obtained from which Harvestables.

## Content Status

- **Current** — already used or explicitly established in active gameplay.
- **Planned** — intended game content not yet fully authored or placed.
- **Working name** — temporary readable content name that may be replaced later.

## Core Rules

1. A gameplay Harvestable may have multiple visual variants.
2. Visual size, growth stage, damaged state, or stump state does not automatically create a new Harvestable type.
3. Drops are defined by the Harvestable, not by the impact used to break it.
4. Each drop entry independently defines its minimum quantity, maximum quantity, and chance.
5. Harvested and processed resources remain distinct.
6. New Harvestables should be added only when they have an intentional gameplay role.

## Current Core Catalogue

The current planned core catalogue contains **16 Harvestable types**:

- 3 Timber Trees
- 4 Fruit Trees
- 1 Stone Deposit
- 3 Metal Nodes
- 5 Crystal Nodes

Greenvale Border Path currently requires only:

- Oak Tree
- Cherry Tree
