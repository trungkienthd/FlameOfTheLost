# Consumable Item Content

This folder documents the authored **Consumable Item** content for **FlameOfTheLost**.

Consumables are documented separately from harvesting because harvesting is only one possible source of a consumable. A consumable may later come from:

- harvestable objects;
- farming;
- fishing;
- enemy drops;
- containers;
- vendors;
- cooking or crafting;
- quests or other gameplay systems.

The availability of a sprite does not automatically require a gameplay item to be created. An icon becomes a consumable only when the game has a reason to represent it as an inventory item.

## Documents

- [ConsumableCatalogue](ConsumableCatalogue.md) - the consumable-item catalogue and content status.
- [FoodSpriteMapping](FoodSpriteMapping.md) - mapping and working identities for the CFRPG16 Food icon sheet.
- [ConsumableGameplayDesign](ConsumableGameplayDesign.md) - design rules for how raw food, prepared food, produce, mushrooms, and fish should participate in gameplay.

## Content Status

The documents use the following statuses:

- **Current** - already established in active gameplay.
- **Planned** - intended content, but its data or gameplay implementation is not yet complete.
- **Working name** - a temporary readable identity derived from the sprite appearance; it can be renamed later.
- **Identity TBD** - the sprite is useful, but its final in-world item identity has not yet been designed.
- **Reserved visual** - the sprite is intentionally not yet represented as a Consumable Item.

## Naming

Current consumable data naming follows:

`CsmItem_<ConsumableName>`

Examples:

- `CsmItem_Cherry`
- `CsmItem_RawMeat`
- `CsmItem_CookedMeat`
- `CsmItem_BrownMushroom`

Sprite-sheet position, colour, or art-pack index should not be encoded into the final gameplay name unless it is part of the actual item identity.
