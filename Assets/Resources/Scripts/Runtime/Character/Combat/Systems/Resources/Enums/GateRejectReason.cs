namespace FlameOfTheLost
{
    public enum GateRejectReason
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
