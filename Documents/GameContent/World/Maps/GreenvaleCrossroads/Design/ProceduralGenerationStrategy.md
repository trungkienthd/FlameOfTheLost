# Greenvale Crossroads — Procedural Generation Strategy

**Status:** Recommended approach, v0.1. No implementation approved.

## 1. Guiding decision

Use **hybrid, hierarchical, editor-time generation**. Handcraft the geography that carries story and gameplay; procedurally assist with roads, terrain textures, vegetation, resource clusters and scenery. **Bake and review** the result as a stable authored map before shipping.

This differs from roguelike regeneration on each visit. The map should have a recognisable, stable identity so players can navigate to recurring NPCs, quests, checkpoints and world transitions.

## 2. Proposed pipeline

```text
Approved gameplay region graph
      ↓
Spatial placement of region footprints and anchors
      ↓
Protected roads / entrances / camp / boss arena
      ↓
Terrain generation (noise + Tilemap rules)
      ↓
Constrained placement (scenery, harvestables, collectibles)
      ↓
Gameplay anchors and encounter placements
      ↓
Connectivity, clearance and encounter validation
      ↓
Designer review, manual refinement and bake
```

| Layer | Method | Responsibility |
|---|---|---|
| Gameplay topology | Graph-based | Preserve R01–R10 and all approved connections |
| Region space | Constraints, optionally Voronoi | Footprints, nonoverlap, distance, reserved anchors |
| Roads | Graph routing and authored junctions | Connect region entrances with readable paths |
| Ground tiles | Noise maps + Unity Tilemap/Rule Tiles | Grass, dirt, forest floor, rocky transitions |
| Forest/rocks | Density and placement constraints | Natural clusters without blocked paths |
| Critical locations | Handcrafted modular layouts | Camp, crossroads core, boss arena, ruins landmarks |
| Validation | Flood fill/BFS/A* + geometry checks | Playable connections and interaction/combat clearance |

**Optional later:** cellular automata for forest patches/ponds, WFC for detailed tile patterns, runtime per-save generation for separate replayable dungeons. None is required for this map's first iteration.

## 3. Reserved versus variable geometry

### Protected / handcrafted

- R01 scene-transition and safe spawn placement.
- R03 social modules, NPC interaction zones, checkpoint and shop clearances.
- R04 four-way junction and directional readability.
- R09 boss approach trigger and warnings.
- R10 arena footprint, combat manoeuvring space, boss/ally positions, eastern transition.
- All graph connections, travel corridors and shortest safe routes.

### Procedural candidates

- Trees, stones, grasses and nonessential scenery.
- Texture variations and vegetation density.
- Minor trail curvature (without changing endpoints or viability).
- Optional gathering nodes and some ordinary enemy encounter pockets.
- Modular decorative ruin arrangements in preapproved bounds.

## 4. Placement constraints

A proposed generator should validate at least:

1. **Walkability:** R01 can reach R03, R04 and R10; every graph edge resolves to playable continuous space.
2. **Character size:** route checks consider collider footprint and intended movement/dodge room, not just empty cells.
3. **No overlap:** new obstacles do not intersect critical actors, interactables, spawn points, structures or each other beyond explicit allowances.
4. **Safe-zone rules:** ordinary enemy groups do not appear inside the camp's protected region.
5. **Combat suitability:** arenas and enemy pockets have appropriate room for attacks, avoidance and ally participation.
6. **Protected anchors:** exits, quest interactions and the boss trigger remain reachable and visible.
7. **Seed reproducibility:** generated drafts record a seed plus tool/config version; designers can regenerate intentionally.
8. **Validation reporting:** failed map attempts surface reasons instead of silently producing an invalid layout.

## 5. Fit with existing project systems

This design should complement—not replace—the existing spawn/refresh approach:

- The current `EnvironmentObjectGroupSpawnerController` handles configured environment entries and world reconstruction.
- The procedural authoring tool could **generate candidate placements**, which are then reviewed and converted/baked into configuration consumed by established systems.
- NPCs, boss progression, and authored transitions should remain explicitly controlled gameplay content.
- Saved game state should track **persistent gameplay consequences**, not assume geometry changes every reload.

An exact data contract and Editor tooling design can be developed once the map graph and spatial layout are approved.

## 6. Generation boundaries by region

| Region | Procedural freedom |
|---|---|
| R01 | Minimal: decorative variations only |
| R02 | Moderate: path curves + roadside dressing |
| R03 | Minimal: camp module remains handcrafted |
| R04 | Minimal: fixed junction and protected sightlines |
| R05 | High: constrained rock/ore clusters |
| R06 | Moderate: forest trail and encounter pockets |
| R07 | High: woodland patches, clearings, gatherables |
| R08 | Moderate: modular ruins within authored footprint |
| R09 | Low: warnings authored, roadside decoration variable |
| R10 | Minimal: fixed combat layout |

## 7. Why not fully runtime-randomised?

Frequent reshuffling would undermine spatial familiarity and complicate NPCs, map transitions, quest objectives, checkpoints, persistent boss defeat, and save restoration. A **generate → inspect → bake** workflow gets much of the production benefit without those runtime risks.

## 8. Next design artifacts (not yet authored)

- Scaled spatial region plan with approximate dimensions and coordinate anchors.
- Visual road schematic and region footprints.
- Terrain/material palette and scene layering conventions.
- Candidate seed and sample placements for editor review.
- Individual NPC, shop, boss, and quest specifications.
