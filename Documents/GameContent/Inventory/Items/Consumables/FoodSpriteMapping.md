# CFRPG16 Food Sprite Mapping

## Purpose

This document records the current interpretation of the sprites in:

- `CFRPG16_Food_Icons_NO_Outline`
- `CFRPG16_Food_Icons_Outline`

Both sheets represent the same content; the Outline version is the outlined presentation variant.

This is a **content-design mapping**, not an authoritative claim about the original asset-pack naming. Where a sprite is visually ambiguous, the entry remains a working identity or `TBD` rather than inventing a permanent gameplay name.

---

## 1. General Mapping Rules

1. The outline and no-outline sheets must use the same gameplay identity.
2. Sprite art is presentation; ItemProfile identity is gameplay content.
3. Do not encode row/column numbers into final item names.
4. Ambiguous icons remain `TBD` until their intended identity is designed.
5. Seed packets are reserved for future farming content and are not Consumable Items.

---

## 2. Top Food Rows

### Meat / prepared-food candidates

The first two rows contain several paired raw/prepared food visuals.

Current working mapping:

- Raw Poultry
- Cooked Poultry
- Raw Meat
- Cooked Meat
- Raw Meat Leg
- Cooked Meat Leg
- Raw Steak
- Cooked Steak

These names describe the intended gameplay relationship and may be refined if the art pack exposes more authoritative food names.

### Mushrooms

Current mapping:

- Brown Mushroom
- Purple Mushroom
- Red Mushroom
- Blue Mushroom

### Other food

Current mapping:

- Egg
- Fried Egg
- Milk - working identity; confirm before final data creation
- Yellow star-shaped prepared food - identity TBD

---

## 3. Fish Row

The sheet contains eight fish sprites.

For now they are reserved as:

- Fish Species 01
- Fish Species 02
- Fish Species 03
- Fish Species 04
- Fish Species 05
- Fish Species 06
- Fish Species 07
- Fish Species 08

These labels are documentation placeholders only. They should not become final `CsmItem_` asset names.

When fishing is designed, each sprite should receive:

- a species/content name;
- an acquisition location or biome;
- rarity or progression placement if needed;
- a direct-use or cooking role.

---

## 4. Produce and Seed Rows

The lower section uses an alternating pattern of produce and matching seed-packet visuals for much of the sheet.

Recognizable or already-established produce represented across the current food/harvesting art includes:

- Wheat
- Tomato
- Carrot
- Corn
- Parsnip
- Eggplant
- Cabbage / leafy crop
- Cucumber / squash-like crop
- Chili Pepper
- coloured pepper variants
- Cherry
- Grapes / purple berry cluster
- Strawberry
- Watermelon
- Peach-like fruit
- berry-family produce

Where the exact visual identity is uncertain, the existing gameplay catalogue should remain authoritative rather than renaming an item solely from sprite appearance.

---

## 5. Seed-Packet Treatment

The seed packet beside a produce icon is not the same item as the produce.

Example conceptual relationship:

`Tomato Seed -> Tomato Plant -> Tomato`

The seed belongs to future farming content. It should receive its own item category or profile only when that system exists.

The same principle applies to other crop/seed pairs.

---

## 6. Outline Variant Usage

The project contains both outlined and non-outlined food sheets.

They should remain synchronized by sprite identity and sub-sprite naming.

Recommended rule:

- gameplay/data references identify the item;
- presentation code or asset-generation tooling selects the appropriate visual variant;
- outlined and non-outlined sprites do not create separate item data.

This mirrors the current approach used for the resource/material icon sheets.
