namespace FlameOfTheLost
{
    public enum AnimalState
    {
        // --- Passive ---
        Idle = 0,
        Wandering = 1,
        Graze = 2,

        // --- Rest Sequence ---
        LieDown = 3,
        Rest = 4,
        Sleep = 5,
        StandUp = 6,

        // --- Awareness ---
        Alert = 7,
        Fleeing = 8,

        // --- Terminal ---
        Dead = 9,
    }
}