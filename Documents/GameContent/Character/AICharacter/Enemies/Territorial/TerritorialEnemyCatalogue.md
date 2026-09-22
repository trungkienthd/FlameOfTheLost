# Territorial Enemy Catalogue

## Purpose

This document records the current authored **Territorial Enemy variants** and their Territorial AI configuration.

Current variants:

- Feralshroom Basher
- Feralshroom Toxic
- Aquabara Pond
- Aquabara Ocean

---

## 1. Feralshroom Variants

Both variants share the base Enemy identity:

`10101_Chr_Enemy_Feralshroom`

### Feralshroom Basher

Combat identity:

```text
Feralshroom Hardened Cap
→ Cap Bash
```

#### Roaming

| Property | Value |
|---|---:|
| Min Walk Duration | 1.2s |
| Max Walk Duration | 2.5s |
| Min Idle Duration | 0.8s |
| Max Idle Duration | 1.8s |

#### Attack

| Property | Value |
|---|---:|
| Ideal Attack Range | 0.75 |
| Pre-Attack Duration | 0.45s |
| Attack Decision Cooldown | 3.5s |
| Attacking State Duration | 0.75s |
| Normal Attack Side | Left |

Gameplay identity:

- close-range Territorial Feralshroom;
- approaches closely before initiating its normal attack;
- slower attack decision cycle than Toxic.

---

### Feralshroom Toxic

Combat identity:

```text
Feralshroom Toxic Sac
→ Toxic Puff
→ Toxic Gas projectile
```

#### Roaming

| Property | Value |
|---|---:|
| Min Walk Duration | 1.2s |
| Max Walk Duration | 2.5s |
| Min Idle Duration | 0.8s |
| Max Idle Duration | 1.8s |

#### Attack

| Property | Value |
|---|---:|
| Ideal Attack Range | 1.8 |
| Pre-Attack Duration | 0.45s |
| Attack Decision Cooldown | 3.0s |
| Attacking State Duration | 0.7s |
| Normal Attack Side | Left |

Gameplay identity:

- ranged-oriented Territorial Feralshroom;
- shares Basher roaming behaviour;
- begins attacking from farther away;
- slightly faster attack decision cycle than Basher.

---

## 2. Aquabara Variants

Both variants share the base Enemy identity:

`10301_Chr_Enemy_Aquabara`

Both currently share the same combat package:

```text
Aquabara Water Sac
→ Water Glob
→ Water Glob projectile
```

Their current distinction is primarily Territorial AI tuning.

### Aquabara Pond

#### Roaming

| Property | Value |
|---|---:|
| Min Walk Duration | 0.75s |
| Max Walk Duration | 1.5s |
| Min Idle Duration | 1.5s |
| Max Idle Duration | 2.0s |

#### Attack

| Property | Value |
|---|---:|
| Ideal Attack Range | 9 |
| Pre-Attack Duration | 0.55s |
| Attack Decision Cooldown | 3.5s |
| Attacking State Duration | 0.8s |
| Normal Attack Side | Left |

Gameplay identity:

- calmer/slower current Aquabara variant;
- shorter roam windows than Ocean;
- slower attack decision cycle;
- slightly shorter preferred attack range.

---

### Aquabara Ocean

#### Roaming

| Property | Value |
|---|---:|
| Min Walk Duration | 0.75s |
| Max Walk Duration | 2.0s |
| Min Idle Duration | 1.5s |
| Max Idle Duration | 3.0s |

#### Attack

| Property | Value |
|---|---:|
| Ideal Attack Range | 10 |
| Pre-Attack Duration | 0.55s |
| Attack Decision Cooldown | 2.5s |
| Attacking State Duration | 0.7s |
| Normal Attack Side | Left |

Gameplay identity:

- more aggressive current Aquabara variant;
- broader roaming duration ranges;
- longer preferred attack range;
- faster attack decision cycle.

---

## 3. Variant Comparison

| Variant | Combat Identity | Ideal Range | Pre-Attack | Cooldown | Attack State |
|---|---|---:|---:|---:|---:|
| Feralshroom Basher | Cap Bash | 0.75 | 0.45s | 3.5s | 0.75s |
| Feralshroom Toxic | Toxic Puff | 1.8 | 0.45s | 3.0s | 0.7s |
| Aquabara Pond | Water Glob | 9 | 0.55s | 3.5s | 0.8s |
| Aquabara Ocean | Water Glob | 10 | 0.55s | 2.5s | 0.7s |

---

## 4. Shared / Generic Character Configuration

The variants above should not receive duplicate documentation for generic Character systems that remain on the common/default configuration.

Examples include:

- general Combat configuration;
- Dodge;
- other unchanged Character subsystems.

Only Enemy- or variant-specific authored differences belong in this catalogue.

---

## 5. Content Rules

1. Feralshroom Basher and Toxic are variants of one Feralshroom Enemy identity.
2. Aquabara Pond and Ocean are variants of one Aquabara Enemy identity.
3. Different Territorial tuning alone does not create a new base Enemy identity.
4. Shared Weapon/Skill identities may be reused between variants.
5. Variant comparisons should focus on meaningful authored differences.
6. Concrete `.asset` data remains authoritative if later tuning changes.
