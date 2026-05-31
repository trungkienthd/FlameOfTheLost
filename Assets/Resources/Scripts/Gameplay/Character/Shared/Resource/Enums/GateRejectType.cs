namespace FlameOfTheLost
{
    public enum GateRejectType
    {
        None = 0,

        Silenced = 1,

        NotEnoughHP = 10,
        NotEnoughFP = 11,
        NotEnoughStamina = 12,

        // Strict reserve rule: even if you have stamina now, you must keep MinStaminaReserve.
        ReserveWouldBreakMinStamina = 20,

        // HP cost would reduce HP to <= 0 while AllowSelfKillWithHPCost is false.
        WouldSelfKill = 30,

        Unknown = 40
    }
}
