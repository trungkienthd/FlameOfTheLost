# Harvestable Design

## Purpose

This document is the authoritative catalogue of Harvestable Objects in **FlameOfTheLost**.

It defines:

- Harvestable gameplay identity;
- content status;
- variants;
- Maximum Durability;
- impact-response configuration;
- direct drops;
- minimum / maximum drop quantity;
- independent drop chance.

Detailed harvesting-impact rules and skill mappings are defined in [Impact Configuration](ImpactConfiguration.md).

---

## 1. Core Rules

1. A visual variant does not automatically create a new core Harvestable type; however, a sprite may become its own Harvestable type when the design intentionally gives it a distinct gameplay configuration.
2. The seven authored Stone sprites are treated as seven separate Stone-like Harvestable types because size/environment presentation is now tied to distinct durability and yield tuning.
3. Drops are defined by the Harvestable / variant, not by the impact used to break it.
4. Each drop entry independently defines minimum quantity, maximum quantity, and chance.
5. Normal mineral Harvestables directly drop only their raw `Chunks` form.
6. Processed forms belong to future refining:
   - Metals: `Chunks -> Ore -> Bar`
   - Crystals: `Chunks -> Crystal -> Block`

---

## 2. Catalogue Summary

| Category | Harvestable | Variant(s) | Status | Max Durability | Preferred Impact | Primary Drop |
|---|---|---|---|---:|---|---|
| Timber Tree | Oak Tree | Normal, Small | Current | 7 / 5 | Chop | Wood |
| Timber Tree | Birch Tree | Normal, Small | Planned | 6 / 4 | Chop | Twig |
| Timber Tree | Spruce Tree | Normal, Small | Planned | 8 / 6 | Chop | Wood |
| Fruit Tree | Apple Tree | Presentation variants | Planned | 7 | Chop | Apple |
| Fruit Tree | Cherry Tree | Presentation variants | Current | 5 | Slash | Cherry |
| Fruit Tree | Peach Tree | Presentation variants | Planned | 6 | Chop | Peach |
| Fruit Tree | Pear Tree | Presentation variants | Planned | 7 | Slash / Chop | Pear |
| Stone Resource | Stone Large | — | Planned | 10 | Blunt | Stone |
| Stone Resource | Stone Normal | — | Planned | 7 | Blunt | Stone |
| Stone Resource | Stone Small 0 | — | Planned | 4 | Blunt | Stone |
| Stone Resource | Stone Small 1 | — | Planned | 5 | Blunt | Stone |
| Stone Resource | Stone Water Large | Water | Planned | 9 | Blunt | Stone |
| Stone Resource | Stone Water Normal | Water | Planned | 6 | Blunt | Stone |
| Stone Resource | Stone Water Small | Water | Planned | 4 | Blunt | Stone |
| Metal Node | Copper Node | Visual variants | Planned | 8 | Blunt | Copper Chunks |
| Metal Node | Iron Node | Visual variants | Planned | 10 | Blunt | Iron Chunks |
| Metal Node | Gold Node | Visual variants | Planned | 9 | Blunt | Gold Chunks |
| Crystal Node | Aquamarine Node | Visual variants | Planned | 8 | Blunt | Aquamarine Chunks |
| Crystal Node | Amber Node | Visual variants | Planned | 7 | Blunt | Amber Chunks |
| Crystal Node | Ruby Node | Visual variants | Planned | 10 | Blunt | Ruby Chunks |
| Crystal Node | Emerald Node | Visual variants | Planned | 9 | Blunt | Emerald Chunks |
| Crystal Node | Amethyst Node | Visual variants | Planned | 8 | Blunt | Amethyst Chunks |

**Total: 22 core Harvestable types.**

Oak, Birch, and Spruce retain Normal / Small variants without increasing the core type count. The seven Stone sprites are intentionally counted as seven separate Stone-like Harvestable types.

---

## 3. Timber Trees

All Timber Trees use:

- `Slash = 1`
- `Chop = 3`
- `Pierce = 1`
- `Blunt = 1`

### Oak Tree

Oak is the balanced general-purpose timber source.

| Variant | Max Durability | Drop | Min | Max | Chance |
|---|---:|---|---:|---:|---:|
| Normal | 7 | Wood | 2 | 4 | 100% |
| Normal | 7 | Twig | 1 | 2 | 55% |
| Small | 5 | Wood | 1 | 2 | 70% |
| Small | 5 | Twig | 2 | 4 | 100% |

### Birch Tree

Birch is the lightest Timber Tree family and the strongest Twig source.

| Variant | Max Durability | Drop | Min | Max | Chance |
|---|---:|---|---:|---:|---:|
| Normal | 6 | Wood | 1 | 2 | 85% |
| Normal | 6 | Twig | 2 | 4 | 100% |
| Small | 4 | Wood | 1 | 1 | 45% |
| Small | 4 | Twig | 3 | 5 | 100% |

### Spruce Tree

Spruce is the toughest Timber Tree family and strongest Wood source.

| Variant | Max Durability | Drop | Min | Max | Chance |
|---|---:|---|---:|---:|---:|
| Normal | 8 | Wood | 3 | 5 | 100% |
| Normal | 8 | Twig | 1 | 2 | 40% |
| Small | 6 | Wood | 2 | 3 | 80% |
| Small | 6 | Twig | 1 | 3 | 75% |

---

## 4. Fruit Trees

### Apple Tree

Apple is a sturdy fruit tree.

| Property | Value |
|---|---:|
| Maximum Durability | 7 |
| Slash | 2 |
| Chop | 3 |
| Pierce | 1 |
| Blunt | 1 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Apple | 2 | 4 | 100% |
| Twig | 1 | 2 | 25% |

### Cherry Tree

Cherry is the lightest fruit tree and the first Greenvale consumable Harvestable.

| Property | Value |
|---|---:|
| Maximum Durability | 5 |
| Slash | 3 |
| Chop | 2 |
| Pierce | 1 |
| Blunt | 1 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Cherry | 3 | 6 | 100% |
| Twig | 1 | 2 | 30% |

### Peach Tree

Peach is a medium-durability, Chop-friendly fruit tree.

| Property | Value |
|---|---:|
| Maximum Durability | 6 |
| Slash | 2 |
| Chop | 3 |
| Pierce | 1 |
| Blunt | 1 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Peach | 1 | 3 | 100% |
| Twig | 1 | 2 | 20% |

### Pear Tree

Pear is neutral between Slash and Chop.

| Property | Value |
|---|---:|
| Maximum Durability | 7 |
| Slash | 2 |
| Chop | 2 |
| Pierce | 1 |
| Blunt | 1 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Pear | 2 | 4 | 100% |
| Twig | 1 | 2 | 25% |

---

## 5. Stone Resources

The outdoor-decor sprite sheet provides seven authored Stone sprites, and the current design treats all seven as separate Stone-like Harvestable types:

- `StoneLarge`
- `StoneNormal`
- `StoneSmall_0`
- `StoneSmall_1`
- `StoneWaterLarge`
- `StoneWaterNormal`
- `StoneWaterSmall`

All seven drop the same `Stone` Material, but they differ in durability and yield.

Shared impact-response configuration:

| Impact | Multiplier |
|---|---:|
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 3 |

`Blunt` is the intended harvesting impact. `Chop` can slowly chip Stone. `Slash` and `Pierce` cause no durability damage.

### Stone Large

Sprite:

`CFRPG16_Outdoor_Decor_27_StoneLarge`

Large land-based stone resource.

| Property | Value |
|---|---:|
| Maximum Durability | 10 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 3 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Stone | 4 | 7 | 100% |

### Stone Normal

Sprite:

`CFRPG16_Outdoor_Decor_28_StoneNormal`

Standard land-based stone resource.

| Property | Value |
|---|---:|
| Maximum Durability | 7 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 3 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Stone | 2 | 4 | 100% |

### Stone Small 0

Sprite:

`CFRPG16_Outdoor_Decor_18_StoneSmall_0`

Small, low-durability stone resource.

| Property | Value |
|---|---:|
| Maximum Durability | 4 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 3 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Stone | 1 | 2 | 100% |

### Stone Small 1

Sprite:

`CFRPG16_Outdoor_Decor_19_StoneSmall_1`

A second small land-stone type with slightly greater mass/yield than `StoneSmall_0`.

| Property | Value |
|---|---:|
| Maximum Durability | 5 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 3 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Stone | 1 | 3 | 100% |

### Stone Water Large

Sprite:

`CFRPG16_Outdoor_Decor_17_StoneWaterLarge`

Large Stone-like Harvestable authored specifically for water environments.

| Property | Value |
|---|---:|
| Maximum Durability | 9 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 3 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Stone | 3 | 6 | 100% |

### Stone Water Normal

Sprite:

`CFRPG16_Outdoor_Decor_29_StoneWaterNormal`

Normal-sized Stone-like Harvestable for water environments.

| Property | Value |
|---|---:|
| Maximum Durability | 6 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 3 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Stone | 2 | 4 | 100% |

### Stone Water Small

Sprite:

`CFRPG16_Outdoor_Decor_21_StoneWaterSmall`

Small Stone-like Harvestable for water environments.

| Property | Value |
|---|---:|
| Maximum Durability | 4 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 3 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Stone | 1 | 2 | 100% |

### Stone Design Notes

The water variants are separate Harvestable types because they have dedicated environment sprites and their own authored durability/yield values. They still preserve the same physical harvesting language as land Stone.

Size remains the strongest balancing factor:

- Large -> highest durability and yield.
- Normal -> middle durability and yield.
- Small -> lowest durability and yield.

`StoneSmall_0` and `StoneSmall_1` remain separate because the art provides two distinct small Stone resources and the design now gives them different durability/yield profiles.

---

## 6. Metal Nodes

### Copper Node

Copper is the easiest and most abundant metal node.

| Property | Value |
|---|---:|
| Maximum Durability | 8 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 3 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Copper Chunks | 2 | 5 | 100% |

### Iron Node

Iron is the toughest common structural metal node.

| Property | Value |
|---|---:|
| Maximum Durability | 10 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 3 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Iron Chunks | 2 | 4 | 100% |

### Gold Node

Gold is deliberately low-yield and slower to extract.

| Property | Value |
|---|---:|
| Maximum Durability | 9 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 2 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Gold Chunks | 1 | 2 | 100% |

---

## 7. Crystal Nodes

### Aquamarine Node

Baseline Crystal Node.

| Property | Value |
|---|---:|
| Maximum Durability | 8 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 3 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Aquamarine Chunks | 1 | 3 | 100% |

### Amber Node

Amber is comparatively brittle and allows Pierce fallback.

| Property | Value |
|---|---:|
| Maximum Durability | 7 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 1 |
| Blunt | 3 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Amber Chunks | 2 | 4 | 100% |

### Ruby Node

Ruby is the hardest Crystal Node and low-yield.

| Property | Value |
|---|---:|
| Maximum Durability | 10 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 0 |
| Blunt | 2 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Ruby Chunks | 1 | 2 | 100% |

### Emerald Node

Emerald rejects Chop but allows Pierce fallback.

| Property | Value |
|---|---:|
| Maximum Durability | 9 |
| Slash | 0 |
| Chop | 0 |
| Pierce | 1 |
| Blunt | 3 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Emerald Chunks | 1 | 3 | 100% |

### Amethyst Node

Amethyst allows both Chop and Pierce fallback.

| Property | Value |
|---|---:|
| Maximum Durability | 8 |
| Slash | 0 |
| Chop | 1 |
| Pierce | 1 |
| Blunt | 3 |

| Drop | Min | Max | Chance |
|---|---:|---:|---:|
| Amethyst Chunks | 1 | 3 | 100% |

---

## 8. Complete Drop Table

| Harvestable | Variant | Max Durability | Drop | Min | Max | Chance |
|---|---|---:|---|---:|---:|---:|
| Oak Tree | Normal | 7 | Wood | 2 | 4 | 100% |
| Oak Tree | Normal | 7 | Twig | 1 | 2 | 55% |
| Oak Tree | Small | 5 | Wood | 1 | 2 | 70% |
| Oak Tree | Small | 5 | Twig | 2 | 4 | 100% |
| Birch Tree | Normal | 6 | Wood | 1 | 2 | 85% |
| Birch Tree | Normal | 6 | Twig | 2 | 4 | 100% |
| Birch Tree | Small | 4 | Wood | 1 | 1 | 45% |
| Birch Tree | Small | 4 | Twig | 3 | 5 | 100% |
| Spruce Tree | Normal | 8 | Wood | 3 | 5 | 100% |
| Spruce Tree | Normal | 8 | Twig | 1 | 2 | 40% |
| Spruce Tree | Small | 6 | Wood | 2 | 3 | 80% |
| Spruce Tree | Small | 6 | Twig | 1 | 3 | 75% |
| Apple Tree | — | 7 | Apple | 2 | 4 | 100% |
| Apple Tree | — | 7 | Twig | 1 | 2 | 25% |
| Cherry Tree | — | 5 | Cherry | 3 | 6 | 100% |
| Cherry Tree | — | 5 | Twig | 1 | 2 | 30% |
| Peach Tree | — | 6 | Peach | 1 | 3 | 100% |
| Peach Tree | — | 6 | Twig | 1 | 2 | 20% |
| Pear Tree | — | 7 | Pear | 2 | 4 | 100% |
| Pear Tree | — | 7 | Twig | 1 | 2 | 25% |
| Stone Large | Land | 10 | Stone | 4 | 7 | 100% |
| Stone Normal | Land | 7 | Stone | 2 | 4 | 100% |
| Stone Small 0 | Land | 4 | Stone | 1 | 2 | 100% |
| Stone Small 1 | Land | 5 | Stone | 2 | 3 | 100% |
| Stone Water Large | Water | 9 | Stone | 3 | 6 | 100% |
| Stone Water Normal | Water | 6 | Stone | 2 | 4 | 100% |
| Stone Water Small | Water | 4 | Stone | 1 | 2 | 100% |
| Copper Node | — | 8 | Copper Chunks | 2 | 5 | 100% |
| Iron Node | — | 10 | Iron Chunks | 2 | 4 | 100% |
| Gold Node | — | 9 | Gold Chunks | 1 | 2 | 100% |
| Aquamarine Node | — | 8 | Aquamarine Chunks | 1 | 3 | 100% |
| Amber Node | — | 7 | Amber Chunks | 2 | 4 | 100% |
| Ruby Node | — | 10 | Ruby Chunks | 1 | 2 | 100% |
| Emerald Node | — | 9 | Emerald Chunks | 1 | 3 | 100% |
| Amethyst Node | — | 8 | Amethyst Chunks | 1 | 3 | 100% |

---

## 9. Greenvale Border Path

Current intended Harvestables:

| Harvestable | Variant | Max Durability | Drops |
|---|---|---:|---|
| Oak Tree | Normal / Small | 7 / 5 | Wood, Twig |
| Cherry Tree | Presentation variant | 5 | Cherry, optional Twig |

This keeps the tutorial focused on:

- `Oak Tree -> crafting Materials`
- `Cherry Tree -> Consumable Item`
