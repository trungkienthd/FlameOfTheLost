# Harvesting Content

This folder contains the GameContent design for Harvestable Objects in **FlameOfTheLost**.

## Documents

- [Harvestable Design](HarvestableDesign.md) — authoritative catalogue for Harvestables, durability, variants, drops, quantities, and chances.
- [Impact Configuration](ImpactConfiguration.md) — skill-side harvesting rules and complete per-Harvestable impact-response configuration.

## Scope

Current Harvestables can directly drop:

- **Material**
- **Consumable Item**

Global Item definitions remain under their Item-specific GameContent documentation.

## Core Rules

1. A core Harvestable may have multiple variants.
2. Variants may change durability and/or drop tuning without becoming new core types.
3. All Harvestables are individually tunable; broad families only define readable physical rules.
4. Drops belong to the Harvestable / variant, not the impact used to break it.
5. Each drop entry has its own minimum quantity, maximum quantity, and chance.
6. Harvested and processed mineral resources remain distinct.

## Current Core Catalogue

The current planned catalogue contains **22 core Harvestable types**:

- 3 Timber Trees
- 4 Fruit Trees
- 7 Stone-like Harvestables
- 3 Metal Nodes
- 5 Crystal Nodes

Oak, Birch, and Spruce additionally have `Normal` and `Small` variants.

The Stone family is different: all seven authored Stone sprites are intentionally treated as separate core Harvestable types:

- Stone Large
- Stone Normal
- Stone Small 0
- Stone Small 1
- Stone Water Large
- Stone Water Normal
- Stone Water Small


## Physical Harvesting Language

- Timber Trees -> `Chop`
- Fruit Trees -> `Slash` and/or `Chop`
- Stone-like Harvestables -> `Blunt`
- Metal Nodes -> `Blunt`
- Crystal Nodes -> primarily `Blunt`, with selected `Chop` / `Pierce` fallback behaviour

## Current Greenvale Scope

Greenvale Border Path currently requires:

- Oak Tree
- Cherry Tree
