using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class AnimalDefaultSpawnPointController : NeutralCharacterDefaultSpawnPointController
    {
        // --- Data / Configs ---
        //  1. Profile
        public AnimalDefaultSpawnPointProfile AnimalProfile => this.Profile as AnimalDefaultSpawnPointProfile;

        //  2. Spawn Entries
        public IReadOnlyList<AnimalSpawnEntry> AnimalSpawnEntries => this.AnimalProfile != null
            ? this.AnimalProfile.NeutralCharacterSpawnEntries
            : null;

        public override IReadOnlyList<NeutralCharacterSpawnEntry> NeutralCharacterSpawnEntries => this.AnimalSpawnEntries;

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Profile != null && this.AnimalProfile == null)
            {
                Debug.LogError($"[{this.name}.{nameof(AnimalDefaultSpawnPointController)}] requires a {nameof(AnimalDefaultSpawnPointProfile)}.", this);
                return;
            }

            if (this.AnimalSpawnEntries == null) return;

            for (int i = 0; i < this.AnimalSpawnEntries.Count; i++)
            {
                AnimalSpawnEntry spawnEntry = this.AnimalSpawnEntries[i];
                if (spawnEntry == null)
                {
                    Debug.LogWarning($"[{this.name}.{nameof(AnimalDefaultSpawnPointController)}] Animal spawn entry at index {i} is null.", this);
                    continue;
                }

                if (!spawnEntry.IsValid)
                {
                    Debug.LogWarning($"[{this.name}.{nameof(AnimalDefaultSpawnPointController)}] Animal spawn entry at index {i} is invalid.", this);
                }
            }
        }
    }
}