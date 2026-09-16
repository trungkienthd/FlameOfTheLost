# Consumable Gameplay Design

## Purpose

This document defines the intended gameplay treatment of food-based Consumable Items in **FlameOfTheLost**.

The current authored values live in [ConsumableEffectConfiguration](ConsumableEffectConfiguration.md). The `.asset` profiles are authoritative when balance changes are made.

---

## 1. Core Principle

A Consumable Item should answer at least one gameplay question:

- Why would the player use it now?
- Why would the player keep it for later?
- Why would the player process or cook it instead of consuming it directly?
- Why is this item distinct from another food item?

The game should avoid a catalogue where every food is just the same heal with slightly different numbers.

Consumables should normally be advantageous overall, but individual contributions can include meaningful trade-offs. When positive and negative contributions coexist, the Status Effect should use `Mixed` affinity.

Examples in the current authored set include:

- Raw Poultry: HP recovery with slower movement and Stamina drain;
- Raw Meat: HP recovery with increased damage received;
- Red Mushroom: very high offence with increased damage received;
- Chili: strong offence and Stamina efficiency paired with true self-damage.

---

## 2. Consumable Profile Responsibilities

A `ConsumableItemProfile` owns:

1. its consumption `ResourceCostEntry`;
2. its `StatusEffectApplicationEntry` list.

The gameplay effect itself belongs to the referenced `StatusEffectProfile`.

Current ordinary food uses no consumption resource cost and normally applies one guaranteed primary Status Effect with:

- Chance = `1.0`;
- Potency = `1.0`.

---

## 3. Status Effect Authoring Rules

### 3.1 Identity

Current consumable Status Effect IDs use `53001-53045`.

Profile naming:

`<ID>_SE_Csm_<ConsumableName>_<EffectIdentity>`

Keys omit the `SE_Csm` prefix and ID, for example:

`CHILI_BERSERK`

Display names use:

`<Effect Identity> (<Consumable Name>)`

### 3.2 Affinity

Use:

- `Buff` when the contribution package is beneficial;
- `Debuff` when it is harmful;
- `Mixed` when meaningful positive and negative contributions coexist.

Affinity should describe the **whole contribution package**, not only its display name.

### 3.3 Flags

Ordinary food currently uses no Status Effect Flags.

Food differentiation is authored through:

- Stat Modifiers;
- Stat Periodics;
- lifetime and stacking behaviour.

### 3.4 Stat Modifiers

Current food uses targets including:

- `DamageReceived`
- `KnockbackReceived`
- `VirtualHPGranted`
- `DamageDealt`
- `KnockbackDealt`
- `MovementSpeedScale`
- `FPCost`
- `StaminaCost`
- `StaminaRegenRate`

`Multiply` uses `1.0` as neutral. `AddFlat` directly adds/subtracts a flat amount.

### 3.5 Stat Periodics

Periodic targets are HP, FP, and Stamina.

Current authored methods include:

- HP: `Heal`, `Damage`;
- FP: `Restore`;
- Stamina: `Restore`, `Drain`.

`AmountPerSecond` is the rate. `TickInterval` controls delivery frequency.

### 3.6 Lifetime and Stacking

There is no longer one universal lifetime/stacking rule for every food.

Current authored patterns are:

- early profiles `53001-53006`: `Max Stacks = 1`, `Extend`;
- most profiles `53007-53042`, `53044-53045`: `Max Stacks = 1`, `Refresh`;
- Cherry `53043`: `Max Stacks = 2`, `Extend`.

`Extend` increases remaining duration and can add stacks up to `Max Stacks`; `Refresh` resets remaining duration to the profile duration.

All current consumable profiles use Stat Modifier Combination Type `AddStacks`.

---

## 4. Consumable Families

### Produce

Produce forms the most readable direct-use family and can cover HP/FP/Stamina recovery, light defence, regeneration, efficiency, or mobility.

Current examples include Cherry, Apple, Peach, Pear, Strawberry, Tomato, Carrot, Cucumber, and larger crops.

### Raw Meat

Raw food remains usable directly but can carry drawbacks or rougher recovery behaviour.

Current raw/prepared pairs are:

- Raw Poultry -> Cooked Poultry
- Raw Meat -> Cooked Meat
- Raw Steak -> Cooked Steak
- Raw Meat Leg -> Cooked Meat Leg
- Egg -> Fried Egg

### Prepared Food

Prepared food should be a meaningful alternative to the raw item through stronger recovery, removed drawbacks, added combat utility, or improved efficiency.

The exact upgrade does not need to follow one fixed formula.

### Mushrooms

Mushrooms are specialist consumables:

- Brown Mushroom: Stamina recovery/regeneration;
- Purple Mushroom: FP recovery with reduced mobility;
- Red Mushroom: high-risk glass-cannon offence;
- Blue Mushroom: FP efficiency and recovery.

### Fish

Fish focus on combat specialization: FP economy, mobility, defence, Stamina, knockback, Virtual HP, and aggressive play.

Colour names remain working identities and may later be replaced by species names.

### Egg and Dairy

Egg, Fried Egg, Cheese, and Milk bridge basic food, recovery, efficiency, and durability.

### Peppers and Chili

- Red Pepper supports offence plus Stamina recovery.
- Orange Pepper supports mobility and Stamina regeneration.
- Green Pepper supports defence and Stamina efficiency.
- Chili is a `Mixed` high-risk stimulant with `DamageDealt ×1.25`, `StaminaCost ×0.90`, and true HP self-damage.

### Drinks

Orange Juice establishes a processed drink identity combining mobility, Stamina regeneration, and FP/Stamina restoration.

---

## 5. Direct Use vs Ingredient Use

A food item can be both:

- directly consumed;
- used later as an ingredient.

This does not require duplicating it as a Material.

---

## 6. Cherry Tutorial Baseline

Cherry remains the first tutorial consumable.

Current authored Status Effect:

- Profile = `53043_SE_Csm_Cherry_PeriodicHeal`;
- Affinity = `Buff`;
- Duration = `4s`;
- Max Stacks = `2`;
- Duration Refresh Type = `Extend`;
- Flags = none;
- Stat Modifiers = none;
- HP Heal = `5/s @0.5s`.

A single uninterrupted application restores 20 HP. Repeated applications can extend the effect and increase stacks up to 2.

Cherry is still simple mechanically because it teaches the Consumable category, but its stacking now also demonstrates repeat-use behaviour.

---

## 7. Acquisition Is Separate from Item Category

Consumable Item describes what the inventory item is, not its source.

Possible sources include harvesting, farming, fishing, enemies, containers, vendors, cooking, quests, and other systems.

---

## 8. Seed Items

Seed packets shown on the Food sprite sheet remain outside the current Consumable Item design.

Conceptually:

`Seed Item -> Plant/Growth State -> Harvestable Crop -> Produce Consumable`

Do not make seed packets Consumable Items merely to reuse an existing item type.

---

## 9. Tutorial and Progression

### Greenvale Border Path

Current minimum loop:

1. encounter Cherry Tree;
2. harvest Cherry;
3. receive a Consumable Item;
4. learn to equip/select/use it;
5. restore Health through `Periodic Heal (Cherry)`.

The wider 45-item catalogue supports later areas and systems without overloading the tutorial.

---

## 10. Content-Creation Rules

Before creating a new Consumable Item, define:

1. gameplay identity/name;
2. icon;
3. source or planned source;
4. direct-use role;
5. effect family;
6. `ResourceCostEntry`;
7. `StatusEffectApplicationEntry`;
8. Status Effect ID/key/display name;
9. `Buff`, `Debuff`, or `Mixed` affinity;
10. modifiers/periodics;
11. lifetime and stacking behaviour;
12. whether it is also an ingredient;
13. progression purpose.

The profile asset remains the authority for concrete gameplay values.
