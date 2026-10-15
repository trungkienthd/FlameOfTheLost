using System.Collections.Generic;

namespace FlameOfTheLost
{
    public abstract class EnemyDefaultSpawnPointProfile<T> : CharacterAIDefaultSpawnPointProfile<T> 
        where T : EnemySpawnEntry
    {
        public IReadOnlyList<T> EnemySpawnEntries => this.CharacterAISpawnEntries;
    }
}