# Skill Catalogue

## Purpose

This document defines the current authored **Skill** catalogue for **FlameOfTheLost**.

The current data set contains **13 Skill profiles**:

- 7 Melee Skills;
- 6 Ranged Skills.

Skills are stored separately from Weapon Items because a Skill defines attack behaviour, requirements, costs, Status Effect applications, harvesting interaction, and — for Ranged Skills — projectile behaviour.

---

## 1. Current Skill Styles

The current `SkillStyle` definition supports:

- `Default`
- `Melee`
- `Ranged`

Concrete authored Skill profile types currently use:

- `MeleeSkillProfile` -> `Melee`
- `RangedSkillProfile` -> `Ranged`

`Default` exists in the enum but is not represented by the current concrete authored profiles listed here.

---

## 2. Current Melee Skills

| Skill ID | Asset Identity | Skill |
|---:|---|---|
| 81001 | `SKM_Slash` | Slash |
| 81002 | `SKM_Dig` | Dig |
| 81011 | `SKM_FeralshroomCapBash` | Feralshroom Cap Bash |
| 81101 | `SKM_Stab` | Stab |
| 81102 | `SKM_Chop` | Chop |
| 81201 | `SKM_HemorrhageSlash` | Hemorrhage Slash |
| 81202 | `SKM_LightningStab` | Lightning Stab |

Melee asset prefix:

`SKM_`

Current Melee folder:

`Assets/Resources/Data/Inventory/Skills/Melee`

---

## 3. Current Ranged Skills

| Skill ID | Asset Identity | Skill |
|---:|---|---|
| 82001 | `SKR_Shot` | Shot |
| 82011 | `SKR_FeralshroomToxicPuff` | Feralshroom Toxic Puff |
| 82012 | `SKR_AquabaraWaterGlob` | Aquabara Water Glob |
| 82101 | `SKR_QuickShot` | Quick Shot |
| 82201 | `SKR_DoubleShot` | Double Shot |
| 82202 | `SKR_TrippleShot` | Tripple Shot |

Ranged asset prefix:

`SKR_`

Current Ranged folder:

`Assets/Resources/Data/Inventory/Skills/Ranged`

> `TrippleShot` is recorded exactly as the current authored asset identity. Renaming should only be done deliberately in the data/codebase rather than silently corrected in documentation.

---

## 4. Current ID Structure

| ID Range | Current Role |
|---|---|
| `81xxx` | Melee Skills |
| `82xxx` | Ranged Skills |

Current authored grouping visible inside those ranges:

### Melee

- `810xx` — base / early Melee and creature-specific Melee Skills
- `811xx` — additional standard Melee Skills
- `812xx` — advanced/specialized Melee Skills

### Ranged

- `820xx` — base / early Ranged and creature-specific Ranged Skills
- `821xx` — additional Ranged Skills
- `822xx` — multi-shot / advanced Ranged Skills

These descriptions reflect the present catalogue organization only. They should not be treated as a permanent semantic allocation unless the project ID convention explicitly defines them that way.

---

## 5. Identity Rules

Skill profile identity contains:

- ID;
- key;
- display name;
- description;
- icon.

Current asset naming formats:

### Melee

`<SkillID>_SKM_<SkillName>`

### Ranged

`<SkillID>_SKR_<SkillName>`

Examples:

- `81001_SKM_Slash`
- `81202_SKM_LightningStab`
- `82001_SKR_Shot`
- `82201_SKR_DoubleShot`

---

## 6. Creature-Specific Skills

Current creature-derived Skills include:

### Feralshroom

- `81011_SKM_FeralshroomCapBash`
- `82011_SKR_FeralshroomToxicPuff`

### Aquabara

- `82012_SKR_AquabaraWaterGlob`

Creature-specific Skills remain normal Skill profiles. Their source does not require a separate Skill system or Skill Style.

---

## 7. Catalogue Summary

| Style | Authored Skills |
|---|---:|
| Melee | 7 |
| Ranged | 6 |
| **Total** | **13** |

The detailed shared authoring rules are documented in `SkillGameplayDesign.md`.
