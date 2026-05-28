namespace FlameOfTheLost
{
    public static class StatusEffectStatModifierUtility
    {
        // Validation helpers (editor-time suggested, but safe at runtime)
        public static bool ValidateOperation(StatusEffectStatModifierTargetType target, StatusEffectStatModifierOperationType operation)
        {
            switch (target)
            {
                case StatusEffectStatModifierTargetType.VirtualHPGranted:
                case StatusEffectStatModifierTargetType.StaminaRegenDelayDelta:
                    return operation == StatusEffectStatModifierOperationType.AddFlat;

                case StatusEffectStatModifierTargetType.MovementSpeedScale:
                    return operation == StatusEffectStatModifierOperationType.Multiply;

                case StatusEffectStatModifierTargetType.DamageReceived:
                case StatusEffectStatModifierTargetType.DamageDealt:
                case StatusEffectStatModifierTargetType.KnockbackReceived:
                case StatusEffectStatModifierTargetType.KnockbackDealt:
                case StatusEffectStatModifierTargetType.FPCost:
                case StatusEffectStatModifierTargetType.StaminaCost:
                case StatusEffectStatModifierTargetType.StaminaRegenRate:
                    return operation == StatusEffectStatModifierOperationType.AddFlat || operation == StatusEffectStatModifierOperationType.Multiply;

                default:
                    return false;
            }
        }
    }
}
