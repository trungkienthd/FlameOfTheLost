using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class EnemyGroupSpawnerController :
        CharacterAIGroupSpawnerController
    {
        // --- References ---
        [Header("References - Enemy Default Spawn Points")]
        [SerializeField]
        private List<EnemyDefaultSpawnPointController> _enemyDefaultSpawnPointControllers = new();
        protected IReadOnlyList<EnemyDefaultSpawnPointController> EnemyDefaultSpawnPointControllers => _enemyDefaultSpawnPointControllers;

        protected override IReadOnlyList<CharacterAIDefaultSpawnPointController> CharacterAIDefaultSpawnPointControllers => this.EnemyDefaultSpawnPointControllers;

        protected override string CharacterAIGroupRootName => "EnemyGroup";

        // --- Events ---
        public event Action<EnemySpawnEntry, CharacterRootController> EnemySpawned;

        // --- Public APIs ---
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