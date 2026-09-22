# Weapon Catalogue

## Purpose

This document defines the current authored **Weapon Item** catalogue for **FlameOfTheLost**.

The current data set contains **9 Weapon Item profiles** across five authored folders:

- Special
- Daggers
- Swords
- Axes
- Bows

Weapon Items are non-stackable equipment. Each Weapon defines its weapon type, fixed Normal Skill, optional innate Special Skill, and capability requirements.

---

## 1. Weapon Profile Rules

A `WeaponProfile` currently defines:

1. Weapon Item identity;
2. `WeaponType`;
3. one required Normal Skill;
4. one optional Innate Special Skill;
5. capability requirements for equipping the Weapon.

Weapon Items are:

- non-stackable;
- maximum stack size `1`;
- `ItemType.Weapon`.

### Skill Resolution

The Special Skill used by a Weapon is resolved in this order:

`Active Special Skill -> Innate Special Skill -> Normal Skill`

The active Special Skill belongs to per-owner runtime state and is not stored in the Weapon profile.

This means:

- every Weapon should define a Normal Skill;
- a Weapon may optionally provide its own innate Special Skill;
- a character may override that innate Special Skill with an active learned/equipped Special Skill;
- if no active or innate Special Skill exists, the Weapon falls back to its Normal Skill.

---

## 2. Weapon Types

The current `WeaponType` definition supports:

### Melee

- `Sword`
- `Axe`
- `Dagger`

### Ranged

- `Bow`
- `Crossbow`
- `Staff`

### Special

- `Special`

Not every supported Weapon Type currently has authored Weapon Item data.

Current authored types represented by the catalogue are:

- `Special`
- `Dagger`
- `Sword`
- `Axe`
- `Bow`

`Crossbow` and `Staff` are supported by the type system but currently have no Weapon profiles in this catalogue.

---

## 3. Current Weapon Catalogue

| Item ID | Asset Identity | Category | Weapon Identity |
|---:|---|---|---|
| 71001 | `WP_FeralshroomHardenedCap` | Special | Feralshroom Hardened Cap |
| 71002 | `WP_FeralshroomToxicSac` | Special | Feralshroom Toxic Sac |
| 71003 | `WP_AquabaraWaterSac` | Special | Aquabara Water Sac |
| 71101 | `WP_Dagger` | Daggers | Dagger |
| 71201 | `WP_WornShortSword` | Swords | Worn Short Sword |
| 71202 | `WP_Longsword` | Swords | Longsword |
| 71301 | `WP_Pickaxe` | Axes | Pickaxe |
| 71302 | `WP_Axe` | Axes | Axe |
| 71401 | `WP_Bow` | Bows | Bow |

---

## 4. ID and Folder Structure

Current Weapon IDs are grouped by authored category:

| ID Range | Current Category |
|---|---|
| `710xx` | Special |
| `711xx` | Daggers |
| `712xx` | Swords |
| `713xx` | Axes |
| `714xx` | Bows |

Current data folder:

`Assets/Resources/Data/Inventory/Items/Weapons`

Current subfolders:

- `Special`
- `Daggers`
- `Swords`
- `Axes`
- `Bows`

Asset naming format:

`<ItemID>_WP_<WeaponName>`

Examples:

- `71001_WP_FeralshroomHardenedCap`
- `71201_WP_WornShortSword`
- `71401_WP_Bow`

The folder category and Weapon ID grouping should remain aligned unless the Weapon data convention is deliberately redesigned.

---

## 5. Category Roles

### Special Weapons

Current Special Weapons are creature-derived Weapon identities:

- Feralshroom Hardened Cap
- Feralshroom Toxic Sac
- Aquabara Water Sac

They use the `Special` Weapon Type rather than pretending to belong to a conventional melee/ranged equipment family.

### Daggers

Current authored Dagger:

- Dagger

### Swords

Current authored Swords:

- Worn Short Sword
- Longsword

### Axes

Current authored Axe-category Weapons:

- Pickaxe
- Axe

The category currently contains both a conventional Axe and the Pickaxe. Their exact Skill assignments determine their combat/harvesting behaviour.

### Bows

Current authored Bow:

- Bow

Bow behaviour depends on compatible Ranged Skills and their projectile configuration.

---

## 6. Weapon and Skill Boundary

A Weapon defines which Skills are available from that Weapon, while the Skill profile defines the actual attack behaviour.

Weapon-side responsibilities:

- Weapon Type;
- Normal Skill reference;
- optional Innate Special Skill reference;
- equip capability requirements.

Skill-side responsibilities include:

- Skill Style;
- provision type;
- Weapon Type compatibility;
- capability requirements;
- damage and knockback;
- harvesting interaction;
- resource cost;
- Status Effect applications;
- ranged projectile configuration where applicable.

Do not duplicate Skill gameplay values into Weapon data.

---

## 7. Authoring Rules

1. Every Weapon must define a non-null Normal Skill.
2. Innate Special Skill is optional.
3. Active Special Skills are runtime/per-owner state and must not be authored into the shared Weapon ScriptableObject.
4. Weapon capability requirements determine whether a character can equip the Weapon.
5. Skill capability requirements independently determine whether a character can use a Skill.
6. A Weapon's Skill references should be compatible with its `WeaponType`.
7. Add a new Weapon category folder only when a distinct authored category actually exists.
8. Do not create separate Weapon identities solely because different characters use the same Weapon profile.
9. Exact attack behaviour belongs to Skill data rather than the Weapon catalogue.

---

## 8. Catalogue Summary

| Category | Authored Weapons |
|---|---:|
| Special | 3 |
| Daggers | 1 |
| Swords | 2 |
| Axes | 2 |
| Bows | 1 |
| **Total** | **9** |

Supported but currently unauthored Weapon Types:

- Crossbow
- Staff
