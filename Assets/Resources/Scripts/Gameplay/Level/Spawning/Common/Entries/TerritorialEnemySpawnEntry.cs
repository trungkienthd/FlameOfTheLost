using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class TerritorialEnemySpawnEntry : EnemySpawnEntry
    {
        // --- Data / Configs ---
        //  1. Territorial Enemy Variant
        public TerritorialEnemyVariantProfile TerritorialEnemyVariant => this.EnemyVariant as TerritorialEnemyVariantProfile;

        //  2. Territorial Home Area
        [Header("Configs - Territorial Home Area")]
        [SerializeField] private TerritorialEnemyHomeAreaEntry _homeAreaEntry;
        public TerritorialEnemyHomeAreaEntry HomeAreaEntry => _homeAreaEntry;

        //  3. Optional Territorial AI Overrides
        [Header("Configs - (Optional) Territorial AI Overrides")]
        [SerializeField] private TerritorialEnemyRoamingConfig _roamingConfigOverride;
        public TerritorialEnemyRoamingConfig RoamingConfigOverride => _roamingConfigOverride;

        // --- Resolved Data / Configs ---
        public TerritorialEnemyRoamingConfig ResolvedRoamingConfig => this.RoamingConfigOverride != null
            ? this.RoamingConfigOverride
            : this.TerritorialEnemyVariant != null ? this.TerritorialEnemyVariant.DefaultRoamingConfig : null;

        public TerritorialEnemyAttackConfig AttackConfig => this.TerritorialEnemyVariant != null
            ? this.TerritorialEnemyVariant.AttackConfig
            : null;

        // --- Public APIs ---
        //  1. Validation
        public bool HasValidTerritorialAIConfig =>
            this.ResolvedRoamingConfig != null &&
            this.AttackConfig != null;

        public override bool IsValid => 
            base.IsValid && 
            this.TerritorialEnemyVariant != null && 
            this.HomeAreaEntry.HasValidArea() && 
            this.HasValidTerritorialAIConfig;
    }
}
