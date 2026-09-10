# Environment

**Major Range:** `3xxxx`

---

## Asset Type

- ScriptableObject

---

## Profile Types

```text
300xx   TerrainProfile

320xx   ItemContainerProfile
321xx   ObstacleProfile
322xx   HarvestableObjectProfile
323xx   CraftingStationProfile
324xx   CollectibleObjectProfile
```

---

## Category Structure

```text
Environment
├─ Terrain
│  └─ TerrainProfile
│
└─ Environment Object
   ├─ Interactable
   │  ├─ ItemContainerProfile
   │  ├─ CraftingStationProfile
   │  └─ CollectibleObjectProfile
   │
   ├─ Obstacle
   │  └─ ObstacleProfile
   │
   └─ Harvestable Object
      └─ HarvestableObjectProfile
```

---

## Sub-Ranges

```text
300xx   Terrain Profiles

301xx   Reserved
302xx   Reserved
303xx   Reserved
304xx   Reserved
305xx   Reserved
306xx   Reserved
307xx   Reserved
308xx   Reserved
309xx   Reserved

31xxx   Reserved

32xxx   Environment Object Profiles

320xx   Item Container Profiles
321xx   Obstacle Profiles
322xx   Harvestable Object Profiles
323xx   Crafting Station Profiles
324xx   Collectible Object Profiles

325xx   Reserved
326xx   Reserved
327xx   Reserved
328xx   Reserved
329xx   Reserved

33xxx   Reserved
34xxx   Reserved
35xxx   Reserved
36xxx   Reserved
37xxx   Reserved
38xxx   Reserved
39xxx   Reserved
```

---

## Assignment Rules

### Terrain Profiles

The ID identifies a **terrain type**.

---

### Environment Object Profiles

The ID identifies an **environment object archetype**.

Environment Object profiles are grouped into more specific sub-aspects such as Interactables, Obstacles, and Harvestable Objects.

All instances using the same profile share the same ID.

---

### Interactable Environment Object Profiles

The ID identifies an **interactable environment object archetype**.

Current interactable environment object profile types include:

```text
320xx   ItemContainerProfile
323xx   CraftingStationProfile
324xx   CollectibleObjectProfile
```

All instances using the same profile share the same ID.

---

### Collectible Object Profiles

The ID identifies a **collectible object world-object archetype**.

```text
324xx   Collectible Object Profiles
```

A `CollectibleObjectProfile` represents an Item as a collectible object that can exist in the game world.

The Collectible ID and Item ID identify different gameplay definitions:

```text
324xx   Collectible world-object definition
7xxxx   Item definition
```

A `CollectibleObjectProfile` therefore owns its own `324xx` ID and references the corresponding `ItemProfile`.

The Collectible ID must not be derived from, or required to numerically match, the referenced Item ID.

Collectible IDs are assigned sequentially within `324xx`.

All world instances using the same `CollectibleObjectProfile` share the same Collectible ID.

---

### Obstacle Profiles

The ID identifies an **obstacle archetype**.

All instances using the same profile share the same ID.

---

### Harvestable Object Profiles

The ID identifies a **harvestable environment object archetype**.

All instances using the same profile share the same ID.

---

## Collectible Object Naming

Collectible Object Profile asset names use:

```text
<ID>_Clt_<WrappedItemName>
```

The wrapped Item name excludes the Item's numeric ID while retaining its Item-category prefix.

Examples:

```text
32401_Clt_Mtr_Wood
32402_Clt_Mtr_Bone
32403_Clt_Mtr_Twig
```

The Collectible ID sequence is independent from the referenced Item ID sequence.

For example:

```text
75001   Mtr_Wood
32401   Clt_Mtr_Wood
```

The matching trailing digits above are incidental and do not establish an ID-mapping rule.

---

## Examples

```text
30001   Grass
30002   Water

32001   Starting Weapon Chest
32002   Basic Supply Chest

32101   Small Rock
32102   Fallen Tree

32201   Oak Tree
32202   Berry Bush
32203   Copper Ore Deposit

32301   Makeshift Worktable

32401   Collectible - Wood
32402   Collectible - Bone
32403   Collectible - Twig
```

---

**End of Document**
