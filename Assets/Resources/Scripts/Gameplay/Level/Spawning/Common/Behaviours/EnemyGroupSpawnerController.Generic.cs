using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EnemyGroupSpawnerController<T> : EnemyGroupSpawnerController where T : EnemySpawnEntry
    {
        // --- References ---
        [Header("References - Player Spawner")]
        [SerializeField] private PlayerSpawnerController _playerSpawnerController;
        private PlayerSpawnerController PlayerSpawnerController => _playerSpawnerController;

        [Header("References - Enemy Root")]
        [SerializeField] private Transform _enemyGroupRoot;
        protected Transform EnemyGroupRoot => _enemyGroupRoot;

        [Header("References - Spawn Entries")]
        [SerializeField] private List<T> _spawnEntries = new();
        protected IReadOnlyList<T> EnemySpawnEntries => _spawnEntries;

        // --- Configs ---
        [Header("Settings")]
        [SerializeField] private bool _isSpawnedEnemiesOnStart = true;
        [SerializeField] private bool _isRespawnedEnemiesWhenPlayerSpawnsOrRespawns = true;
        private bool IsSpawnedEnemiesOnStart => _isSpawnedEnemiesOnStart;
        private bool IsRespawnedEnemiesWhenPlayerSpawnsOrRespawns => _isRespawnedEnemiesWhenPlayerSpawnsOrRespawns;

        // --- Runtime ---
        [Header("Runtime - Enemies (GameObject)")]
        [SerializeField] private List<GameObject> _spawnedEnemies = new();
        protected List<GameObject> SpawnedEnemies => _spawnedEnemies;

        private readonly Dictionary<T, List<GameObject>> _spawnedEnemiesByEntry = new();
        private IReadOnlyDictionary<T, List<GameObject>> SpawnedEnemiesByEntry => _spawnedEnemiesByEntry;

        private bool _hasBoundExternalReferences;
        private bool HasBoundExternalReferences
        {
            get => _hasBoundExternalReferences;
            set => _hasBoundExternalReferences = value;
        }

        // --- Events ---
        public event Action<EnemySpawnedContext<T>> TypedEnemySpawned;

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            if (!this.HasBoundExternalReferences) return;

            this.SubscribeToPlayerSpawner();
        }
        private void OnDisable()
        {
            this.UnsubscribeFromPlayerSpawner();
        }

        // --- Public APIs ---
        //  1. Spawn Entry Queries
        public override bool TryGetFirstSpawnEntry(out EnemySpawnEntry spawnEntry)
        {
            spawnEntry = null;

            if (!this.TryGetFirstSpawnEntryTyped(out T typedSpawnEntry)) return false;

            spawnEntry = typedSpawnEntry;
            return true;
        }

        //  2. Runtime Enemy Queries
        public override bool TryGetFirstSpawnedEnemy(EnemySpawnEntry spawnEntry, out CharacterRootController character)
        {
            character = null;

            if (spawnEntry is not T typedSpawnEntry) return false;
            if (!this.SpawnedEnemiesByEntry.TryGetValue(typedSpawnEntry, out List<GameObject> spawnedEnemies)) return false;

            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                GameObject spawnedEnemy = spawnedEnemies[i];

                if (spawnedEnemy == null) continue;

                CharacterRootController spawnedCharacter = spawnedEnemy.GetComponentInChildren<CharacterRootController>(true);

                if (spawnedCharacter == null) continue;

                character = spawnedCharacter;
                return true;
            }

            return false;
        }

        //  3. Enemy Spawning
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
        //  1. Spawn Entry Queries
        protected bool TryGetFirstSpawnEntryTyped(out T spawnEntry)
        {
            spawnEntry = null;

            if (this.EnemySpawnEntries == null) return false;
            if (this.EnemySpawnEntries.Count <= 0) return false;

            T firstEntry = this.EnemySpawnEntries[0];

            if (firstEntry == null) return false;
            if (!firstEntry.IsValid) return false;

            spawnEntry = firstEntry;
            return true;
        }

        //  2. Spawning
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

            if (remainingSpawnCount <= 0) return;

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

            this.ConfigureSpawnedEnemy(spawnedEnemy, spawnEntry);

            CharacterRefreshHandler enemyRefreshHandler = spawnedEnemy.GetComponentInChildren<CharacterRefreshHandler>(true);
            if (enemyRefreshHandler == null)
            {
                Debug.LogWarning($"[{spawnedEnemy.name}.{nameof(EnemyGroupSpawnerController<T>)}] Spawned enemy has no {nameof(CharacterRefreshHandler)}.", spawnedEnemy);

                this.NotifyEnemySpawned(spawnEntry, spawnedEnemy);

                return spawnedEnemy;
            }

            CharacterRefreshTarget spawnResult = spawnEntry.ResolveSpawnResult();
            enemyRefreshHandler.Refresh(spawnResult, CharacterRefreshOrigin.DefaultSpawnPoint);

            this.NotifyEnemySpawned(spawnEntry, spawnedEnemy);

            return spawnedEnemy;
        }
        protected virtual void ConfigureSpawnedEnemy(GameObject spawnedEnemy, T spawnEntry)
        {
            if (spawnedEnemy == null) return;
            if (spawnEntry == null) return;

            this.ConfigureSpawnedEnemyInventory(spawnedEnemy, spawnEntry);
        }
        private void ConfigureSpawnedEnemyInventory(GameObject spawnedEnemyGameObject, T spawnEntry)
        {
            EnemyRootController spawnedEnemy = spawnedEnemyGameObject.GetComponentInChildren<EnemyRootController>(true);
            if (spawnedEnemy == null)
            {
                Debug.LogWarning($"[{spawnedEnemyGameObject.name}.{nameof(EnemyGroupSpawnerController<T>)}] Spawned enemy has no {nameof(EnemyRootController)}.", spawnedEnemyGameObject);
                return;
            }

            InventoryController inventoryController = spawnedEnemy.InventoryController;
            if (inventoryController == null)
            {
                Debug.LogWarning($"[{spawnedEnemyGameObject.name}.{nameof(EnemyGroupSpawnerController<T>)}] Spawned enemy has no {nameof(InventoryController)}.", spawnedEnemyGameObject);
                return;
            }

            inventoryController.InitializeInventoryEntries(spawnEntry.InitialInventoryEntries);
        }
        private void NotifyEnemySpawned(T spawnEntry, GameObject spawnedEnemy)
        {
            if (spawnEntry == null) return;
            if (spawnedEnemy == null) return;

            CharacterRootController character = spawnedEnemy.GetComponentInChildren<CharacterRootController>(true);
            if (character == null)
            {
                Debug.LogWarning($"[{spawnedEnemy.name}.{nameof(EnemyGroupSpawnerController<T>)}] " + $"Spawned enemy has no {nameof(CharacterRootController)}.", spawnedEnemy);
                return;
            }

            this.TypedEnemySpawned?.Invoke(new EnemySpawnedContext<T>(spawnEntry, character));

            this.NotifyEnemySpawnedBase(spawnEntry, character);
        }

        //  3. Runtime Enemy Registration
        protected void ClearSpawnedEnemies()
        {
            for (int i = this.SpawnedEnemies.Count - 1; i >= 0; i--)
            {
                GameObject spawnedEnemy = this.SpawnedEnemies[i];

                if (spawnedEnemy != null) UnityEngine.Object.Destroy(spawnedEnemy);
            }

            this.SpawnedEnemies.Clear();
            _spawnedEnemiesByEntry.Clear();
        }
        private void RegisterSpawnedEnemy(T spawnEntry, GameObject spawnedEnemy)
        {
            if (spawnEntry == null || spawnedEnemy == null) return;

            this.SpawnedEnemies.Add(spawnedEnemy);

            if (!this.SpawnedEnemiesByEntry.TryGetValue(spawnEntry, out List<GameObject> spawnedEnemiesForEntry))
            {
                spawnedEnemiesForEntry = new List<GameObject>();
                _spawnedEnemiesByEntry.Add(spawnEntry, spawnedEnemiesForEntry);
            }

            spawnedEnemiesForEntry.Add(spawnedEnemy);
        }
        private int GetSpawnedEnemyCount(T spawnEntry)
        {
            if (spawnEntry == null) return 0;
            if (!this.SpawnedEnemiesByEntry.TryGetValue(spawnEntry, out List<GameObject> spawnedEnemiesForEntry)) return 0;

            spawnedEnemiesForEntry.RemoveAll(spawnedEnemy => spawnedEnemy == null);

            return spawnedEnemiesForEntry.Count;
        }
        private void PruneDestroyedSpawnedEnemies()
        {
            this.SpawnedEnemies.RemoveAll(spawnedEnemy => spawnedEnemy == null);

            List<T> emptySpawnEntries = new();

            foreach (KeyValuePair<T, List<GameObject>> pair in this.SpawnedEnemiesByEntry)
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

        //  4. Event Handling
        private void HandlePlayerSpawnedOrRespawned()
        {
            if (!this.IsRespawnedEnemiesWhenPlayerSpawnsOrRespawns) return;

            this.SpawnEnemiesAfterPlayerRespawn();
        }

        //  5. Subscriptions
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

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.SpawnedEnemies.Clear();
            _spawnedEnemiesByEntry.Clear();

            this.HasBoundExternalReferences = false;
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.PlayerSpawnerController == null)
            {
                if (this.transform.parent != null) _playerSpawnerController = this.transform.parent.GetComponentInChildren<PlayerSpawnerController>();
                if (this.PlayerSpawnerController == null) _playerSpawnerController = UnityEngine.Object.FindFirstObjectByType<PlayerSpawnerController>();
            }

            if (this.EnemyGroupRoot == null)
            {
                GameObject enemyGroupRoot = GameObject.Find("EnemyGroup");
                _enemyGroupRoot = enemyGroupRoot != null ? enemyGroupRoot.transform : this.transform;
            }
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.PlayerSpawnerController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(EnemyGroupSpawnerController<T>)}] No {nameof(FlameOfTheLost.PlayerSpawnerController)} assigned.", this);
            }
            if (this.EnemyGroupRoot == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(EnemyGroupSpawnerController<T>)}] No {nameof(this.EnemyGroupRoot)} assigned.", this);
            }
        }
        protected override void BindExternalReferences()
        {
            base.BindExternalReferences();

            this.HasBoundExternalReferences = true;
            this.SubscribeToPlayerSpawner();
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            if (!this.IsSpawnedEnemiesOnStart) return;

            this.SpawnEnemiesOnStart();
        }
    }
}
