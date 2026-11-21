using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterAIDefaultSpawnPointController : CharacterDefaultSpawnPointController
    {
        // --- Data / Configs ---
        //  1. Profile
        public CharacterAIDefaultSpawnPointProfile CharacterAIDefaultSpawnPointProfile => this.Profile as CharacterAIDefaultSpawnPointProfile;

        //  2. Character AI Spawn Entries
        public abstract IReadOnlyList<CharacterAISpawnEntry> CharacterAISpawnEntries { get; }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Profile != null && this.CharacterAIDefaultSpawnPointProfile == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterAIDefaultSpawnPointController)}] requires its Profile to be a {nameof(CharacterAIDefaultSpawnPointProfile)}.", this);
            }
        }
    }
}