# Environment

**Major Range:** `3xxxx`

---

## Asset Type

- ScriptableObject

---

## Profile Types

```text
30xxx   TerrainProfile
31xxx   CraftingStationProfile
32xxx   ItemContainerProfile
33xxx   CollectibleObjectProfile
34xxx   HarvestableObjectProfile
35xxx   ObstacleProfile
36xxx   SceneTransitionPointProfile
```

---

## Category Structure

```text
World
└─ Environment
   ├─ Terrain
   │  └─ TerrainProfile
   │
   ├─ Interactable
   │  ├─ CraftingStation
   │  │  └─ CraftingStationProfile
   │  ├─ ItemContainer
   │  │  └─ ItemContainerProfile
   │  ├─ Collectible
   │  │  └─ CollectibleObjectProfile
   │  └─ MapTransitionPoint
   │     └─ MapTransitionPointProfile
   │
   ├─ Harvestable Object
   │  └─ HarvestableObjectProfile
   │
   └─ Obstacle
      └─ ObstacleProfile
```

The numeric ID hierarchy does not need to mirror the runtime or semantic hierarchy exactly.

`Interactable` is a gameplay grouping shared by several Environment object types, but Crafting Stations, Item Containers, Collectible Objects, and Scene Transition Points each receive their own second-digit range because they are distinct authored Profile categories.

---

## Sub-Ranges

```text
30xxx   Terrain Profiles
31xxx   Crafting Station Profiles
32xxx   Item Container Profiles
33xxx   Collectible Object Profiles
34xxx   Harvestable Object Profiles
35xxx   Obstacle Profiles
36xxx   Map Transition Point Profiles
37xxx   Doors and Door Switches
38xxx   Key Item Interactables

39xxx   Reserved
```

Each allocated Environment Profile type owns the full three trailing digits of its range.

Examples:

```text
31000-31999   Crafting Station Profiles
32000-32999   Item Container Profiles
33000-33999   Collectible Object Profiles
34000-34999   Harvestable Object Profiles
35000-35999   Obstacle Profiles
36000-36999   Map Transition Point Profiles
```

This allows a Profile type to scale beyond 100 authored objects without requiring an unnecessary additional Profile sub-layer.

---

## Assignment Rules

### Terrain Profiles

The ID identifies a **terrain type**.

```text
30xxx   Terrain Profiles
```

Terrain Profile IDs are assigned sequentially within `30xxx`.

---

### Crafting Station Profiles

The ID identifies a **crafting station archetype**.

```text
31xxx   Crafting Station Profiles
```

All instances using the same `CraftingStationProfile` share the same ID.

Crafting Station Profile IDs are assigned sequentially within `31xxx`.

---

### Item Container Profiles

The ID identifies an **item container archetype**.

```text
32xxx   Item Container Profiles
```

All instances using the same `ItemContainerProfile` share the same ID.

Item Container Profile IDs are assigned sequentially within `32xxx`.

---

### Collectible Object Profiles

The ID identifies a **collectible object world-object archetype**.

```text
33xxx   Collectible Object Profiles
```

A `CollectibleObjectProfile` represents an Item as a collectible object that can exist in the game world.

The Collectible ID and Item ID identify different gameplay definitions:

```text
33xxx   Collectible world-object definition
7xxxx   Item definition
```

A `CollectibleObjectProfile` therefore owns its own `33xxx` ID and references the corresponding `ItemProfile`.

The Collectible ID must not be derived from, or required to numerically match, the referenced Item ID.

Collectible IDs are assigned sequentially within `33xxx`.

All world instances using the same `CollectibleObjectProfile` share the same Collectible ID.

---

### Harvestable Object Profiles

The ID identifies a **harvestable environment object archetype**.

```text
34xxx   Harvestable Object Profiles
```

All instances using the same `HarvestableObjectProfile` share the same ID.

Harvestable Object Profile IDs are assigned sequentially within `34xxx`.

---

### Obstacle Profiles

The ID identifies an **obstacle archetype**.

```text
35xxx   Obstacle Profiles
```

All instances using the same `ObstacleProfile` share the same ID.

Obstacle Profile IDs are assigned sequentially within `35xxx`.

---

### Scene Transition Point Profiles

The ID identifies an **authored scene transition point definition**, including its configured destination scene and destination arrival key.

```text
36xxx   Scene Transition Point Profiles
```

Scene Transition Point Profile IDs are assigned sequentially within `36xxx`.

Instances referencing the same `SceneTransitionPointProfile` share its ID and destination configuration. Distinct destinations should use distinct Profiles.

The numeric Profile ID is separate from the scene-local `ArrivalKey` used to locate an arrival point; arrival keys must be unique within their destination scene and need not match a Profile ID.

Scene Transition Point Profile asset names use:

```text
<ID>_STP_<DescriptiveName>
```

First allocation (Whispering Grove, Greenvale Crossroads):

```text
36001_STP_WhisperingGroveShrine
```

This Profile represents the statue/shrine entrance to the planned **Forgotten Sanctum** dungeon. The destination scene name and destination arrival key must be configured to match the actual dungeon scene and its arrival point when created.

---

## Collectible Object Naming

Collectible Object Profile asset names use:

```text
<ID>_Clt_<WrappedItemName>
```

The wrapped Item name excludes the Item's numeric ID while retaining its Item-category prefix.

Examples after the Environment ID remap:

```text
33001_Clt_Mtr_Wood
33002_Clt_Mtr_Bone
33003_Clt_Mtr_Twig
```

The Collectible ID sequence is independent from the referenced Item ID sequence.

For example:

```text
75001   Mtr_Wood
33001   Clt_Mtr_Wood
```

The matching trailing digits above are incidental and do not establish an ID-mapping rule.

---

## Migration From Previous Environment Allocation

The previous Environment convention grouped most Environment Object Profiles under `32xxx`.

The new allocation gives each major Environment Profile type its own second-digit range.

```text
Previous   New     Profile Type

300xx      30xxx   Terrain Profiles
323xx      31xxx   Crafting Station Profiles
320xx      32xxx   Item Container Profiles
324xx      33xxx   Collectible Object Profiles
322xx      34xxx   Harvestable Object Profiles
321xx      35xxx   Obstacle Profiles
```

Existing Profile assets should be reassigned according to the new ranges while preserving their relative sequence where practical.

---

## Examples

```text
30001   Grass
30002   Water

31001   Makeshift Worktable

32001   Starting Weapon Chest
32002   Basic Supply Chest

33001   Collectible - Wood
33002   Collectible - Bone
33003   Collectible - Twig

34001   Oak Tree
34002   Cherry Tree
34003   Iron Node

35001   Small Rock
35002   Fallen Tree

36001   Whispering Grove Shrine (Forgotten Sanctum entrance)
```

---

**End of Document**
