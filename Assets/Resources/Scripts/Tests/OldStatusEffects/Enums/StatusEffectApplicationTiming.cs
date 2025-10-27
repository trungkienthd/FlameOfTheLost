namespace FlameOfTheLost
{
    public enum StatusEffectApplicationTiming
    {
        OnMeleeHitConfirm,              // from MeleeHitboxHandler a hit lands
        OnProjectileImpact,             // from projectile impact script
        OnAreaOfEffectTick,             // for future Area of Effect status effects-related skills
        OnBuffStart,                    // applies to self on cast
    }
}
