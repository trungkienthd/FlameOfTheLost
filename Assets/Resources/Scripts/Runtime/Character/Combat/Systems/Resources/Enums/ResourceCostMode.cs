namespace FlameOfTheLost
{
    public enum ResourceCostMode
    {
        OnStart,                       // Pay on skill begin
        OnHit,                         // Pay on first hit confirm
        OnEnd,                         // Pay when your existing On...End signal fires
        PerSecond                      // Channels/holds, paid continuously
    }
}
