# Greenvale Crossroads — Map Design Overview

**Status:** Pre-implementation proposal, v0.1  
**Preceding map:** Greenvale Border Path  
**Core format:** Semi-open top-down RPG map with a stable geographic identity and branching regions.

## 1. Purpose

Greenvale Crossroads is intended to transition *FlameOfTheLost* from tutorial mechanics to interconnected RPG systems: NPC dialogue, shopping, quests, optional exploration, preparation, temporary ally assistance, and a persistent boss victory.

It should feel like a **place**, not a sequence of isolated encounters. The player should recognise the crossroads as an orientation landmark and the Traveller's Camp as a convenient safe base.

## 2. Design pillars

1. **Readable geography.** Southern entry, camp, central four-way crossroads, optional northern and western routes, main eastern boss route.
2. **Meaningful choices.** Optional quests and exploration grant resources or assistance without requiring them to reach the boss.
3. **Connected exploration.** Two optional shortcuts/loops reduce backtracking.
4. **Clear risk escalation.** Safe arrival → camp → exposed roads → hostile outskirts → boss arena.
5. **Persistent consequences.** Boss victory unlocks the eastern route and changes NPC dialogue; defeated major bosses should stay defeated after reload.
6. **Reusable content patterns.** General NPC/shop/quest/ally/encounter concepts rather than one-off logic tied to this map.

## 3. Player experience (intended)

The player arrives from Greenvale Border Path, follows a short safe road, and encounters a Traveller's Camp. Here, the player can talk, trade, receive a regional quest, use a checkpoint, and prepare. The central crossroads then offers distinct directions:

- **North:** Forest Trail → Northern Woods, exploration and gathering.
- **West:** Western Ruins → Old Quarry, optional combat and resources.
- **East:** Eastern Approach → Boss Arena, the main progression route.
- **South:** Traveller's Camp → Southern Road → Greenvale Border Path.

The player may challenge the boss without completing optional tasks. A proposed NPC preparation quest can unlock **temporary assistance** during that encounter, making earlier interactions matter.

## 4. Map organisation

See [Map Region Graph](Design/MapRegionGraph.md) for the authoritative provisional connectivity proposal and [Region Catalogue](Regions/RegionCatalogue.md) for local gameplay requirements.

| Route | Narrative/gameplay role | Requirement |
|---|---|---|
| South → hub | Introduction and safe preparation | Open |
| North branch | Optional forest exploration | Open |
| West branch | Optional ruins/quarry | Open |
| East → boss arena | Main story encounter | Boss can be approached without side-quest completion |
| Beyond boss → eastern exit | New map progression | Boss defeated |

## 5. Environmental identity

Suggested tone: a modest travelling crossroads where trade has been disrupted by a dangerous creature occupying the eastern passage. The camp is lived-in and comparatively welcoming; the northern woodland is overgrown; the western ruins and quarry show signs of old activity; the eastern road is damaged and increasingly ominous.

Possible environmental storytelling: signposts, merchant carts, supply crates, broken barricades, abandoned stonework, clearings, and traces of disrupted travel. These are suggestions, not a confirmed object list.

## 6. World transitions

- **South:** Return to Greenvale Border Path (required).
- **East beyond R10:** Progress to a future map after boss defeat (required concept; destination TBD).
- **North/West:** Potential future-map exits may be reserved visually, but should not imply functioning transitions until designed.

## 7. Out of scope for this stage

No Unity implementation, map dimensions, boss stats, precise tile palettes, final NPC dialogue, weapon/shop pricing, production prefab architecture, numerical spawn IDs, or formal quest-data schema are approved by this document.

## 8. Pre-production acceptance criteria

- The region graph supports both the main path and optional exploration without contradictions.
- Traveller's Camp is safely reachable on arrival.
- All required areas are connected for the player's collider footprint.
- The boss can be encountered alone; ally assistance is optional.
- No procedural vegetation or obstacles may obstruct critical paths or interaction clearances.
- The approach can be authored and reviewed in the Unity Editor before runtime systems change.
