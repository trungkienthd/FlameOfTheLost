using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class TerritorialEnemySpawnEntry : EnemySpawnEntry
    {
        [Header("Territorial AI Config")]
        [SerializeField] private TerritorialEnemyRoamingConfig _roamingConfig;
        [SerializeField] private TerritorialEnemyAttackConfig _attackConfig;
        public TerritorialEnemyRoamingConfig RoamingConfig => _roamingConfig;
        public TerritorialEnemyAttackConfig AttackConfig => _attackConfig;

        public bool HasValidTerritorialAIConfig => this.RoamingConfig != null && this.AttackConfig != null;

        [Header("Territorial Home Area")]
        [SerializeField] private TerritorialEnemyHomeAreaEntry _homeAreaEntry;
        public TerritorialEnemyHomeAreaEntry HomeAreaEntry => _homeAreaEntry;
    }
}
