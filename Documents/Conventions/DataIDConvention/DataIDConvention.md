# Data ID Convention

**Project:** FlameOfTheLost  
**Version:** 3.1  
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

Only project-owned **ScriptableObject** assets own IDs.

Runtime `MonoBehaviour` components do **not** own independent IDs.

Runtime objects should reference their associated data asset when an ID is required.

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

Each ScriptableObject asset owns exactly one ID.

IDs must never be reused by unrelated assets.

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

100xx   Player Characters
101xx   Mushroom Enemies
102xx   Humanoid Enemies
103xx   Wildlife
104xx   Friendly NPCs
105xx   Bosses

2xxxx   Spawning Data

200xx   Player Default Spawn Point Profiles
201xx   Enemy Default Spawn Point Profiles
210xx   Death Trigger Profiles

3xxxx   Environment Data

300xx   Terrain Profiles

31xxx   Reserved

320xx   Item Container Profiles
321xx   Obstacle Profiles
322xx   Harvestable Object Profiles

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
750xx   Materials

8xxxx   Skill Data

810xx   Melee Skills
820xx   Ranged Skills
```

---

# 6. Assignment Workflow

When creating a new ScriptableObject asset:

1. Identify its gameplay system.
2. Open the corresponding category document.
3. Select the correct sub-range.
4. Assign the next available ID.
5. Verify the ID is unused.
6. Commit the asset with the assigned ID.

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

When introducing a new gameplay system:

1. Allocate an unused major range.
2. Create a corresponding category document.
3. Update this document.
4. Add the new range to the summary table.
5. Document all sub-ranges before assigning IDs.

---

# 10. Governance

- Every new ID range must be documented before use.
- Contributors should consult the corresponding category document before creating new assets.
- ID collisions must be resolved before merging.
- Deprecated IDs should not be reused unless the original asset was never committed or referenced.

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
