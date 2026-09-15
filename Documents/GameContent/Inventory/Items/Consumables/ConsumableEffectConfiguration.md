# Consumable Effect Configuration

## Purpose

This document defines the current authored numerical baseline for the created food-based `ConsumableItemProfile` assets in **FlameOfTheLost**.

It complements [ConsumableGameplayDesign](ConsumableGameplayDesign.md):

- `ConsumableGameplayDesign.md` explains **why** each family behaves the way it does;
- this document records the concrete `ResourceCostEntry`, `StatusEffectApplicationEntry`, and `StatusEffectProfile` configuration to author into data.

These values are an initial balance baseline and can be tuned after gameplay testing.

---

## 1. Shared ConsumableItemProfile Configuration

Unless explicitly overridden, every food in this document uses the following `ResourceCostEntry`:

| Field | Value |
|---|---:|
| HP Flat Cost | 0 |
| FP Flat Cost | 0 |
| Stamina Flat Cost | 0 |
| HP Percent of Max Cost | 0 |
| FP Percent of Max Cost | 0 |
| Stamina Percent of Max Cost | 0 |
| Cost Trigger Type | `OnAttackStart` |
| Blocked By Silence | `false` |
| Allow Self Kill With HP Cost | `false` |

Therefore `HasAnyCost` is false for ordinary food.

### Shared StatusEffectApplicationEntry

Each current food applies one primary Status Effect:

| Field | Value |
|---|---:|
| Chance | 1.0 |
| Potency | 1.0 |

The referenced `StatusEffectProfile` contains the actual behaviour.

---

## 2. Shared StatusEffectProfile Defaults

Unless overridden in an item's row:

| Field | Value |
|---|---|
| Max Stacks | 1 |
| Duration Refresh Type | `Refresh` |
| Stat Modifier Combination Type | `AddStacks` |
| Flags | None |
| VFX | None initially |

Food should not receive Status Effect Flags merely to create variety. Use Stat Modifiers and Stat Periodics first.

### Periodic Interpretation

`AmountPerSecond` is the true rate. `TickInterval` is how often that rate is delivered.

Example:

`HP / Heal / 3/s / 0.5s`

means 3 HP per second, delivered every 0.5 seconds.

For all HP healing, FP restoration, and Stamina restoration/drain entries below, leave unrelated HP-damage policy fields at their defaults.

For the Chili self-damage periodic specifically:

- `TreatHPDamageAsTrueDamage = false`
- `TrueDamageBypassVirtualHP = false`

This makes the Chili drawback behave as self-resource loss rather than incoming combat damage.

---

## 3. Meat, Poultry, and Eggs

| ID | Consumable | Affinity | Duration | Stat Modifiers | Stat Periodics |
|---:|---|---|---:|---|---|
| 72001 | Raw Poultry | Mixed | 6s | MovementSpeedScale ×0.95 | HP Heal 3/s @0.5s; Stamina Drain 1/s @0.5s |
| 72002 | Cooked Poultry | Buff | 6s | None | HP Heal 5/s @0.5s; Stamina Restore 1/s @0.5s |
| 72003 | Raw Meat | Mixed | 6s | DamageReceived ×1.08 | HP Heal 4/s @0.5s |
| 72004 | Cooked Meat | Buff | 7s | None | HP Heal 6/s @0.5s |
| 72009 | Raw Steak | Mixed | 8s | DamageDealt ×1.05; DamageReceived ×1.10 | HP Heal 4/s @0.5s |
| 72010 | Cooked Steak | Buff | 10s | DamageDealt ×1.08 | HP Heal 6/s @0.5s |
| 72011 | Raw Meat Leg | Mixed | 10s | MovementSpeedScale ×0.90 | HP Heal 5/s @0.5s; Stamina Drain 1/s @0.5s |
| 72012 | Cooked Meat Leg | Buff | 10s | DamageReceived ×0.94 | HP Heal 7/s @0.5s |
| 72015 | Egg | Buff | 5s | None | HP Heal 3/s @0.5s; Stamina Restore 1/s @0.5s |
| 72016 | Fried Egg | Buff | 7s | StaminaCost ×0.95 | HP Heal 4/s @0.5s; Stamina Restore 2/s @0.5s |

### Design Intent

Prepared versions are always preferable to their raw counterparts, but raw meat remains usable as emergency food rather than becoming a dead inventory item.

---

## 4. Mushrooms

| ID | Consumable | Affinity | Duration | Stat Modifiers | Stat Periodics |
|---:|---|---|---:|---|---|
| 72005 | Brown Mushroom | Buff | 8s | StaminaRegenRate ×1.15 | Stamina Restore 2/s @0.5s |
| 72006 | Purple Mushroom | Mixed | 10s | MovementSpeedScale ×0.92 | FP Restore 3/s @0.5s |
| 72013 | Red Mushroom | Mixed | 10s | DamageDealt ×1.15; DamageReceived ×1.10 | None |
| 72014 | Blue Mushroom | Buff | 10s | FPCost ×0.90 | FP Restore 2/s @0.5s |

### Design Intent

- Brown = Stamina recovery.
- Purple = strong FP recovery at a mobility cost.
- Red = glass-cannon offence.
- Blue = efficient sustained special-skill use.

---

## 5. Dairy

| ID | Consumable | Affinity | Duration | Stat Modifiers | Stat Periodics |
|---:|---|---|---:|---|---|
| 72007 | Cheese | Buff | 12s | VirtualHPGranted +8 | HP Heal 2/s @1.0s |
| 72008 | Milk | Buff | 8s | StaminaCost ×0.95 | HP Heal 2/s @0.5s; Stamina Restore 2/s @0.5s |

---

## 6. Fish

| ID | Consumable | Affinity | Duration | Stat Modifiers | Stat Periodics |
|---:|---|---|---:|---|---|
| 72017 | Blue Fish | Buff | 10s | FPCost ×0.90 | FP Restore 1.5/s @0.5s |
| 72018 | Cyan Fish | Buff | 10s | MovementSpeedScale ×1.08 | Stamina Restore 1/s @0.5s |
| 72019 | Dark Blue Fish | Buff | 12s | DamageReceived ×0.92 | HP Heal 2/s @0.5s |
| 72020 | Green Fish | Buff | 10s | StaminaRegenRate ×1.20 | Stamina Restore 2/s @0.5s |
| 72021 | Brown Fish | Buff | 10s | KnockbackReceived ×0.80 | HP Heal 3/s @0.5s |
| 72022 | Red Fish | Mixed | 10s | DamageDealt ×1.12; DamageReceived ×1.06 | HP Heal 2/s @0.5s |
| 72023 | Yellow Fish | Buff | 10s | StaminaCost ×0.88 | Stamina Restore 1/s @0.5s |
| 72024 | Grey Fish | Buff | 12s | KnockbackDealt ×1.15; DamageReceived ×0.96 | None |

The colour names remain working identities; later species renaming should preserve or deliberately redesign the gameplay niche rather than silently changing it.

---

## 7. Grain and Large Produce

| ID | Consumable | Affinity | Duration | Stat Modifiers | Stat Periodics |
|---:|---|---|---:|---|---|
| 72025 | Wheat | Buff | 6s | None | Stamina Restore 2/s @0.5s |
| 72027 | Green Squash | Buff | 8s | StaminaRegenRate ×1.10 | HP Heal 2/s @0.5s; Stamina Restore 1/s @0.5s |
| 72043 | Cabbage | Buff | 10s | DamageReceived ×0.96 | HP Heal 3/s @0.5s |
| 72049 | Pumpkin | Buff | 12s | VirtualHPGranted +6 | HP Heal 3/s @0.5s |
| 72057 | Corn | Buff | 10s | StaminaCost ×0.94 | Stamina Restore 2/s @0.5s |
| 72059 | Parsnip | Buff | 8s | StaminaRegenRate ×1.10 | HP Heal 3/s @0.5s |
| 72063 | Watermelon | Buff | 8s | None | HP Heal 4/s @0.5s; Stamina Restore 3/s @0.5s |

---

## 8. Peppers and Chili

| ID | Consumable | Affinity | Duration | Stat Modifiers | Stat Periodics |
|---:|---|---|---:|---|---|
| 72029 | Red Pepper | Buff | 10s | DamageDealt ×1.07 | Stamina Restore 1/s @0.5s |
| 72037 | Orange Pepper | Buff | 10s | MovementSpeedScale ×1.05; StaminaRegenRate ×1.08 | None |
| 72045 | Green Pepper | Buff | 10s | DamageReceived ×0.96; StaminaCost ×0.96 | None |
| 72053 | Chili | Mixed | 8s | DamageDealt ×1.15; MovementSpeedScale ×1.10; DamageReceived ×1.12 | HP Damage 1/s @0.5s |

### Chili HP-Damage Policy

For Chili's HP periodic:

- Target = HP
- Method = Damage
- Amount Per Second = `1`
- Tick Interval = `0.5s`
- Treat HP Damage As True Damage = `false`
- True Damage Bypass Virtual HP = `false`

Chili is intentionally advantageous overall but dangerous: high offence and mobility are exchanged for increased incoming damage and gradual self-damage.

---

## 9. Ordinary Vegetables

| ID | Consumable | Affinity | Duration | Stat Modifiers | Stat Periodics |
|---:|---|---|---:|---|---|
| 72033 | Tomato | Buff | 6s | None | HP Heal 3/s @0.5s |
| 72035 | Carrot | Buff | 8s | MovementSpeedScale ×1.04 | Stamina Restore 1.5/s @0.5s |
| 72051 | Cucumber | Buff | 6s | StaminaRegenRate ×1.08 | Stamina Restore 2/s @0.5s |

These values are intentionally modest so common produce does not invalidate cooked food, specialist mushrooms, or fish.

---

## 10. Fruit

| ID | Consumable | Affinity | Duration | Stat Modifiers | Stat Periodics |
|---:|---|---|---:|---|---|
| 72031 | Apple | Buff | 6s | None | HP Heal 3/s @0.5s; Stamina Restore 1/s @0.5s |
| 72039 | Peach | Buff | 6s | StaminaRegenRate ×1.05 | HP Heal 4/s @0.5s |
| 72041 | Grapes | Buff | 6s | None | FP Restore 2/s @0.5s; Stamina Restore 1/s @0.5s |
| 72047 | Pear | Buff | 7s | DamageReceived ×0.98 | HP Heal 3/s @0.5s |
| 72055 | Cherry | Buff | 5s | None | HP Heal 3/s @0.5s |
| 72065 | Strawberry | Buff | 6s | None | HP Heal 4/s @0.5s; FP Restore 1/s @0.5s |

### Cherry Baseline

Cherry has:

- no Flags;
- no Stat Modifiers;
- one Periodic:
  - Target = HP
  - Method = Heal
  - Amount Per Second = `3`
  - Tick Interval = `0.5s`

Over its `5s` duration, the complete effect restores `15 HP`.

---

## 11. Drink

| ID | Consumable | Affinity | Duration | Stat Modifiers | Stat Periodics |
|---:|---|---|---:|---|---|
| 72064 | Orange Juice | Buff | 8s | MovementSpeedScale ×1.05; StaminaRegenRate ×1.10 | Stamina Restore 3/s @0.5s; FP Restore 1/s @0.5s |

Orange Juice is intentionally stronger and more specialized than ordinary raw produce because it represents a processed consumable.

---

## 12. Profiles Not Yet Included

The sprite mapping reserves the following consumable identities, but they are not in the current created profile list and therefore have no numerical effect design in this document yet:

- `Csm_Turnip`
- `Csm_PurpleBerry`

Do not generate corresponding Status Effect data from this document until their `ConsumableItemProfile` assets are intentionally added.

---

## 13. Balance Summary by Family

| Family | Primary Gameplay Identity |
|---|---|
| Raw meat | Recovery with a drawback |
| Cooked meat | Strong HP recovery / combat sustain |
| Mushrooms | Specialist FP, Stamina, or offensive effects |
| Dairy | Durability and balanced sustain |
| Fish | Combat-stat specialization |
| Basic vegetables | Cheap mild sustain |
| Peppers | Combat-oriented modifiers |
| Chili | High-risk offensive stimulant |
| Fruit | Fast/light recovery |
| Large produce | Sustained HP/Stamina recovery |
| Processed drink | Stronger multi-resource utility |

The goal is for the catalogue to remain readable while avoiding dozens of functionally identical healing foods.
