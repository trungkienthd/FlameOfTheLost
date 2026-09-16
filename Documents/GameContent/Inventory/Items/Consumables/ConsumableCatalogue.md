# Consumable Catalogue

## Purpose

This document defines the Consumable Item catalogue for **FlameOfTheLost**.

The current authored set contains **45 Consumable Item identities with corresponding consumable Status Effect profiles**. `Csm_Turnip` and `Csm_PurpleBerry` remain reserved visuals without current profiles.

Exact Status Effect configuration is documented in [ConsumableEffectConfiguration](ConsumableEffectConfiguration.md).

---

## 1. Current Tutorial Consumable

### Cherry

**Status:** Current

Item:

`72055_Csm_Cherry`

Status Effect:

`53043_SE_Csm_Cherry_PeriodicHeal`

Current source:

- Cherry Tree

Current gameplay role:

- first harvested consumable introduced in Greenvale Border Path Section 3;
- direct HP-over-time recovery;
- simple introduction to consumable use with repeat-use stacking.

Current effect identity:

- Affinity = `Buff`;
- Duration = `4s`;
- Max Stacks = `2`;
- Duration Refresh Type = `Extend`;
- no Flags;
- no Stat Modifiers;
- HP Heal `5/s @0.5s`.

A single uninterrupted application restores 20 HP.

Tutorial flow:

`Harvest Cherry Tree -> obtain Cherry -> learn Consumable Item use -> apply Periodic Heal`

---

## 2. Meat and Prepared Meat

| Item ID | Consumable | Status Effect ID | Gameplay Identity |
|---:|---|---:|---|
| 72001 | Raw Poultry | 53001 | HP recovery with reduced mobility and Stamina drain |
| 72002 | Cooked Poultry | 53002 | Strong HP recovery plus Stamina restoration |
| 72003 | Raw Meat | 53003 | HP recovery with increased damage received |
| 72004 | Cooked Meat | 53004 | Straightforward strong HP recovery |
| 72009 | Raw Steak | 53005 | Strong offence and HP recovery with increased vulnerability |
| 72010 | Cooked Steak | 53006 | High offence plus sustained HP recovery |
| 72011 | Raw Meat Leg | 53007 | HP recovery with mobility and Stamina drawbacks |
| 72012 | Cooked Meat Leg | 53008 | Defensive HP sustain |

Raw and cooked versions remain separate item identities.

---

## 3. Mushrooms

| Item ID | Consumable | Status Effect ID | Gameplay Identity |
|---:|---|---:|---|
| 72005 | Brown Mushroom | 53011 | Stamina regeneration/restoration |
| 72006 | Purple Mushroom | 53012 | FP restoration with reduced mobility |
| 72013 | Red Mushroom | 53013 | Very high offence with increased damage received |
| 72014 | Blue Mushroom | 53014 | Flat FP-cost reduction plus FP restoration |

---

## 4. Eggs, Dairy, and Drink

| Item ID | Consumable | Status Effect ID | Gameplay Identity |
|---:|---|---:|---|
| 72007 | Cheese | 53015 | Virtual HP plus light HP recovery |
| 72008 | Milk | 53016 | Balanced HP/Stamina sustain and Stamina efficiency |
| 72015 | Egg | 53009 | Light HP and Stamina recovery |
| 72016 | Fried Egg | 53010 | Improved HP/Stamina recovery with Stamina efficiency |
| 72064 | Orange Juice | 53045 | Mobility, Stamina regeneration, FP recovery, and Stamina recovery |

---

## 5. Fish

| Item ID | Consumable | Status Effect ID | Gameplay Identity |
|---:|---|---:|---|
| 72017 | Blue Fish | 53017 | FP efficiency and restoration |
| 72018 | Cyan Fish | 53018 | Mobility plus Stamina recovery |
| 72019 | Dark Blue Fish | 53019 | Defensive HP sustain |
| 72020 | Green Fish | 53020 | Stamina regeneration/restoration |
| 72021 | Brown Fish | 53021 | Knockback resistance plus HP recovery |
| 72022 | Red Fish | 53022 | Virtual HP and strong offence with increased incoming damage/knockback |
| 72023 | Yellow Fish | 53023 | Stamina efficiency, regen, and recovery |
| 72024 | Grey Fish | 53024 | Knockback output plus damage reduction |

Fish colour names remain working identities.

---

## 6. Produce

| Item ID | Consumable | Status Effect ID | Gameplay Identity |
|---:|---|---:|---|
| 72025 | Wheat | 53025 | Simple Stamina recovery |
| 72027 | Green Squash | 53026 | Balanced HP/Stamina sustain and Stamina regen |
| 72029 | Red Pepper | 53032 | Offensive bonus plus Stamina recovery |
| 72031 | Apple | 53039 | HP/FP/Stamina recovery |
| 72033 | Tomato | 53036 | Simple HP recovery |
| 72035 | Carrot | 53037 | Mobility plus FP restoration |
| 72037 | Orange Pepper | 53033 | Mobility and Stamina regeneration |
| 72039 | Peach | 53040 | HP recovery, Stamina regen, and Virtual HP |
| 72041 | Grapes | 53041 | FP efficiency plus FP/Stamina recovery |
| 72043 | Cabbage | 53027 | Strong defence plus Virtual HP |
| 72045 | Green Pepper | 53034 | Defence plus Stamina efficiency |
| 72047 | Pear | 53042 | Defence, Virtual HP, and HP recovery |
| 72049 | Pumpkin | 53028 | Large Virtual HP boost plus mobility |
| 72051 | Cucumber | 53038 | Stamina regeneration/restoration |
| 72053 | Chili | 53035 | High-risk offence/Stamina-efficiency stimulant with true self-damage |
| 72055 | Cherry | 53043 | Stackable tutorial HP-over-time recovery |
| 72057 | Corn | 53029 | Stamina efficiency and recovery |
| 72059 | Parsnip | 53030 | HP recovery plus Stamina regeneration |
| 72063 | Watermelon | 53031 | Strong HP/FP/Stamina recovery |
| 72065 | Strawberry | 53044 | HP and FP recovery |

### Reserved Produce Visuals Without Current Profiles

- `Csm_Turnip`
- `Csm_PurpleBerry`

These remain reserved/planned and are not part of the current 45-profile consumable set.

---

## 7. Seed Packets Are Not Consumables

Seed packet visuals remain reserved for a future farming/growing system and should not be created as `ConsumableItemProfile` data.

---

## 8. Catalogue Design Rules

1. A sprite does not automatically imply an inventory item.
2. A consumable should have a gameplay purpose before permanent data is created.
3. Raw and prepared foods are separate identities when preparation changes gameplay value.
4. Raw food may remain usable while carrying a drawback.
5. Prepared food should be meaningfully distinct, not necessarily by one universal upgrade formula.
6. Colour-based fish names are acceptable working identities.
7. Seed packets are not Consumable Items.
8. A consumable can also be an ingredient without being duplicated as a Material.
9. Acquisition source and inventory category are separate concerns.
10. Ordinary food currently uses modifiers/periodics rather than Status Effect Flags.
11. `Mixed` affinity is appropriate when meaningful positive and negative contributions coexist.
12. Concrete effect values are authoritative in the `StatusEffectProfile` assets and mirrored in `ConsumableEffectConfiguration.md`.

---

## 9. Current Greenvale Scope

Greenvale Border Path should continue to introduce only:

`Cherry`

The wider 45-item authored catalogue supports later areas and systems without overloading the first tutorial.
