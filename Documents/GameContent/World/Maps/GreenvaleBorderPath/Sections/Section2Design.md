# Greenvale Border Path — Section 2 Design

## 1. Section Purpose

Section 2 is the second gameplay-introduction area of Greenvale Border Path.

Its purpose is to expand the player's interaction with the world after the
basic spawn, Item Container, death, and respawn concepts introduced in
Section 1.

Section 2 currently introduces:

1. Harvestable Objects;
2. Material acquisition from Harvestables;
3. loose Material Collectibles;
4. the second weapon — Bow;
5. Projectile / Arrow acquisition;
6. the Crafting Station system;
7. Arrow crafting;
8. the first enemy encounter — Feralshroom Basher;
9. denser environmental composition using Scenery Objects.

Section 2 should remain focused on these systems and should not yet introduce
Consumables, Checkpoints, additional enemy groups, or the broader content
reserved for Section 3.

---

## 2. Current Layout Reference

### Current Implementation Reference

![Greenvale Border Path Section 2 — Current Implementation](../Images/Section2_CurrentImplementation.png)

This image represents the current in-game layout, World Object placement, and
Scenery Object placement for Section 2.

### Scenery Placement Sketch

![Greenvale Border Path Section 2 — Scenery Placement Sketch](../Images/Section2_SceneryPlacementSketch.png)

The sketch is an illustrative map-design reference. It communicates the current
content distribution and visual intent of Section 2, but it should not be
treated as a strict tile-by-tile reconstruction of the Unity scene.

---

## 3. Main Areas

Section 2 is designed as a connected resource, crafting, and combat tutorial
space.

### 3.1 Section 1 Entrance

The southern bridge connects Section 1 to Section 2.

The player should enter into a relatively readable area where the first
Harvestable and its tutorial guidance can be understood without immediate
combat pressure.

### 3.2 Central Harvesting Area

The central area contains the primary Oak Tree used to introduce harvesting.

It also contains the tutorial guidance object explaining that Harvestable
Objects can be attacked to obtain Materials.

This area should provide enough open space around the Oak Tree for the player to
experiment with attacks and observe the Harvestable response clearly.

### 3.3 Western Resource Branch

The western side contains:

- a Small Oak Tree;
- a loose Twig Collectible;
- the Bow Item Container;
- supporting Scenery Objects.

This branch rewards exploration while reinforcing the relationship between
Harvestables, Materials, Collectibles, and weapon acquisition.

### 3.4 Northern Supply Area

The northern branch contains an Item Container used as an Arrow support supply.

Its purpose is to prevent the player from being completely dependent on
crafting or previously collected Arrows while learning the Bow.

### 3.5 Crafting Area

The lower-right area contains:

- the Crafting tutorial object / guidance sign;
- the actual Makeshift Worktable.

These are separate objects.

The tutorial object communicates that Crafting Stations can be used to create
new Items.

The large brown worktable to its right is the actual **Makeshift Worktable** and
is the first Crafting Station introduced to the player.

### 3.6 First Enemy Area

The eastern clearing contains the first enemy encounter:

- **Feralshroom Basher**
- Territorial Enemy
- melee-oriented encounter

The clearing should remain comparatively open so that the player can test the
Worn Short Sword, Bow, and basic movement without decorative objects interfering
with combat readability.

The eastern bridge continues toward Section 3.

---

## 4. Gameplay Learning Goals

### 4.1 Harvesting

Section 2 introduces the concept that certain Environment Objects can be
attacked to obtain Materials.

The primary tutorial Harvestable is the **Oak Tree**.

The current design uses two Oak presentations:

- Normal Oak Tree — primary harvesting introduction;
- Small Oak Tree — optional reinforcement / additional resource source.

Using two variants of the same core Harvestable family reinforces harvesting
without introducing another harvesting language too early.

### 4.2 Material Acquisition

The player can obtain basic timber Materials through harvesting.

The Section 2 resource loop currently focuses on:

- Wood;
- Twig.

A loose Twig Collectible is also placed in the western branch.

This demonstrates that an inventory Material can come from more than one world
source and prevents the crafting lesson from depending entirely on a random
secondary Harvestable drop.

### 4.3 Second Weapon — Bow

The western Item Container introduces the player's second weapon:

- **Bow**

The container may also provide a small initial Arrow supply.

The Bow introduces ranged combat while remaining directly connected to the
Arrow crafting lesson in the same section.

### 4.4 Arrow Supply

A second Item Container in the northern area provides additional Arrows.

This chest acts as a support supply rather than introducing another major Item
category.

Its purpose is to:

- give the player room to experiment with the Bow;
- reduce the risk of the player exhausting all Arrows before the first enemy;
- support the crafting lesson without making crafting the only Arrow source.

### 4.5 Crafting

The **Makeshift Worktable** is the first Crafting Station.

The initial crafting lesson focuses on producing Arrows from the timber
Materials introduced in the same section.

Conceptually:

```text
Harvest Oak Tree
    ↓
Obtain Wood / Twig
    ↓
Collect additional Twig if needed
    ↓
Use Makeshift Worktable
    ↓
Craft Arrows
```

The nearby tutorial object is guidance only and must not be treated as the
Crafting Station itself.

### 4.6 First Enemy Encounter

The Feralshroom Basher is the first enemy introduced on Greenvale Border Path.

The encounter provides a controlled opportunity to apply the systems learned so
far:

- movement;
- Worn Short Sword combat;
- Bow combat;
- Arrow use;
- basic enemy engagement.

Only one enemy is introduced here so that enemy behaviour does not compete with
the harvesting and crafting lessons.

---

## 5. Harvestable Scope

Section 2 intentionally keeps the Harvestable scope narrow.

### 5.1 Included

Current Section 2 Harvestables:

- Oak Tree — Normal;
- Oak Tree — Small.

### 5.2 Not Introduced Yet

Section 2 should not introduce:

- Stone Harvestables;
- Metal Nodes;
- Crystal Nodes;
- Fruit Trees;
- other new harvesting families.

The section is intended to teach the harvesting system through timber first.

Additional harvesting families can be introduced later when the player has the
appropriate gameplay context and harvesting language.

---

## 6. Collectibles

Section 2 introduces a loose Material Collectible through **Twig**.

The loose Twig should be visually distinguishable from Fallen Wood Scenery.

The player should be able to understand that:

```text
Scenery Fallen Wood
    = visual environment only

Twig Collectible
    = inventory Material pickup
```

The placement should therefore preserve enough visual separation and pickup
feedback to prevent ambiguity.

---

## 7. Item Containers

Section 2 currently contains two Item Containers with different tutorial roles.

### 7.1 Bow Item Container

Purpose:

- introduce the Bow;
- optionally provide a small starter Arrow supply;
- reward exploration of the western branch.

### 7.2 Arrow Support Item Container

Purpose:

- provide additional Arrows;
- support early Bow experimentation;
- reduce the chance of the player reaching the first enemy with no ammunition.

The two containers should remain visually and spatially distinct so that each
has a clear purpose.

---

## 8. Crafting Station

The first Crafting Station is the **Makeshift Worktable**.

It is the large brown worktable located in the lower-right portion of the
section.

The smaller tutorial object to its left displays crafting guidance and is not
the Crafting Station.

### Current Tutorial Role

The first recipe should reinforce the Materials learned through harvesting.

Current intended crafting loop:

```text
Wood + Twig
    ↓
Makeshift Worktable
    ↓
Arrows
```

Exact recipe quantities remain part of the Crafting system configuration rather
than the map-layout document.

---

## 9. Enemy Design within Section 2

### 9.1 Feralshroom Basher

Section 2 contains one Feralshroom Basher in the eastern clearing.

Its role is to be the first controlled enemy encounter.

The enemy area should:

- remain relatively open;
- provide enough room for melee and ranged testing;
- avoid dense Scenery near the combat center;
- keep the bridge toward Section 3 visually readable.

### 9.2 Enemy Scope Restriction

Section 2 should not contain:

- additional Feralshroom groups;
- Toxic Feralshroom;
- Aquabara;
- other enemy families.

Those broader enemy encounters are reserved for later progression.

---

## 10. Scenery Object Design

Section 2 uses Scenery Objects more heavily than Section 1 because the player is
moving from a minimal tutorial environment into a more natural resource-bearing
area.

Current Scenery families include:

- Ground Flora;
- Wildflowers;
- Mushrooms;
- Shrubs;
- Fallen Wood;
- Broken Stumps.

For general Scenery Object rules, see:

`../../Environment/Scenery/SceneryObjectDesign.md`

For the current Scenery catalogue, see:

`../../Environment/Scenery/SceneryObjectCatalogue.md`

---

## 11. Scenery Placement Principles

### 11.1 Preserve Harvestable Readability

Do not place dense Scenery immediately around either Oak Tree.

The player should clearly recognize the Oak as a gameplay object and have enough
space to attack it.

### 11.2 Preserve Collectible Readability

Do not visually bury the loose Twig Collectible among Fallen Wood or similarly
shaped scenery.

### 11.3 Preserve Crafting Readability

The Makeshift Worktable should remain visually identifiable as the Crafting
Station.

The guidance object should be close enough to establish the relationship but
must remain visually distinct from the Worktable.

### 11.4 Preserve Combat Space

The Feralshroom clearing should remain one of the least cluttered areas in the
section.

Scenery can frame the outer boundary but should not interfere with the first
enemy encounter.

### 11.5 Use Scenery to Reward Exploration

The western and northern edges can use denser clusters of:

- Shrubs;
- Fallen Wood;
- Broken Stumps;
- Ground Flora;
- Wildflowers.

This makes optional branches feel intentionally authored without adding extra
gameplay systems.

---

## 12. Pathing and Player Flow

The intended high-level progression is:

```text
From Section 1
    ↓
Primary Oak Tree + Harvesting Tutorial
    ↓
Explore western branch
    ├── Small Oak Tree
    ├── loose Twig Collectible
    └── Bow Item Container
    ↓
Explore / obtain Arrow support supply
    ↓
Makeshift Worktable + Crafting Tutorial
    ↓
Craft / prepare Arrows
    ↓
Feralshroom Basher
    ↓
Bridge toward Section 3
```

The layout should allow some exploration freedom while maintaining this
underlying learning sequence.

---

## 13. Content Restrictions

Section 2 intentionally does **not** introduce:

- Consumables;
- Cherry Tree;
- Checkpoints;
- additional enemies;
- enemy groups;
- Aquabara;
- Stone / Metal / Crystal harvesting;
- Section 3 progression systems.

This keeps the section focused on:

**Harvest → Materials → Bow / Arrows → Craft → First Enemy**

---

## 14. Transition from Section 1

Section 1 teaches:

- spawning;
- Item Container interaction;
- first weapon acquisition;
- death;
- respawn;
- basic Scenery readability.

Section 2 builds directly on that foundation by introducing world resources and
combat preparation.

The southern bridge should therefore feel like a transition from a controlled
basic tutorial area into a more system-rich woodland space.

---

## 15. Transition to Section 3

The eastern bridge leads toward Section 3.

Section 2 should leave the player with:

- two weapon options;
- experience obtaining Materials;
- experience with a loose Collectible;
- experience using a Crafting Station;
- experience crafting Arrows;
- experience fighting the first enemy.

Section 3 can then expand the map with additional enemy encounters, Consumables,
Checkpoints, and other systems without requiring Section 2 to carry those
lessons.

---

## 16. Implementation Notes

Current implementation should preserve the distinction between:

- Harvestable Oak Trees;
- non-interactable Scenery;
- loose Twig Collectible;
- tutorial guidance objects;
- actual Makeshift Worktable;
- Item Containers;
- Territorial Feralshroom Basher.

The Worktable and the crafting tutorial object must remain separate in both the
Hierarchy and documentation.

The Scenery sketch is a design reference rather than an exact placement mandate.

---

## 17. Future Review

Section 2 should be reviewed again when:

- Bow and Arrow quantities are finalized;
- the Arrow recipe is finalized;
- Harvestable drop values are balanced in gameplay;
- the Feralshroom Basher encounter is playtested;
- Tutorial Progress conditions are wired to the Section 2 flow;
- Section 3 is fully documented and the transition between Sections 2 and 3 is
  finalized.
