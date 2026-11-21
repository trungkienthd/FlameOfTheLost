using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class AnimalSpawnEntry : NeutralCharacterSpawnEntry
    {
        // --- Data / Configs ---
        //  1. Animal Variant
        public AnimalVariantProfile AnimalVariant => this.NeutralCharacterVariant as AnimalVariantProfile;

        //  2. Animal Home Area
        [Header("Configs - Animal Home Area")]
        [SerializeField] private AnimalHomeAreaEntry _homeAreaEntry;
        public AnimalHomeAreaEntry HomeAreaEntry => _homeAreaEntry;

        //  3. Optional Animal AI Overrides
        [Header("Configs - (Optional) Animal AI Overrides")]
        [SerializeField] private AnimalRoamingConfig _roamingConfigOverride;
        [SerializeField] private AnimalFleeConfig _fleeConfigOverride;
        public AnimalRoamingConfig RoamingConfigOverride => _roamingConfigOverride;
        public AnimalFleeConfig FleeConfigOverride => _fleeConfigOverride;

        // --- Resolved Data / Configs ---
        public AnimalRoamingConfig ResolvedRoamingConfig => this.RoamingConfigOverride != null
            ? this.RoamingConfigOverride
            : this.AnimalVariant != null
                ? this.AnimalVariant.RoamingConfig
                : null;

        public AnimalFleeConfig ResolvedFleeConfig => this.FleeConfigOverride != null
            ? this.FleeConfigOverride
            : this.AnimalVariant != null
                ? this.AnimalVariant.FleeConfig
                : null;

        // --- Public APIs ---
        //  1. Validation
        public bool HasValidAnimalAIConfig => this.ResolvedRoamingConfig != null && this.ResolvedFleeConfig != null;

        public override bool IsValid => base.IsValid &&
            this.AnimalVariant != null &&
            this.HomeAreaEntry.HasValidArea() &&
            this.HasValidAnimalAIConfig;
    }
}