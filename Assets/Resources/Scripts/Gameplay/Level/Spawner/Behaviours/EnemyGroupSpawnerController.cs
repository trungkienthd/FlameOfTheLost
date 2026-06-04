using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EnemyGroupSpawnerController<T> : FOTLMonoBehaviour where T : EnemySpawnEntry
    {
        // --- States ---
        [Header("References - Player Spawner")]
        [SerializeField] private PlayerSpawnerController _playerSpawnerController;
        private PlayerSpawnerController PlayerSpawnerController => _playerSpawnerController;

        [Header("References - Enemy Root")]
        [SerializeField] private Transform _enemyGroupRoot;
        protected Transform EnemyGroupRoot => _enemyGroupRoot;

        [Header("References - Spawn Entries")]
        [SerializeField] private List<T> _spawnEntries = new();
        protected IReadOnlyList<T> EnemySpawnEntries => _spawnEntries;

        [Header("Runtime Enemies (GameObject)")]
        [SerializeField] private List<GameObject> _spawnedEnemies = new();
        protected List<GameObject> SpawnedEnemies => _spawnedEnemies;

        private readonly Dictionary<T, List<GameObject>> _spawnedEnemiesByEntry = new();

        [Header("Settings")]
        [SerializeField] private bool _isSpawnedEnemiesOnStart = true;
        [SerializeField] private bool _isRespawnedEnemiesWhenPlayerSpawnsOrRespawns = true;
        private bool IsSpawnedEnemiesOnStart => _isSpawnedEnemiesOnStart;
        private bool IsRespawnedEnemiesWhenPlayerSpawnsOrRespawns => _isRespawnedEnemiesWhenPlayerSpawnsOrRespawns;

        // --- Unity Callbacks ---
        private void Start()
        {
            if (!this.IsSpawnedEnemiesOnStart) return;

            this.SpawnEnemiesOnStart();
        }
        private void Reset()
        {
            this.CacheReferences();
        }
        //private void OnValidate()
        //{
        //    this.CacheReferences();
        //}
        private void OnEnable()
        {
            this.SubscribeToPlayerSpawner();
        }
        private void OnDisable()
        {
            this.UnsubscribeFromPlayerSpawner();
        }

        // --- Public APIs ---
        public void SpawnEnemiesOnStart()
        {
            foreach (T spawnEntry in this.EnemySpawnEntries)
            {
                if (spawnEntry == null) continue;
                if (!spawnEntry.SpawnOnStart) continue;

                this.SpawnFromEntry(spawnEntry);
            }
        }

        public void SpawnEnemiesAfterPlayerRespawn()
        {
            // this.ClearSpawnedEnemies();

            foreach (T spawnEntry in this.EnemySpawnEntries)
            {
                if (spawnEntry == null) continue;
                if (!spawnEntry.SpawnWhenPlayerRespawns) continue;

                this.SpawnFromEntry(spawnEntry);
            }
        }

        // --- Helpers ---
        //  0. Spawning
        protected virtual void SpawnFromEntry(T spawnEntry)
        {
            if (spawnEntry == null || !spawnEntry.IsValid)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(EnemyGroupSpawnerController<T>)}] Invalid enemy spawn entry.", this);
                return;
            }

            this.PruneDestroyedSpawnedEnemies();

            int currentSpawnedCount = this.GetSpawnedEnemyCount(spawnEntry);
            int remainingSpawnCount = spawnEntry.SpawnCount - currentSpawnedCount;

            if (remainingSpawnCount <= 0)
            {
                return;
            }

            for (int i = 0; i < remainingSpawnCount; i++)
            {
                this.SpawnSingleEnemy(spawnEntry);
            }
        }
        protected virtual GameObject SpawnSingleEnemy(T spawnEntry)
        {
            GameObject spawnedEnemy = UnityEngine.Object.Instantiate(spawnEntry.EnemyPrefab, this.EnemyGroupRoot);

            if (spawnedEnemy == null) return null;

            this.RegisterSpawnedEnemy(spawnEntry, spawnedEnemy);

            CharacterPlacementHandler enemyPlacementHandler = spawnedEnemy.GetComponentInChildren<CharacterPlacementHandler>(true);
            if (enemyPlacementHandler == null)
            {
                Debug.LogWarning($"[{spawnedEnemy.name}.{nameof(EnemyGroupSpawnerController<T>)}] Spawned enemy has no CharacterPlacementHandler.", spawnedEnemy);

                return spawnedEnemy;
            }

            SpawnResolveResult spawnResult = spawnEntry.ResolveSpawnResult();
            enemyPlacementHandler.PlaceCharacter(spawnResult, CharacterPlacementSourceType.DefaultSpawnPoint);

            this.ConfigureSpawnedEnemy(spawnedEnemy, spawnEntry);

            return spawnedEnemy;
        }
        protected virtual void ConfigureSpawnedEnemy(GameObject spawnedEnemy, T spawnEntry)
        {
            // Base enemy spawner has no extra configuration.
            // Subclasses can configure enemy-specific runtime data here.
        }
        protected void ClearSpawnedEnemies()
        {
            for (int i = this.SpawnedEnemies.Count - 1; i >= 0; i--)
            {
                GameObject spawnedEnemy = this.SpawnedEnemies[i];

                if (spawnedEnemy != null)
                {
                    UnityEngine.Object.Destroy(spawnedEnemy);
                }
            }

            this.SpawnedEnemies.Clear();
            _spawnedEnemiesByEntry.Clear();
        }
        private void RegisterSpawnedEnemy(T spawnEntry, GameObject spawnedEnemy)
        {
            if (spawnEntry == null || spawnedEnemy == null) return;

            this.SpawnedEnemies.Add(spawnedEnemy);

            if (!_spawnedEnemiesByEntry.TryGetValue(spawnEntry, out List<GameObject> spawnedEnemiesForEntry))
            {
                spawnedEnemiesForEntry = new List<GameObject>();
                _spawnedEnemiesByEntry.Add(spawnEntry, spawnedEnemiesForEntry);
            }

            spawnedEnemiesForEntry.Add(spawnedEnemy);
        }
        private int GetSpawnedEnemyCount(T spawnEntry)
        {
            if (spawnEntry == null) return 0;

            if (!_spawnedEnemiesByEntry.TryGetValue(spawnEntry, out List<GameObject> spawnedEnemiesForEntry))
            {
                return 0;
            }

            spawnedEnemiesForEntry.RemoveAll(spawnedEnemy => spawnedEnemy == null);

            return spawnedEnemiesForEntry.Count;
        }
        private void PruneDestroyedSpawnedEnemies()
        {
            this.SpawnedEnemies.RemoveAll(spawnedEnemy => spawnedEnemy == null);

            List<T> emptySpawnEntries = new();

            foreach (KeyValuePair<T, List<GameObject>> pair in _spawnedEnemiesByEntry)
            {
                pair.Value.RemoveAll(spawnedEnemy => spawnedEnemy == null);

                if (pair.Value.Count <= 0)
                {
                    emptySpawnEntries.Add(pair.Key);
                }
            }

            foreach (T emptySpawnEntry in emptySpawnEntries)
            {
                _spawnedEnemiesByEntry.Remove(emptySpawnEntry);
            }
        }

        //  1. Handle Events
        private void HandlePlayerSpawnedOrRespawned()
        {
            if (!this.IsRespawnedEnemiesWhenPlayerSpawnsOrRespawns) return;

            this.SpawnEnemiesAfterPlayerRespawn();
        }

        //  2. Subscriptions
        private void SubscribeToPlayerSpawner()
        {
            if (this.PlayerSpawnerController == null) return;

            this.PlayerSpawnerController.OnPlayerSpawnedOrRespawned -= this.HandlePlayerSpawnedOrRespawned;
            this.PlayerSpawnerController.OnPlayerSpawnedOrRespawned += this.HandlePlayerSpawnedOrRespawned;
        }
        private void UnsubscribeFromPlayerSpawner()
        {
            if (this.PlayerSpawnerController == null) return;

            this.PlayerSpawnerController.OnPlayerSpawnedOrRespawned -= this.HandlePlayerSpawnedOrRespawned;
        }

        //  3. Cache & Validate References
        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.PlayerSpawnerController == null)
            {
                if (this.transform.parent != null)
                {
                    _playerSpawnerController = this.transform.parent.GetComponentInChildren<PlayerSpawnerController>();
                }
                if (this.PlayerSpawnerController == null)
                {
                    _playerSpawnerController = UnityEngine.Object.FindFirstObjectByType<PlayerSpawnerController>();
                }
            }

            if (this.EnemyGroupRoot == null)
            {
                GameObject enemyGroupRoot = GameObject.Find("EnemyGroup");
                _enemyGroupRoot = enemyGroupRoot != null ? enemyGroupRoot.transform : this.transform;
            }
        }
        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.PlayerSpawnerController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(EnemyGroupSpawnerController<T>)}] No PlayerSpawnerController assigned.", this);
            }

            if (this.EnemyGroupRoot == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(EnemyGroupSpawnerController<T>)}] No EnemyGroupRoot assigned.", this);
            }
        }
    }
}
