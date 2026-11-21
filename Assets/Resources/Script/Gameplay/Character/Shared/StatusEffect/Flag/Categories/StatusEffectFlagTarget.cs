namespace FlameOfTheLost
{
    // Binary / rule-based effects (capability locks & immunities)
    public enum StatusEffectFlagTarget
    {
        None = 0,

        // --- Control / locks
        CannotMove              = 10,
        CannotDodge             = 11,
        CannotAttack            = 12,           // Including both normal and special attacks
        CannotCastSpecialSkill  = 13,           // Including special attacks only
        CannotSwapLoadout       = 14,

        // --- Immunities
        IgnoreDamageReceived    = 30,           // "damage receiving immune"
        IgnoreKnockbackReceived = 31,
        IgnoreMovementLock      = 32,
        IgnoreDodgeLock         = 33,
        IgnoreAttackLock        = 34,
        IgnoreSwapLoadoutLock   = 35,

        IgnoreAllStatusEffects      = 40,       // Remove the existing status effects, and immune to new status effects being applied
        IgnoreDebuffStatusEffects   = 41,       // Immune to debuffs being applied
        IgnoreBuffStatusEffects     = 42,       // Immune to buffs being applied
    }
}
