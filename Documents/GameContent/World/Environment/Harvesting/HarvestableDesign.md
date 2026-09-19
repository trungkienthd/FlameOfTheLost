# Harvestable Design

## Purpose
This document is the authoritative catalogue of Harvestable Objects in **FlameOfTheLost**.

## 1. Core Rules
1. A sprite becomes its own Harvestable type when the design gives it distinct gameplay configuration.
2. Oak, Birch, and Spruce keep `Normal` / `Small` variants under one core tree identity.
3. The seven Stone sprites are seven separate Stone-like Harvestables.
4. Each Metal / Crystal family has four separate node types:
   - `Large`
   - `Normal`
   - `Small`
   - `Rich`
5. `Rich` is the working name for the fourth brighter / shard-like node sprite.
6. Mineral Nodes always drop `Chunks` and may also drop `Ore` / `Crystal`.
7. `Bar` and `Block` are processing-only and never normal direct Harvestable drops.
8. Each drop entry independently defines min quantity, max quantity, and chance.

### Resource Progression
Metals: `Node -> Chunks + optional Ore`, then `Chunks -> Ore -> Bar`

Crystals: `Node -> Chunks + optional Crystal`, then `Chunks -> Crystal -> Block`

---

## 2. Catalogue Summary

| Category | Core Harvestables |
|---|---:|
| Timber Trees | 3 |
| Fruit Trees | 4 |
| Stone-like Harvestables | 7 |
| Metal Nodes | 12 |
| Crystal Nodes | 20 |
| **Total** | **46** |

### Mineral Families
Each of Copper, Iron, Gold, Aquamarine, Amber, Ruby, Emerald, and Amethyst has:
- Large Node
- Normal Node
- Small Node
- Rich Node

---

## 3. Timber Trees

All Timber Trees: `Slash 1 / Chop 3 / Pierce 1 / Blunt 1`.

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

---

## 4. Fruit Trees

| Harvestable | Max Durability | Slash | Chop | Pierce | Blunt | Primary Drop | Qty | Chance |
|---|---:|---:|---:|---:|---:|---|---|---:|
| Apple Tree | 7 | 2 | 3 | 1 | 1 | Apple | 2–4 | 100% |
| Cherry Tree | 5 | 3 | 2 | 1 | 1 | Cherry | 3–6 | 100% |
| Peach Tree | 6 | 2 | 3 | 1 | 1 | Peach | 1–3 | 100% |
| Pear Tree | 7 | 2 | 2 | 1 | 1 | Pear | 2–4 | 100% |

Secondary Twig drops:
- Apple: `1–2 @ 25%`
- Cherry: `1–2 @ 30%`
- Peach: `1–2 @ 20%`
- Pear: `1–2 @ 25%`

---

## 5. Stone Resources

All Stones: `Slash 0 / Chop 1 / Pierce 0 / Blunt 3`.

| Harvestable | Environment | Max Durability | Stone Qty | Chance |
|---|---|---:|---|---:|
| Stone Large | Land | 10 | 4–7 | 100% |
| Stone Normal | Land | 7 | 2–4 | 100% |
| Stone Small 0 | Land | 4 | 1–2 | 100% |
| Stone Small 1 | Land | 5 | 1–3 | 100% |
| Stone Water Large | Water | 9 | 3–6 | 100% |
| Stone Water Normal | Water | 6 | 2–4 | 100% |
| Stone Water Small | Water | 4 | 1–2 | 100% |

---

## 6. Metal Nodes

### Copper
Impact: `Slash 0 / Chop 1 / Pierce 0 / Blunt 3`

| Node | Max Durability | Chunks | Chance | Ore | Chance |
|---|---:|---|---:|---|---:|
| Large | 11 | 4–7 | 100% | 1–2 | 35% |
| Normal | 8 | 2–5 | 100% | 1 | 20% |
| Small | 5 | 1–3 | 100% | 1 | 8% |
| Rich | 9 | 3–5 | 100% | 1–3 | 60% |

### Iron
Impact: `Slash 0 / Chop 1 / Pierce 0 / Blunt 3`

| Node | Max Durability | Chunks | Chance | Ore | Chance |
|---|---:|---|---:|---|---:|
| Large | 14 | 4–6 | 100% | 1–2 | 30% |
| Normal | 10 | 2–4 | 100% | 1 | 15% |
| Small | 6 | 1–2 | 100% | 1 | 5% |
| Rich | 12 | 3–5 | 100% | 1–3 | 55% |

### Gold
Impact: `Slash 0 / Chop 1 / Pierce 0 / Blunt 2`

| Node | Max Durability | Chunks | Chance | Ore | Chance |
|---|---:|---|---:|---|---:|
| Large | 13 | 2–4 | 100% | 1–2 | 25% |
| Normal | 9 | 1–2 | 100% | 1 | 12% |
| Small | 6 | 1 | 100% | 1 | 5% |
| Rich | 11 | 2–3 | 100% | 1–2 | 45% |

Bars are never direct drops.

---

## 7. Crystal Nodes

### Aquamarine
Impact: `Slash 0 / Chop 1 / Pierce 0 / Blunt 3`

| Node | Max Durability | Chunks | Chance | Crystal | Chance |
|---|---:|---|---:|---|---:|
| Large | 11 | 3–6 | 100% | 1–2 | 25% |
| Normal | 8 | 1–3 | 100% | 1 | 12% |
| Small | 5 | 1–2 | 100% | 1 | 5% |
| Rich | 9 | 2–4 | 100% | 1–3 | 50% |

### Amber
Impact: `Slash 0 / Chop 1 / Pierce 1 / Blunt 3`

| Node | Max Durability | Chunks | Chance | Crystal | Chance |
|---|---:|---|---:|---|---:|
| Large | 10 | 4–7 | 100% | 1–2 | 30% |
| Normal | 7 | 2–4 | 100% | 1 | 15% |
| Small | 4 | 1–3 | 100% | 1 | 7% |
| Rich | 8 | 3–5 | 100% | 1–3 | 55% |

### Ruby
Impact: `Slash 0 / Chop 1 / Pierce 0 / Blunt 2`

| Node | Max Durability | Chunks | Chance | Crystal | Chance |
|---|---:|---|---:|---|---:|
| Large | 15 | 2–4 | 100% | 1–2 | 20% |
| Normal | 10 | 1–2 | 100% | 1 | 10% |
| Small | 7 | 1 | 100% | 1 | 4% |
| Rich | 13 | 2–3 | 100% | 1–2 | 40% |

### Emerald
Impact: `Slash 0 / Chop 0 / Pierce 1 / Blunt 3`

| Node | Max Durability | Chunks | Chance | Crystal | Chance |
|---|---:|---|---:|---|---:|
| Large | 12 | 3–5 | 100% | 1–2 | 25% |
| Normal | 9 | 1–3 | 100% | 1 | 12% |
| Small | 6 | 1–2 | 100% | 1 | 5% |
| Rich | 10 | 2–4 | 100% | 1–3 | 50% |

### Amethyst
Impact: `Slash 0 / Chop 1 / Pierce 1 / Blunt 3`

| Node | Max Durability | Chunks | Chance | Crystal | Chance |
|---|---:|---|---:|---|---:|
| Large | 11 | 3–6 | 100% | 1–2 | 25% |
| Normal | 8 | 1–3 | 100% | 1 | 12% |
| Small | 5 | 1–2 | 100% | 1 | 5% |
| Rich | 9 | 2–4 | 100% | 1–3 | 50% |

Blocks are never direct drops.

---

## 8. Mineral Variant Roles

- **Large** — highest mass, high durability, highest Chunks quantity.
- **Normal** — baseline world node.
- **Small** — lowest durability and yield.
- **Rich** — compact high-quality deposit with the highest Ore / Crystal secondary-drop chance.

---

## 9. Greenvale Border Path

Current intended Harvestables remain:
- Oak Tree
- Cherry Tree

Stone and Mineral Nodes can be introduced later when the game teaches mining-oriented harvesting.
