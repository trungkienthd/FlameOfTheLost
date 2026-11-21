namespace FlameOfTheLost
{
    public static class StatusEffectStatModifierUtility
    {
        // Validation helpers (editor-time suggested, but safe at runtime)
        public static bool ValidateOperation(StatusEffectStatModifierTarget target, StatusEffectStatModifierOperationType operation)
        {
            switch (target)
            {
                case StatusEffectStatModifierTarget.VirtualHPGranted:
                case StatusEffectStatModifierTarget.StaminaRegenDelayDelta:
                    return operation == StatusEffectStatModifierOperationType.AddFlat;

                case StatusEffectStatModifierTarget.MovementSpeedScale:
                    return operation == StatusEffectStatModifierOperationType.Multiply;

                case StatusEffectStatModifierTarget.DamageReceived:
                case StatusEffectStatModifierTarget.DamageDealt:
                case StatusEffectStatModifierTarget.KnockbackReceived:
                case StatusEffectStatModifierTarget.KnockbackDealt:
                case StatusEffectStatModifierTarget.FPCost:
                case StatusEffectStatModifierTarget.StaminaCost:
                case StatusEffectStatModifierTarget.StaminaRegenRate:
                    return operation == StatusEffectStatModifierOperationType.AddFlat || operation == StatusEffectStatModifierOperationType.Multiply;

                default:
                    return false;
            }
        }
    }
}
