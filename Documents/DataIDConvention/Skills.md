# Skills

**Major Range:** `80xxx`

---

## Asset Type

- ScriptableObject

---

## Base Profile Type

- `SkillProfile`

---

## Skill Type Sub-Ranges

```text
810xx   Melee Skills
820xx   Ranged Skills

83xxx   Reserved
84xxx   Reserved
85xxx   Reserved
86xxx   Reserved
87xxx   Reserved
88xxx   Reserved
89xxx   Reserved
```

---

## Skill Usage Sub-Ranges

The third digit identifies how the skill is obtained or used.

```text
8x0xx   Weapon Normal Attacks
8x1xx   Weapon Innate Skills
8x2xx   Character-Learned Special Skills
```

---

## Melee Skills

**Range:** `810xx`  
**Profile Type:** `MeleeSkillProfile`

```text
810xx   Melee Weapon Normal Attacks
811xx   Melee Weapon Innate Skills
812xx   Melee Character-Learned Special Skills
```

---

## Ranged Skills

**Range:** `820xx`  
**Profile Type:** `RangedSkillProfile`

```text
820xx   Ranged Weapon Normal Attacks
821xx   Ranged Weapon Innate Skills
822xx   Ranged Character-Learned Special Skills
```

---

## Assignment Rule

The ID identifies a **skill definition**.

Skills are grouped first by skill type, then by how the skill is obtained or used.

---

## Examples

```text
81001   Basic Dagger Normal Attack
81101   Dagger Innate Skill
81201   Character-Learned Melee Special Skill

82001   Basic Bow Normal Attack
82101   Bow Innate Skill
82201   Character-Learned Ranged Special Skill
```

---

**End of Document**
