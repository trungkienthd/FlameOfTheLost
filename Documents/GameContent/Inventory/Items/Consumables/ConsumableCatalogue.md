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

The CFRPG16 Food sheet contains several visually paired raw/cooked meat icons. These should be treated as a small reusable food family rather than as unrelated one-off items.

| Consumable | Suggested Asset | Status | Notes |
|---|---|---|---|
| Raw Poultry | `Csm_RawPoultry` | Planned / working name | Raw poultry-leg visual |
| Cooked Poultry | `Csm_CookedPoultry` | Planned / working name | Prepared counterpart |
| Raw Meat | `Csm_RawMeat` | Planned / working name | Generic raw red-meat visual |
| Cooked Meat | `Csm_CookedMeat` | Planned / working name | Prepared counterpart |
| Raw Meat Leg | `Csm_RawMeatLeg` | Planned / working name | Second bone-in raw-meat visual |
| Cooked Meat Leg | `Csm_CookedMeatLeg` | Planned / working name | Prepared counterpart |
| Raw Steak | `Csm_RawSteak` | Planned / working name | Steak/cut visual |
| Cooked Steak | `Csm_CookedSteak` | Planned / working name | Prepared counterpart |

### Design direction

Raw and cooked versions are intentionally separate item identities.

The expected future relationship is:

`Raw Food -> preparation/cooking -> Prepared Food`

The cooking system itself is not part of the current implementation scope. These items can remain Planned until that loop is designed.

---

## 3. Mushrooms

The food sheet contains four clearly differentiated mushroom colours.

| Consumable | Suggested Asset | Status |
|---|---|---|
| Brown Mushroom | `Csm_BrownMushroom` | Planned |
| Purple Mushroom | `Csm_PurpleMushroom` | Planned |
| Red Mushroom | `Csm_RedMushroom` | Planned |
| Blue Mushroom | `Csm_BlueMushroom` | Planned |

The colours are acceptable working identities because they are visually distinct, but their final gameplay effects should be designed before they are introduced into progression.

Mushrooms are a good future category for differentiated consumable effects rather than four copies of the same healing item.

---

## 4. Eggs, Dairy, and Other Prepared Food

The sheet visibly supports a small set of non-produce food icons.

| Consumable | Suggested Asset | Status | Notes |
|---|---|---|---|
| Egg | `Csm_Egg` | Planned | Raw ingredient / basic food |
| Fried Egg | `Csm_FriedEgg` | Planned | Prepared counterpart |
| Milk | `Csm_Milk` | Planned / working name | Bottle/container visual; identity should be confirmed before data creation |
| Unidentified Prepared Food A | TBD | Identity TBD | Yellow star-shaped food visual |

The unidentified icon should not receive a permanent item asset until its intended identity is known.

---

## 5. Fish

The food sheet contains eight fish icons distinguished primarily by colour.

These should not be permanently named `BlueFish`, `GreenFish`, `Fish01`, and similar technical identities simply because the art is available.

| Entry | Status | Intended Treatment |
|---|---|---|
| Fish Species 01 | Identity TBD | Reserve sprite for a future named species |
| Fish Species 02 | Identity TBD | Reserve sprite for a future named species |
| Fish Species 03 | Identity TBD | Reserve sprite for a future named species |
| Fish Species 04 | Identity TBD | Reserve sprite for a future named species |
| Fish Species 05 | Identity TBD | Reserve sprite for a future named species |
| Fish Species 06 | Identity TBD | Reserve sprite for a future named species |
| Fish Species 07 | Identity TBD | Reserve sprite for a future named species |
| Fish Species 08 | Identity TBD | Reserve sprite for a future named species |

The species should receive setting-specific names when fishing or aquatic resource gathering is actually designed.

---

## 6. Fruits, Berries, and Produce

### Existing planned produce

The following consumables were already established through the harvesting-content design.

| Consumable | Suggested Asset | Status | Existing Intended Source |
|---|---|---|---|
| Apple | `Csm_Apple` | Planned | Apple Tree |
| Cherry | `Csm_Cherry` | Current | Cherry Tree |
| Peach | `Csm_Peach` | Planned | Peach Tree |
| Red Berry | `Csm_RedBerry` | Planned / working name | Red Berry Bush |
| Purple Berry | `Csm_PurpleBerry` | Planned / working name | Purple Berry Bush |
| Wheat | `Csm_Wheat` | Planned | Wheat Crop |
| Tomato | `Csm_Tomato` | Planned | Tomato Plant |
| Carrot | `Csm_Carrot` | Planned | Carrot Crop |
| Eggplant | `Csm_Eggplant` | Planned | Eggplant Crop |
| Corn | `Csm_Corn` | Planned | Corn Crop |
| Pumpkin | `Csm_Pumpkin` | Planned | Pumpkin Crop |
| Parsnip | `Csm_Parsnip` | Planned | Parsnip Crop |
| Cabbage | `Csm_Cabbage` | Planned | Cabbage Crop |
| Cucumber | `Csm_Cucumber` | Planned | Cucumber Crop |
| Chili Pepper | `Csm_ChiliPepper` | Planned | Chili Crop |
| Red Pepper | `Csm_RedPepper` | Planned | Red Pepper Crop |
| Orange Pepper | `Csm_OrangePepper` | Planned | Orange Pepper Crop |
| Green Pepper | `Csm_GreenPepper` | Planned | Green Pepper Crop |
| Blue Squash | `Csm_BlueSquash` | Planned / working name | Blue Squash Crop |

### Additional food-sheet candidates

The Food sheet also provides visuals suitable for additional produce. These can extend the catalogue when their source and gameplay purpose are designed.

| Consumable | Suggested Asset | Status |
|---|---|---|
| Strawberry | `Csm_Strawberry` | Planned |
| Grapes | `Csm_Grapes` | Planned |
| Watermelon | `Csm_Watermelon` | Planned |

Other visually ambiguous produce icons should remain **Identity TBD** until their authoritative or setting-specific identities are chosen.

---

## 7. Seed Packets Are Not Consumables

Many produce icons on the CFRPG16 Food sheet have matching seed-packet visuals.

These packets should **not** be created as `ConsumableItemProfile` data merely because they appear on the same sprite sheet.

They are reserved for a future farming/growing system.

Possible future item concept:

`Seed Item -> planted into a farming system -> grows Crop -> harvested Produce`

Their eventual item category and data type should be decided when farming is designed.

Until then, the seed-packet sprites are **Reserved visual** content.

---

## 8. Catalogue Design Rules

1. A sprite does not automatically imply an inventory item.
2. A consumable should have a gameplay purpose before its data is created.
3. Raw and prepared foods can be separate item identities when preparation changes gameplay value.
4. Colour-based names are acceptable as working names, but should not replace stronger setting-specific identities when those become available.
5. Fish should receive species identities before permanent ItemProfile creation.
6. Seed packets are not Consumable Items.
7. A consumable can also be a future cooking/crafting ingredient without being duplicated as a Material.
8. Harvesting, fishing, farming, cooking, vendors, containers, and enemies describe **sources**, not the consumable category itself.

---

## 9. Current Greenvale Scope

Greenvale Border Path should continue to introduce only:

`Cherry`

The wider catalogue exists to support later areas and systems without overloading the first tutorial.
