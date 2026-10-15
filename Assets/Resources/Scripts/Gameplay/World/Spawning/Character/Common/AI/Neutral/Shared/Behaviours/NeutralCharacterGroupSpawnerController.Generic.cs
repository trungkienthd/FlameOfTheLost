using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class NeutralCharacterGroupSpawnerController<T> : NeutralCharacterGroupSpawnerController
        where T : NeutralCharacterSpawnEntry
    {
        // --- Events ---
        public event Action<NeutralCharacterSpawnedContext<T>> TypedNeutralCharacterSpawned;

        // --- Hooks ---
        //  1. Spawn Entry Filtering
        protected override IEnumerable<CharacterAISpawnEntry> GetSpawnEntries(CharacterAIDefaultSpawnPointController spawnPoint)
        {
            if (spawnPoint is not NeutralCharacterDefaultSpawnPointController neutralSpawnPoint) yield break;
            if (neutralSpawnPoint.NeutralCharacterSpawnEntries == null) yield break;

            for (int i = 0; i < neutralSpawnPoint.NeutralCharacterSpawnEntries.Count; i++)
            {
                NeutralCharacterSpawnEntry spawnEntry = neutralSpawnPoint.NeutralCharacterSpawnEntries[i];

                if (spawnEntry is not T typedSpawnEntry) continue;
                
                yield return typedSpawnEntry;
            }
        }

        //  2. Spawned Neutral Character Configuration
        protected sealed override void ConfigureSpawnedCharacter(GameObject spawnedCharacter, CharacterAIDefaultSpawnPointController spawnPoint, CharacterAISpawnEntry spawnEntry)
        {
            base.ConfigureSpawnedCharacter(spawnedCharacter, spawnPoint, spawnEntry);

            if (spawnPoint is not NeutralCharacterDefaultSpawnPointController neutralSpawnPoint) return;
            if (spawnEntry is not T typedSpawnEntry) return;

            this.ConfigureSpawnedNeutralCharacter(spawnedCharacter, neutralSpawnPoint, typedSpawnEntry);
        }

        protected virtual void ConfigureSpawnedNeutralCharacter(GameObject spawnedCharacter, NeutralCharacterDefaultSpawnPointController spawnPoint, T spawnEntry)
        {
        }

        //  3. Spawn Notification
        protected override void NotifyCharacterAISpawned(CharacterAIDefaultSpawnPointController spawnPoint, CharacterAISpawnEntry spawnEntry, CharacterRootController character)
        {
            base.NotifyCharacterAISpawned(spawnPoint, spawnEntry, character);

            if (spawnPoint is not NeutralCharacterDefaultSpawnPointController neutralSpawnPoint) return;
            if (spawnEntry is not T typedSpawnEntry) return;

            var neutralCharacterSpawnedContext = new NeutralCharacterSpawnedContext<T>(neutralSpawnPoint, typedSpawnEntry, character);
            this.TypedNeutralCharacterSpawned?.Invoke(neutralCharacterSpawnedContext);
        }
    }
}