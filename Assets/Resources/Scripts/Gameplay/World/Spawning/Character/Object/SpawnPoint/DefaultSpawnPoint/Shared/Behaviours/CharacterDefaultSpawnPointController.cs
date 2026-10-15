using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterDefaultSpawnPointController : CharacterSpawnPointController
    {
        public CharacterDefaultSpawnPointProfile DefaultSpawnPointProfile => this.Profile as CharacterDefaultSpawnPointProfile;

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Profile != null && this.DefaultSpawnPointProfile == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterDefaultSpawnPointController)}] requires its Profile to be a {nameof(CharacterDefaultSpawnPointProfile)}.", this);
            }
        }
    }
}