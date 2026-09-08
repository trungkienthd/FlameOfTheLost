using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class EnemyDefaultSpawnPointProfile<T> : CharacterDefaultSpawnPointProfile where T : EnemySpawnEntry
    {
        [SerializeField] private List<T> _enemySpawnEntries = new();
        public IReadOnlyList<T> EnemySpawnEntries => _enemySpawnEntries;
    }
}