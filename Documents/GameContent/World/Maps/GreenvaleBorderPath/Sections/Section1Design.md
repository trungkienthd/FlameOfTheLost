# Greenvale Border Path — Section 1 Design

## 1. Section Purpose

Section 1 is the first gameplay-introduction area of Greenvale Border Path.

Its purpose is to teach a deliberately small set of concepts before the player
encounters more complex world content.

The section currently introduces:

1. Player Default Spawn Point;
2. Item Container interaction and receiving the first weapon;
3. Death Trigger behaviour;
4. death and respawn;
5. non-interactable Scenery Objects as environmental dressing.

Section 1 should remain mechanically simple.

---

## 2. Current Layout Reference

### Current Implementation Reference

![Greenvale Border Path Section 1 — Current Implementation](../Images/Section1_CurrentImplementation.png)

This image represents the current in-game layout and current Scenery Object
placement.

### Scenery Placement Sketch

![Greenvale Border Path Section 1 — Scenery Placement Sketch](../Images/Section1_SceneryPlacementSketch.png)

The sketch is an illustrative map-design reference. It communicates the visual
intent and content distribution of Section 1, but it should not be treated as a
strict tile-by-tile reconstruction of the Unity scene.

---

## 3. Main Areas

Section 1 currently consists of two visually separate gameplay areas.

### 3.1 Main Starting Island

The Main Starting Island contains:

- Player Default Spawn Point 1;
- introductory signs;
- the first Item Container;
- the main path leading toward the next section;
- Scenery Objects used to make the space feel natural without introducing
  additional gameplay systems.

### 3.2 Wayside Pit

Wayside Pit is the dedicated death-and-respawn teaching area.

It contains:

- Player DSP 2;
- the Death Trigger;
- warning / respawn instructional signs.

The Death Trigger area itself should remain visually clear and should not be
filled with Scenery Objects.

---

## 4. Gameplay Learning Goals

### 4.1 Default Spawn

The player begins at **Player DSP 1**.

The spawn location should be visually readable immediately after the game
starts. Scenery around it should remain relatively sparse so that the player is
not visually distracted from the spawn sign and nearby path.

### 4.2 First Item Container

The first Item Container introduces the player to obtaining an Item from a
container.

Current intended content:

- **Worn Short Sword**

The area around the Item Container should maintain a clear visual buffer so
that the chest remains one of the dominant gameplay objects in the scene.

### 4.3 Death and Respawn

Wayside Pit introduces:

- approaching a Death Trigger;
- player death;
- respawning at the latest valid Spawn Point.

The pit should read clearly as a hazard rather than as an explorable or
resource-rich area.

---

## 5. Content Restrictions

Section 1 intentionally excludes several gameplay content types.

### 5.1 No Harvestables

Section 1 should contain **no Harvestable Objects**.

This includes Harvestable Trees and Harvestable Stones.

The player should not be introduced to harvesting in this section.

### 5.2 No Loose Collectibles

Section 1 should contain **no loose Collectible Objects**.

The first acquisition lesson is intentionally focused on receiving an Item
through an Item Container.

### 5.3 No Enemies

Section 1 should remain focused on basic world interaction, death, and respawn
rather than combat encounters.

---

## 6. Scenery Object Design

Scenery Objects are used to make Section 1 visually complete without adding
new gameplay interactions.

The current Scenery Object families used or available for this section are:

- Ground Flora;
- Wildflowers;
- Mushrooms;
- Shrubs;
- Fallen Wood.

Wetland Flora is not required for the current Section 1 layout.

For general Scenery Object rules, see:

`../../Environment/Scenery/SceneryObjectDesign.md`

For the current Scenery catalogue, see:

`../../Environment/Scenery/SceneryObjectCatalogue.md`

---

## 7. Scenery Placement Principles

### 7.1 Preserve Gameplay Focus

Scenery should not obscure or visually compete with:

- Player DSP 1;
- the first Item Container;
- signs;
- paths;
- Player DSP 2;
- the Wayside Pit Death Trigger.

### 7.2 Keep the Main Route Clear

The central path and the route toward the bridge should remain visually
cleaner than the surrounding grass.

This helps the player understand the intended movement direction without
requiring explicit path markers everywhere.

### 7.3 Use Higher Density Away from Key Objects

Denser scenery can be placed:

- near island edges;
- in unused corners;
- around the southern and outer grass areas;
- inside the enclosed central grass patch where it does not interfere with
  pathing.

### 7.4 Keep Wayside Pit Restrained

Scenery may frame the outer parts of the Wayside Pit island, but the actual
Death Trigger area should remain free of decorative clutter.

The hazard should remain the visual focus of this area.

---

## 8. Current Scenery Composition

The current implementation uses a mixture of:

- leaf / sprout Ground Flora;
- Wildflowers;
- coloured Cap Mushrooms;
- Shrubs;
- Fallen Wood logs;
- Broken Stumps.

The Main Starting Island uses a greater scenery density than Wayside Pit.

This creates a visual distinction between:

- the broader starting environment; and
- the mechanically focused death tutorial area.

---

## 9. Visual Progression

The intended visual progression through Section 1 is:

```text
Player DSP 1
    ↓
Lightly decorated safe starting area
    ↓
Main path and first Item Container
    ↓
More natural scenery toward unused edges
    ↓
Transition toward the next section
```

Wayside Pit acts as a side teaching space with a separate visual purpose:

```text
Player DSP 2
    ↓
Warning / instructional signage
    ↓
Clearly readable Death Trigger
    ↓
Death
    ↓
Respawn
```

---

## 10. Implementation Notes

Section 1 Scenery Objects should remain lightweight and non-interactable.

They should not:

- receive harvesting damage;
- receive combat damage;
- drop Items;
- show interaction prompts;
- participate in quests;
- behave as hidden resources.

The current design intentionally separates visual richness from mechanical
complexity.

---

## 11. Future Review

Section 1 should be reviewed again when:

- the tutorial progression is finalized;
- final Spawn Point behaviour is confirmed;
- the first Item Container content is finalized;
- visual hierarchy is tested in actual gameplay;
- Section 2 is documented and the transition between the two sections is
  finalized.
