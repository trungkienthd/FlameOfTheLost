# CFRPG16 Food Sprite Mapping

## Purpose

This document records the current authored interpretation of the sprites in:

- `CFRPG16_Food_Icons_NO_Outline`
- `CFRPG16_Food_Icons_Outline`

Both sheets represent the same content; the Outline version is the outlined presentation variant.

The mapping below follows the current **FlameOfTheLost** consumable design. Seed packets remain outside the Consumable Item naming scheme.

Gameplay effect values are intentionally documented separately in [ConsumableEffectConfiguration](ConsumableEffectConfiguration.md); this file remains the authoritative sprite-to-identity mapping.

---

## 1. General Mapping Rules

1. The outline and no-outline sheets must use the same gameplay identity.
2. Sprite art is presentation; ItemProfile identity is gameplay content.
3. The numeric sprite index may remain in the sprite sub-asset name, but it is not part of the gameplay item identity.
4. Consumable sprites use the `Csm_` identity suffix.
5. Seed-packet sprites remain under their original names until a farming/seed item system exists.

---

## 2. Food Mapping

### Meat, Mushrooms, Eggs, and Dairy

| Index | Identity |
|---:|---|
| 0 | `Csm_RawPoultry` |
| 1 | `Csm_CookedPoultry` |
| 2 | `Csm_RawMeat` |
| 3 | `Csm_CookedMeat` |
| 4 | `Csm_BrownMushroom` |
| 5 | `Csm_PurpleMushroom` |
| 6 | `Csm_Cheese` |
| 7 | `Csm_Milk` |
| 8 | `Csm_RawSteak` |
| 9 | `Csm_CookedSteak` |
| 10 | `Csm_RawMeatLeg` |
| 11 | `Csm_CookedMeatLeg` |
| 12 | `Csm_RedMushroom` |
| 13 | `Csm_BlueMushroom` |
| 14 | `Csm_Egg` |
| 15 | `Csm_FriedEgg` |

### Fish

| Index | Identity |
|---:|---|
| 16 | `Csm_BlueFish` |
| 17 | `Csm_CyanFish` |
| 18 | `Csm_DarkBlueFish` |
| 19 | `Csm_GreenFish` |
| 20 | `Csm_BrownFish` |
| 21 | `Csm_RedFish` |
| 22 | `Csm_YellowFish` |
| 23 | `Csm_GreyFish` |

These are current colour-based working identities. A future fishing system may replace them with setting-specific species names.

### Produce and Seed-Packet Sequence

| Index | Current Mapping |
|---:|---|
| 24 | `Csm_Wheat` |
| 25 | Seed packet - reserved / original name |
| 26 | `Csm_GreenSquash` |
| 27 | Seed packet - reserved / original name |
| 28 | `Csm_RedPepper` |
| 29 | Seed packet - reserved / original name |
| 30 | `Csm_Apple` |
| 31 | Seed packet - reserved / original name |
| 32 | `Csm_Tomato` |
| 33 | Seed packet - reserved / original name |
| 34 | `Csm_Carrot` |
| 35 | Seed packet - reserved / original name |
| 36 | `Csm_OrangePepper` |
| 37 | Seed packet - reserved / original name |
| 38 | `Csm_Peach` |
| 39 | Seed packet - reserved / original name |
| 40 | `Csm_Grapes` |
| 41 | Seed packet - reserved / original name |
| 42 | `Csm_Cabbage` |
| 43 | Seed packet - reserved / original name |
| 44 | `Csm_GreenPepper` |
| 45 | Seed packet - reserved / original name |
| 46 | `Csm_Pear` |
| 47 | Seed packet - reserved / original name |
| 48 | `Csm_Pumpkin` |
| 49 | Seed packet - reserved / original name |
| 50 | `Csm_Cucumber` |
| 51 | Seed packet - reserved / original name |
| 52 | `Csm_Chili` |
| 53 | Seed packet - reserved / original name |
| 54 | `Csm_Cherry` |
| 55 | Seed packet - reserved / original name |
| 56 | `Csm_Corn` |
| 57 | Seed packet - reserved / original name |
| 58 | `Csm_Parsnip` |
| 59 | Seed packet - reserved / original name |
| 60 | `Csm_Turnip` |
| 61 | Seed packet - reserved / original name |
| 62 | `Csm_Watermelon` |
| 63 | `Csm_OrangeJuice` |
| 64 | `Csm_Strawberry` |
| 65 | `Csm_PurpleBerry` |

---

## 3. Seed-Packet Treatment

Seed packets are deliberately not assigned Consumable identities.

They should remain with their original sprite names until the farming design establishes:

- whether seeds have their own Item type;
- their data/profile type;
- their naming prefix;
- how they relate to crops and harvesting.

The current sprite sheet therefore intentionally mixes authored Consumable names with untouched seed-packet names.

---

## 4. Outline Variant Usage

The project contains both outlined and non-outlined food sheets.

They should remain synchronized by sprite identity and sub-sprite naming.

Recommended rule:

- gameplay/data references identify the item;
- presentation code or asset-generation tooling selects the appropriate visual variant;
- outlined and non-outlined sprites do not create separate item data.
