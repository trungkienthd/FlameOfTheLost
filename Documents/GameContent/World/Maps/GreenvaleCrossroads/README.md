# Greenvale Crossroads — Documentation Index

**Project:** FlameOfTheLost  
**Map:** Greenvale Crossroads  
**Status:** Pre-implementation proposal (v0.1)  
**Last updated:** 2026-10-15

Greenvale Crossroads is the proposed second major map after **Greenvale Border Path**. Unlike the tutorial's section-by-section structure, this map is designed around a **region graph**, branching exploration, a social camp, optional preparation, and a boss encounter that can involve an NPC ally.

## Folder structure

```text
GreenvaleCrossroads/
├── README.md
├── GreenvaleCrossroadsDesign.md
├── Design/
│   ├── MapRegionGraph.md
│   ├── ProceduralGenerationStrategy.md
│   └── GameplayAndProgression.md
└── Regions/
    └── RegionCatalogue.md
```

## Reading order

1. [GreenvaleCrossroadsDesign.md](GreenvaleCrossroadsDesign.md) — vision, scope, and design pillars.
2. [Design/MapRegionGraph.md](Design/MapRegionGraph.md) — canonical proposal for nodes, edges, loops, and progression gates.
3. [Regions/RegionCatalogue.md](Regions/RegionCatalogue.md) — intent and constraints of each region.
4. [Design/GameplayAndProgression.md](Design/GameplayAndProgression.md) — NPC, shop, quest, ally, boss, save/refresh concepts.
5. [Design/ProceduralGenerationStrategy.md](Design/ProceduralGenerationStrategy.md) — editor-first hybrid generation and validation requirements.

## Documentation rules

- **Proposed** means subject to gameplay/layout review, not implemented.
- `R01`–`R10` are **provisional design labels**, **not** production IDs in the game's asset-ID convention.
- NPC names, item offers, resource counts, quest rewards, boss identity, and map dimensions remain **TBD** unless explicitly labelled illustrative.
- Diagrams represent **logical connectivity**, not literal terrain placement.
- This folder specifies **what the map should accomplish**; it intentionally contains no implementation classes or production code contracts.

## Key decisions to revisit before implementation

- Confirm region roster, node connections, and whether the two optional loops are desired.
- Confirm the boss concept and next map connected to the eastern exit.
- Confirm the main NPC's identity and whether ally aid is unlocked through a side quest.
- Choose approximate dimensions, map tile scale, playable road widths, region footprints, and art palette.
- Decide whether a separate checkpoint belongs in the camp and whether a crafting station is needed there.
- Approve an editor-generated, **baked** map for the first version (recommended) versus per-save procedural layouts.

## Relationship to Greenvale Border Path

`GreenvaleBorderPath/` documents a largely handcrafted, linear tutorial divided into sections. `GreenvaleCrossroads/` is instead arranged by **map-wide design, graph topology, region catalogue, and generation rules**. Its structure can expand later with `Quests/`, `NPCs/`, `Encounters/`, or `Images/` when those designs become concrete.
