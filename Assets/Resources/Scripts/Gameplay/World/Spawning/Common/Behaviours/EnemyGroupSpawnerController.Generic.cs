using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EnemyGroupSpawnerController<T> : EnemyGroupSpawnerController where T : EnemySpawnEntry
    {
        // --- References ---
        //  1. Player Spawner
        [Header("References - Player Spawner")]
        [SerializeField] private PlayerSpawnerController _playerSpawnerController;
        private PlayerSpawnerController PlayerSpawnerController => _playerSpawnerController;

        //  2. Enemy Root
        [Header("References - Enemy Root")]
        [SerializeField] private Transform _enemyGroupRoot;
        protected Transform EnemyGroupRoot => _enemyGroupRoot;

        //  3. Enemy Default Spawn Points
        [Header("References - Enemy Default Spawn Points")]
        [SerializeField] private List<EnemyDefaultSpawnPointController> _enemyDefaultSpawnPointControllers = new();
        protected IReadOnlyList<EnemyDefaultSpawnPointController> EnemyDefaultSpawnPointControllers => _enemyDefaultSpawnPointControllers;

        // --- Configs ---
        [Header("Settings")]
        [SerializeField] private bool _isSpawnedEnemiesOnStart = true;
        [SerializeField] private bool _isRespawnedEnemiesWhenPlayerSpawnsOrRespawns = true;
        private bool IsSpawnedEnemiesOnStart => _isSpawnedEnemiesOnStart;
        private bool IsRespawnedEnemiesWhenPlayerSpawnsOrRespawns => _isRespawnedEnemiesWhenPlayerSpawnsOrRespawns;

        // --- Runtime ---
        //  1. Spawned Enemies
        [Header("Runtime - Enemies (GameObject)")]
        [SerializeField] private List<GameObject> _spawnedEnemies = new();
        protected List<GameObject> SpawnedEnemies => _spawnedEnemies;

        //  2. Spawned Enemies By Entry
        private readonly Dictionary<T, List<GameObject>> _spawnedEnemiesByEntry = new();
        private IReadOnlyDictionary<T, List<GameObject>> SpawnedEnemiesByEntry => _spawnedEnemiesByEntry;

        //  3. External Binding State
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

            if (!this.TryGetFirstSpawnEntryTyped(out _, out T typedSpawnEntry)) return false;

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
            foreach (EnemyDefaultSpawnPointController spawnPoint in this.EnemyDefaultSpawnPointControllers)
            {
                if (spawnPoint == null) continue;

                foreach (T spawnEntry in this.GetSpawnEntries(spawnPoint))
                {
                    if (spawnEntry == null) continue;
                    if (!spawnEntry.SpawnOnStart) continue;

                    this.SpawnFromEntry(spawnPoint, spawnEntry);
                }
            }
        }
        public void SpawnEnemiesAfterPlayerRespawn()
        {
            // this.ClearSpawnedEnemies();

            foreach (EnemyDefaultSpawnPointController spawnPoint in this.EnemyDefaultSpawnPointControllers)
            {
                foreach (T spawnEntry in this.GetSpawnEntries(spawnPoint))
                {
                    if (spawnEntry == null) continue;
                    if (!spawnEntry.SpawnWhenPlayerRespawns) continue;

                    this.SpawnFromEntry(spawnPoint, spawnEntry);
                }
            }
        }

        // --- Helpers ---
        //  1. Spawn Entry Queries
        protected bool TryGetFirstSpawnEntryTyped(out EnemyDefaultSpawnPointController spawnPoint, out T spawnEntry)
        {
            spawnPoint = null;
            spawnEntry = null;

            if (this.EnemyDefaultSpawnPointControllers == null) return false;

            for (int spawnPointIndex = 0; spawnPointIndex < this.EnemyDefaultSpawnPointControllers.Count; spawnPointIndex++)
            {
                EnemyDefaultSpawnPointController candidateSpawnPoint = this.EnemyDefaultSpawnPointControllers[spawnPointIndex];

                if (candidateSpawnPoint == null) continue;

                foreach (T candidateSpawnEntry in this.GetSpawnEntries(candidateSpawnPoint))
                {
                    if (candidateSpawnEntry == null) continue;
                    if (!candidateSpawnEntry.IsValid) continue;

                    spawnPoint = candidateSpawnPoint;
                    spawnEntry = candidateSpawnEntry;

                    return true;
                }
            }

            return true;
        }

        protected IEnumerable<T> GetSpawnEntries(EnemyDefaultSpawnPointController spawnPoint)
        {
            if (spawnPoint == null) yield break;
            if (spawnPoint.EnemySpawnEntries == null) yield break;

            IReadOnlyList<EnemySpawnEntry> enemySpawnEntries = spawnPoint.EnemySpawnEntries;

            for (int i = 0; i < enemySpawnEntries.Count; i++)
            {
                EnemySpawnEntry enemySpawnEntry = enemySpawnEntries[i];

                if (enemySpawnEntry is not T typedSpawnEntry) continue;

                yield return typedSpawnEntry;
            }
        }

        //  2. Spawning
        protected virtual void SpawnFromEntry(EnemyDefaultSpawnPointController spawnPoint, T spawnEntry)
        {
            if (spawnPoint == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(EnemyGroupSpawnerController<T>)}] Enemy default spawn point is null.", this);
                return;
            }
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
                this.SpawnSingleEnemy(spawnPoint, spawnEntry);
            }
        }
        protected virtual GameObject SpawnSingleEnemy(EnemyDefaultSpawnPointController spawnPoint, T spawnEntry)
        {
            if (spawnPoint == null) return null;
            if (spawnEntry == null) return null;

            EnemyVariantProfile enemyVariant = spawnEntry.EnemyVariant;

            if (enemyVariant == null) return null;
            if (enemyVariant.CharacterProfile.Prefab == null) return null;

            GameObject spawnedEnemy = UnityEngine.Object.Instantiate(enemyVariant.CharacterProfile.Prefab, this.EnemyGroupRoot);

            if (spawnedEnemy == null) return null;

            this.RegisterSpawnedEnemy(spawnEntry, spawnedEnemy);

            this.ConfigureSpawnedEnemy(spawnedEnemy, spawnPoint, spawnEntry);

            CharacterRefreshHandler enemyRefreshHandler = spawnedEnemy.GetComponentInChildren<CharacterRefreshHandler>(true);
            if (enemyRefreshHandler == null)
            {
                Debug.LogWarning($"[{spawnedEnemy.name}.{nameof(EnemyGroupSpawnerController<T>)}] Spawned enemy has no {nameof(CharacterRefreshHandler)}.", spawnedEnemy);

                this.NotifyEnemySpawned(spawnPoint, spawnEntry, spawnedEnemy);

                return spawnedEnemy;
            }

            CharacterRefreshTarget spawnResult = spawnEntry.ResolveSpawnResult(spawnPoint);
            enemyRefreshHandler.Refresh(spawnResult, CharacterRefreshOrigin.DefaultSpawnPoint);

            this.NotifyEnemySpawned(spawnPoint, spawnEntry, spawnedEnemy);

            return spawnedEnemy;
        }
        protected virtual void ConfigureSpawnedEnemy(GameObject spawnedEnemy, EnemyDefaultSpawnPointController spawnPoint, T spawnEntry)
        {
            if (spawnedEnemy == null) return;
            if (spawnPoint == null) return;
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

            EnemyVariantProfile enemyVariant = spawnEntry.EnemyVariant;

            if (enemyVariant == null) return;

            inventoryController.InitializeInventoryEntries(enemyVariant.InitialInventoryEntries);
        }
        private void NotifyEnemySpawned(EnemyDefaultSpawnPointController spawnPoint, T spawnEntry, GameObject spawnedEnemy)
        {
            if (spawnPoint == null) return;
            if (spawnEntry == null) return;
            if (spawnedEnemy == null) return;

            CharacterRootController character = spawnedEnemy.GetComponentInChildren<CharacterRootController>(true);
            if (character == null)
            {
                Debug.LogWarning($"[{spawnedEnemy.name}.{nameof(EnemyGroupSpawnerController<T>)}] " + $"Spawned enemy has no {nameof(CharacterRootController)}.", spawnedEnemy);
                return;
            }

            this.TypedEnemySpawned?.Invoke(new EnemySpawnedContext<T>(spawnPoint, spawnEntry, character));

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
