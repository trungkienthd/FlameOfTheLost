# Skill Gameplay Design

## Purpose

This document defines the shared gameplay and authoring model for **Skills** in **FlameOfTheLost**.

Concrete Skill identities are listed in `SkillCatalogue.md`.

The ScriptableObject assets remain authoritative for concrete per-Skill values such as damage, resource costs, requirements, Status Effect applications, harvesting configuration, and projectile parameters.

---

## 1. Skill Responsibilities

A `SkillProfile` currently owns:

1. identity;
2. icon;
3. Skill Style;
4. default Provision Type;
5. compatible Weapon Types;
6. capability requirements;
7. attack damage;
8. knockback force;
9. harvest damage configuration;
10. resource cost;
11. Status Effect applications.

`RangedSkillProfile` additionally owns projectile-specific configuration.

---

## 2. Skill Styles

Current styles:

- `Melee`
- `Ranged`

The enum also contains `Default`, but the current concrete authored Skill types use explicit Melee or Ranged styles.

### Melee

`MeleeSkillProfile` currently adds no extra serialized gameplay fields beyond the shared `SkillProfile` data. Its primary specialization is:

`Style = Melee`

### Ranged

`RangedSkillProfile` specializes Skills with:

- required projectile type;
- whether a projectile Item is consumed;
- optional/default projectile;
- projectile speed;
- projectile lifetime;
- whether the projectile is destroyed on hit;
- projectile count;
- angle step between adjacent projectiles.

---

## 3. Skill Provision

Current provision types are:

| Provision Type | Meaning |
|---|---|
| `WeaponNormal` | Fixed Normal attack defined by a Weapon |
| `WeaponInnateSpecial` | Built-in Special Skill supplied by a Weapon |
| `CharacterLearned` | Learned/unlocked by a character and then equipped onto Weapons |

The provision value stored in a Skill profile is an **authoring default/hint**. Runtime systems may override how a Skill is currently provided.

### Weapon Relationship

A Weapon has:

- one Normal Skill;
- optional Innate Special Skill.

Special Skill resolution follows:

`Active learned/equipped Special -> Weapon innate Special -> Weapon Normal`

The active Special is per-owner runtime state and should not be stored in the shared Weapon profile.

---

## 4. Weapon Compatibility

Each Skill can define a list of attachable `WeaponType` values.

Compatibility rule:

- if the compatibility list is null or empty, the Skill is treated as compatible with any Weapon Type;
- otherwise, the target Weapon Type must be explicitly present in the list.

Supported Weapon Types currently include:

- Sword
- Axe
- Dagger
- Bow
- Crossbow
- Staff
- Special

Compatibility is separate from Skill Style.

For example, `Melee` or `Ranged` describes how the Skill behaves, while `WeaponType` compatibility describes what equipment may provide/equip it.

---

## 5. Capability Requirements

Skills may define character capability requirements.

A Skill can only be used when the character progression state satisfies those requirements.

This is independent from Weapon equip requirements:

- Weapon requirements determine whether the Weapon can be equipped;
- Skill requirements determine whether the Skill can be used.

Do not merge the two checks into one content concept.

---

## 6. Attack Properties

Every Skill currently defines:

- `DamageAmount`;
- `KnockbackForce`.

These are shared by Melee and Ranged Skills.

Concrete values belong to the individual Skill assets and should not be duplicated into this design document unless a later balance/configuration document is intentionally created.

---

## 7. Harvesting Interaction

Every Skill contains a `HarvestDamageEntry`.

This keeps harvesting interaction on the attack/Skill side rather than treating harvesting as a separate input system.

A Skill may therefore participate in harvesting according to its authored harvest configuration, including attacks whose impact is ineffective against a particular Harvestable.

The Harvesting documents remain authoritative for:

- Harvestable durability;
- impact response multipliers;
- Harvestable drop configuration;
- Harvestable family roles.

Skill data remains authoritative for the attack-side harvest damage request/configuration.

---

## 8. Resource Cost

Every Skill contains a `ResourceCostEntry`.

The shared profile currently supports costs involving:

- HP;
- FP;
- Stamina;

with both flat and percentage-of-maximum fields available in the underlying entry.

The current default `SkillProfile` setup uses:

- zero default HP/FP/Stamina cost;
- cost trigger on attack start;
- silence blocking enabled;
- self-kill from HP cost disabled.

Individual Skill assets may override these values.

Do not assume every Skill has the default zero-cost configuration; inspect the authored Skill profile when concrete balance information is required.

---

## 9. Status Effect Applications

A Skill can contain zero or more `SkillStatusEffectApplicationEntry` values.

This allows Skills to apply Status Effects as part of their authored attack behaviour.

The Skill profile owns the application entries. The referenced Status Effect profile remains responsible for the Status Effect's own lifetime, modifiers, periodics, flags, VFX, and other effect behaviour.

Do not duplicate Status Effect internals into Skill data.

---

## 10. Ranged Projectile Rules

A Ranged Skill defines a required projectile classification.

### Projectile Cost

If `CostProjectile = true`:

- the Skill expects the appropriate projectile Item/resource behaviour defined by the runtime system.

If `CostProjectile = false`:

- the Ranged Skill can use its authored default projectile;
- this allows the Skill to fire even when the projectile slot is empty or mismatched.

A Ranged Skill that does not cost projectile Items should therefore define a valid default projectile.

### Default Projectile Consistency

When a default projectile exists, its `ProjectileType` should match the Skill's required projectile type.

This keeps projectile classification consistent between:

- Skill requirement;
- default projectile fallback.

### Projectile Motion

Each Ranged Skill can independently define:

- projectile speed;
- projectile lifetime;
- destroy-on-hit behaviour.

These belong to the Skill because different attacks using the same broad projectile family may still behave differently.

---

## 11. Multi-Projectile Skills

Ranged Skills support:

- `ProjectileCount`, minimum `1`;
- `ProjectileAngleStep`, minimum `0`.

`ProjectileAngleStep` is the angular gap in degrees between adjacent projectiles.

This configuration supports single-projectile and multi-projectile attacks within the same Ranged Skill model.

Current catalogue identities that clearly represent multi-shot concepts include:

- Double Shot
- Tripple Shot

Their exact authored projectile counts and angle steps should be read from the corresponding assets rather than inferred from their names.

---

## 12. Content Boundaries

### Weapon Profile owns

- Weapon Type;
- Normal Skill reference;
- optional Innate Special Skill reference;
- Weapon equip requirements.

### Skill Profile owns

- Skill identity and presentation;
- Style;
- default Provision Type;
- Weapon compatibility;
- Skill-use requirements;
- damage;
- knockback;
- harvesting attack configuration;
- resource cost;
- Status Effect applications.

### Ranged Skill Profile additionally owns

- projectile requirement;
- projectile consumption/fallback configuration;
- projectile movement/lifetime;
- projectile count/spread.

This boundary should remain clear when adding new content.

---

## 13. Authoring Rules

1. Use `SKM_` for authored Melee Skill asset identities.
2. Use `SKR_` for authored Ranged Skill asset identities.
3. Keep Skill Style aligned with the concrete profile type.
4. Set a meaningful default Provision Type.
5. Configure Weapon compatibility deliberately; an empty list means unrestricted compatibility.
6. Keep Skill-use requirements separate from Weapon equip requirements.
7. Treat the Skill asset as authoritative for concrete attack and cost values.
8. Configure harvesting on the Skill's `HarvestDamageEntry`; do not duplicate Harvestable response data there.
9. Ranged Skills that do not consume projectile Items should have a valid default projectile.
10. A default projectile's classification should match the required projectile classification.
11. Multi-projectile behaviour should be authored through projectile count and angle step rather than separate runtime special cases where the shared model is sufficient.
12. Do not infer concrete gameplay configuration solely from a Skill's display/asset name.
13. Creature-specific Skills still follow the same Skill data model unless a genuinely different system becomes necessary.

---

## 14. When Additional Skill Documents Are Needed

A separate concrete Skill configuration document would become useful if the project needs to mirror and compare large numbers of authored values such as:

- damage;
- knockback;
- FP/Stamina/HP costs;
- capability requirements;
- Weapon compatibility;
- Status Effect applications;
- projectile speed/lifetime/count/spread;
- detailed harvest damage values.

Until then:

- `SkillCatalogue.md` should remain the identity/content catalogue;
- `SkillGameplayDesign.md` should remain the shared authoring/design authority;
- the `.asset` profiles should remain authoritative for concrete values.
