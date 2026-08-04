# ID Attribute Convention
**Project:** FlameOfTheLost  
**Version:** 1.1  
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

The first two digits identify the major gameplay system. Additional digits identify subcategories where applicable.

Do not assign IDs randomly.

Do not reuse the same ID for different assets, GameObjects, Behaviours, or ScriptableObjects.

---

## 3. Approved ID Range Registry

### 3.1 Character Identities

**Range:** `10xxx`  
**Asset Type:** ScriptableObject  
**Profile Type:** `CharacterIdentityProfile`

### Usage Scope

- Player character identity
- Enemy character identity
- Ally/NPC character identity
- Future character identity profiles

### Examples

```text
10001   Player
10101   Toxicschroom
```

---

### 3.2 Default Spawn Points

**Range:** `20xxx`  
**Object Type:** GameObject / Behaviour  
**Base Behaviour:** `CharacterDefaultSpawnPointController`

### Sub-Ranges

```text
200xx   Player default spawn points
201xx   Enemy-only default spawn points
```

### Behaviour Mapping

```text
200xx   PlayerDefaultSpawnPointController
201xx   CharacterDefaultSpawnPointController used by enemy-only spawn points
```

### Usage Scope

- Player default spawn points
- Enemy default spawn points
- Character placement anchors used as default spawn references

### Examples

```text
20001   Player default spawn point - Tutorial Area 1
20101   Enemy default spawn point - Tutorial Area 3
```

---

### 3.3 Death Triggers

**Range:** `30xxx`  
**Object Type:** GameObject / Behaviour  
**Behaviour Type:** `DeathTriggerController`

### Usage Scope

- Pitfall death triggers
- Water/deep-water death triggers
- Hazard death zones
- Out-of-bounds death triggers

### Examples

```text
30001   Tutorial pitfall death trigger
30002   Tutorial water death trigger
```

---

### 3.4 Tutorial World Visuals

**Range:** `40xxx`  
**Object Type:** GameObject / Behaviour  
**Behaviour Type:** `TutorialWorldVisualController`

### Usage Scope

- Tutorial visual blockers
- Tutorial gates
- Tutorial visual guidance objects
- World visual state controllers used by tutorial progression

### Examples

```text
40001   Tutorial Area 1 visual controller
40002   Tutorial Area 2 visual controller
```

---

### 3.5 Status Effects

**Range:** `50xxx`  
**Asset Type:** ScriptableObject  
**Profile Type:** `StatusEffectProfile`

### Sub-Ranges

```text
500xx   Status effects from terrains
510xx   Status effects from character skill applications
520xx   Status effects from consumable items
```

### Future Expansion

```text
530xx   Reserved for future status effect source
540xx   Reserved for future status effect source
550xx   Reserved for future status effect source
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

**Range:** `60xxx`  
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
60001   Grass terrain
60002   Water terrain
```

---

### 3.7 Items

**Range:** `70xxx`  
**Asset Type:** ScriptableObject  
**Base Profile Type:** `ItemProfile`

### Sub-Ranges

```text
700xx   Light sources
710xx   Weapons
720xx   Consumable items
730xx   Projectiles
740xx   Key items
750xx   Materials
```

---

#### 3.7.1 Light Sources

**Range:** `700xx`  
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

**Range:** `710xx`  
**Profile Type:** `WeaponProfile`  
**Base Type:** `ItemProfile`

### Weapon Sub-Ranges

```text
7100x   Special weapons
7110x   Daggers
7120x   Swords
7130x   Axes
7140x   Bows
```

### Future Expansion

```text
7150x   Reserved for future weapon type
7160x   Reserved for future weapon type
7170x   Reserved for future weapon type
7180x   Reserved for future weapon type
7190x   Reserved for future weapon type
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

**Range:** `720xx`  
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

**Range:** `80xxx`  
**Asset Type:** ScriptableObject  
**Base Profile Type:** `SkillProfile`

### Skill Type Sub-Ranges

```text
810xx   Melee skills
820xx   Ranged skills
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

**Range:** `810xx`  
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

**Range:** `820xx`  
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

### 3.9 Harvestable Objects

**Range:** `90xxx`  
**Object Type:** GameObject / Behaviour  
**Behaviour Type:** `HarvestableObjectController`

### Harvestable Type Sub-Ranges

```text
900xx   Trees
901xx   Bushes
902xx   Tall Grass
903xx   Rocks / Ore Deposits
904xx   Mushrooms
905xx   Crops / Plants
906xx   Crystals
907xx   Special Harvestables
908xx   Quest Harvestables
909xx   Reserved
```

### Usage Scope

- Trees that can be harvested
- Bushes that can be harvested
- Tall grass
- Ore deposits
- Crystal deposits
- Mushroom patches
- Harvestable plants
- Special harvestable world objects
- Quest-related harvestable objects

### ID Placement Rule

The ID identifies the **harvestable archetype (prefab/content)** rather than an individual scene instance.

Multiple placed instances of the same harvestable prefab should share the same ID.

Scene objects do **not** require unique IDs unless a future save system introduces persistent per-instance world state.

### Examples

```text
90001   Oak Tree
90002   Pine Tree
90003   Dead Tree

90101   Berry Bush
90102   Thorn Bush

90201   Tall Grass
90202   Wheat Grass

90301   Stone Deposit
90302   Copper Ore Deposit

90401   Common Mushroom
90402   Glowing Mushroom

90501   Herb Plant

90601   Blue Crystal Deposit

90701   Ancient Vine

90801   Ancient Sacred Tree
```

---

## 4. Summary Table

```text
10xxx   Character identities
20xxx   Default spawn points
30xxx   Death triggers
40xxx   Tutorial world visuals
50xxx   Status effects
60xxx   Terrains
70xxx   Items
80xxx   Skills
90xxx   Harvestable objects
```

### Detailed Summary

```text
10xxx   CharacterIdentityProfile

200xx   PlayerDefaultSpawnPointController
201xx   Enemy-only CharacterDefaultSpawnPointController

30xxx   DeathTriggerController

40xxx   TutorialWorldVisualController

500xx   Terrain status effects
510xx   Character skill application status effects
520xx   Consumable item status effects

60xxx   TerrainProfile

700xx   LightSourceProfile
7100x   Special WeaponProfile
7110x   Dagger WeaponProfile
7120x   Sword WeaponProfile
7130x   Axe WeaponProfile
7140x   Bow WeaponProfile
720xx   ConsumableItemProfile
730xx   ProjectileProfile
740xx   KeyItemProfile
750xx   MaterialProfile

810xx   Melee weapon normal attacks
811xx   Melee weapon innate skills
812xx   Melee character-learned special skills

820xx   Ranged weapon normal attacks
821xx   Ranged weapon innate skills
822xx   Ranged character-learned special skills

90xxx   HarvestableObjectController

900xx   Trees
901xx   Bushes
902xx   Tall Grass
903xx   Rocks / Ore Deposits
904xx   Mushrooms
905xx   Crops / Plants
906xx   Crystals
907xx   Special Harvestables
908xx   Quest Harvestables
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
Range: 7140x
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

If creating a new harvestable oak tree:

```text
Major category: Harvestable Objects
Harvestable category: Trees
Range: 900xx
Next available ID: 90001
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
00000   None / Unassigned / Invalid
-1      Runtime-only invalid value if needed by code
```

`00000` should not be used for real content.

If an enum, profile, or behaviour needs a default invalid ID, use `00000` unless the code has a specific reason to distinguish between `00000` and `-1`.

---

## 8. Future Expansion Rule

When adding a new ID category or subcategory:

1. Choose a range that does not overlap with existing ranges.
2. Update this document before assigning IDs in project assets.
3. Add the new range to the summary table.
4. Add examples for the new category.
5. Keep the range broad enough for future content growth.

### Example

If a new category is introduced for environmental objects:

```text
91xxx   Environmental objects
```

The new range should be documented before any corresponding assets or behaviours are assigned IDs.

---

## 9. Governance

- This document must be updated whenever a new ID range is introduced.
- All manually assigned ID attributes must comply with this convention.
- Contributors should check this document before creating new profile assets or scene ID behaviours.
- ID collisions must be fixed before merge approval.
- Deprecated IDs should not be reused unless the original content was never committed or referenced.

---

**End of Document**