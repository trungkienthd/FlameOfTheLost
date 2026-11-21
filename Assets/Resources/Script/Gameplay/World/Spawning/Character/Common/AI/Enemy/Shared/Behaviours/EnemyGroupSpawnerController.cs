using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class EnemyGroupSpawnerController : CharacterAIGroupSpawnerController
    {
        // --- References ---
        [Header("References - Enemy Default Spawn Points")]
        [SerializeField] private List<EnemyDefaultSpawnPointController> _enemyDefaultSpawnPointControllers = new();
        protected IReadOnlyList<EnemyDefaultSpawnPointController> EnemyDefaultSpawnPointControllers => _enemyDefaultSpawnPointControllers;

        protected override IReadOnlyList<CharacterAIDefaultSpawnPointController> CharacterAIDefaultSpawnPointControllers => this.EnemyDefaultSpawnPointControllers;

        // --- Events ---
        public event Action<EnemySpawnEntry, CharacterRootController> EnemySpawned;

        // --- Public APIs ---
        public bool TryGetSpawnEntryBySpawnPointId(int spawnPointId, out EnemySpawnEntry spawnEntry)
        {
            spawnEntry = null;

            if (spawnPointId <= 0) return false;
            if (this.EnemyDefaultSpawnPointControllers == null) return false;

            foreach (EnemyDefaultSpawnPointController spawnPoint in this.EnemyDefaultSpawnPointControllers)
            {
                if (spawnPoint == null || spawnPoint.Id != spawnPointId) continue;

                foreach (CharacterAISpawnEntry candidate in this.GetSpawnEntries(spawnPoint))
                {
                    if (candidate is not EnemySpawnEntry enemyEntry || !enemyEntry.IsValid) continue;

                    // A single DSP must uniquely identify the tutorial target's entry.
                    if (spawnEntry != null)
                    {
                        Debug.LogError($"[{this.gameObject.name}.{nameof(EnemyGroupSpawnerController)}] Multiple valid enemy entries exist for DSP ID {spawnPointId}. Specify a unique tutorial spawn point.", this);

                        spawnEntry = null;
                        return false;
                    }

                    spawnEntry = enemyEntry;
                }
            }

            return spawnEntry != null;
        }

        public bool TryGetFirstSpawnEntry(out EnemySpawnEntry spawnEntry)
        {
            spawnEntry = null;

            if (!this.TryGetFirstCharacterAISpawnEntry(out CharacterAISpawnEntry characterAISpawnEntry)) return false;

            spawnEntry = characterAISpawnEntry as EnemySpawnEntry;

            return spawnEntry != null;
        }

        public bool TryGetFirstSpawnedEnemy(EnemySpawnEntry spawnEntry, out CharacterRootController character)
        {
            return this.TryGetFirstSpawnedCharacter(spawnEntry, out character);
        }

        // --- Helpers ---
        //  0. Hooks
        protected override void NotifyCharacterAISpawned(CharacterAIDefaultSpawnPointController spawnPoint, CharacterAISpawnEntry spawnEntry, CharacterRootController character)
        {
            base.NotifyCharacterAISpawned(spawnPoint, spawnEntry, character);

            if (spawnEntry is not EnemySpawnEntry enemySpawnEntry) return;

            this.EnemySpawned?.Invoke(enemySpawnEntry, character);
        }
    }
}