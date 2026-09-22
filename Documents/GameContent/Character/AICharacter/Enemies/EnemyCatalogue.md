# Enemy Catalogue

## Purpose

This document lists the current authored **Enemy identities** in **FlameOfTheLost** and their current variants.

Detailed Enemy-type behaviour belongs to the corresponding Enemy-type documentation.

Current authored Enemy identities:

- Feralshroom
- Aquabara

Current Enemy AI type:

- Territorial Enemy

---

## 1. Feralshroom

Base Enemy profile:

`10101_Chr_Enemy_Feralshroom`

Current variants:

- Feralshroom Basher
- Feralshroom Toxic

Current shared base configuration:

| Property | Value |
|---|---:|
| Movement Speed | 1.5 |
| Acceleration | 15 |
| Deceleration | 20 |
| Base HP | 30 |
| Base FP | 1 |
| Base Stamina | 50 |
| Lost Ember Reward | 30 |

### Territorial Variants

#### Feralshroom Basher

Combat identity:

```text
Feralshroom Hardened Cap
→ Cap Bash
```

Role:

- close-range Feralshroom variant;
- shorter preferred attack range;
- melee-like normal attack identity.

#### Feralshroom Toxic

Combat identity:

```text
Feralshroom Toxic Sac
→ Toxic Puff
→ Toxic Gas projectile
```

Role:

- ranged Feralshroom variant;
- longer preferred attack range;
- toxic projectile normal attack identity.

---

## 2. Aquabara

Base Enemy profile:

`10301_Chr_Enemy_Aquabara`

Current variants:

- Aquabara Pond
- Aquabara Ocean

Current shared base configuration:

| Property | Value |
|---|---:|
| Movement Speed | 1.0 |
| Acceleration | 10 |
| Deceleration | 13 |
| Base HP | 40 |
| Base FP | 15 |
| Base Stamina | 60 |
| Lost Ember Reward | 40 |

### Shared Combat Identity

Both current Aquabara variants use:

```text
Aquabara Water Sac
→ Water Glob
→ Water Glob projectile
```

Their distinction currently comes primarily from Territorial AI tuning rather than separate weapon/skill identities.

---

## 3. Catalogue Summary

| Enemy | Variants | Current Enemy Type |
|---|---:|---|
| Feralshroom | 2 | Territorial |
| Aquabara | 2 | Territorial |
| **Total** | **4 variants** | |

Detailed variant AI values are documented in:

`Territorial/TerritorialEnemyCatalogue.md`

---

## 4. Catalogue Rules

1. Enemy identity and Enemy variant are separate concepts.
2. Do not create a new base Enemy identity only because AI tuning differs.
3. Shared stats/movement/reward belong to the base Enemy identity when authored that way.
4. Variant-specific initial inventory and Territorial configuration belong to the variant.
5. Enemy-type classification describes AI behaviour, not species identity.
