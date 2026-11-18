# Items

**Major Range:** `7xxxx`

---

## Asset Type

- ScriptableObject

---

## Base Profile Type

- `ItemProfile`

---

## Sub-Ranges

```text
700xx   Light Sources
710xx   Weapons
720xx   Consumable Items
730xx   Projectiles
740xx   Key Items

75xxx   Materials

76xxx   Reserved
77xxx   Reserved
78xxx   Reserved
79xxx   Reserved
```

---

## Profile Types

```text
700xx   LightSourceProfile
710xx   WeaponProfile
720xx   ConsumableItemProfile
730xx   ProjectileProfile
740xx   KeyItemProfile
75xxx   MaterialProfile
```

---

## Weapon Sub-Ranges

```text
7100x   Special Weapons
7110x   Daggers
7120x   Swords
7130x   Axes
7140x   Bows
7150x   Staffs
7160x   Maces
7170x   Spears

7180x   Reserved
7190x   Reserved
```

---

## Material Sub-Ranges

```text
750xx   General / Natural Materials
751xx   Metal Materials
752xx   Crystal Materials

753xx   Reserved
754xx   Reserved
755xx   Reserved
756xx   Reserved
757xx   Reserved
758xx   Reserved
759xx   Reserved
```

### General / Natural Materials

`750xx` contains general-purpose materials that do not belong to the dedicated metal or crystal families.

Current allocations include:

```text
75001   Wood
75002   Bone
75003   Twig

75004   Unassigned
75005   Unassigned

75006   Purple Orb
75007   Stone
```

Unused values may remain intentionally unassigned until suitable gameplay content is defined.

### Metal Materials

`751xx` contains metal material families.

Each current metal family uses three sequential IDs representing:

```text
Chunks -> Ore -> Bar
```

Current allocations:

```text
75101   Iron Chunks
75102   Iron Ore
75103   Iron Bar

75104   Copper Chunks
75105   Copper Ore
75106   Copper Bar

75107   Gold Chunks
75108   Gold Ore
75109   Gold Bar
```

The three-stage grouping should be preserved when adding comparable metal families unless a future design requires a different structure.

### Crystal Materials

`752xx` contains crystal material families.

Each current crystal family uses three sequential IDs representing:

```text
Chunks -> Crystal -> Block
```

Current allocations:

```text
75201   Aquamarine Chunks
75202   Aquamarine Crystal
75203   Aquamarine Block

75204   Amber Chunks
75205   Amber Crystal
75206   Amber Block

75207   Ruby Chunks
75208   Ruby Crystal
75209   Ruby Block

75210   Emerald Chunks
75211   Emerald Crystal
75212   Emerald Block

75213   Amethyst Chunks
75214   Amethyst Crystal
75215   Amethyst Block
```

The three-stage grouping should be preserved when adding comparable crystal families unless a future design requires a different structure.

---

## Assignment Rules

### Items

The ID identifies an **item definition**.

Items are grouped by item category.

### Weapons

Weapon IDs are further grouped by weapon type.

### Materials

Material IDs are first grouped by broad material family:

- `750xx` for general / natural materials.
- `751xx` for metals.
- `752xx` for crystals.

Metal and crystal families currently reserve three consecutive IDs per family so related processing stages remain adjacent.

Do not fill intentional gaps merely to keep the sequence continuous.

---

## Examples

```text
70001   Basic Torch
70002   Oil Lantern

71001   Special Weapon
71101   Basic Dagger
71201   Basic Sword
71301   Basic Axe
71401   Basic Bow

72001   Small Healing Item
72002   Stamina Recovery Item

73001   Basic Arrow

74001   Tutorial Key

75001   Wood
75002   Bone
75003   Twig
75006   Purple Orb
75007   Stone

75101   Iron Chunks
75102   Iron Ore
75103   Iron Bar

75201   Aquamarine Chunks
75202   Aquamarine Crystal
75203   Aquamarine Block
```

---

**End of Document**
