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

The availability of a sprite does not automatically require a gameplay item to be created. Seed-packet sprites in particular remain outside the Consumable Item type until the future farming system defines their role.

## Documents

- [ConsumableCatalogue](ConsumableCatalogue.md) - the consumable-item catalogue, content status, and high-level gameplay role.
- [FoodSpriteMapping](FoodSpriteMapping.md) - the current authored mapping for the CFRPG16 Food icon sheet.
- [ConsumableGameplayDesign](ConsumableGameplayDesign.md) - shared design rules for direct use, raw/prepared food, effect families, and status-effect behaviour.
- [ConsumableEffectConfiguration](ConsumableEffectConfiguration.md) - the current authored numerical baseline for `ResourceCostEntry` and consumable `StatusEffectProfile` applications.

## Content Status

The documents use the following statuses:

- **Current** - already established in active gameplay.
- **Data Created** - a `ConsumableItemProfile` asset exists, but the item may not yet be introduced through active world content.
- **Planned** - intended content, but its data or gameplay implementation is not yet complete.
- **Working name** - a readable authored identity that can still be refined later.
- **Reserved visual** - a sprite intentionally not yet represented as a Consumable Item, such as the current seed packets.

## Naming

Consumable gameplay/data naming follows:

`Csm_<ConsumableName>`

Examples:

- `Csm_Cherry`
- `Csm_RawMeat`
- `Csm_CookedMeat`
- `Csm_BrownMushroom`

The sprite sub-asset keeps the art-pack prefix and sprite index, followed by the consumable identity, for example:

`CFRPG16_Food_Icons_NO_Outline_54_Csm_Cherry`

Seed-packet sprites currently retain their original names because there is no Seed Item type or naming convention yet.

## Effect Authoring Boundary

`ConsumableItemProfile` owns the use resource cost and the list of status-effect applications. The detailed gameplay behaviour belongs to reusable `StatusEffectProfile` assets.

The current food baseline deliberately avoids Status Effect Flags for ordinary food. Food identity should normally be expressed through:

- periodic HP / FP / Stamina contributions;
- stat modifiers;
- or a deliberate mixture of positive and negative contributions.

Major capability locks, immunities, and similar binary effects remain reserved for systems where they are semantically appropriate rather than being used merely to make food feel different.
