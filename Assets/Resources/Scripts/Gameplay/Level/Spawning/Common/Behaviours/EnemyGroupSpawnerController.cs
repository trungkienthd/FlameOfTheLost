using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class EnemyGroupSpawnerController : FOTLMonoBehaviour
    {
        // --- Events ---
        public event Action<EnemySpawnEntry, CharacterRootController> EnemySpawned;

        // --- Public APIs ---
        //  1. Spawn Entry Queries
        public abstract bool TryGetFirstSpawnEntry(out EnemySpawnEntry spawnEntry);

        //  2. Runtime Enemy Queries
        public abstract bool TryGetFirstSpawnedEnemy(EnemySpawnEntry spawnEntry, out CharacterRootController character);

        // --- Helpers ---
        protected void NotifyEnemySpawnedBase(EnemySpawnEntry spawnEntry, CharacterRootController character)
        {
            if (spawnEntry == null) return;
            if (character == null) return;

            this.EnemySpawned?.Invoke(spawnEntry, character);
        }
    }
}
