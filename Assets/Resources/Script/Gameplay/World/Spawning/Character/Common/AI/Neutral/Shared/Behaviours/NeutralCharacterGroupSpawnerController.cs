using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class NeutralCharacterGroupSpawnerController : CharacterAIGroupSpawnerController
    {
        // --- References ---
        [Header("References - Neutral Character Default Spawn Points")]
        [SerializeField]
        private List<NeutralCharacterDefaultSpawnPointController> _neutralCharacterDefaultSpawnPointControllers = new();
        protected IReadOnlyList<NeutralCharacterDefaultSpawnPointController> NeutralCharacterDefaultSpawnPointControllers => _neutralCharacterDefaultSpawnPointControllers;
        protected override IReadOnlyList<CharacterAIDefaultSpawnPointController> CharacterAIDefaultSpawnPointControllers => this.NeutralCharacterDefaultSpawnPointControllers;

        // --- Events ---
        public event Action<NeutralCharacterSpawnEntry, CharacterRootController> NeutralCharacterSpawned;

        // --- Public APIs ---
        public bool TryGetFirstSpawnEntry(out NeutralCharacterSpawnEntry spawnEntry)
        {
            spawnEntry = null;

            if (!this.TryGetFirstCharacterAISpawnEntry(out CharacterAISpawnEntry characterAISpawnEntry)) return false;

            spawnEntry = characterAISpawnEntry as NeutralCharacterSpawnEntry;

            return spawnEntry != null;
        }

        public bool TryGetFirstSpawnedNeutralCharacter(NeutralCharacterSpawnEntry spawnEntry, out CharacterRootController character)
        {
            return this.TryGetFirstSpawnedCharacter(spawnEntry, out character);
        }

        // --- Helpers ---
        //  1. Hooks
        protected override void NotifyCharacterAISpawned(CharacterAIDefaultSpawnPointController spawnPoint, CharacterAISpawnEntry spawnEntry, CharacterRootController character)
        {
            base.NotifyCharacterAISpawned(spawnPoint, spawnEntry, character);

            if (spawnEntry is not NeutralCharacterSpawnEntry neutralSpawnEntry) return;

            this.NeutralCharacterSpawned?.Invoke(neutralSpawnEntry, character);
        }
    }
}