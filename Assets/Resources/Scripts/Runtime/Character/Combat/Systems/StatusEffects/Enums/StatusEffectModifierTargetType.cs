namespace FlameOfTheLost
{
    public enum StatusEffectModifierTargetType
    {
        // Intake (defender)
        DamageTaken,
        KnockbackTaken,
        MaxHealthDelta,

        // Output (attacker)
        DamageDealt,
        CooldownScale,

        // Movement
        MovementSpeedScale,

        // Resources
        AbilityCostScaleStamina,
        AbilityCostScaleFP,
        StaminaRegenRateDelta,
        FPRegenRateDelta,
        StaminaRegenDelayDelta,
        FPRegenDelayDelta,
    }
}
