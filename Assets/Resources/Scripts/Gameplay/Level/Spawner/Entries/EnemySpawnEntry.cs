using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class EnemySpawnEntry
    {
        // --- States ---
        [Header("References - Enemy Prefab")]
        [SerializeField] private GameObject _enemyPrefab;
        public GameObject EnemyPrefab => _enemyPrefab;

        [Header("References - Spawn Point")]
        [SerializeField] private CharacterDefaultSpawnPointController _spawnPoint;
        public CharacterDefaultSpawnPointController SpawnPoint => _spawnPoint;

        [Header("Spawn Settings")]
        [SerializeField, Min(1)] private int _spawnCount = 1;
        [SerializeField] private bool _spawnOnStart = true;
        [SerializeField] private bool _spawnWhenPlayerRespawns = true;
        public int SpawnCount => Mathf.Max(1, _spawnCount);
        public bool SpawnOnStart => _spawnOnStart;
        public bool SpawnWhenPlayerRespawns => _spawnWhenPlayerRespawns;

        [Header("Optional Position Offset")]
        [SerializeField] private float _randomSpawnOffsetRadius = 0f;
        public float RandomSpawnOffsetRadius => Mathf.Max(0f, _randomSpawnOffsetRadius);

        // --- Public APIs ---
        public bool IsValid => EnemyPrefab != null && SpawnPoint != null;
        
        public CharacterRefreshTarget ResolveSpawnResult()
        {
            Vector3 position = this.SpawnPoint.SpawnPosition;

            if (this.RandomSpawnOffsetRadius > 0f)
            {
                Vector2 randomOffset = Random.insideUnitCircle * this.RandomSpawnOffsetRadius;
                position += new Vector3(randomOffset.x, randomOffset.y, 0f);
            }

            return new CharacterRefreshTarget
            (
                position,
                this.SpawnPoint.SpawnRotation,
                this.SpawnPoint.DefaultFacing
            );
        }
    }
}