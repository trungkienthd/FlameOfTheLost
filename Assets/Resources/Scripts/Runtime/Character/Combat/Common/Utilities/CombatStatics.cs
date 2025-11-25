using System;

namespace FlameOfTheLost
{
    public static class CombatStatics
    {
        // Wrap an index into [0, count). If count <= 0, returns 0.
        public static int WrapIndex(int idx, int count) => ((idx % count) + count) % count;
    }
}
