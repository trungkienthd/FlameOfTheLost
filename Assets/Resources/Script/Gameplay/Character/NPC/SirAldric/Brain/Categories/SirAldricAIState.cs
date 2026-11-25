namespace FlameOfTheLost
{
    public enum SirAldricAIState
    {
        None = 0,

        // Non-combat
        Imprisoned = 10,
        WaitingForPlayer = 11,
        Following = 12,
        Regrouping = 13,
        Conversing = 14,
        AssistingPuzzle = 15,
        Resting = 16,

        // Combat
        Alert = 20,
        Chasing = 21,
        Positioning = 22,
        NormalAttacking = 23,
        SpecialAttacking = 24,
        Recovering = 25,
        Retreating = 26,

        // Hostile
        HostileAlert = 30,

        // Terminal
        Dead = 100,
    }
}
