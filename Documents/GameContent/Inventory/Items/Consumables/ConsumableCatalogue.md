# Consumable Catalogue

## Purpose

This document defines the Consumable Item catalogue for **FlameOfTheLost**.

A Consumable Item is an inventory item intended to be used directly by the player or to participate in a future food-preparation system. Its source is documented separately because consumables are not limited to harvesting.

The current catalogue combines:

1. the already-established Greenvale consumable content;
2. planned produce already documented for harvesting;
3. additional food families represented by the CFRPG16 Food icon sheet.

---

## 1. Current Consumables

### Cherry

**Status:** Current

Asset:

`Csm_Cherry`

Current source:

- Cherry Tree

Current gameplay role:

- Restores Character Health.
- First harvested consumable introduced in Greenvale Border Path Section 3.

Tutorial flow:

`Harvest Cherry Tree -> obtain Cherry -> learn Consumable Item use -> restore Health`

Cherry remains the only Consumable Item that the Greenvale tutorial currently needs to teach.

---

## 2. Meat and Prepared Meat

The CFRPG16 Food sheet contains several visually paired raw/cooked meat icons.

| Consumable | Suggested Asset | Status | Notes |
|---|---|---|---|
| Raw Poultry | `Csm_RawPoultry` | Planned | Raw poultry-leg visual |
| Cooked Poultry | `Csm_CookedPoultry` | Planned | Prepared counterpart |
| Raw Meat | `Csm_RawMeat` | Planned | Generic raw red-meat visual |
| Cooked Meat | `Csm_CookedMeat` | Planned | Prepared counterpart |
| Raw Meat Leg | `Csm_RawMeatLeg` | Planned | Bone-in raw-meat visual |
| Cooked Meat Leg | `Csm_CookedMeatLeg` | Planned | Prepared counterpart |
| Raw Steak | `Csm_RawSteak` | Planned | Steak/cut visual |
| Cooked Steak | `Csm_CookedSteak` | Planned | Prepared counterpart |

Raw and cooked versions are intentionally separate item identities.

Expected future relationship:

`Raw Food -> preparation/cooking -> Prepared Food`

---

## 3. Mushrooms

| Consumable | Suggested Asset | Status |
|---|---|---|
| Brown Mushroom | `Csm_BrownMushroom` | Planned |
| Purple Mushroom | `Csm_PurpleMushroom` | Planned |
| Red Mushroom | `Csm_RedMushroom` | Planned |
| Blue Mushroom | `Csm_BlueMushroom` | Planned |

The colours are part of the current authored identities for these sprites. Their final gameplay effects can be differentiated later.

---

## 4. Eggs, Dairy, and Other Food

| Consumable | Suggested Asset | Status | Notes |
|---|---|---|---|
| Egg | `Csm_Egg` | Planned | Basic food / ingredient |
| Fried Egg | `Csm_FriedEgg` | Planned | Prepared counterpart |
| Milk | `Csm_Milk` | Planned | Bottled dairy item |
| Cheese | `Csm_Cheese` | Planned / working identity | Yellow prepared-food visual |
| Orange Juice | `Csm_OrangeJuice` | Planned / working identity | Orange bottled drink |

---

## 5. Fish

The food sheet contains eight visually distinct fish sprites. Their current consumable identities are colour-based working names.

| Consumable | Suggested Asset | Status |
|---|---|---|
| Blue Fish | `Csm_BlueFish` | Planned / working name |
| Cyan Fish | `Csm_CyanFish` | Planned / working name |
| Dark Blue Fish | `Csm_DarkBlueFish` | Planned / working name |
| Green Fish | `Csm_GreenFish` | Planned / working name |
| Brown Fish | `Csm_BrownFish` | Planned / working name |
| Red Fish | `Csm_RedFish` | Planned / working name |
| Yellow Fish | `Csm_YellowFish` | Planned / working name |
| Grey Fish | `Csm_GreyFish` | Planned / working name |

These names are intentionally simple visual identities. They can later be replaced by setting-specific species names if the fishing/aquatic-resource system requires stronger worldbuilding identities.

---

## 6. Fruits, Berries, and Produce

| Consumable | Suggested Asset | Status |
|---|---|---|
| Wheat | `Csm_Wheat` | Planned |
| Green Squash | `Csm_GreenSquash` | Planned / working name |
| Red Pepper | `Csm_RedPepper` | Planned |
| Apple | `Csm_Apple` | Planned |
| Tomato | `Csm_Tomato` | Planned |
| Carrot | `Csm_Carrot` | Planned |
| Orange Pepper | `Csm_OrangePepper` | Planned |
| Peach | `Csm_Peach` | Planned |
| Grapes | `Csm_Grapes` | Planned |
| Cabbage | `Csm_Cabbage` | Planned |
| Green Pepper | `Csm_GreenPepper` | Planned |
| Pear | `Csm_Pear` | Planned |
| Pumpkin | `Csm_Pumpkin` | Planned |
| Cucumber | `Csm_Cucumber` | Planned |
| Chili | `Csm_Chili` | Planned |
| Cherry | `Csm_Cherry` | Current |
| Corn | `Csm_Corn` | Planned |
| Parsnip | `Csm_Parsnip` | Planned |
| Turnip | `Csm_Turnip` | Planned |
| Watermelon | `Csm_Watermelon` | Planned |
| Strawberry | `Csm_Strawberry` | Planned |
| Purple Berry | `Csm_PurpleBerry` | Planned / working name |

The catalogue should follow the authored sprite identity rather than forcing previously proposed produce names onto visually different sprites.

---

## 7. Seed Packets Are Not Consumables

Many produce icons on the CFRPG16 Food sheet have matching seed-packet visuals.

These packet sprites currently remain under their original sprite names and should **not** be created as `ConsumableItemProfile` data.

They are reserved for a future farming/growing system.

Conceptually:

`Seed Item -> planted into farming system -> grows Crop -> harvested Produce`

The future farming design will determine their item category, data type, and naming convention.

---

## 8. Catalogue Design Rules

1. A sprite does not automatically imply an inventory item.
2. A consumable should have a gameplay purpose before its data is created.
3. Raw and prepared foods can be separate item identities when preparation changes gameplay value.
4. Colour-based names are acceptable working identities when they intentionally distinguish otherwise-similar sprites.
5. Fish currently use colour-based working identities and may receive species names later.
6. Seed packets are not Consumable Items and remain unnamed by the Consumable convention.
7. A consumable can also be a future cooking/crafting ingredient without being duplicated as a Material.
8. Harvesting, fishing, farming, cooking, vendors, containers, and enemies describe **sources**, not the consumable category itself.

---

## 9. Current Greenvale Scope

Greenvale Border Path should continue to introduce only:

`Cherry`

The wider catalogue exists to support later areas and systems without overloading the first tutorial.
