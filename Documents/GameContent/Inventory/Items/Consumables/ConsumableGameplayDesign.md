# Consumable Gameplay Design

## Purpose

This document defines the intended gameplay treatment of food-based Consumable Items in **FlameOfTheLost**.

It does not assign final numerical effects. Exact restoration values, durations, status strengths, drop rates, prices, and recipes should be balanced in data rather than fixed by this document.

---

## 1. Core Principle

A Consumable Item should answer at least one gameplay question:

- Why would the player use it now?
- Why would the player keep it for later?
- Why would the player process or cook it instead of consuming it directly?
- Why is this item distinct from another food item?

The game should avoid creating a large catalogue of foods that all perform the same action at slightly different numbers.

---

## 2. Consumable Families

### Produce

Examples:

- Cherry
- Apple
- Peach
- berries
- crops and vegetables

Typical role:

- simple, readable direct-use consumables;
- early-game recovery;
- future cooking ingredients;
- farming/harvesting rewards.

Produce should form the most understandable entry point into the Consumable Item system.

Cherry is the first example.

### Raw Meat

Typical role:

- ingredient first;
- optional weak or situational direct use if desired later;
- source item for cooking/preparation.

Raw meat does not need to be introduced as a direct-use item simply because it has a Consumable Item icon.

### Prepared Meat / Prepared Food

Typical role:

- stronger recovery or utility than raw ingredients;
- reward for investing in a future cooking/preparation loop;
- potentially higher value, longer-duration effects, or multiple effects.

### Mushrooms

Typical role:

- effect-oriented consumables;
- ingredients for specialist recipes;
- stronger differentiation through status effects rather than pure Health restoration.

The four mushroom colours should eventually have distinct gameplay identities.

### Fish

Typical role:

- fishing reward;
- direct food or cooking ingredient;
- biome/progression-specific resource.

Fish identity should be designed alongside the fishing/aquatic-resource loop rather than from colour alone.

### Egg and Dairy

Typical role:

- ingredients that bridge gathering/farming and cooking;
- simple prepared-food pairs such as Egg -> Fried Egg.

---

## 3. Raw vs Prepared Food

When both raw and prepared sprites exist, they should represent meaningful states rather than cosmetic duplicates.

Recommended relationship:

`Raw Item -> Cooking/Preparation -> Prepared Item`

A prepared item should normally provide at least one meaningful advantage, such as:

- stronger immediate recovery;
- an additional status benefit;
- longer duration;
- better inventory/value efficiency;
- removal of a drawback associated with raw consumption.

The exact advantage should be designed per recipe rather than assumed globally.

---

## 4. Direct Use vs Ingredient Use

A food item can support both roles:

- **Direct Use** - the player consumes the item immediately.
- **Ingredient Use** - the item participates in future cooking/crafting.

These roles do not require duplicate item identities.

Example:

`Cherry` can remain one Consumable Item whether it is eaten directly or later used in a recipe.

A food should not be duplicated as a Material solely because crafting uses it.

---

## 5. Effect Design

The current Cherry establishes the simplest baseline:

`consume -> restore Health`

Future consumables can expand into other effect families, but the item should remain readable to the player.

Possible design spaces include:

- instant recovery;
- temporary recovery over time;
- offensive bonuses;
- defensive bonuses;
- movement or utility bonuses;
- resistance/status-oriented effects;
- resource restoration if additional character resources are introduced.

These are design spaces, not yet-assigned effects for specific food icons.

Mushrooms and prepared foods are the strongest candidates for more specialized effects; common produce should generally remain simpler.

---

## 6. Acquisition Is Separate from Item Category

Consumable Item describes **what the inventory item is**, not where it comes from.

Possible sources include:

- harvestable fruit trees;
- berry bushes;
- crops;
- future farming;
- future fishing;
- enemy drops;
- containers;
- vendors;
- cooking/preparation;
- quests.

Source-specific documentation should reference the shared consumable identity rather than redefine it.

---

## 7. Seed Items

Seed packets shown on the Food sprite sheet are intentionally outside the current Consumable Item design.

They represent a likely future farming resource.

Conceptually:

`Seed Item -> Plant/Growth State -> Harvestable Crop -> Produce Consumable`

The future farming design should decide whether seeds are:

- a dedicated item category;
- a Material-like resource;
- a key/progression resource;
- or another specialized item type.

Do not make them Consumable Items simply to reuse the existing inventory type.

---

## 8. Tutorial and Progression

### Greenvale Border Path

The first tutorial should teach only the minimum loop:

1. encounter Cherry Tree;
2. harvest Cherry;
3. receive a Consumable Item;
4. learn how to equip/select/use the consumable;
5. restore Health.

This establishes the category without introducing cooking, farming, fishing, food quality, or multiple competing effects.

### Later progression

Additional families can be introduced when the relevant world/system context exists:

- new produce through new biomes or farming;
- mushrooms when differentiated effects become useful;
- fish alongside fishing/aquatic gathering;
- raw/prepared food alongside cooking;
- stronger prepared foods when the player understands the basic consumable loop.

---

## 9. Content-Creation Rules

Before creating a new Consumable Item data asset, define:

1. its gameplay identity/name;
2. its icon;
3. its acquisition source or planned source;
4. whether it can be directly consumed;
5. its intended effect family;
6. whether it is also an ingredient;
7. its progression purpose.

If several of these are unknown, keeping the sprite reserved is preferable to creating placeholder inventory content.
