# Outdoor Decor Sprite Mapping

## 1. Purpose

This document maps sprites from the outdoor decoration sprite sheets to their
semantic Scenery Object names and categories.

Current source sheet:

`CFRPG16_Outdoor_Decor`

This mapping is used to maintain consistent sprite naming, prefab naming, and
future Scenery Object generation.

---

## 2. Naming Principle

Sprite names should describe the represented object rather than its position
inside the source sprite sheet.

Preferred examples:

- `GroundFlora_GrassTuft_01`
- `Wildflower_Red_01`
- `Wildflower_Purple_01`
- `WetlandFlora_LilyPad_01`
- `Mushroom_Brown_01`
- `Shrub_Green_01`
- `FallenWood_Log_01`
- `FallenWood_LogMushroom_01`

Avoid names such as:

- `Decor_01`
- `OutdoorSprite_17`
- `Plant_Row2_04`

---

## 3. Variant Naming

Variants belonging to the same visual family should use numeric suffixes.

Example:

`GroundFlora_GrassTuft_01`  
`GroundFlora_GrassTuft_02`  
`GroundFlora_GrassTuft_03`

Where a visual feature materially changes the semantic identity, include it in
the name.

Example:

`FallenWood_Log_Plain_01`  
`FallenWood_Log_Sprout_01`  
`FallenWood_Log_Mushroom_01`

---

## 4. Mapping Status

Exact sprite-index mapping will be documented after the
`CFRPG16_Outdoor_Decor` sheet has been formally named and reviewed.

Stone sprites already reserved for Harvestables should be excluded from the
Scenery mapping.