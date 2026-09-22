# Scenery Object Design

## 1. Overview

Scenery Objects are non-interactable world objects used to visually construct,
decorate, and communicate the environment of FlameOfTheLost.

Unlike gameplay-oriented Environment Objects such as Harvestables,
Item Containers, Crafting Stations, or Collectibles, Scenery Objects do not
provide direct gameplay interactions.

Their primary functions are:

- visual composition;
- environmental storytelling;
- area differentiation;
- landmark support;
- path framing;
- reduction of visually empty terrain;
- reinforcement of biome and location identity.

Scenery Objects should support gameplay readability without competing with
gameplay-relevant World Objects.

---

## 2. Design Definition

A Scenery Object is an Environment Object whose purpose is primarily visual.

By default, a Scenery Object:

- cannot be interacted with;
- cannot be harvested;
- cannot be collected;
- does not contain Items;
- does not receive combat or harvesting damage;
- does not have health;
- does not drop rewards;
- does not display interaction prompts;
- does not apply Status Effects;
- does not participate in quests or progression conditions.

Scenery Objects are therefore intentionally lightweight compared with
gameplay-oriented World Objects.

---

## 3. Gameplay Readability

Gameplay-relevant objects must remain visually distinguishable from scenery.

This is especially important for natural Environment Objects because some
visual categories may exist as both scenery and gameplay objects.

Examples include:

- Stones;
- Trees;
- Plants;
- Mushrooms;
- Logs;
- Ores or mineral formations.

A Scenery Object should not closely imitate an interactable object unless the
player can reliably distinguish the two.

The player should not need to attack or interact with every environmental
object merely to determine whether it has gameplay behaviour.

Sprites already assigned to Harvestables must not also be treated as Scenery
Objects.

---

## 4. Scenery Categories

The initial Scenery Object set is divided into the following categories:

1. Ground Flora
2. Wildflowers
3. Wetland Flora
4. Mushrooms
5. Shrubs
6. Fallen Wood

These categories describe visual and environmental purpose rather than
gameplay behaviour.

All currently defined Scenery Object categories remain non-interactable.

---

## 5. Ground Flora

Ground Flora consists of small leaves, sprouts, crop tops, and similar
vegetation placed directly on terrain.

The current outdoor decor sheet provides:

- three leaf variants;
- one generic sprout;
- one carrot top.

### Purpose

Ground Flora is primarily used to:

- break up large areas of grass terrain;
- soften path boundaries;
- decorate the bases of larger environmental objects;
- fill small visual gaps;
- indicate lightly overgrown terrain.

### Placement

Ground Flora should generally appear in irregular arrangements.

Avoid obvious grids or equal spacing unless required by a deliberately
maintained location.

---

## 6. Wildflowers

Wildflowers are small decorative flowering plants with stronger colour accents
than ordinary Ground Flora.

The current outdoor decor sheet provides three Wildflower variants.

### Purpose

Wildflowers can:

- introduce colour variation;
- visually distinguish local areas;
- frame landmarks;
- decorate safe or peaceful spaces;
- guide attention through subtle visual composition.

Because strongly coloured flowers attract attention, they should not overwhelm
important gameplay objects.

---

## 7. Wetland Flora

Wetland Flora is decorative vegetation associated primarily with water.

The current outdoor decor sheet provides Lily Pad variants including:

- Single;
- Double;
- Flowering;
- Cluster Flowering.

Wetland Flora is distinct from Harvestable wetland vegetation such as
Harvestable Reed Patches.

### Purpose

Wetland Flora is used to:

- reinforce aquatic environments;
- make ponds and wetlands visually richer;
- provide variation along water boundaries.

---

## 8. Mushrooms

Decorative Mushrooms are small non-interactable fungi used primarily around:

- trees;
- fallen logs;
- shaded vegetation;
- damp environments;
- forest edges.

The current outdoor decor sheet contains two visually distinct Mushroom
families.

### Cap Mushrooms

Cap Mushrooms are defined primarily by their rounded coloured caps.

Current variants:

- Red Cap — Large;
- Purple Cap — Large;
- Blue Cap — Large;
- Red Cap — Small;
- Purple Cap — Small;
- Blue Cap — Small.

### Brown Stem Mushrooms

Brown Stem Mushrooms use a taller, exposed-stem silhouette.

Current variants:

- Brown Stem — Large;
- Brown Stem — Small.

If Mushroom Items or Harvestables are introduced in the future, their gameplay
presentation should remain visually distinguishable from decorative Mushrooms.

---

## 9. Shrubs

Shrubs are medium-to-large vegetation used to occupy more visual space than
Ground Flora or Wildflowers.

The current outdoor decor sheet contains:

- Round Small Shrub;
- Dense Large Shrub.

### Purpose

Shrubs may be used to:

- break up large open grass areas;
- frame terrain boundaries;
- fill corners;
- visually separate paths from open terrain;
- create denser vegetation zones.

Shrubs are non-blocking by default.

---

## 10. Fallen Wood

Fallen Wood includes decorative fallen logs, broken stumps, and decaying wood.

The current outdoor decor sheet contains the following families.

### Logs

- Plain;
- Sprouting 01;
- Sprouting 02;
- Purple Fungus;
- Red Fungus;
- Blue Fungus.

### Broken Stumps

- Plain;
- Sprouting;
- Hollow.

### Purpose

Fallen Wood is particularly useful for environmental storytelling.

It may communicate:

- age;
- forest density;
- natural decay;
- abandoned areas;
- storm or environmental damage;
- reduced maintenance.

Decorative Fallen Wood must remain visually distinguishable from any future
Harvestable Wood objects.

---

## 11. Collision Policy

Scenery Objects are non-blocking by default.

Objects such as:

- Ground Flora;
- Wildflowers;
- Mushrooms;
- Lily Pads;
- Shrubs;
- Fallen Wood

should normally have no gameplay collider.

If an environmental object is intentionally intended to prevent movement,
that behaviour should be treated as a separate gameplay responsibility rather
than being implicitly assigned to ordinary Scenery Objects.

A future Static Obstacle or similar Environment Object type may be introduced
for this purpose.

---

## 12. Technical Complexity

Scenery Objects should remain lightweight.

A basic Scenery Object generally requires only:

- GameObject;
- Transform;
- SpriteRenderer.

They should not require the Brain/Sub-Behaviour architecture unless future
gameplay behaviour makes such architecture necessary.

Additional components may be added for purely rendering-related purposes where
needed.

---

## 13. Naming

`Scn` is the abbreviation used for Scenery in asset names.

Scenery sprite names use the general form:

`<SourceSheet>_<Index>_Scn_<SemanticName>`

The semantic portion should remain readable without overusing underscores.

Examples:

- `CFRPG16_Outdoor_Decor_0_Scn_GroundFloraLeaf_1`
- `CFRPG16_Outdoor_Decor_9_Scn_WetlandFloraLilyPadSingle`
- `CFRPG16_Outdoor_Decor_30_Scn_FallenWoodLogSprouting_01`

Underscores may still be used where they are useful for an existing variant
suffix or established family distinction.

---

## 14. Placement Principles

Scenery placement should follow several general principles.

### Avoid Uniform Distribution

Natural scenery should normally use irregular spacing, density, and grouping.

### Preserve Gameplay Readability

Do not obscure:

- enemies;
- Collectibles;
- Harvestables;
- Item Containers;
- Checkpoints;
- entrances;
- important paths.

### Use Clustering

Small scenery generally looks more natural when arranged in loose clusters
rather than uniformly scattered.

### Use Density to Communicate Space

Lower density may suggest:

- maintained terrain;
- frequently travelled routes;
- safe areas.

Higher density may suggest:

- wilderness;
- unused corners;
- forest edges;
- abandoned areas.

---

## 15. Initial Asset Source

The initial Scenery Object sprites are sourced from:

`CFRPG16_Outdoor_Decor`

Only sprites formally assigned the `Scn` role are part of the initial Scenery
Object set.

Sprites assigned to Harvestables, including the Stone variants from the same
sheet, are excluded from Scenery.
