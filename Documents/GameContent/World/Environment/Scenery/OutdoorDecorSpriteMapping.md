# Outdoor Decor Sprite Mapping

## 1. Purpose

This document maps sprites from the outdoor decoration sprite sheet to their
semantic Scenery Object names and categories.

Current source sheet:

`CFRPG16_Outdoor_Decor`

The sheet contains both Scenery and Harvestable sprites. This document records
the Scenery mappings only. Existing Harvestable sprite names are preserved and
excluded from the Scenery catalogue.

---

## 2. Naming Principle

`Scn` is the abbreviation for Scenery.

Scenery sprites from this sheet use the general form:

`CFRPG16_Outdoor_Decor_<Index>_Scn_<SemanticName>`

The semantic name should describe the represented object rather than its row or
column position in the source sheet.

The semantic portion uses compact PascalCase-style naming and does not overuse
underscores.

Examples:

- `CFRPG16_Outdoor_Decor_0_Scn_GroundFloraLeaf_1`
- `CFRPG16_Outdoor_Decor_9_Scn_WetlandFloraLilyPadSingle`
- `CFRPG16_Outdoor_Decor_30_Scn_FallenWoodLogSprouting_01`

Existing numeric suffixes may retain underscores where they improve variant
readability.

---

## 3. Scenery Sprite Mapping

| Index | Scenery Category | Semantic Name | Final Sprite Name |
|---:|---|---|---|
| 0 | Ground Flora | Ground Flora Leaf 1 | `CFRPG16_Outdoor_Decor_0_Scn_GroundFloraLeaf_1` |
| 1 | Ground Flora | Ground Flora Leaf 2 | `CFRPG16_Outdoor_Decor_1_Scn_GroundFloraLeaf_2` |
| 2 | Ground Flora | Ground Flora Leaf 3 | `CFRPG16_Outdoor_Decor_2_Scn_GroundFloraLeaf_3` |
| 3 | Wildflowers | Wildflower 1 | `CFRPG16_Outdoor_Decor_3_Scn_Wildflower_1` |
| 4 | Wildflowers | Wildflower 2 | `CFRPG16_Outdoor_Decor_4_Scn_Wildflower_2` |
| 5 | Wildflowers | Wildflower 3 | `CFRPG16_Outdoor_Decor_5_Scn_Wildflower_3` |
| 6 | Mushrooms | Red Cap Large | `CFRPG16_Outdoor_Decor_6_Scn_MushroomRedCapLarge` |
| 7 | Mushrooms | Purple Cap Large | `CFRPG16_Outdoor_Decor_7_Scn_MushroomPurpleCapLarge` |
| 8 | Mushrooms | Blue Cap Large | `CFRPG16_Outdoor_Decor_8_Scn_MushroomBlueCapLarge` |
| 9 | Wetland Flora | Lily Pad Single | `CFRPG16_Outdoor_Decor_9_Scn_WetlandFloraLilyPadSingle` |
| 10 | Wetland Flora | Lily Pad Cluster Flowering | `CFRPG16_Outdoor_Decor_10_Scn_WetlandFloraLilyPadClusterFlowering` |
| 11 | Wetland Flora | Lily Pad Flowering | `CFRPG16_Outdoor_Decor_11_Scn_WetlandFloraLilyPadFlowering` |
| 12 | Mushrooms | Red Cap Small | `CFRPG16_Outdoor_Decor_12_Scn_MushroomRedCapSmall` |
| 13 | Mushrooms | Purple Cap Small | `CFRPG16_Outdoor_Decor_13_Scn_MushroomPurpleCapSmall` |
| 14 | Mushrooms | Blue Cap Small | `CFRPG16_Outdoor_Decor_14_Scn_MushroomBlueCapSmall` |
| 15 | Wetland Flora | Lily Pad Double | `CFRPG16_Outdoor_Decor_15_Scn_WetlandFloraLilyPadDouble` |
| 20 | Mushrooms | Brown Stem Large | `CFRPG16_Outdoor_Decor_20_Scn_Mushroom_BrownStemLarge` |
| 22 | Ground Flora | Sprout | `CFRPG16_Outdoor_Decor_22_Scn_GroundFloraSprout` |
| 23 | Shrubs | Round Small | `CFRPG16_Outdoor_Decor_23_Scn_ShrubRoundSmall` |
| 24 | Ground Flora | Carrot Top | `CFRPG16_Outdoor_Decor_24_Scn_GroundFloraCarrotTop` |
| 25 | Fallen Wood | Broken Stump Plain | `CFRPG16_Outdoor_Decor_25_Scn_FallenWoodBrokenStumpPlain` |
| 26 | Mushrooms | Brown Stem Small | `CFRPG16_Outdoor_Decor_26_Scn_Mushroom_BrownStemSmall` |
| 30 | Fallen Wood | Log Sprouting 01 | `CFRPG16_Outdoor_Decor_30_Scn_FallenWoodLogSprouting_01` |
| 32 | Fallen Wood | Log Sprouting 02 | `CFRPG16_Outdoor_Decor_32_Scn_FallenWoodLogSprouting_02` |
| 34 | Fallen Wood | Log Plain | `CFRPG16_Outdoor_Decor_34_Scn_FallenWoodLogPlain` |
| 36 | Fallen Wood | Log Purple Fungus | `CFRPG16_Outdoor_Decor_36_Scn_FallenWoodLogPurpleFungus` |
| 38 | Fallen Wood | Log Red Fungus | `CFRPG16_Outdoor_Decor_38_Scn_FallenWoodLogRedFungus` |
| 40 | Fallen Wood | Log Blue Fungus | `CFRPG16_Outdoor_Decor_40_Scn_FallenWoodLogBlueFungus` |
| 42 | Fallen Wood | Broken Stump Sprouting | `CFRPG16_Outdoor_Decor_42_Scn_FallenWoodBrokenStumpSprouting` |
| 44 | Fallen Wood | Broken Stump Hollow | `CFRPG16_Outdoor_Decor_44_Scn_FallenWoodBrokenStumpHollow` |
| 45 | Shrubs | Dense Large | `CFRPG16_Outdoor_Decor_45_Scn_ShrubDenseLarge` |

---

## 4. Harvestable Exclusions

The following source-sheet indices shown in the current setup are already
assigned to Harvestables and are intentionally omitted from the Scenery
mapping:

- 17
- 18
- 19
- 21
- 27
- 28
- 29

These include Stone and Water Stone variants.

Their existing `Hvt` names should remain unchanged.

---

## 5. Category Summary

The current Scenery mapping contains:

- 5 Ground Flora sprites;
- 3 Wildflower sprites;
- 4 Wetland Flora sprites;
- 8 Mushroom sprites;
- 2 Shrub sprites;
- 9 Fallen Wood sprites.

Total documented Scenery sprites: **31**.

Harvestable sprites from the same source sheet are deliberately not counted.
