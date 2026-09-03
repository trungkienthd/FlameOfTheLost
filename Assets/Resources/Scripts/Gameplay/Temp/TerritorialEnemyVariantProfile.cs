using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "TerritorialEnemyVariantProfile", menuName = "Scriptable Objects/TerritorialEnemyVariantProfile")]
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
