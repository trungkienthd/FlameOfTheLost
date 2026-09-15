# Consumable Gameplay Design

## Purpose

This document defines the intended gameplay treatment of food-based Consumable Items in **FlameOfTheLost**.

It defines the shared design rules and gameplay identities. The current numerical baseline is documented separately in [ConsumableEffectConfiguration](ConsumableEffectConfiguration.md), so balance values can evolve without weakening the higher-level design rules in this document.

---

## 1. Core Principle

A Consumable Item should answer at least one gameplay question:

- Why would the player use it now?
- Why would the player keep it for later?
- Why would the player process or cook it instead of consuming it directly?
- Why is this item distinct from another food item?

The game should avoid creating a large catalogue of foods that all perform the same action at slightly different numbers.

Consumables should be **advantageous overall**, but an individual food does not need every contribution to be beneficial. A food can intentionally trade one attribute for a stronger gain elsewhere.

Examples:

- raw food can provide emergency recovery while temporarily reducing mobility or increasing vulnerability;
- an aggressive mushroom can increase damage dealt while also increasing damage received;
- Chili can act as a high-risk offensive stimulant rather than a conventional healing item.

When a single Status Effect contains both positive and negative contributions, its affinity should be `Mixed`.

---

## 2. Consumable Profile Responsibilities

A `ConsumableItemProfile` owns:

1. its `ResourceCostEntry` used when the item is consumed;
2. its list of `StatusEffectApplicationEntry` entries.

The actual gameplay behaviour belongs to the referenced `StatusEffectProfile` assets.

This keeps item identity separate from reusable status-effect behaviour and allows later systems to reuse, scale, or replace effects without changing the inventory item model.

### Current Resource-Cost Rule

Normal food currently has **no use resource cost**:

- HP Flat Cost = `0`
- FP Flat Cost = `0`
- Stamina Flat Cost = `0`
- HP Percent of Max Cost = `0`
- FP Percent of Max Cost = `0`
- Stamina Percent of Max Cost = `0`
- Blocked By Silence = `false`
- Allow Self Kill With HP Cost = `false`

`CostTriggerType` should use the canonical serialized value `OnAttackStart`; while all costs are zero, the trigger has no gameplay effect.

This rule can be broken later for explicitly magical, cursed, sacrificial, or other special consumables, but ordinary food should not consume combat resources merely because it is used.

### Current Status-Effect Application Rule

The initial food catalogue normally uses one guaranteed application:

- Chance = `1.0`
- Potency = `1.0`

A consumable can gain multiple application entries later when there is a clear gameplay reason to separate effects into independent profiles.

---

## 3. Status Effect Authoring Rules

### 3.1 Flags

Ordinary food should normally have **no Status Effect Flags**.

Flags represent binary gameplay rules such as movement locks, attack locks, immunities, or ignoring status-effect classes. These are too strong and categorical to use simply as flavour for normal food.

Food differentiation should primarily come from Stat Modifiers and Stat Periodics.

### 3.2 Stat Modifiers

Current useful food modifier targets include:

- `DamageReceived`
- `KnockbackReceived`
- `VirtualHPGranted`
- `DamageDealt`
- `KnockbackDealt`
- `MovementSpeedScale`
- `FPCost`
- `StaminaCost`
- `StaminaRegenRate`
- `StaminaRegenDelayDelta`

Multiplier values use `1.0` as neutral. For example:

- `DamageDealt x1.10` = 10% more damage dealt;
- `DamageReceived x0.92` = 8% less damage received;
- `MovementSpeedScale x0.90` = 10% slower movement.

`VirtualHPGranted` is additive.

### 3.3 Stat Periodics

Periodic targets are:

- HP
- FP
- Stamina

Periodic methods are:

- HP: `Heal` or `Damage`
- FP / Stamina: `Restore` or `Drain`

`AmountPerSecond` is the actual rate. `TickInterval` controls delivery frequency rather than multiplying the per-second rate.

Example:

`HP / Heal / 3 AmountPerSecond / 0.5 TickInterval`

means **3 HP per second**, delivered in half-second ticks.

### 3.4 Default Lifetime and Stacking

Unless a consumable-specific design requires otherwise, the current food effects use:

- Max Stacks = `1`
- Duration Refresh Type = `Refresh`
- Stat Modifier Combination Type = `AddStacks`
- no Status Effect VFX initially

The one-stack rule prevents repeatedly consuming the same food from multiplying one status effect into an extreme value. Different food Status Effect Profiles can still coexist.

---

## 4. Consumable Families

### Produce

Examples:

- Cherry
- Apple
- Peach
- Pear
- Strawberry
- crops and vegetables

Typical role:

- readable direct-use consumables;
- early-game recovery;
- light HP / Stamina sustain;
- future cooking ingredients;
- farming/harvesting rewards.

Produce should form the most understandable entry point into the Consumable Item system.

Cherry is the baseline example.

### Raw Meat

Typical role:

- ingredient first;
- still usable directly in an emergency;
- useful recovery paired with a temporary drawback;
- source item for cooking/preparation.

Raw food should not be made useless merely to encourage cooking. Instead, cooking should remove drawbacks and/or improve the positive effect.

### Prepared Meat / Prepared Food

Examples include Cooked Poultry, Cooked Meat, Cooked Steak, Cooked Meat Leg, Fried Egg, and Cheese.

Typical role:

- stronger recovery or utility than raw ingredients;
- no raw-food drawback;
- longer duration or additional benefit;
- reward for investing in a future cooking/preparation loop.

### Mushrooms

Mushrooms are specialist effect-oriented consumables rather than four differently coloured healing items.

Current identities:

- Brown Mushroom - Stamina recovery / regeneration.
- Purple Mushroom - strong FP recovery with a mobility trade-off.
- Red Mushroom - aggressive damage output with increased vulnerability.
- Blue Mushroom - FP efficiency / sustained special-skill use.

### Fish

The current fish identities are colour-based working identities. Their gameplay family focuses on **combat specialization**, so individual fish can support FP economy, mobility, defence, Stamina, knockback, or aggressive play.

The names can later be replaced by setting-specific species names without requiring the gameplay niches to disappear.

### Egg and Dairy

Typical role:

- ingredients that bridge gathering/farming and cooking;
- balanced recovery;
- durability or Stamina support;
- simple prepared-food relationships such as Egg -> Fried Egg.

### Peppers and Chili

Peppers are combat-oriented produce rather than generic healing vegetables.

- Red Pepper favours offence.
- Orange Pepper favours mobility and Stamina recovery.
- Green Pepper favours efficiency and defence.
- Chili is intentionally a stronger **Mixed** effect with meaningful risk.

### Drinks

Orange Juice establishes a processed drink family.

Processed consumables should normally be meaningfully stronger or more specialized than a simple raw ingredient. Orange Juice therefore focuses on mobility and resource recovery rather than behaving like another small fruit heal.

---

## 5. Raw vs Prepared Food

When both raw and prepared sprites exist, they should represent meaningful states rather than cosmetic duplicates.

Recommended relationship:

`Raw Item -> Cooking/Preparation -> Prepared Item`

A prepared item should normally provide at least one meaningful advantage:

- stronger recovery;
- removal of the raw-food drawback;
- an additional status benefit;
- longer duration;
- better inventory/value efficiency.

Current intended pair hierarchy:

- Raw Poultry -> Cooked Poultry
- Raw Meat -> Cooked Meat
- Raw Steak -> Cooked Steak
- Raw Meat Leg -> Cooked Meat Leg
- Egg -> Fried Egg

---

## 6. Direct Use vs Ingredient Use

A food item can support both roles:

- **Direct Use** - the player consumes the item immediately.
- **Ingredient Use** - the item participates in future cooking/crafting.

These roles do not require duplicate item identities.

Example:

`Cherry` can remain one Consumable Item whether it is eaten directly or later used in a recipe.

A food should not be duplicated as a Material solely because crafting uses it.

---

## 7. Cherry Baseline

Cherry establishes the simplest tutorial consumable effect:

- Resource Cost: none;
- Status Effect Affinity: Buff;
- Flags: none;
- Stat Modifiers: none;
- Duration: `5s`;
- Max Stacks: `1`;
- Duration Refresh: `Refresh`;
- Periodic:
  - Target = HP
  - Method = Heal
  - Amount Per Second = `3`
  - Tick Interval = `0.5s`

This produces `15 HP` of total healing over the full duration.

Cherry should remain deliberately simple because it teaches the category rather than the entire status-effect system.

---

## 8. Acquisition Is Separate from Item Category

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

## 9. Seed Items

Seed packets shown on the Food sprite sheet are intentionally outside the current Consumable Item design.

They remain under their original sprite names because there is currently no authored Seed Item type.

Conceptually:

`Seed Item -> Plant/Growth State -> Harvestable Crop -> Produce Consumable`

The future farming design should decide whether seeds are:

- a dedicated item category;
- a Material-like resource;
- a key/progression resource;
- or another specialized item type.

Do not make them Consumable Items simply to reuse the existing inventory type.

---

## 10. Tutorial and Progression

### Greenvale Border Path

The first tutorial should teach only the minimum loop:

1. encounter Cherry Tree;
2. harvest Cherry;
3. receive a Consumable Item;
4. learn how to equip/select/use the consumable;
5. restore Health through the Cherry periodic-heal Status Effect.

This establishes the category without introducing cooking, farming, fishing, food quality, or multiple competing effects.

### Later progression

Additional families can be introduced when the relevant world/system context exists:

- new produce through new biomes or farming;
- mushrooms when differentiated effects become useful;
- fish alongside fishing/aquatic gathering;
- raw/prepared food alongside cooking;
- drinks when processed consumables become useful;
- stronger prepared foods when the player understands the basic consumable loop.

---

## 11. Content-Creation Rules

Before creating a new Consumable Item data asset, define:

1. its gameplay identity/name;
2. its icon;
3. its acquisition source or planned source;
4. whether it can be directly consumed;
5. its intended effect family;
6. its `ResourceCostEntry`;
7. its `StatusEffectApplicationEntry` list;
8. whether its applied Status Effect is `Buff`, `Debuff`, or `Mixed`;
9. whether it is also an ingredient;
10. its progression purpose.

A sprite identity may exist before all gameplay values are finalized, but permanent data should still be created deliberately rather than automatically from sprite availability.
