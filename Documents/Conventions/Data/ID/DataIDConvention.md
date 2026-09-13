# Data ID Convention

**Project:** FlameOfTheLost  
**Version:** 3.9  
**Scope:** ID attributes assigned to project-owned **ScriptableObject** assets.

---

# 1. Purpose

This document defines the ID convention used by ScriptableObject data assets throughout **FlameOfTheLost**.

The convention groups IDs by gameplay system to keep them:

- Predictable
- Searchable
- Maintainable
- Easy to expand

Each major gameplay system owns a dedicated numeric range.

Detailed allocation rules are documented in the corresponding category document.

---

# 2. Objectives

This convention aims to:

- Keep IDs readable and traceable.
- Prevent ID collisions.
- Organize data assets by gameplay system.
- Keep IDs stable throughout development.
- Support future project expansion.

---

# 3. Core Rules

## 3.1 Data Ownership

IDs identify project-owned gameplay data definitions.

For Characters specifically, the ID identifies the **Character archetype/type**, not the `CharacterProfile` asset as a separate entity. `CharacterProfile` stores and exposes the Character ID as part of the Character's fixed data.

Runtime `MonoBehaviour` components do **not** own independent IDs.

Runtime objects should reference their associated data definition when an ID is required.

---

## 3.2 Integer IDs

All IDs are stored as integers.

Do not use strings or GUIDs as gameplay content identifiers.

---

## 3.3 Range Hierarchy

The **first digit** identifies the major gameplay system.

Additional leading digits identify subcategories within that system.

Example:

```text
1xxxx   Characters
2xxxx   Spawning
3xxxx   Environment
4xxxx   Tutorial
5xxxx   Status Effects
7xxxx   Items
8xxxx   Skills
```

A category may reserve more than two trailing digits when additional internal grouping is required.

For example, Materials use the full `75xxx` range and are further divided into `750xx`, `751xx`, and `752xx` sub-ranges.

For Environment, the second digit directly identifies the Environment Profile type. The remaining three digits are available for Profile IDs within that type.

---

## 3.4 Stability

Once assigned and referenced by gameplay content, an ID should never change.

Changing IDs may invalidate:

- Scene references
- ScriptableObject references
- Save data
- Runtime lookups

---

## 3.5 Uniqueness

Each ID identifies exactly one gameplay data definition.

For most data categories, that definition is represented directly by a ScriptableObject asset. For Characters, the ID identifies the Character itself and is stored by its `CharacterProfile`.

IDs must never be reused by unrelated gameplay definitions.

---

# 4. Major Category Registry

| Range | Gameplay System | Documentation |
|--------|-----------------|---------------|
| **1xxxx** | Characters | Characters.md |
| **2xxxx** | Spawning | Spawning.md |
| **3xxxx** | Environment | Environment.md |
| **4xxxx** | Tutorial | Tutorial.md |
| **5xxxx** | Status Effects | StatusEffects.md |
| **7xxxx** | Items | Items.md |
| **8xxxx** | Skills | Skills.md |

---

# 5. Summary Table

```text
1xxxx   Character Data

10xxx   Characters

100xx   Player Characters
101xx   Mushroom Enemies
102xx   Humanoid Enemies
103xx   Wildlife
104xx   Friendly NPCs
105xx   Bosses

11xxx   Character Crafting Data

110xx   Crafting Recipe Profiles

2xxxx   Spawning Data

20xxx   Player Default Spawn Point Profiles

21xxx   Player Checkpoint Profiles

22xxx   Enemy Default Spawn Point Profiles
220xx   Territorial Enemy Default Spawn Point Profiles

29xxx   Death Trigger Profiles

3xxxx   Environment Data

30xxx   Terrain Profiles
31xxx   Crafting Station Profiles
32xxx   Item Container Profiles
33xxx   Collectible Object Profiles
34xxx   Harvestable Object Profiles
35xxx   Obstacle Profiles

36xxx   Reserved
37xxx   Reserved
38xxx   Reserved
39xxx   Reserved

4xxxx   Tutorial Data

400xx   Tutorial Object Profiles
41xxx   Tutorial Prompt Trigger Profiles

5xxxx   Status Effect Data

500xx   Terrain-Applied Status Effects
510xx   Skill-Applied Status Effects
520xx   Consumable-Applied Status Effects

7xxxx   Item Data

700xx   Light Sources
710xx   Weapons
720xx   Consumable Items
730xx   Projectiles
740xx   Key Items

75xxx   Materials
750xx   General / Natural Materials
751xx   Metal Materials
752xx   Crystal Materials

8xxxx   Skill Data

810xx   Melee Skills
820xx   Ranged Skills
```

---

# 6. Assignment Workflow

When creating a new ID-bearing gameplay data definition:

1. Identify its gameplay system.
2. Open the corresponding category document.
3. Select the correct sub-range.
4. Assign the next available ID according to that category's allocation rules.
5. Verify the ID is unused.
6. Store the ID in the corresponding data asset or profile.
7. Commit the data with the assigned ID.

For Characters, assign the ID to the Character definition and store it in the corresponding `CharacterProfile`.

---

# 7. Restrictions

The following are **not permitted**:

- Reusing IDs.
- Assigning IDs outside approved ranges.
- Mixing unrelated gameplay systems.
- Changing IDs after assets have been referenced.
- Using temporary IDs in committed assets.
- Using negative IDs as valid content.
- Using `00000` for real gameplay assets.

---

# 8. Reserved Values

```text
00000   None / Unassigned / Invalid
-1      Runtime-only invalid value
```

`00000` must never be assigned to gameplay content.

---

# 9. Future Expansion

When introducing a new gameplay system or a new documented sub-range:

1. Allocate an unused range.
2. Create or update the corresponding category document.
3. Update this document when the top-level summary changes.
4. Document the allocation before assigning IDs.
5. Preserve existing IDs and reserved gaps.

---

# 10. Governance

- Every new ID range must be documented before use.
- Contributors should consult the corresponding category document before creating new assets.
- ID collisions must be resolved before merging.
- Deprecated IDs should not be reused unless the original asset was never committed or referenced.
- Intentional gaps in an allocated range may remain unused until suitable content is defined.

---

# 11. Related Documents

```text
Characters.md
Spawning.md
Environment.md
Tutorial.md
StatusEffects.md
Items.md
Skills.md
```

Each document contains the detailed allocation rules, sub-ranges, assignment rules, and examples for its gameplay system.

---

**End of Document**
