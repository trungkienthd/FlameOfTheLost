# ID Attribute Convention
**Project:** FlameOfTheLost  
**Version:** 1.0  
**Scope:** ID attributes used by project-owned GameObjects, Behaviours, and ScriptableObjects

---

## 1. Purpose

This document defines the ID attribute convention used across FlameOfTheLost.

The purpose of this convention is to keep manually assigned integer IDs predictable, searchable, and grouped by gameplay/system category.

### Objectives

- Keep ID values readable and traceable by category
- Prevent ID collisions between unrelated systems
- Make ScriptableObject and scene GameObject references easier to inspect
- Support future content expansion without restructuring existing IDs
- Provide a consistent rule for assigning new IDs during development

All manually assigned ID attributes should follow this convention.

---

## 2. Core Rule

ID attributes are stored as integers.

Each major system category owns a dedicated numeric range.  
The first digit, or first few digits, identify the category or subcategory.

Do not assign IDs randomly.

Do not reuse the same ID for different assets, GameObjects, Behaviours, or ScriptableObjects.

---

## 3. Approved ID Range Registry

### 3.1 Character Identities

**Range:** `1xxx`  
**Asset Type:** ScriptableObject  
**Profile Type:** `CharacterIdentityProfile`

### Usage Scope

- Player character identity
- Enemy character identity
- Ally/NPC character identity
- Future character identity profiles

### Examples

```text
1001    Player
1101    Toxicschroom
```

---

### 3.2 Default Spawn Points

**Range:** `2xxx`  
**Object Type:** GameObject / Behaviour  
**Base Behaviour:** `CharacterDefaultSpawnPointController`

### Sub-Ranges

```text
20xx    Player default spawn points
21xx    Enemy-only default spawn points
```

### Behaviour Mapping

```text
20xx    PlayerDefaultSpawnPointController
21xx    CharacterDefaultSpawnPointController used by enemy-only spawn points
```

### Usage Scope

- Player default spawn points
- Enemy default spawn points
- Character placement anchors used as default spawn references

### Examples

```text
2001    Player default spawn point - Tutorial Area 1
2101    Enemy default spawn point - Tutorial Area 3
```

---

### 3.3 Death Triggers

**Range:** `3xxx`  
**Object Type:** GameObject / Behaviour  
**Behaviour Type:** `DeathTriggerController`

### Usage Scope

- Pitfall death triggers
- Water/deep-water death triggers
- Hazard death zones
- Out-of-bounds death triggers

### Examples

```text
3001    Tutorial pitfall death trigger
3002    Tutorial water death trigger
```

---

### 3.4 Tutorial World Visuals

**Range:** `4xxx`  
**Object Type:** GameObject / Behaviour  
**Behaviour Type:** `TutorialWorldVisualController`

### Usage Scope

- Tutorial visual blockers
- Tutorial gates
- Tutorial visual guidance objects
- World visual state controllers used by tutorial progression

### Examples

```text
4001    Tutorial Area 1 visual controller
4002    Tutorial Area 2 visual controller
```

---

### 3.5 Status Effects

**Range:** `5xxxx`  
**Asset Type:** ScriptableObject  
**Profile Type:** `StatusEffectProfile`

### Sub-Ranges

```text
50xxx   Status effects from terrains
51xxx   Status effects from character skill applications
52xxx   Status effects from consumable items
```

### Future Expansion

```text
53xxx   Reserved for future status effect source
54xxx   Reserved for future status effect source
55xxx   Reserved for future status effect source
```

### Usage Scope

- Terrain-applied status effects
- Skill-applied status effects
- Consumable item-applied status effects
- Future gameplay systems that apply status effects

### Examples

```text
50001   Terrain slow effect - grass
50002   Terrain slow effect - water
51001   Poison effect from Toxic Puff
52001   Healing-over-time effect from consumable item
```

---

### 3.6 Terrains

**Range:** `6xxx`  
**Asset Type:** ScriptableObject  
**Profile Type:** `TerrainProfile`

### Usage Scope

- Grass terrain profiles
- Water terrain profiles
- Mud terrain profiles
- Hazard terrain profiles
- Future terrain interaction profiles

### Examples

```text
6001    Grass terrain
6002    Water terrain
```

---

### 3.7 Items

**Range:** `7xxxx`  
**Asset Type:** ScriptableObject  
**Base Profile Type:** `ItemProfile`

### Sub-Ranges

```text
70xxx   Light sources
71xxx   Weapons
72xxx   Consumable items
73xxx   Projectiles
74xxx   Key items
75xxx   Materials
```

---

#### 3.7.1 Light Sources

**Range:** `70xxx`  
**Profile Type:** `LightSourceProfile`  
**Base Type:** `ItemProfile`

### Usage Scope

- Torches
- Lanterns
- Magical light sources
- Future carriable light source items

### Examples

```text
70001   Basic torch
70002   Oil lantern
```

---

#### 3.7.2 Weapons

**Range:** `71xxx`  
**Profile Type:** `WeaponProfile`  
**Base Type:** `ItemProfile`

### Weapon Sub-Ranges

```text
710xx   Special weapons
711xx   Daggers
712xx   Swords
713xx   Axes
714xx   Bows
```

### Future Expansion

```text
715xx   Reserved for future weapon type
716xx   Reserved for future weapon type
717xx   Reserved for future weapon type
718xx   Reserved for future weapon type
719xx   Reserved for future weapon type
```

### Usage Scope

- Special weapons
- Daggers
- Swords
- Axes
- Bows
- Future weapon categories

### Examples

```text
71001   Special weapon
71101   Basic dagger
71201   Basic sword
71301   Basic axe
71401   Basic bow
```

---

#### 3.7.3 Consumable Items

**Range:** `72xxx`  
**Profile Type:** `ConsumableItemProfile`  
**Base Type:** `ItemProfile`

### Usage Scope

- Healing consumables
- Resource restoration consumables
- Buff consumables
- Status-cleansing consumables
- Future consumable item categories

### Examples

```text
72001   Small healing item
72002   Stamina recovery item
```

---

## 3.8 Skills

**Range:** `8xxxx`  
**Asset Type:** ScriptableObject  
**Base Profile Type:** `SkillProfile`

### Skill Type Sub-Ranges

```text
81xxx   Melee skills
82xxx   Ranged skills
```

### Third Digit Rule

The third digit identifies how the skill is obtained or used.

```text
8x0xx   Weapon normal attack
8x1xx   Weapon innate skill
8x2xx   Weapon character-learned special skill
```

---

#### 3.8.1 Melee Skills

**Range:** `81xxx`  
**Profile Type:** `MeleeSkillProfile`  
**Base Type:** `SkillProfile`

### Sub-Ranges

```text
810xx   Melee weapon normal attacks
811xx   Melee weapon innate skills
812xx   Melee weapon character-learned special skills
```

### Examples

```text
81001   Basic dagger normal attack
81101   Dagger innate skill
81201   Character-learned melee special skill
```

---

#### 3.8.2 Ranged Skills

**Range:** `82xxx`  
**Profile Type:** `RangedSkillProfile`  
**Base Type:** `SkillProfile`

### Sub-Ranges

```text
820xx   Ranged weapon normal attacks
821xx   Ranged weapon innate skills
822xx   Ranged weapon character-learned special skills
```

### Examples

```text
82001   Basic bow normal attack
82101   Bow innate skill
82201   Character-learned ranged special skill
```

---

## 4. Summary Table

```text
1xxx    Character identities
2xxx    Default spawn points
3xxx    Death triggers
4xxx    Tutorial world visuals
5xxxx   Status effects
6xxx    Terrains
7xxxx   Items
8xxxx   Skills
```

### Detailed Summary

```text
1xxx    CharacterIdentityProfile

20xx    PlayerDefaultSpawnPointController
21xx    Enemy-only CharacterDefaultSpawnPointController

3xxx    DeathTriggerController

4xxx    TutorialWorldVisualController

50xxx   Terrain status effects
51xxx   Character skill application status effects
52xxx   Consumable item status effects

6xxx    TerrainProfile

70xxx   LightSourceProfile
710xx   Special WeaponProfile
711xx   Dagger WeaponProfile
712xx   Sword WeaponProfile
713xx   Axe WeaponProfile
714xx   Bow WeaponProfile
72xxx   ConsumableItemProfile

810xx   Melee weapon normal attacks
811xx   Melee weapon innate skills
812xx   Melee character-learned special skills

820xx   Ranged weapon normal attacks
821xx   Ranged weapon innate skills
822xx   Ranged character-learned special skills
```

---

## 5. Assignment Rules

When assigning a new ID:

1. Identify the correct major category.
2. Identify the correct sub-range if the category has one.
3. Pick the next available ID inside that range.
4. Ensure the ID is not already used by another asset, GameObject, Behaviour, or ScriptableObject.
5. Keep the ID stable after it has been referenced by gameplay data.

### Example Workflow

If creating a new bow weapon:

```text
Major category: Items
Item category: Weapons
Weapon category: Bows
Range: 714xx
Next available ID: 71401
```

If creating the bow's normal ranged attack:

```text
Major category: Skills
Skill category: Ranged skills
Skill usage type: Weapon normal attack
Range: 820xx
Next available ID: 82001
```

---

## 6. Restrictions

The following are not permitted:

- Reusing the same ID for multiple entries
- Assigning IDs outside the approved range without updating this document
- Mixing unrelated systems in the same range
- Changing an existing ID after it has been used by saved data, scene references, or profile references
- Using temporary IDs in committed assets
- Using negative IDs
- Using `0` as a valid content ID

---

## 7. Reserved Values

The following values are reserved:

```text
0       None / Unassigned / Invalid
-1      Runtime-only invalid value if needed by code
```

`0` should not be used for real content.

If an enum, profile, or behaviour needs a default invalid ID, use `0` unless the code has a specific reason to distinguish between `0` and `-1`.

---

## 8. Future Expansion Rule

When adding a new ID category or subcategory:

1. Choose a range that does not overlap with existing ranges.
2. Update this document before assigning IDs in project assets.
3. Add the new range to the summary table.
4. Add examples for the new category.
5. Keep the range broad enough for future content growth.

### Example

If projectile items are later separated from consumable items:

```text
73xxx   Projectile items
```

The new range should be documented before any projectile item profiles are assigned IDs.

---

## 9. Governance

- This document must be updated whenever a new ID range is introduced.
- All manually assigned ID attributes must comply with this convention.
- Contributors should check this document before creating new profile assets or scene ID behaviours.
- ID collisions must be fixed before merge approval.
- Deprecated IDs should not be reused unless the original content was never committed or referenced.

---

**End of Document**
