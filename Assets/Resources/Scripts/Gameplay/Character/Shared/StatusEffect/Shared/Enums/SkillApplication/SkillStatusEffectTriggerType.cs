namespace FlameOfTheLost
{
    public enum SkillStatusEffectTriggerType
    {
        OnAttackStart   = 0,                            // Triggered at the beginning of the attack
        OnHitConfirmed  = 1,                            // Triggered when the hit (of the hitbox) is confirmed
        OnAttackEnd     = 2,                            // Triggered at the end of the attack
    }
}
