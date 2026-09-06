using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Variant_AI_Enemy_Trl_New", menuName = "Data/FlameOfTheLost/Character/Enemy/Territorial/VariantProfile")]
    public sealed class TerritorialEnemyVariantProfile : EnemyVariantProfile
    {
        // --- Data / Configs ---
        //  1. Territorial AI
        [Header("Configs - Territorial AI")]
        [SerializeField] private TerritorialEnemyRoamingConfig _defaultRoamingConfig;
        [SerializeField] private TerritorialEnemyAttackConfig _attackConfig;
        public TerritorialEnemyRoamingConfig DefaultRoamingConfig => _defaultRoamingConfig;
        public TerritorialEnemyAttackConfig AttackConfig => _attackConfig;

        // --- Public APIs ---
        public override bool IsValid =>
            base.IsValid &&
            this.DefaultRoamingConfig != null &&
            this.AttackConfig != null;
    }
}
