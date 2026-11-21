using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EnemyGroupSpawnerController<T> : EnemyGroupSpawnerController
        where T : EnemySpawnEntry
    {
        // --- Events ---
        public event Action<EnemySpawnedContext<T>> TypedEnemySpawned;

        // --- Helpers ---
        //  1. Spawn Entry Filtering
        protected override IEnumerable<CharacterAISpawnEntry> GetSpawnEntries(CharacterAIDefaultSpawnPointController spawnPoint)
        {
            if (spawnPoint is not EnemyDefaultSpawnPointController enemySpawnPoint) yield break;
            if (enemySpawnPoint.EnemySpawnEntries == null) yield break;

            for (int i = 0; i < enemySpawnPoint.EnemySpawnEntries.Count; i++)
            {
                EnemySpawnEntry spawnEntry = enemySpawnPoint.EnemySpawnEntries[i];

                if (spawnEntry is not T typedSpawnEntry) continue;

                yield return typedSpawnEntry;
            }
        }

        //  2. Spawned Enemy Configuration
        protected sealed override void ConfigureSpawnedCharacter(GameObject spawnedCharacter, CharacterAIDefaultSpawnPointController spawnPoint, CharacterAISpawnEntry spawnEntry)
        {
            base.ConfigureSpawnedCharacter
            (
                spawnedCharacter,
                spawnPoint,
                spawnEntry
            );

            if (spawnPoint is not EnemyDefaultSpawnPointController enemySpawnPoint) return;            
            if (spawnEntry is not T typedSpawnEntry) return;

            this.ConfigureSpawnedEnemy(spawnedCharacter, enemySpawnPoint, typedSpawnEntry);
        }

        protected virtual void ConfigureSpawnedEnemy(GameObject spawnedEnemy, EnemyDefaultSpawnPointController spawnPoint, T spawnEntry)
        {
        }

        //  3. Spawn Notification
        protected override void NotifyCharacterAISpawned(CharacterAIDefaultSpawnPointController spawnPoint, CharacterAISpawnEntry spawnEntry, CharacterRootController character)
        {
            base.NotifyCharacterAISpawned(spawnPoint, spawnEntry, character);

            if (spawnPoint is not EnemyDefaultSpawnPointController enemySpawnPoint) return;
            if (spawnEntry is not T typedSpawnEntry) return;

            var enemySpawnedContext = new EnemySpawnedContext<T>(enemySpawnPoint, typedSpawnEntry, character);
            this.TypedEnemySpawned?.Invoke(enemySpawnedContext);
        }
    }
}