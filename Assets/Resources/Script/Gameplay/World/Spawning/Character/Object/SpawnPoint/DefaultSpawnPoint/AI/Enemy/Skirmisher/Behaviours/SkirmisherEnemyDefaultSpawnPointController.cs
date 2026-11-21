using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class SkirmisherEnemyDefaultSpawnPointController : EnemyDefaultSpawnPointController
    {
        // --- Data / Configs ---
        //  1. Profile
        public SkirmisherEnemyDefaultSpawnPointProfile SkirmisherProfile => this.Profile as SkirmisherEnemyDefaultSpawnPointProfile;

        //  2. Spawn Entries
        public IReadOnlyList<SkirmisherEnemySpawnEntry> SkirmisherEnemySpawnEntries => this.SkirmisherProfile != null
            ? this.SkirmisherProfile.EnemySpawnEntries
            : null;

        public override IReadOnlyList<EnemySpawnEntry> EnemySpawnEntries => this.SkirmisherEnemySpawnEntries;

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {   
            base.ValidateInternalReferences();

            if (this.Profile != null && this.SkirmisherProfile == null)
            {
                Debug.LogError($"[{this.name}.{nameof(SkirmisherEnemyDefaultSpawnPointController)}] requires a {nameof(SkirmisherEnemyDefaultSpawnPointProfile)}.", this);
                return;
            }

            if (this.SkirmisherEnemySpawnEntries == null) return;

            for (int i = 0; i < this.SkirmisherEnemySpawnEntries.Count; i++)
            {
                SkirmisherEnemySpawnEntry spawnEntry = this.SkirmisherEnemySpawnEntries[i];

                if (spawnEntry == null)
                {
                    Debug.LogWarning($"[{this.name}.{nameof(SkirmisherEnemyDefaultSpawnPointController)}] Enemy spawn entry at index {i} is null.", this);
                    continue;
                }

                if (!spawnEntry.IsValid)
                {
                    Debug.LogWarning($"[{this.name}.{nameof(SkirmisherEnemyDefaultSpawnPointController)}] Enemy spawn entry at index {i} is invalid.", this);
                }
            }
        }
    }
}
