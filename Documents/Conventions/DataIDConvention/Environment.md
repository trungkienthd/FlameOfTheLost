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

323xx   Reserved
324xx   Reserved
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
```

---

**End of Document**
