using System.Collections.Generic;

namespace FlameOfTheLost
{
    public abstract class EnemyDefaultSpawnPointController : CharacterAIDefaultSpawnPointController
    {
        // --- Data / Configs ---
        //  1. Enemy Spawn Entries
        public abstract IReadOnlyList<EnemySpawnEntry> EnemySpawnEntries 
        {
            get;
        }

        public sealed override IReadOnlyList<CharacterAISpawnEntry> CharacterAISpawnEntries => this.EnemySpawnEntries;
    }
}
