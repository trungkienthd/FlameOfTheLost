
namespace FlameOfTheLost
{
    // How reapplications behave
    public enum StatusEffectStackingPolicy
    {
        RefreshDuration,                    // Keep 1 stack; Refresh timer
        AdditiveStacks,                     // Add stacks up to MaxStacks; Usually refresh timer 
        StrongestOnly                       // Keep strongest potency; Refresh if stronger or equal
    }
}
