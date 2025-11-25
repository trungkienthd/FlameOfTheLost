namespace FlameOfTheLost
{
    public enum GateRejectStatus
    {
        None = 0,
        OnCooldown,
        Silenced,
        NotEnoughStamina,
        NotEnoughFP,
        ReserveWouldBreakMinStamina,
        Unknown
    }
}
