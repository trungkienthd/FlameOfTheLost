using System.Collections.Generic;

namespace FlameOfTheLost
{
    public abstract class NeutralCharacterDefaultSpawnPointController : CharacterAIDefaultSpawnPointController
    {
        // --- Data / Configs ---
        //  1. Neutral Character Spawn Entries
        public abstract IReadOnlyList<NeutralCharacterSpawnEntry> NeutralCharacterSpawnEntries
        {
            get;
        }

        public sealed override IReadOnlyList<CharacterAISpawnEntry> CharacterAISpawnEntries => this.NeutralCharacterSpawnEntries;
    }
}