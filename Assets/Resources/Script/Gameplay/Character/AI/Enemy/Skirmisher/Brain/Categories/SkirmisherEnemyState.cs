namespace FlameOfTheLost
{
    public enum SkirmisherEnemyState
    {
        Idle              = 0,
        Wandering         = 1,

        Alert             = 2,
        Chasing           = 3,
        Strafing          = 4,

        NormalAttacking   = 5,
        SpecialAttacking  = 6,
        Recovering        = 7,

        Retreating        = 8,
        Returning         = 9,

        Dead              = 10,
    }
}