using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class SkirmisherEnemySpawnEntry : EnemySpawnEntry
    {
        // --- Data / Configs ---
        //  1. Skirmisher Enemy Variant
        public SkirmisherEnemyVariantProfile SkirmisherEnemyVariant => this.EnemyVariant as SkirmisherEnemyVariantProfile;

        //  2. Skirmisher Home Area
        [Header("Configs - Skirmisher Home Area")]
        [SerializeField] private TerritorialEnemyHomeAreaEntry _homeAreaEntry;
        public TerritorialEnemyHomeAreaEntry HomeAreaEntry => _homeAreaEntry;

        //  3. Optional Skirmisher AI Overrides
        [Header("Configs - (Optional) Skirmisher AI Overrides")]
        [SerializeField] private SkirmisherEnemyRoamingConfig _roamingConfigOverride;
        public SkirmisherEnemyRoamingConfig RoamingConfigOverride => _roamingConfigOverride;

        // --- Data / Configs ---
        public SkirmisherEnemyRoamingConfig ResolvedRoamingConfig => this.RoamingConfigOverride != null
            ? this.RoamingConfigOverride
            : this.SkirmisherEnemyVariant != null
                ? this.SkirmisherEnemyVariant.DefaultRoamingConfig
                : null;

        public SkirmisherEnemyEngagementConfig EngagementConfig => this.SkirmisherEnemyVariant != null
            ? this.SkirmisherEnemyVariant.EngagementConfig
            : null;

        public SkirmisherEnemyPositioningConfig PositioningConfig => this.SkirmisherEnemyVariant != null
            ? this.SkirmisherEnemyVariant.PositioningConfig
            : null;

        public SkirmisherEnemyCombatConfig CombatConfig => this.SkirmisherEnemyVariant != null
            ? this.SkirmisherEnemyVariant.CombatConfig
            : null;

        public SkirmisherEnemyRetreatConfig RetreatConfig => this.SkirmisherEnemyVariant != null
            ? this.SkirmisherEnemyVariant.RetreatConfig
            : null;

        // --- Public APIs ---
        //  1. Validation
        public bool HasValidSkirmisherAIConfig =>
            this.ResolvedRoamingConfig  != null &&
            this.EngagementConfig       != null &&
            this.PositioningConfig      != null &&
            this.CombatConfig           != null &&
            this.RetreatConfig          != null;

        public override bool IsValid =>
            base.IsValid &&
            this.SkirmisherEnemyVariant != null &&
            this.HomeAreaEntry.HasValidArea() &&
            this.HasValidSkirmisherAIConfig;
    }
}
