namespace FlameOfTheLost
{
    // What stat (and operation) a modifier targets
    public enum StatModifierKey
    {
        // --- Resource & costs ---
        StaminaCostMultiplier,                      // x%
        FPCostMultiplier,                           // x%

        MinStaminaReserveFlat,                      // +N (points)

        StaminaRegenPerSecondFlat,                  // +N per sec
        FPRegenPerSecondFlat,                       // +N per sec

        StaminaRegenDelaySecondsFlat,               // +N secs
        FPRegenDelaySecondsFlat,                    // +N secs

        // --- Damage model ---
        DamageTakenMultiplier,                      // x%
        DamageDealtMultiplier,                      // x%

        KnockbackTakenMultiplier,                   // x%

        // --- Health ---
        MaxHealthFlat,                              // +N (points)

        // --- Movement / Combat
        MovementSpeedMultiplier,                        // x%

        AttackSpeedMultiplier,                      // x%

        PoiseRegenPerSecondFlat,                    // +N per sec

        PoiseDamageTakenMultiplier,                 // x%
    }
}
