using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EnemyGroupSpawnerController : FOTLMonoBehaviour
    {
        // --- States ---
        [Header("References (Player Spawner)")]
        [SerializeField] private PlayerSpawnerController _playerSpawnerController;
        private PlayerSpawnerController PlayerSpawnerController => _playerSpawnerController;

        [Header("References (Enemy Respawns)")]
        [SerializeField] private List<EnemyRespawnHandler> _enemyRespawnHandlers = new();
        private IReadOnlyList<EnemyRespawnHandler> EnemyRespawnHandlers => _enemyRespawnHandlers;

        [Header("Settings")]
        [SerializeField] private bool _respawnEnemiesWhenPlayerSpawnsOrRespawns = true;
        private bool RespawnEnemiesWhenPlayerSpawnsOrRespawns => _respawnEnemiesWhenPlayerSpawnsOrRespawns;

        // --- Unity Callbacks ---
        private void Reset()
        {
            this.CacheReferences();
        }

        private void OnValidate()
        {
            this.CacheReferences();
        }

        private void OnEnable()
        {
            this.SubscribeToPlayerSpawner();
        }

        private void OnDisable()
        {
            this.UnsubscribeFromPlayerSpawner();
        }

        // --- Public APIs ---
        public void RespawnAllRegisteredEnemies()
        {
            foreach (EnemyRespawnHandler enemyRespawnHandler in this.EnemyRespawnHandlers)
            {
                if (enemyRespawnHandler == null) continue;
                if (!enemyRespawnHandler.RespawnWhenPlayerRespawns) continue;

                enemyRespawnHandler.Respawn();
            }
        }

        // --- Helpers ---
        //  1. Handle Events
        private void HandlePlayerSpawnedOrRespawned()
        {
            if (!this.RespawnEnemiesWhenPlayerSpawnsOrRespawns) return;

            this.RespawnAllRegisteredEnemies();
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
                _playerSpawnerController = this.transform.parent.GetComponentInChildren<PlayerSpawnerController>();
                if (this.PlayerSpawnerController == null)
                {
                    _playerSpawnerController = UnityEngine.Object.FindFirstObjectByType<PlayerSpawnerController>();
                }
            }

            if (_enemyRespawnHandlers == null || this.EnemyRespawnHandlers.Count == 0)
            {
                _enemyRespawnHandlers = UnityEngine.Object.FindObjectsByType<EnemyRespawnHandler>(FindObjectsSortMode.None)
                    .Where(enemyRespawnHandler => enemyRespawnHandler != null)
                    .ToList();
            }
        }
        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.PlayerSpawnerController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.EnemyRespawnGroupController] No PlayerSpawnerController assigned.", this);
            }

            if (this.EnemyRespawnHandlers == null || this.EnemyRespawnHandlers.Count == 0)
            {
                Debug.LogWarning($"[{this.gameObject.name}.EnemyRespawnGroupController] No enemy respawn handlers assigned.", this);
            }
        }
    }
}
