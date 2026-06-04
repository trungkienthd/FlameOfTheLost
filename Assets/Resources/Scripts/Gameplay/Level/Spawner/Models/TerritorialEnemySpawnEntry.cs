using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class TerritorialEnemySpawnEntry : EnemySpawnEntry
    {
        [Header("Territorial Home Area")]
        [SerializeField] private TerritorialEnemyHomeAreaEntry _homeAreaEntry;
        public TerritorialEnemyHomeAreaEntry HomeAreaEntry => _homeAreaEntry;
    }
}
