# Greenvale Crossroads — Region Catalogue

**Status:** Proposed design v0.1. Local content ideas are illustrative until the overall layout is approved.

## At-a-glance catalogue

| ID | Region | Risk | Placement strategy | Connections |
|---|---|---|---|---|
| R01 | Southern Gate | Safe | Handcrafted entry | R02; previous map |
| R02 | Southern Road | Low | Constrained trail + decoration | R01, R03 |
| R03 | Traveller's Camp | Safe | Handcrafted social module | R02, R04, R05 |
| R04 | Central Crossroads | Low | Fixed road junction; generated edges | R03, R06, R08, R09 |
| R05 | Old Quarry | Medium | Resource constraints + natural dressing | R08, R03 |
| R06 | Forest Trail | Medium | Routed path + encounter pockets | R04, R07 |
| R07 | Northern Woods | Medium | Noise/cluster-based terrain | R06, R08 |
| R08 | Western Ruins | Medium–high | Modular ruins + authored landmarks | R04, R05, R07 |
| R09 | Eastern Approach | High | Authored warnings + routed road | R04, R10 |
| R10 | Boss Arena | Boss | Handcrafted encounter geometry | R09; gated eastern exit |

## R01 — Southern Gate

**Player purpose:** Orientation and arrival. **Must include:** entry spawn destination, map transition back to Greenvale Border Path, safe path to R02. **May include:** boundary scenery and a road marker. **Must avoid:** immediate hostile pressure or blocked spawn clearance.

## R02 — Southern Road

**Player purpose:** Establish the new map's mood without demanding complex interactions. **Must include:** clear, readable route between R01 and R03. **May include:** signposts, travelling debris, sparse harvestables beyond the travel lane. **Generation freedom:** gentle path variation and decoration, never severing the route.

## R03 — Traveller's Camp

**Player purpose:** Social base and gameplay preparation. **Proposed facilities:** merchant stall, quest-giving combat NPC, checkpoint, optional crafting station, campfire and tents. **Must include:** safe access from R02 and R04, reserved interaction space, unobstructed shortcut endpoint from R05. **Generation freedom:** minor dressing only. NPC roster and exact positions TBD.

## R04 — Central Crossroads

**Player purpose:** Readable central navigation landmark. **Must include:** south/camp, north/forest, west/ruins, east/boss directional cues; enough clear space for traversal. **May include:** central signpost, landmark tree, road edges, nonblocking scenery. **Generation freedom:** terrain around protected junction; no randomisation of the four major exits.

## R05 — Old Quarry

**Player purpose:** Optional resource collection building on the tutorial's harvesting/crafting concepts. **Proposed content:** harvestable stones, possible ore/crystal nodes, small enemy pockets, materials useful for optional preparation. **Must include:** accessible routes from R08 and R03; gatherable objects placed without sealing exits. **Generation freedom:** resource clusters and rocks constrained by clearance rules.

## R06 — Forest Trail

**Player purpose:** Introductory wilderness encounters beyond the social hub. **Proposed content:** Feralshroom groups or new variants (final roster TBD), roadside gathering and obstacles. **Must include:** traversable path between R04 and R07, combat space and safe separation from camp. **Generation freedom:** winding trail and controlled encounter pockets.

## R07 — Northern Woods

**Player purpose:** Reward optional exploration. **Proposed content:** dense forest, optional loot, hidden clearing, neutral wildlife, gatherables. **Must include:** traversable exits to R06 and R08. **Generation freedom:** forest density, clearings, noncritical item/harvestable positions. **Future possibility:** reserved northern map edge, not currently functional.

## R08 — Western Ruins

**Player purpose:** Optional tactical combat and discovery. **Proposed content:** derelict outpost, stronger enemies, containers and environmental storytelling. **Must include:** links to R04, R05 and R07, readable entrances and combat space. **Generation freedom:** authored modular ruin pieces and constrained dressing. **Future possibility:** reserved western exit, not currently functional.

## R09 — Eastern Approach

**Player purpose:** Foreshadow the regional boss and signal danger. **Proposed content:** damaged merchant carts, broken barricades, warning signs, sparse threats and preparation space. **Must include:** uninterrupted route from R04 to R10; an encounter start boundary that does not accidentally trigger from normal exploration. **Generation freedom:** decoration outside the protected approach lane.

## R10 — Boss Arena

**Player purpose:** Major encounter with optional temporary NPC support. **Must include:** boss spawn/encounter space, player entrance clearance, safe ally spawn/entry if unlocked, and gated eastern map transition. **May include:** rock edges, damaged structures, strong visual landmark. **Generation freedom:** small nonblocking decoration only. **Critical:** defeated boss remains defeated across save/reload once persistence is supported.

## Cross-region rules

- Keep NPC interaction radii and shop/checkpoint approaches clear.
- Maintain a playable footprint for the player's collider, enemy navigation, and dodge movement; final numeric clearances TBD.
- Spawn entities via compatible existing world spawning/refresh responsibilities where possible.
- Preserve main roads and shortcuts during terrain and object generation.
- Let visual density communicate risk without hiding required paths.
