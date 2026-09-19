# Harvesting Impact Configuration

## Purpose
Defines skill-side harvest damage and the durability / impact-response configuration for every Harvestable family.

## 1. Standard Harvest Damage
- `Combat Damage Multiplier = 0`
- `Flat Harvest Damage = 1`
- `Minimum Harvest Damage = 1`
- `Trigger Self On Hit Status Effects = false`

## 2. Impact Types
- `Slash`
- `Chop`
- `Pierce`
- `Blunt`
- `None`

## 3. Response Tiers
| Multiplier | Meaning |
|---:|---|
| 0 | No durability damage |
| 1 | Weak / fallback |
| 2 | Effective |
| 3 | Preferred |

Use `Default Impact Damage Multiplier = 0`.

## 4. Trees

### Timber Trees
| Harvestable | Variant | Durability | Slash | Chop | Pierce | Blunt |
|---|---|---:|---:|---:|---:|---:|
| Oak | Normal | 7 | 1 | 3 | 1 | 1 |
| Oak | Small | 5 | 1 | 3 | 1 | 1 |
| Birch | Normal | 6 | 1 | 3 | 1 | 1 |
| Birch | Small | 4 | 1 | 3 | 1 | 1 |
| Spruce | Normal | 8 | 1 | 3 | 1 | 1 |
| Spruce | Small | 6 | 1 | 3 | 1 | 1 |

### Fruit Trees
| Harvestable | Durability | Slash | Chop | Pierce | Blunt |
|---|---:|---:|---:|---:|---:|
| Apple | 7 | 2 | 3 | 1 | 1 |
| Cherry | 5 | 3 | 2 | 1 | 1 |
| Peach | 6 | 2 | 3 | 1 | 1 |
| Pear | 7 | 2 | 2 | 1 | 1 |

## 5. Stones
| Harvestable | Durability | Slash | Chop | Pierce | Blunt |
|---|---:|---:|---:|---:|---:|
| Stone Large | 10 | 0 | 1 | 0 | 3 |
| Stone Normal | 7 | 0 | 1 | 0 | 3 |
| Stone Small 0 | 4 | 0 | 1 | 0 | 3 |
| Stone Small 1 | 5 | 0 | 1 | 0 | 3 |
| Stone Water Large | 9 | 0 | 1 | 0 | 3 |
| Stone Water Normal | 6 | 0 | 1 | 0 | 3 |
| Stone Water Small | 4 | 0 | 1 | 0 | 3 |

## 6. Metal Nodes
| Harvestable | Durability | Slash | Chop | Pierce | Blunt |
|---|---:|---:|---:|---:|---:|
| Copper Large | 11 | 0 | 1 | 0 | 3 |
| Copper Normal | 8 | 0 | 1 | 0 | 3 |
| Copper Small | 5 | 0 | 1 | 0 | 3 |
| Copper Rich | 9 | 0 | 1 | 0 | 3 |
| Iron Large | 14 | 0 | 1 | 0 | 3 |
| Iron Normal | 10 | 0 | 1 | 0 | 3 |
| Iron Small | 6 | 0 | 1 | 0 | 3 |
| Iron Rich | 12 | 0 | 1 | 0 | 3 |
| Gold Large | 13 | 0 | 1 | 0 | 2 |
| Gold Normal | 9 | 0 | 1 | 0 | 2 |
| Gold Small | 6 | 0 | 1 | 0 | 2 |
| Gold Rich | 11 | 0 | 1 | 0 | 2 |

## 7. Crystal Nodes
| Harvestable | Durability | Slash | Chop | Pierce | Blunt |
|---|---:|---:|---:|---:|---:|
| Aquamarine Large | 11 | 0 | 1 | 0 | 3 |
| Aquamarine Normal | 8 | 0 | 1 | 0 | 3 |
| Aquamarine Small | 5 | 0 | 1 | 0 | 3 |
| Aquamarine Rich | 9 | 0 | 1 | 0 | 3 |
| Amber Large | 10 | 0 | 1 | 1 | 3 |
| Amber Normal | 7 | 0 | 1 | 1 | 3 |
| Amber Small | 4 | 0 | 1 | 1 | 3 |
| Amber Rich | 8 | 0 | 1 | 1 | 3 |
| Ruby Large | 15 | 0 | 1 | 0 | 2 |
| Ruby Normal | 10 | 0 | 1 | 0 | 2 |
| Ruby Small | 7 | 0 | 1 | 0 | 2 |
| Ruby Rich | 13 | 0 | 1 | 0 | 2 |
| Emerald Large | 12 | 0 | 0 | 1 | 3 |
| Emerald Normal | 9 | 0 | 0 | 1 | 3 |
| Emerald Small | 6 | 0 | 0 | 1 | 3 |
| Emerald Rich | 10 | 0 | 0 | 1 | 3 |
| Amethyst Large | 11 | 0 | 1 | 1 | 3 |
| Amethyst Normal | 8 | 0 | 1 | 1 | 3 |
| Amethyst Small | 5 | 0 | 1 | 1 | 3 |
| Amethyst Rich | 9 | 0 | 1 | 1 | 3 |

## 8. Mineral Variant Roles
- Large: high durability, highest Chunks quantity.
- Normal: baseline node.
- Small: lowest durability and yield.
- Rich: best Ore / Crystal secondary-drop chance.

## 9. Authoring Rules
1. Set `Default Impact Damage Multiplier = 0`.
2. Explicitly configure all four impact types.
3. Use `0` for impacts that should not damage durability.
4. Mineral Nodes guarantee Chunks.
5. Ore / Crystal is optional secondary loot.
6. Bar / Block must never be normal direct Harvestable loot.
