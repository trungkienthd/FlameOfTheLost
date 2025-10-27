namespace FlameOfTheLost
{
    // How a single modifier transforms the base value
    public enum StatModifierFormula
    {
        AddFlat,                        // base + X
        AddPercentage,                  // base * (1 + SUM_X)   (% bonuses summed first, then applied once)
        Multiply,                       // base * MULT(1 + X)   (multiplicative chain across entries)
    }
}
