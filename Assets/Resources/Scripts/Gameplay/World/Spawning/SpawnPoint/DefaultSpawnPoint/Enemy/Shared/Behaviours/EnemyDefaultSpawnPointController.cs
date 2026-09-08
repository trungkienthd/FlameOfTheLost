using System.Collections.Generic;

namespace FlameOfTheLost
{
    public abstract class EnemyDefaultSpawnPointController : CharacterDefaultSpawnPointController
    {
        // --- Data / Configs ---
        //  1. Enemy Spawn Entries
        public abstract IReadOnlyList<EnemySpawnEntry> EnemySpawnEntries { get; }
    }
}
