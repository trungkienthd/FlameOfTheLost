using System.Collections.Generic;

namespace FlameOfTheLost
{
    public abstract class NeutralCharacterDefaultSpawnPointProfile<T> : CharacterAIDefaultSpawnPointProfile<T>
        where T : NeutralCharacterSpawnEntry
    {
        public IReadOnlyList<T> NeutralCharacterSpawnEntries => this.CharacterAISpawnEntries;
    }
}