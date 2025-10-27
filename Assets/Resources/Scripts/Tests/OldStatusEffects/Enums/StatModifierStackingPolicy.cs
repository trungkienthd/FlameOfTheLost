namespace FlameOfTheLost
{
    // How multiple modifiers of the same type stack
    public enum StatModifierStackingPolicy
    {
        StackAll,                          // All contribute
        HighestOnly,                       // Only the highest value contributes
        LowestOnly,                        // Only the lowest value contributes
    }
}
