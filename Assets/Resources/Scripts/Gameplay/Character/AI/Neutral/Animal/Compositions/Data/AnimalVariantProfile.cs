using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class AnimalVariantProfile : NeutralCharacterVariantProfile<AnimalProfile>
    {
        // --- Data / Configs ---
        [Header("Configs - Animal AI")]
        [SerializeField] private AnimalRoamingConfig _roamingConfig;
        [SerializeField] private AnimalFleeConfig _fleeConfig;

        public AnimalRoamingConfig RoamingConfig => _roamingConfig;
        public AnimalFleeConfig FleeConfig => _fleeConfig;

        public AnimalProfile AnimalProfile => this.TypedNeutralCharacterProfile;

        // --- Public APIs ---
        public override bool IsValid => base.IsValid && this.RoamingConfig != null && this.FleeConfig != null;
    }
}