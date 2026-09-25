# Characters

**Major Range:** `1xxxx`

---

## Asset Type

- Character-related data represented by project-owned ScriptableObject assets.
- For Character definitions, the ID identifies the **Character**, not the `CharacterProfile` asset itself.
- The `10xxx` range is a special Player / Player-related range and may also contain Player-related Profile types.

---

## Data Types

```text
10xxx   Player / Player-related Data
11xxx   Ally Characters
12xxx   Enemy Characters
13xxx   Neutral Characters

14xxx   Reserved
15xxx   Reserved
16xxx   Reserved
17xxx   Reserved
18xxx   Reserved
19xxx   Reserved
```

---

## Character ID Structure

For multi-character factions (`11xxx`, `12xxx`, and `13xxx`), Character IDs follow:

```text
1FSTI
```

```text
1       Character-related
F       Faction
S       Faction-specific Sub-class
T       Traverse Type
I       Individual Character
```

### Faction Digit

```text
0       Player / Player-related
1       Ally
2       Enemy
3       Neutral
4-9     Reserved
```

### Traverse Type Digit

```text
0       Humanoid
1       Land Creature
2       Aquatic
3       Amphibious
4       Flying
5       Ethernal
6-9     Reserved
```

The Traverse Type digit is the fourth digit of the ID.

For example:

```text
1xx0x   Humanoid
1xx1x   Land Creature
1xx2x   Aquatic
1xx3x   Amphibious
1xx4x   Flying
1xx5x   Ethernal
```

### Individual Character Digit

The fifth digit identifies the individual Character within the same Faction, Sub-class, and Traverse Type grouping.

```text
1FST0   Reserved / category base
1FST1   Character 1
1FST2   Character 2
...
1FST9   Character 9
```

Real Character definitions use `1-9` in the final digit. `0` remains reserved as the category base.

---

## Sub-Ranges

### Player / Player-related

`10xxx` is an explicit exception to the normal Character classification structure.

There is only one Player Character, so the remaining digits are used for Player-related Profile types instead of Faction Sub-class and Traverse Type classification.

```text
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
```

---

### Ally

The Ally faction is reserved for future Character sub-classes.

```text
11xxx   Ally Characters

110xx   Reserved
111xx   Reserved
112xx   Reserved
113xx   Reserved
114xx   Reserved
115xx   Reserved
116xx   Reserved
117xx   Reserved
118xx   Reserved
119xx   Reserved
```

When Ally sub-classes are introduced, the third digit will identify the Ally-specific sub-class and the fourth digit will continue to identify Traverse Type.

---

### Enemy

```text
12xxx   Enemy Characters

120xx   Territorial Enemy
121xx   Boss

122xx   Reserved
123xx   Reserved
124xx   Reserved
125xx   Reserved
126xx   Reserved
127xx   Reserved
128xx   Reserved
129xx   Reserved
```

#### Territorial Enemy

```text
1200x   Territorial Enemy - Humanoid
1201x   Territorial Enemy - Land Creature
1202x   Territorial Enemy - Aquatic
1203x   Territorial Enemy - Amphibious
1204x   Territorial Enemy - Flying
1205x   Territorial Enemy - Ethernal

1206x   Reserved
1207x   Reserved
1208x   Reserved
1209x   Reserved
```

#### Boss

```text
1210x   Boss - Humanoid
1211x   Boss - Land Creature
1212x   Boss - Aquatic
1213x   Boss - Amphibious
1214x   Boss - Flying
1215x   Boss - Ethernal

1216x   Reserved
1217x   Reserved
1218x   Reserved
1219x   Reserved
```

---

### Neutral

```text
13xxx   Neutral Characters

130xx   Animal

131xx   Reserved
132xx   Reserved
133xx   Reserved
134xx   Reserved
135xx   Reserved
136xx   Reserved
137xx   Reserved
138xx   Reserved
139xx   Reserved
```

#### Animal

```text
1300x   Animal - Humanoid
1301x   Animal - Land Creature
1302x   Animal - Aquatic
1303x   Animal - Amphibious
1304x   Animal - Flying
1305x   Animal - Ethernal

1306x   Reserved
1307x   Reserved
1308x   Reserved
1309x   Reserved
```

---

## Assignment Rules

### Characters

A Character ID represents a **character archetype/type**.

The ID belongs conceptually to the Character itself.

`CharacterProfile` stores and exposes this ID as the Character's fixed data definition, but the ID must not be interpreted as identifying the `CharacterProfile` asset independently.

All runtime instances representing the same Character share the same Character ID.

Different versions or variants of the same Character should not receive a new Character ID unless they are intentionally treated as a different Character definition.

For `11xxx`, `12xxx`, and `13xxx` Characters:

1. Select the Character's faction.
2. Select the faction-specific sub-class.
3. Select the Character's Traverse Type.
4. Assign the next available individual Character digit from `1-9`.
5. Keep the `0` final digit reserved as the category base.

---

### Player-related Profiles

Player-related Profile types use reserved sub-ranges inside `10xxx`.

`10001` is permanently assigned to the Player Character.

Other `10xxx` allocations do not use the normal Faction / Sub-class / Traverse Type / Individual Character interpretation.

---

### Crafting Recipe Profiles

`101xx` is allocated to `CraftingRecipeProfile`.

A Crafting Recipe ID represents a **crafting recipe definition** associated with Player crafting.

A crafting recipe defines:

- The resulting item.
- The resulting quantity.
- The materials required to craft it.

Crafting stations may reference these recipe profiles to determine which recipes they provide to the Player.

---

## Examples

```text
10001   Player

10101   Arrow Crafting Recipe

12011   Feralshroom
12031   Aquabara

13011   Cow
```

ID interpretation examples:

```text
12011
1       Character-related
2       Enemy
0       Territorial Enemy
1       Land Creature
1       Character 1

13011
1       Character-related
3       Neutral
0       Animal
1       Land Creature
1       Character 1
```

---

**End of Document**
