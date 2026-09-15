# Consumable Catalogue

## Purpose

This document defines the Consumable Item catalogue for **FlameOfTheLost**.

A Consumable Item is an inventory item intended to be used directly by the player or to participate in a future food-preparation system. Its source is documented separately because consumables are not limited to harvesting.

The current catalogue combines:

1. the already-established Greenvale consumable content;
2. Consumable Item profiles already created from the CFRPG16 Food icon sheet;
3. reserved/future sprite identities that do not yet have corresponding Consumable Item profiles.

Exact numerical effect values are documented in [ConsumableEffectConfiguration](ConsumableEffectConfiguration.md).

---

## 1. Current Tutorial Consumable

### Cherry

**Status:** Current

Asset:

`72055_Csm_Cherry`

Current source:

- Cherry Tree

Current gameplay role:

- first harvested consumable introduced in Greenvale Border Path Section 3;
- simple Health-over-time recovery;
- baseline introduction to Consumable Item use.

Current effect identity:

- no resource cost;
- one guaranteed Buff Status Effect application;
- no Flags;
- no Stat Modifiers;
- HP Heal periodic at `3 HP/s` with a `0.5s` tick interval for `5s`.

Tutorial flow:

`Harvest Cherry Tree -> obtain Cherry -> learn Consumable Item use -> apply periodic Health recovery`

Cherry remains the only Consumable Item that the Greenvale tutorial currently needs to teach.

---

## 2. Meat and Prepared Meat

The CFRPG16 Food sheet contains several visually paired raw/cooked meat icons. Their data profiles now exist; world acquisition and cooking progression can still be introduced later.

| ID | Consumable | Asset | Status | Gameplay Identity |
|---:|---|---|---|---|
| 72001 | Raw Poultry | `Csm_RawPoultry` | Data Created | Emergency HP recovery with minor mobility/Stamina drawback |
| 72002 | Cooked Poultry | `Csm_CookedPoultry` | Data Created | Stronger HP recovery plus light Stamina restoration |
| 72003 | Raw Meat | `Csm_RawMeat` | Data Created | HP recovery with increased damage taken |
| 72004 | Cooked Meat | `Csm_CookedMeat` | Data Created | Straightforward strong HP recovery |
| 72009 | Raw Steak | `Csm_RawSteak` | Data Created | Small offence gain and HP recovery at increased vulnerability |
| 72010 | Cooked Steak | `Csm_CookedSteak` | Data Created | Offensive food plus strong HP sustain |
| 72011 | Raw Meat Leg | `Csm_RawMeatLeg` | Data Created | High recovery with a mobility/Stamina drawback |
| 72012 | Cooked Meat Leg | `Csm_CookedMeatLeg` | Data Created | Strong HP sustain plus defensive benefit |

Raw and cooked versions are intentionally separate item identities.

Expected future relationship:

`Raw Food -> preparation/cooking -> Prepared Food`

Cooking should make the item clearly preferable without making the raw version unusable.

---

## 3. Mushrooms

| ID | Consumable | Asset | Status | Gameplay Identity |
|---:|---|---|---|---|
| 72005 | Brown Mushroom | `Csm_BrownMushroom` | Data Created | Stamina regeneration/restoration |
| 72006 | Purple Mushroom | `Csm_PurpleMushroom` | Data Created | Strong FP restoration with reduced mobility |
| 72013 | Red Mushroom | `Csm_RedMushroom` | Data Created | High-risk offensive damage buff |
| 72014 | Blue Mushroom | `Csm_BlueMushroom` | Data Created | FP efficiency and sustained skill usage |

The four mushrooms deliberately occupy different gameplay niches rather than being colour variants of the same recovery effect.

---

## 4. Eggs, Dairy, and Drink

| ID | Consumable | Asset | Status | Gameplay Identity |
|---:|---|---|---|---|
| 72007 | Cheese | `Csm_Cheese` | Data Created / working identity | Temporary durability through Virtual HP plus light HP recovery |
| 72008 | Milk | `Csm_Milk` | Data Created | Balanced HP/Stamina sustain and Stamina efficiency |
| 72015 | Egg | `Csm_Egg` | Data Created | Light HP and Stamina recovery |
| 72016 | Fried Egg | `Csm_FriedEgg` | Data Created | Improved Egg with stronger recovery and Stamina efficiency |
| 72064 | Orange Juice | `Csm_OrangeJuice` | Data Created / working identity | Processed mobility and FP/Stamina recovery drink |

---

## 5. Fish

The food sheet contains eight visually distinct fish sprites. Their current consumable identities are colour-based working names and their gameplay family focuses on combat specialization.

| ID | Consumable | Asset | Status | Gameplay Identity |
|---:|---|---|---|---|
| 72017 | Blue Fish | `Csm_BlueFish` | Data Created / working name | FP efficiency and restoration |
| 72018 | Cyan Fish | `Csm_CyanFish` | Data Created / working name | Mobility plus light Stamina recovery |
| 72019 | Dark Blue Fish | `Csm_DarkBlueFish` | Data Created / working name | Defensive sustain |
| 72020 | Green Fish | `Csm_GreenFish` | Data Created / working name | Strong Stamina recovery |
| 72021 | Brown Fish | `Csm_BrownFish` | Data Created / working name | Knockback resistance plus HP recovery |
| 72022 | Red Fish | `Csm_RedFish` | Data Created / working name | Aggressive damage bonus with increased vulnerability |
| 72023 | Yellow Fish | `Csm_YellowFish` | Data Created / working name | Stamina-cost efficiency |
| 72024 | Grey Fish | `Csm_GreyFish` | Data Created / working name | Knockback output plus defensive benefit |

These names can later be replaced by setting-specific species names if the fishing/aquatic-resource system requires stronger worldbuilding identities.

---

## 6. Produce

| ID | Consumable | Asset | Status | Gameplay Identity |
|---:|---|---|---|---|
| 72025 | Wheat | `Csm_Wheat` | Data Created | Simple Stamina recovery |
| 72027 | Green Squash | `Csm_GreenSquash` | Data Created / working name | Balanced HP/Stamina sustain |
| 72029 | Red Pepper | `Csm_RedPepper` | Data Created | Light offensive bonus plus Stamina sustain |
| 72031 | Apple | `Csm_Apple` | Data Created | Simple HP/Stamina recovery |
| 72033 | Tomato | `Csm_Tomato` | Data Created | Simple HP recovery |
| 72035 | Carrot | `Csm_Carrot` | Data Created | Light mobility plus Stamina recovery |
| 72037 | Orange Pepper | `Csm_OrangePepper` | Data Created | Mobility and Stamina-regeneration bonus |
| 72039 | Peach | `Csm_Peach` | Data Created | HP recovery with minor Stamina-regeneration support |
| 72041 | Grapes | `Csm_Grapes` | Data Created | FP/Stamina recovery |
| 72043 | Cabbage | `Csm_Cabbage` | Data Created | Defensive HP sustain |
| 72045 | Green Pepper | `Csm_GreenPepper` | Data Created | Defensive and Stamina-cost efficiency |
| 72047 | Pear | `Csm_Pear` | Data Created | Light defensive HP recovery |
| 72049 | Pumpkin | `Csm_Pumpkin` | Data Created | Virtual HP plus HP sustain |
| 72051 | Cucumber | `Csm_Cucumber` | Data Created | Stamina regeneration/restoration |
| 72053 | Chili | `Csm_Chili` | Data Created | High-risk offence/mobility stimulant with self-damage |
| 72055 | Cherry | `Csm_Cherry` | Current | Tutorial HP-over-time recovery |
| 72057 | Corn | `Csm_Corn` | Data Created | Stamina-cost efficiency and recovery |
| 72059 | Parsnip | `Csm_Parsnip` | Data Created | HP recovery plus Stamina-regeneration support |
| 72063 | Watermelon | `Csm_Watermelon` | Data Created | Strong dual HP/Stamina recovery |
| 72065 | Strawberry | `Csm_Strawberry` | Data Created | HP recovery plus light FP restoration |

### Reserved Produce Visuals Without Current Profiles

The sprite mapping also contains:

- `Csm_Turnip`
- `Csm_PurpleBerry`

These identities remain reserved/planned and are **not** part of the currently created Consumable Item profile set or the current numerical effect configuration.

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
3. Raw and prepared foods are separate item identities when preparation changes gameplay value.
4. Raw food can remain useful while carrying a temporary drawback.
5. Prepared food should normally remove the drawback and/or improve the benefit.
6. Colour-based names are acceptable working identities when they intentionally distinguish otherwise-similar sprites.
7. Fish currently use colour-based working identities and may receive species names later.
8. Seed packets are not Consumable Items and remain unnamed by the Consumable convention.
9. A consumable can also be a future cooking/crafting ingredient without being duplicated as a Material.
10. Harvesting, fishing, farming, cooking, vendors, containers, and enemies describe **sources**, not the consumable category itself.
11. Ordinary food should normally use Stat Modifiers and Stat Periodics rather than Status Effect Flags.
12. A consumable may use a `Mixed` Status Effect when its overall value is advantageous but includes a meaningful drawback.

---

## 9. Current Greenvale Scope

Greenvale Border Path should continue to introduce only:

`Cherry`

The wider catalogue and its authored effects exist to support later areas and systems without overloading the first tutorial.
