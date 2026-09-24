using UnityEngine;

namespace FlameOfTheLost
{
    public class AnimalRootController : NeutralCharacterRootController<AnimalProfile>
    {
        // --- Data / Configs ---
        public AnimalProfile AnimalProfile => this.TypedNeutralCharacterProfile;

        // --- References ---
        [Header("References (Animal-only)")]
        [SerializeField] private AnimalBrainController _animalBrainController;
        public AnimalBrainController AnimalBrainController => _animalBrainController;

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.AnimalBrainController == null) _animalBrainController = this.GetComponentInChildren<AnimalBrainController>(true);
        }

        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.AnimalBrainController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] No {nameof(FlameOfTheLost.AnimalBrainController)} assigned.", this);
            }
        }

    }
}