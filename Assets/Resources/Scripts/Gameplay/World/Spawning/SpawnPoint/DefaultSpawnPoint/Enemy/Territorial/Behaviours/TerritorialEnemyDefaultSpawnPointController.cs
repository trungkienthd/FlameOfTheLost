using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class TerritorialEnemyDefaultSpawnPointController : EnemyDefaultSpawnPointController
    {
        // --- Data / Configs ---
        //  1. Profile
        public TerritorialEnemyDefaultSpawnPointProfile TerritorialProfile => this.Profile as TerritorialEnemyDefaultSpawnPointProfile;

        //  2. Spawn Entries
        public IReadOnlyList<TerritorialEnemySpawnEntry> TerritorialEnemySpawnEntries => this.TerritorialProfile != null
            ? this.TerritorialProfile.EnemySpawnEntries
            : null;

        public override IReadOnlyList<EnemySpawnEntry> EnemySpawnEntries => this.TerritorialEnemySpawnEntries;

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Profile != null && this.TerritorialProfile == null)
            {
                Debug.LogError($"[{this.name}.{nameof(TerritorialEnemyDefaultSpawnPointController)}] requires a {nameof(TerritorialEnemyDefaultSpawnPointProfile)}.", this);
                return;
            }

            if (this.TerritorialEnemySpawnEntries == null) return;

            for (int i = 0; i < this.TerritorialEnemySpawnEntries.Count; i++)
            {
                TerritorialEnemySpawnEntry spawnEntry = this.TerritorialEnemySpawnEntries[i];

                if (spawnEntry == null)
                {
                    Debug.LogWarning($"[{this.name}.{nameof(TerritorialEnemyDefaultSpawnPointController)}] Enemy spawn entry at index {i} is null.", this);
                    continue;
                }

                if (!spawnEntry.IsValid)
                {
                    Debug.LogWarning($"[{this.name}.{nameof(TerritorialEnemyDefaultSpawnPointController)}] Enemy spawn entry at index {i} is invalid.", this);
                }
            }
        }
    }
}
