using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public abstract class EnemySpawnEntry
    {
        // --- Data / Configs ---
        //  1. Enemy Variant
        [Header("Configs - Enemy Variant")]
        [SerializeField] private EnemyVariantProfile _enemyVariant;
        public EnemyVariantProfile EnemyVariant => _enemyVariant;

        //  2. Spawn Settings
        [Header("Configs - Spawn Settings")]
        [SerializeField, Min(1)] private int _spawnCount = 1;
        [SerializeField] private bool _spawnOnStart = true;
        [SerializeField] private bool _spawnWhenPlayerRespawns = true;

        public int SpawnCount => Mathf.Max(1, _spawnCount);
        public bool SpawnOnStart => _spawnOnStart;
        public bool SpawnWhenPlayerRespawns => _spawnWhenPlayerRespawns;

        //  3. Position Offset
        [Header("Configs - Spawn Position")]
        [SerializeField, Min(0f)] private float _randomSpawnOffsetRadius = 0f;
        public float RandomSpawnOffsetRadius => Mathf.Max(0f, _randomSpawnOffsetRadius);

        // --- Public APIs ---
        //  1. Validation
        public virtual bool IsValid => this.EnemyVariant != null && this.EnemyVariant.IsValid;

        //  2. Spawn Result
        public CharacterRefreshTarget ResolveSpawnResult(CharacterDefaultSpawnPointController spawnPoint)
        {
            Vector3 position = spawnPoint.SpawnPosition;

            if (this.RandomSpawnOffsetRadius > 0f)
            {
                Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * this.RandomSpawnOffsetRadius;
                position += new Vector3(randomOffset.x, randomOffset.y, 0f);
            }

            return new CharacterRefreshTarget(position, spawnPoint.SpawnRotation, spawnPoint.DefaultFacing);
        }
    }
}