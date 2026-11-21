using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCDefaultSpawnPointController : CharacterDefaultSpawnPointController
    {
        // --- Data / Configs ---
        //  1. Profile
        public NPCDefaultSpawnPointProfile NPCSpawnPointProfile => this.Profile as NPCDefaultSpawnPointProfile;

        //  2. NPC Spawn Entries
        public IReadOnlyList<NPCSpawnEntry> NPCSpawnEntries => this.NPCSpawnPointProfile != null
            ? this.NPCSpawnPointProfile.NPCSpawnEntries
            : null;

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Profile != null && this.NPCSpawnPointProfile == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCDefaultSpawnPointController)}] Requires a {nameof(NPCDefaultSpawnPointProfile)}.", this);
            }

            if (this.NPCSpawnEntries == null) return;

            for (int i = 0; i < this.NPCSpawnEntries.Count; i++)
            {
                NPCSpawnEntry spawnEntry = this.NPCSpawnEntries[i];

                if (spawnEntry != null && spawnEntry.IsValid) continue;

                Debug.LogWarning($"[{this.gameObject.name}.{nameof(NPCDefaultSpawnPointController)}] NPC spawn entry at index {i} is invalid.", this);
            }
        }
    }
}
