# Environment

**Major Range:** `30xxx`

---

## Asset Type

- ScriptableObject

---

## Profile Types

```text
300xx   TerrainProfile

310xx   TutorialObjectProfile
311xx   TutorialPromptTriggerProfile

320xx   ItemContainerProfile
321xx   ObstacleProfile
322xx   HarvestableObjectProfile
```

---

## Sub-Ranges

```text
300xx   Terrain Profiles

310xx   Tutorial Object Profiles
311xx   Tutorial Prompt Trigger Profiles

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

The ID identifies a terrain type.

---

### Tutorial Profiles

The ID identifies a reusable tutorial object or tutorial prompt trigger.

---

### Environment Object Profiles

The ID identifies an environment object archetype.

All instances using the same profile share the same ID.

---

## Examples

```text
30001   Grass
30002   Water

31001   Movement Tutorial Object
31002   Combat Tutorial Object

31101   Movement Tutorial Prompt
31102   Attack Tutorial Prompt

32001   Tutorial Starting Weapon Chest
32002   Basic Supply Chest

32101   Small Rock
32102   Fallen Tree

32201   Oak Tree
32202   Berry Bush
32203   Copper Ore Deposit
```

---

**End of Document**