# Consumable Item Content

This folder documents the authored **Consumable Item** content for **FlameOfTheLost**.

The current authored catalogue contains **45 Consumable Item identities with corresponding consumable `StatusEffectProfile` assets**. Two additional food-sheet identities, `Csm_Turnip` and `Csm_PurpleBerry`, remain reserved visuals without current profiles.

Consumables are documented separately from harvesting because harvesting is only one possible source. A consumable may later come from harvesting, farming, fishing, enemies, containers, vendors, cooking/crafting, quests, or other systems.

## Documents

- [ConsumableCatalogue](ConsumableCatalogue.md) - catalogue, content status, tutorial scope, and high-level gameplay roles.
- [FoodSpriteMapping](FoodSpriteMapping.md) - authoritative CFRPG16 Food sprite-to-identity mapping.
- [ConsumableGameplayDesign](ConsumableGameplayDesign.md) - shared authoring rules, family roles, affinity, lifetime, and stacking principles.
- [ConsumableEffectConfiguration](ConsumableEffectConfiguration.md) - concrete authored configuration for Status Effect IDs `53001-53045`.

## Current Data Authority

The concrete gameplay values stored in the current `.asset` profiles are authoritative.

Documentation should mirror those assets rather than treating an older balance draft as the source of truth.

## Content Status

- **Current** - established in active gameplay.
- **Data Created** - authored data exists but may not yet be introduced in world content.
- **Planned** - intended content not yet implemented.
- **Working name** - readable identity that can still be refined.
- **Reserved visual** - mapped sprite without current Consumable data.

## Naming

Consumable Item identity:

`Csm_<ConsumableName>`

Consumable Status Effect asset:

`<StatusEffectID>_SE_Csm_<ConsumableName>_<EffectIdentity>`

Current Consumable Status Effect ID range:

`53001-53045`

Examples:

- `53043_SE_Csm_Cherry_PeriodicHeal`
- `53013_SE_Csm_RedMushroom_GlassCannon`
- `53035_SE_Csm_Chili_Berserk`

The sprite sub-asset keeps the art-pack prefix and sprite index, followed by the Consumable identity, for example:

`CFRPG16_Food_Icons_NO_Outline_54_Csm_Cherry`

Seed-packet sprites retain their original names until a Seed Item system exists.

## Effect Authoring Boundary

`ConsumableItemProfile` owns consumption cost and Status Effect applications.

`StatusEffectProfile` owns:

- affinity;
- lifetime and stacking;
- flags;
- stat modifiers;
- stat periodics;
- VFX.

Ordinary food currently uses no Status Effect Flags. Differentiation comes from modifiers, periodics, affinity, and lifetime/stacking behaviour.

## Current Tutorial Baseline

Cherry currently uses:

`53043_SE_Csm_Cherry_PeriodicHeal`

with:

- `Buff`;
- `4s`;
- `Max Stacks = 2`;
- `Extend`;
- HP Heal `5/s @0.5s`.

The wider catalogue remains available for later progression.
