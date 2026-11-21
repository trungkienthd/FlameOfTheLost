using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Variant_Chr_AI_Enemy_Skr_New", menuName = "Data/FlameOfTheLost/Character/AI/Enemy/Skirmisher/VariantProfile")]
    public class SkirmisherEnemyVariantProfile : EnemyVariantProfile
    {
        // --- Data / Configs ---
        //  1. Skirmisher AI
        [Header("Configs - Skirmisher AI")]
        [SerializeField] private SkirmisherEnemyRoamingConfig _defaultRoamingConfig;
        [SerializeField] private SkirmisherEnemyEngagementConfig _engagementConfig;
        [SerializeField] private SkirmisherEnemyPositioningConfig _positioningConfig;
        [SerializeField] private SkirmisherEnemyCombatConfig _combatConfig;
        [SerializeField] private SkirmisherEnemyRetreatConfig _retreatConfig;
        public SkirmisherEnemyRoamingConfig DefaultRoamingConfig => _defaultRoamingConfig;
        public SkirmisherEnemyEngagementConfig EngagementConfig => _engagementConfig;
        public SkirmisherEnemyPositioningConfig PositioningConfig => _positioningConfig;
        public SkirmisherEnemyCombatConfig CombatConfig => _combatConfig;
        public SkirmisherEnemyRetreatConfig RetreatConfig => _retreatConfig;

        // --- Public APIs ---
        public override bool IsValid =>
            base.IsValid &&
            this.DefaultRoamingConfig != null &&
            this.EngagementConfig != null &&
            this.PositioningConfig != null &&
            this.CombatConfig != null &&
            this.RetreatConfig != null;
    }
}
