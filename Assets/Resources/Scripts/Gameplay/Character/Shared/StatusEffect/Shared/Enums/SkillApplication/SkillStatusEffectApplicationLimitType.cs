namespace FlameOfTheLost
{
    public enum SkillStatusEffectApplicationLimitType
    {
        EveryValidHit = 0,          // apply on every confirmed hit (subject to hit filtering)
        FirstHitOnly = 1,           // apply only on first confirmed hit of the attack instance
        OncePerTargetPerAttack = 2, // apply at most once per target per attack instance
    }
}
