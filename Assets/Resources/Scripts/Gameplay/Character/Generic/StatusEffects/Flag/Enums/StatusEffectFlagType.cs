namespace FlameOfTheLost
{
    // Binary / rule-based effects (capability locks & immunities)
    public enum StatusEffectFlagType
    {
        None = 0,

        // --- Control / locks
        CannotMove              = 10,
        CannotAttack            = 11,           // Including both normal and special attacks
        CannotCastSpecialSkill  = 12,           // Including special attacks only
        CannotSwapLoadout       = 13,

        // --- Immunities
        IgnoreDamageReceived    = 30,           // "damage receiving immune"
        IgnoreKnockbackReceived = 31,
        IgnoreMovementLock      = 32,
        IgnoreAttackLock        = 33,
        IgnoreSwapLoadoutLock   = 34,

        IgnoreAllStatusEffects      = 40,       // Remove the existing status effects, and immune to new status effects being applied
        IgnoreDebuffStatusEffects   = 41,       // Immune to debuffs being applied
        IgnoreBuffStatusEffects     = 42,       // Immune to buffs being applied
    }
}
