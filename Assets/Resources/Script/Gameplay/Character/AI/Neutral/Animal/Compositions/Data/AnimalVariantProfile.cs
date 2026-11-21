using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Variant_Chr_AI_Ntr_Animal_New", menuName = "Data/FlameOfTheLost/Character/AI/Neutral/Animal/VariantProfile")]
    public class AnimalVariantProfile : NeutralCharacterVariantProfile<AnimalProfile>
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