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
324xx   CollectibleItemProfile
```

---

## Category Structure

```text
Environment
├─ Terrain
│  └─ TerrainProfile
│
├─ Interactable
│  ├─ ItemContainerProfile
│  ├─ CraftingStationProfile
│  └─ CollectibleItemProfile
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

320xx   Item Container Profiles
321xx   Obstacle Profiles
322xx   Harvestable Object Profiles
323xx   Crafting Station Profiles
324xx   Collectible Item Profiles

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

### Interactable Environment Object Profiles

The ID identifies an **interactable environment object archetype**.

Current interactable environment object profile types include:

```text
320xx   ItemContainerProfile
323xx   CraftingStationProfile
324xx   CollectibleItemProfile
```

All instances using the same profile share the same ID.

---

### Collectible Item Profiles

The ID identifies a **collectible item world-object archetype**.

All instances using the same profile share the same ID. The collected `ItemProfile` remains identified by the Item data convention (`7xxxx`); the `324xx` ID identifies the collectible world-object definition itself.

---

### Obstacle Profiles

The ID identifies an **obstacle archetype**.

All instances using the same profile share the same ID.

---

### Harvestable Object Profiles

The ID identifies a **harvestable environment object archetype**.

All instances using the same profile share the same ID.

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

32301   Makeshift Workbench

32401   Collectible - Reed Stalk
```

---

**End of Document**
