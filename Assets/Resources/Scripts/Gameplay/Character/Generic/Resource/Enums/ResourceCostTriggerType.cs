namespace FlameOfTheLost
{
    // When the cost is applied.
    public enum ResourceCostTriggerType
    {
        OnAttackStart   = 0,                            // Paid immediately when skill begins
        OnHitConfirmed  = 1,                            // Reserved at start (strict mode), then finalized on hit confirm (your reserve model)
        OnAttackEnd     = 2,                            // Reserved at start, then finalized at end
    }
}
