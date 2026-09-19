# Harvesting Content

This folder contains the GameContent design for Harvestable Objects in **FlameOfTheLost**.

## Documents
- [Harvestable Design](HarvestableDesign.md) — authoritative Harvestable catalogue, durability, variants, and drops.
- [Impact Configuration](ImpactConfiguration.md) — harvest-impact rules and per-Harvestable durability / impact-response configuration.

## Current Core Catalogue
The current planned catalogue contains **46 core Harvestable types**:

- 3 Timber Trees
- 4 Fruit Trees
- 7 Stone-like Harvestables
- 12 Metal Nodes
- 20 Crystal Nodes

## Mineral Node Variants
Each Metal / Crystal family has four node types:
1. Large
2. Normal
3. Small
4. Rich

`Rich` is the working name for the fourth brighter / shard-like sprite.

## Mineral Drops
Metal Nodes:
- guaranteed Chunks
- optional Ore
- never direct Bar

Crystal Nodes:
- guaranteed Chunks
- optional Crystal
- never direct Block

## Physical Harvesting Language
- Timber Trees -> Chop
- Fruit Trees -> Slash and/or Chop
- Stone-like Harvestables -> Blunt
- Metal Nodes -> Blunt
- Crystal Nodes -> primarily Blunt, with selected fallback behaviour

## Greenvale Scope
Greenvale Border Path currently requires:
- Oak Tree
- Cherry Tree
