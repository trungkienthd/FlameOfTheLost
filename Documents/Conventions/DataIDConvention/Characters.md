# Characters

**Major Range:** `1xxxx`

---

## Asset Type

- ScriptableObject

---

## Profile Types

```text
10xxx   CharacterIdentityProfile
110xx   CraftingRecipeProfile
```

---

## Sub-Ranges

```text
10xxx   Character Identity Data

100xx   Player Characters
101xx   Mushroom Enemies
102xx   Humanoid Enemies
103xx   Wildlife
104xx   Friendly NPCs
105xx   Bosses

106xx   Reserved
107xx   Reserved
108xx   Reserved
109xx   Reserved

11xxx   Character Crafting Data

110xx   Crafting Recipe Profiles

111xx   Reserved
112xx   Reserved
113xx   Reserved
114xx   Reserved
115xx   Reserved
116xx   Reserved
117xx   Reserved
118xx   Reserved
119xx   Reserved

12xxx   Reserved
13xxx   Reserved
14xxx   Reserved
15xxx   Reserved
16xxx   Reserved
17xxx   Reserved
18xxx   Reserved
19xxx   Reserved
```

---

## Assignment Rules

### Character Identity Profiles

A Character Identity ID represents a **character archetype**.

All instances of the same character share the same ID.

---

### Crafting Recipe Profiles

A Crafting Recipe ID represents a **crafting recipe definition** associated with character crafting.

A crafting recipe defines:

- The resulting item.
- The resulting quantity.
- The materials required to craft it.

Crafting stations may reference these recipe profiles to determine which recipes they provide to the player.

---

## Examples

```text
10001   Player

10101   Toxicschroom
10102   Feralshroom

10201   Bandit

10301   Wolf

10401   Village Merchant

10501   Ancient Guardian

11001   Arrow
```

---

**End of Document**
