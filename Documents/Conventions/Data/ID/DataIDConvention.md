# Data ID Convention

**Project:** FlameOfTheLost  
**Version:** 3.11  
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

The Character major range also contains Player-related Profile types under the special `10xxx` allocation.

Runtime `MonoBehaviour` components do **not** own independent IDs.

Runtime objects should reference their associated data definition when an ID is required.

---

## 3.2 Integer IDs

All IDs are stored as integers.

Do not use strings or GUIDs as gameplay content identifiers.

---

## 3.3 Range Hierarchy

The **first digit** identifies the major gameplay system.

Additional digits may encode category-specific classifications.

Example:

```text
1xxxx   Character-related
2xxxx   Spawning
3xxxx   Environment
4xxxx   Tutorial
5xxxx   Status Effects
7xxxx   Items
8xxxx   Skills
```

A category may reserve more than two trailing digits when additional internal grouping is required.

For Character IDs in the multi-character factions (`11xxx`, `12xxx`, and `13xxx`), the five digits follow:

```text
1FSTI

1       Character-related
F       Faction
S       Faction-specific Sub-class
T       Traverse Type
I       Individual Character
```

The `10xxx` Player / Player-related range is an explicit exception because there is only one Player Character. `10001` identifies the Player, while other `10xxx` sub-ranges may be allocated to Player-related Profile types.

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
| **1xxxx** | Character-related | Characters.md |
| **2xxxx** | Spawning | Spawning.md |
| **3xxxx** | Environment | Environment.md |
| **4xxxx** | Tutorial | Tutorial.md |
| **5xxxx** | Status Effects | StatusEffects.md |
| **7xxxx** | Items | Items.md |
| **8xxxx** | Skills | Skills.md |

---

# 5. Summary Table

```text
1xxxx   Character-related Data

10xxx   Player / Player-related Data
10001   Player
101xx   CraftingRecipeProfile

102xx   Reserved
103xx   Reserved
104xx   Reserved
105xx   Reserved
106xx   Reserved
107xx   Reserved
108xx   Reserved
109xx   Reserved

11xxx   Ally Characters
110xx-119xx   Reserved for Ally Sub-classes

12xxx   Enemy Characters
120xx   Territorial Enemy
121xx   Boss
122xx-129xx   Reserved

13xxx   Neutral Characters
130xx   Animal
131xx-139xx   Reserved

14xxx   Reserved
15xxx   Reserved
16xxx   Reserved
17xxx   Reserved
18xxx   Reserved
19xxx   Reserved

Character Traverse Type digit for 11xxx-13xxx:
1xx0x   Humanoid
1xx1x   Land Creature
1xx2x   Aquatic
1xx3x   Amphibious
1xx4x   Flying
1xx5x   Ethernal
1xx6x-1xx9x   Reserved

2xxxx   Spawning Data

20xxx   Player Default Spawn Point Profiles

21xxx   Player Checkpoint Profiles

22xxx   Enemy Default Spawn Point Profiles
220xx   Territorial Enemy Default Spawn Point Profiles

23xxx   Neutral Character Default Spawn Point Profiles
230xx   Animal Default Spawn Point Profiles

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

For Characters in `11xxx`, `12xxx`, and `13xxx`:

1. Select the Character's faction.
2. Select the faction-specific sub-class.
3. Select the Traverse Type.
4. Assign the next available individual Character digit from `1-9`.
5. Store the ID in the corresponding `CharacterProfile`.

The final digit `0` is reserved as the category base for these Character classifications.

For Player-related data, follow the dedicated `10xxx` allocations documented in `Characters.md`. `10001` is permanently assigned to the Player Character.

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
- Assigning a multi-character faction Character to a final-digit `0` category-base ID.

---

# 8. Reserved Values

```text
00000   None / Unassigned / Invalid
-1      Runtime-only invalid value
```

`00000` must never be assigned to gameplay content.

Within classified Character ranges (`11xxx`, `12xxx`, and `13xxx`), IDs ending in `0` are reserved as category bases and are not assigned to Character definitions.

---

# 9. Future Expansion

When introducing a new gameplay system or a new documented sub-range:

1. Allocate an unused range.
2. Create or update the corresponding category document.
3. Update this document when the top-level summary changes.
4. Document the allocation before assigning IDs.
5. Preserve existing IDs and reserved gaps.

For Character-related expansion:

- New factions may use currently reserved second-digit ranges.
- New faction-specific Character sub-classes must be documented before use.
- New Traverse Types must be allocated consistently across classified Character factions.
- New Player-related Profile types may use currently reserved `10xxx` sub-ranges.

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
