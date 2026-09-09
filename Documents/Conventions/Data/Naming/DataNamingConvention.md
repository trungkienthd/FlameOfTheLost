# Data Naming Convention

**Project:** FlameOfTheLost  
**Scope:** Project-owned ScriptableObject data assets.

---

## 1. Purpose

This document defines the file naming convention for ScriptableObject data in **FlameOfTheLost**.

The convention is intentionally separate from:

- **Data ID Convention** — defines which numeric ID belongs to each data definition.
- **Asset Naming Convention** — defines general asset/type prefixes and broader asset naming rules.

This document only defines how an ID-bearing data asset should be named in the Project.

---

## 2. Core Format

All ID-bearing data assets should use:

```text
<ID>_<DataName>
```

Where:

- `<ID>` is the data definition's assigned numeric ID.
- `<DataName>` is the existing descriptive asset name, including its established type/category prefix where applicable.

Examples:

```text
75001_Mtr_Wood
75003_Mtr_Twig
75101_Mtr_IronChunks
75208_Mtr_RubyCrystal
```

The numeric ID is always placed first.

---

## 3. File Name and Unity Object Name

The asset filename and the ScriptableObject's Unity object name should match.

Example:

```text
File:
75001_Mtr_Wood.asset

Unity Object Name:
75001_Mtr_Wood
```

Do not intentionally maintain different names for the `.asset` file and its contained ScriptableObject.

---

## 4. Preserve Existing Data Prefixes

Adding the ID prefix does **not** replace existing category or type prefixes.

For example:

```text
Mtr_Wood
```

becomes:

```text
75001_Mtr_Wood
```

not:

```text
75001_Wood
```

Likewise, existing prefixes used by other data categories should remain part of `<DataName>`.

The ID communicates the data allocation; the existing prefix communicates the data's semantic/type identity.

---

## 5. Naming Rules

### 5.1 ID First

Always place the numeric ID at the beginning:

```text
75001_Mtr_Wood
```

Do not use:

```text
Mtr_75001_Wood
Mtr_Wood_75001
Wood_75001
```

### 5.2 Underscore Separator

Use one underscore between the ID and the existing data name:

```text
<ID>_<DataName>
```

### 5.3 No Spaces

Do not use spaces in data filenames.

Use the established PascalCase / prefix-based asset naming instead.

Example:

```text
75102_Mtr_IronOre
```

not:

```text
75102 Iron Ore
```

### 5.4 ID Must Match the Data

The numeric filename prefix must match the ID stored by the data definition.

Example:

```text
75108_Mtr_GoldOre
```

must contain:

```text
ID: 75108
```

A mismatch between filename ID and stored ID is invalid.

### 5.5 Renaming After ID Assignment

When an existing data asset receives an ID-based filename, preserve its established descriptive name and prepend the ID.

Example:

```text
Mtr_Twig
```

becomes:

```text
75003_Mtr_Twig
```

Do not unnecessarily rename the descriptive portion during the same operation unless the content itself is also being renamed.

---

## 6. Examples

### Materials

```text
75001_Mtr_Wood
75002_Mtr_Bone
75003_Mtr_Twig
75006_Mtr_PurpleOrb
75007_Mtr_Stone

75101_Mtr_IronChunks
75102_Mtr_IronOre
75103_Mtr_IronBar

75201_Mtr_AquamarineChunks
75202_Mtr_AquamarineCrystal
75203_Mtr_AquamarineBlock
```

### General Pattern

If an existing data asset is named:

```text
<DataName>
```

and receives ID:

```text
12345
```

its resulting name should be:

```text
12345_<DataName>
```

---

## 7. Scope

Apply this convention to project-owned ScriptableObject data assets that participate in the FlameOfTheLost Data ID system.

Do not apply the numeric prefix to:

- Runtime GameObjects.
- MonoBehaviour component names.
- Prefabs that do not themselves represent ID-bearing data definitions.
- Art/source files that are not part of the Data ID system.
- Third-party package assets.

Whether a data category owns an ID is determined by the **Data ID Convention**, not by this naming document.

---

## 8. Relationship to Data IDs

The filename prefix is a human-readable reflection of the assigned Data ID.

The ID stored by the data definition remains the authoritative gameplay identifier.

Renaming a file does not create or reassign an ID.

When creating a new data asset:

1. Assign its ID according to the relevant Data ID Convention document.
2. Create or rename the asset using `<ID>_<DataName>`.
3. Verify the filename ID matches the stored data ID.
4. Preserve the existing data/type prefix inside `<DataName>`.

---

## 9. Summary

```text
Format:
<ID>_<DataName>

Example:
75001_Mtr_Wood

Rules:
- ID comes first.
- Use one underscore after the ID.
- Preserve existing data/type prefixes.
- File name and Unity object name should match.
- Filename ID must match the stored Data ID.
- Only ID-bearing ScriptableObject data uses this convention.
```

---

**End of Document**
