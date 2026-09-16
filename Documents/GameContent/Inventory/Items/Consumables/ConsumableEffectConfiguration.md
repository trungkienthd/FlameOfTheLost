# Consumable Effect Configuration

## Purpose

This document records the **current authored `StatusEffectProfile` configuration** for the 45 created food-based Consumable Items in **FlameOfTheLost**.

The `.asset` profiles are the gameplay authority. This document mirrors their current configuration so later balance changes should be made deliberately and then reflected here.

It complements [ConsumableGameplayDesign](ConsumableGameplayDesign.md):

- `ConsumableGameplayDesign.md` defines shared authoring principles and family-level intent;
- this document records the concrete Status Effect identity, lifetime, stacking, affinity, modifiers, and periodics currently authored in data.

---

## 1. Shared ConsumableItemProfile Configuration

Current ordinary food uses no consumption resource cost unless a future item explicitly overrides that rule.

Current food applies one guaranteed primary Status Effect:

| Field | Value |
|---|---:|
| Chance | 1.0 |
| Potency | 1.0 |

The referenced `StatusEffectProfile` contains the gameplay behaviour.

### Status Effect Profile Identity

Status Effect IDs for the current consumable set occupy:

`53001-53045`

Naming format:

`<StatusEffectID>_SE_Csm_<ConsumableName>_<EffectIdentity>`

Key format:

`<CONSUMABLE_NAME>_<EFFECT_IDENTITY>`

Display-name format:

`<Effect Identity> (<Consumable Name>)`

Examples:

- `53043_SE_Csm_Cherry_PeriodicHeal`
- `53013_SE_Csm_RedMushroom_GlassCannon`
- `53035_SE_Csm_Chili_Berserk`

---

## 2. Shared StatusEffectProfile Rules

The current 45 profiles use:

- no Status Effect Flags;
- no Status Effect VFX initially;
- Stat Modifier Combination Type = `AddStacks`;
- VFX Sorting Order Offset = `5`.

Lifetime/stacking is **not universal**:

- `53001-53006` use `Max Stacks = 1` and `Duration Refresh Type = Extend`;
- `53007-53042` and `53044-53045` use `Max Stacks = 1` and `Duration Refresh Type = Refresh`;
- `53043 Cherry` uses `Max Stacks = 2` and `Duration Refresh Type = Extend`.

### Periodic Interpretation

`AmountPerSecond` is the authored rate. `TickInterval` controls delivery frequency.

For entries with `TickInterval = 0`, this document records that serialized value directly as `continuous/default tick` rather than inventing a delivery interval.


---

## 3. Meat, Poultry, and Eggs

| SE ID | Item ID | Consumable | StatusEffectProfile | Affinity | Duration | Stacks / Refresh | Stat Modifiers | Stat Periodics |
|---:|---:|---|---|---|---:|---|---|---|
| 53001 | 72001 | Raw Poultry | `SE_Csm_RawPoultry_RecoveryFatigue` | Mixed | 6s | 1 / Extend | MovementSpeedScale ×0.95 | HP Heal 3/s @0.5s; Stamina Drain 1/s @0.5s |
| 53002 | 72002 | Cooked Poultry | `SE_Csm_CookedPoultry_Recovery` | Buff | 6s | 1 / Extend | None | HP Heal 5/s @0.5s; Stamina Restore 1/s @0.5s |
| 53003 | 72003 | Raw Meat | `SE_Csm_RawMeat_RecoveryVulnerability` | Mixed | 6s | 1 / Extend | DamageReceived ×1.08 | HP Heal 4/s @0.5s |
| 53004 | 72004 | Cooked Meat | `SE_Csm_CookedMeat_Recovery` | Buff | 7s | 1 / Extend | None | HP Heal 6/s @0.5s |
| 53005 | 72009 | Raw Steak | `SE_Csm_RawSteak_BerserkRecovery` | Mixed | 8s | 1 / Extend | DamageDealt ×1.2; DamageReceived ×1.05 | HP Heal 4/s @1s |
| 53006 | 72010 | Cooked Steak | `SE_Csm_CookedSteak_PowerRecovery` | Buff | 10s | 1 / Extend | DamageDealt ×1.2; DamageDealt +1 | HP Heal 5/s @1s |
| 53007 | 72011 | Raw Meat Leg | `SE_Csm_RawMeatLeg_HeavyRecovery` | Mixed | 10s | 1 / Refresh | MovementSpeedScale -0.5 | HP Heal 3/s @0.5s; Stamina Drain 1/s @0.5s |
| 53008 | 72012 | Cooked Meat Leg | `SE_Csm_CookedMeatLeg_FortifiedRecovery` | Buff | 10s | 1 / Refresh | DamageReceived ×0.94 | HP Heal 4/s @0.5s |
| 53009 | 72015 | Egg | `SE_Csm_Egg_LightRecovery` | Buff | 5s | 1 / Refresh | None | HP Heal 1/s @0.5s; Stamina Restore 1/s @0.5s |
| 53010 | 72016 | Fried Egg | `SE_Csm_FriedEgg_StaminaEfficiencyRecovery` | Buff | 5s | 1 / Refresh | StaminaCost ×0.95 | HP Heal 2/s @0.5s; Stamina Restore 2/s @0.5s |

### Design Notes

The current raw/prepared pairs are intentionally not normalized to one balance formula. Raw food can carry drawbacks, while prepared food can shift toward stronger recovery, offence, defence, or efficiency according to the authored asset.

---

## 4. Mushrooms

| SE ID | Item ID | Consumable | StatusEffectProfile | Affinity | Duration | Stacks / Refresh | Stat Modifiers | Stat Periodics |
|---:|---:|---|---|---|---:|---|---|---|
| 53011 | 72005 | Brown Mushroom | `SE_Csm_BrownMushroom_StaminaRecovery` | Buff | 8s | 1 / Refresh | StaminaRegenRate ×1.15 | Stamina Restore 2/s @1s |
| 53012 | 72006 | Purple Mushroom | `SE_Csm_PurpleMushroom_FPRecoverySlow` | Mixed | 10s | 1 / Refresh | MovementSpeedScale ×0.92 | FP Restore 3/s @1s |
| 53013 | 72013 | Red Mushroom | `SE_Csm_RedMushroom_GlassCannon` | Mixed | 10s | 1 / Refresh | DamageReceived ×1.2; DamageDealt ×1.35 | None |
| 53014 | 72014 | Blue Mushroom | `SE_Csm_BlueMushroom_FPEfficiency` | Buff | 5s | 1 / Refresh | FPCost -2 | FP Restore 3/s @1s |

### Design Notes

The mushroom family remains specialist-oriented: Brown emphasizes Stamina, Purple trades mobility for FP recovery, Red is a high-risk glass-cannon effect, and Blue focuses on FP efficiency/restoration.

---

## 5. Dairy

| SE ID | Item ID | Consumable | StatusEffectProfile | Affinity | Duration | Stacks / Refresh | Stat Modifiers | Stat Periodics |
|---:|---:|---|---|---|---:|---|---|---|
| 53015 | 72007 | Cheese | `SE_Csm_Cheese_FortifiedRecovery` | Buff | 12s | 1 / Refresh | VirtualHPGranted +10 | HP Heal 2/s @1s |
| 53016 | 72008 | Milk | `SE_Csm_Milk_BalancedRecovery` | Buff | 8s | 1 / Refresh | StaminaCost ×0.95 | HP Heal 2/s @0.5s; Stamina Restore 2/s @0.5s |

---

## 6. Fish

| SE ID | Item ID | Consumable | StatusEffectProfile | Affinity | Duration | Stacks / Refresh | Stat Modifiers | Stat Periodics |
|---:|---:|---|---|---|---:|---|---|---|
| 53017 | 72017 | Blue Fish | `SE_Csm_BlueFish_FPEfficiencyRecovery` | Buff | 10s | 1 / Refresh | FPCost ×0.95 | FP Restore 1/s @0.5s |
| 53018 | 72018 | Cyan Fish | `SE_Csm_CyanFish_MobilityRecovery` | Buff | 10s | 1 / Refresh | MovementSpeedScale ×1.1 | Stamina Restore 1/s @0.5s |
| 53019 | 72019 | Dark Blue Fish | `SE_Csm_DarkBlueFish_DefensiveRecovery` | Buff | 12s | 1 / Refresh | DamageReceived ×0.92 | HP Heal 2/s @0.5s |
| 53020 | 72020 | Green Fish | `SE_Csm_GreenFish_StaminaRecovery` | Buff | 10s | 1 / Refresh | StaminaRegenRate ×1.15 | Stamina Restore 2/s @0.5s |
| 53021 | 72021 | Brown Fish | `SE_Csm_BrownFish_KnockbackResistanceRecovery` | Buff | 10s | 1 / Refresh | KnockbackReceived ×0.8 | HP Heal 1/s @0.5s |
| 53022 | 72022 | Red Fish | `SE_Csm_RedFish_OffensiveRecovery` | Mixed | 10s | 1 / Refresh | VirtualHPGranted +20; DamageDealt ×1.35; DamageReceived ×1.1; KnockbackReceived ×1.1 | None |
| 53023 | 72023 | Yellow Fish | `SE_Csm_YellowFish_StaminaEfficiency` | Buff | 10s | 1 / Refresh | StaminaCost ×0.9; StaminaRegenRate ×1.05 | Stamina Restore 1/s @0.5s |
| 53024 | 72024 | Grey Fish | `SE_Csm_GreyFish_CombatStability` | Buff | 12s | 1 / Refresh | KnockbackDealt ×1.25; DamageReceived ×0.95 | None |

### Design Notes

Fish remain combat-specialization food. Their current colour names are working identities; future species naming should preserve or deliberately redesign the authored niche.

---

## 7. Grain and Large Produce

| SE ID | Item ID | Consumable | StatusEffectProfile | Affinity | Duration | Stacks / Refresh | Stat Modifiers | Stat Periodics |
|---:|---:|---|---|---|---:|---|---|---|
| 53025 | 72025 | Wheat | `SE_Csm_Wheat_StaminaRecovery` | Buff | 6s | 1 / Refresh | None | Stamina Restore 2/s @0.5s |
| 53026 | 72027 | Green Squash | `SE_Csm_GreenSquash_BalancedRecovery` | Buff | 8s | 1 / Refresh | StaminaRegenRate ×1.1 | HP Heal 2/s @0.5s; Stamina Restore 1/s @0.5s |
| 53027 | 72043 | Cabbage | `SE_Csm_Cabbage_DefensiveRecovery` | Buff | 10s | 1 / Refresh | DamageReceived ×0.9; VirtualHPGranted +10 | None |
| 53028 | 72049 | Pumpkin | `SE_Csm_Pumpkin_FortifiedRecovery` | Buff | 12s | 1 / Refresh | VirtualHPGranted +25; MovementSpeedScale ×1.05 | None |
| 53029 | 72057 | Corn | `SE_Csm_Corn_StaminaEfficiencyRecovery` | Buff | 10s | 1 / Refresh | StaminaCost ×0.9 | Stamina Restore 2/s @0.5s |
| 53030 | 72059 | Parsnip | `SE_Csm_Parsnip_RecoveryRegen` | Buff | 8s | 1 / Refresh | StaminaRegenRate ×1.1 | HP Heal 3/s @0.5s |
| 53031 | 72063 | Watermelon | `SE_Csm_Watermelon_BalancedRecovery` | Buff | 8s | 1 / Refresh | None | HP Heal 5/s @0.5s; FP Restore 1/s @0.5s; Stamina Restore 3/s @0.5s |

---

## 8. Peppers and Chili

| SE ID | Item ID | Consumable | StatusEffectProfile | Affinity | Duration | Stacks / Refresh | Stat Modifiers | Stat Periodics |
|---:|---:|---|---|---|---:|---|---|---|
| 53032 | 72029 | Red Pepper | `SE_Csm_RedPepper_OffensiveStamina` | Buff | 10s | 1 / Refresh | DamageDealt ×1.07 | Stamina Restore 1/s @0.5s |
| 53033 | 72037 | Orange Pepper | `SE_Csm_OrangePepper_MobilityStaminaRegen` | Buff | 10s | 1 / Refresh | MovementSpeedScale ×1.05; StaminaRegenRate ×1.08 | None |
| 53034 | 72045 | Green Pepper | `SE_Csm_GreenPepper_DefensiveEfficiency` | Buff | 10s | 1 / Refresh | DamageReceived ×0.96; StaminaCost ×0.92 | None |
| 53035 | 72053 | Chili | `SE_Csm_Chili_Berserk` | Mixed | 8s | 1 / Refresh | DamageDealt ×1.25; StaminaCost ×0.9 | HP Damage 1/s @0.5s [True Damage] |

### Chili HP-Damage Policy

`53035_SE_Csm_Chili_Berserk` currently applies:

- `DamageDealt ×1.25`;
- `StaminaCost ×0.90`;
- HP Damage `1/s @0.5s`;
- `TreatHPDamageAsTrueDamage = true`;
- `TrueDamageBypassVirtualHP = false`.

Chili is therefore a `Mixed` effect: it grants strong offensive/efficiency benefits while applying unavoidable self-damage that does not bypass Virtual HP.

---

## 9. Ordinary Vegetables

| SE ID | Item ID | Consumable | StatusEffectProfile | Affinity | Duration | Stacks / Refresh | Stat Modifiers | Stat Periodics |
|---:|---:|---|---|---|---:|---|---|---|
| 53036 | 72033 | Tomato | `SE_Csm_Tomato_PeriodicHeal` | Buff | 6s | 1 / Refresh | None | HP Heal 3/s @0.5s |
| 53037 | 72035 | Carrot | `SE_Csm_Carrot_MobilityStamina` | Buff | 8s | 1 / Refresh | MovementSpeedScale ×1.04 | FP Restore 1/s @0.5s |
| 53038 | 72051 | Cucumber | `SE_Csm_Cucumber_StaminaRecovery` | Buff | 5s | 1 / Refresh | StaminaRegenRate ×1.09 | Stamina Restore 2/s @0.5s |

---

## 10. Fruit

| SE ID | Item ID | Consumable | StatusEffectProfile | Affinity | Duration | Stacks / Refresh | Stat Modifiers | Stat Periodics |
|---:|---:|---|---|---|---:|---|---|---|
| 53039 | 72031 | Apple | `SE_Csm_Apple_BalancedRecovery` | Buff | 6s | 1 / Refresh | None | HP Heal 2/s @0.5s; FP Restore 1/s @0.5s; Stamina Restore 1/s @0.5s |
| 53040 | 72039 | Peach | `SE_Csm_Peach_HealingRegen` | Buff | 6s | 1 / Refresh | StaminaRegenRate ×1.05; VirtualHPGranted +10 | HP Heal 4/s @0.5s |
| 53041 | 72041 | Grapes | `SE_Csm_Grapes_FPStaminaRecovery` | Buff | 6s | 1 / Refresh | FPCost ×0.96 | FP Restore 2/s (continuous/default tick); Stamina Restore 2/s (continuous/default tick) |
| 53042 | 72047 | Pear | `SE_Csm_Pear_DefensiveRecovery` | Buff | 5s | 1 / Refresh | DamageReceived ×0.92; VirtualHPGranted +5 | HP Heal 2/s @0.5s |
| 53043 | 72055 | Cherry | `SE_Csm_Cherry_PeriodicHeal` | Buff | 4s | 2 / Extend | None | HP Heal 5/s @0.5s |
| 53044 | 72065 | Strawberry | `SE_Csm_Strawberry_HPFPRecovery` | Buff | 6s | 1 / Refresh | None | HP Heal 4/s @0.5s; FP Restore 1/s @0.5s |

### Cherry Baseline

`53043_SE_Csm_Cherry_PeriodicHeal` is the tutorial baseline:

- Affinity = `Buff`;
- Duration = `4s`;
- Max Stacks = `2`;
- Duration Refresh Type = `Extend`;
- no Flags;
- no Stat Modifiers;
- HP Heal = `5/s @0.5s`.

A single uninterrupted application therefore restores 20 HP before stacking/extension behaviour is considered.

---

## 11. Drink

| SE ID | Item ID | Consumable | StatusEffectProfile | Affinity | Duration | Stacks / Refresh | Stat Modifiers | Stat Periodics |
|---:|---:|---|---|---|---:|---|---|---|
| 53045 | 72064 | Orange Juice | `SE_Csm_OrangeJuice_UtilityRecovery` | Buff | 8s | 1 / Refresh | MovementSpeedScale ×1.05; StaminaRegenRate ×1.12 | FP Restore 1/s @0.5s; Stamina Restore 1/s @0.5s |

---

## 12. Profiles Not Yet Included

The sprite mapping still reserves:

- `Csm_Turnip`
- `Csm_PurpleBerry`

These do not currently have created Consumable Item / Consumable Status Effect profile entries in this authored set.

---

## 13. Family Summary

| Family | Current Gameplay Direction |
|---|---|
| Raw meat | Recovery with deliberate drawbacks or trade-offs |
| Cooked/prepared meat | Stronger sustain, offence, defence, or efficiency |
| Mushrooms | Specialist FP, Stamina, or high-risk offensive effects |
| Dairy | Durability and balanced sustain |
| Fish | Combat-stat specialization |
| Basic vegetables | Readable recovery and light utility |
| Peppers | Combat-oriented modifiers |
| Chili | High-risk offensive/efficiency stimulant with true self-damage |
| Fruit | Multi-resource recovery, defence, or healing utility |
| Large produce | Sustained recovery, Virtual HP, and broader utility |
| Processed drink | Multi-resource utility with mobility/regen support |
