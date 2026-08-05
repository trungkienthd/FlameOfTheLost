using System;

namespace FlameOfTheLost
{
    [Flags]
    public enum HarvestImpactType
    {
        None = 0,
        Cut = 1 << 0,
        Chop = 1 << 1,
        Smash = 1 << 2
    }
}
