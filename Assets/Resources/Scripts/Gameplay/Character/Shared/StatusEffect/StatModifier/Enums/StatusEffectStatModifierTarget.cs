namespace FlameOfTheLost
{
    public enum StatusEffectStatModifierTarget
    {
        // Intake (defender)
        DamageReceived,            // This does not include HP cost
        KnockbackReceived,            
        VirtualHPGranted,

        // Output (attacker)
        DamageDealt,
        KnockbackDealt,

        // Movement
        MovementSpeedScale,

        // Resources
        HPCost,  
        FPCost,
        StaminaCost,
        StaminaRegenRate,
        StaminaRegenDelayDelta,
    }
}
