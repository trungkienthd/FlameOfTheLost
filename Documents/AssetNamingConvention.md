# Asset Naming Convention  
**Project:** FlameOfTheLost  
**Version:** 1.0  
**Scope:** All assets moved into `Assets/Resources/`

---

## 1. Purpose

This document defines the naming convention for third-party assets integrated into the project.

### Objectives

- Ensure asset traceability (source identification)
- Prevent naming collisions
- Maintain licensing clarity
- Improve searchability and filtering
- Support scalable project growth

All files placed inside `Assets/Resources/` must follow this convention.

---

## 2. Naming Format

All assets must use the following structure: {PREFIX}{Category}{SpecificName}_{Variant}

### Definitions

- **PREFIX** → Identifies the original asset source  
- **Category** → Functional grouping (e.g., VFX, Icon, Tile, UI, Character)  
- **SpecificName** → Descriptive name  
- **Variant** → Optional modifier (color, level, type, version, etc.)

### Examples

BGE118_VFX_SlashArc_Red.prefab
CFRPG16_Tile_Grass_01.png
CPSKI_Icon_HemorrhageSlash.png
SFUI_Panel_Dialogue.png

---

## 3. Approved Prefix Registry (Current Assets)

### 3.1 118 Sprite Effects Bundle  
**Publisher:** bestgamekits  

**Prefix:**  BGE118_

**Usage Scope:**
- Slash effects  
- Hit impacts  
- Explosions  
- Magic VFX  
- Effect sprite sheets  

---

### 3.2 Cute Fantasy RPG – 16x16 Top Down  
**Publisher:** kenmi  

**Prefix:**  CFRPG16_

**Usage Scope:**
- Environment tiles  
- Character sprites  
- Props  
- NPC sprites  
- Tilesets  

---

### 3.3 Simple Free Pixel Art Styled UI Pack  
**Publisher:** Jakub Varga  

**Prefix:**  SFUI_

**Usage Scope:**
- Buttons  
- Panels  
- HUD frames  
- UI decorative elements  

---

### 3.4 4416 RPG Icons Pixel Art
**Publisher:** CraftPix  

**Prefix:**  CPRIPA_

**Usage Scope:**
- Skill icons  
- Ability icons  
- Status icons  

---

### 3.5 2D Pixel Quest Vol.3 - The UI/GUI
**Publisher:** 7Soul

**Prefix:**  BAPQ3UI_

**Usage Scope:**
- UI icons
- GUI elements
- Buttons
- Panels
- HUD elements
- Pixel-art interface decorations

---

## 4. Custom Project Assets

All internally created assets must use a dedicated project prefix: FTL_

### Example
FTL_Character_Base.prefab
FTL_VFX_BleedTick.prefab
FTL_UI_SkillSlot.prefab

This ensures a clear separation between:
- Third-party assets
- Custom-developed content

---

## 6. Restrictions

The following are not permitted:

- Removing or altering source prefixes  
- Mixing multiple source prefixes in one file  
- Inconsistent abbreviation styles  
- Renaming `.meta` files manually  
- Using files inside `Resources/` without prefix  

---

## 7. Future Asset Integration Rule

When importing new third-party assets:

1. Assign a unique prefix.
2. Base the prefix on publisher + pack identifier.
3. Register it in this document before usage.
4. Maintain consistency across the project.

---

## 8. Governance

- This document must be updated whenever a new asset pack is integrated.
- All contributors must comply with this naming convention.
- Deviations require refactoring before merge approval.

---

**End of Document**

