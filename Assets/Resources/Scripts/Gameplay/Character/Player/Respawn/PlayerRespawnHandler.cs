using System.Collections;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerRespawnHandler : CharacterRespawnHandler
    {
        // --- States ---
        [Header("References (Spawner)")]
        [SerializeField] private PlayerSpawnerController _playerSpawnerCtrl;
        private PlayerSpawnerController PlayerSpawnerController => _playerSpawnerCtrl;

        private PlayerInputHandler PlayerInputHandler
        {
            get
            {
                var playerRootController = this.CharacterRootController as PlayerRootController;
                return playerRootController != null ? playerRootController.PlayerInputHandler : null;
            }
        }

        [Header("Respawn Settings")]
        [SerializeField] private float _respawnDelaySeconds = 1.0f;
        private float RespawnDelaySeconds => _respawnDelaySeconds;  

        private bool _isRespawning;
        private bool IsRespawning
        {
            get => _isRespawning;
            set => _isRespawning = value;
        }

        // --- Public APIs ---
        public void RegisterVisitedSpawnPoint(PlayerDefaultSpawnPointController spawnPoint)
        {
            if (spawnPoint == null) return;

            if (this.PlayerSpawnerController == null)
            {
                Debug.LogError($"[{this.CharacterName}.PlayerRespawnHandler] No PlayerSpawnerController assigned.", this);
                return;
            }

            this.PlayerSpawnerController.ActivateSpawnPoint(spawnPoint);
        }

        public override void Respawn()
        {
            if (this.IsRespawning) return;
            
            this.StartCoroutine(this.RespawnRoutine());
        }

        // --- Helpers ---
        private IEnumerator RespawnRoutine()
        {
            this.IsRespawning = true;

            this.DisablePlayerBeforeRespawn();

            yield return new WaitForSeconds(this.RespawnDelaySeconds);

            if (this.PlayerSpawnerController != null)
            {
                this.PlayerSpawnerController.RespawnPlayer();
            }
            else
            {
                Debug.LogError($"[{this.CharacterName}.PlayerRespawnHandler] No PlayerSpawnerController assigned.", this);
            }

            this.EnablePlayerAfterRespawn();

            this.IsRespawning = false;
        }

        private void DisablePlayerBeforeRespawn()
        {
            if (this.PlayerInputHandler == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.PlayerRespawnHandler] No PlayerInputHandler found to block inputs.", this);
                return;
            }

            this.PlayerInputHandler.BlockAllPlayerInputs();
        }

        private void EnablePlayerAfterRespawn()
        {
            if (this.PlayerInputHandler == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.PlayerRespawnHandler] No PlayerInputHandler found to unblock inputs.", this);
                return;
            }

            this.PlayerInputHandler.UnblockAllPlayerInputs();
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.PlayerSpawnerController == null)
            {
                GameObject playerSpawner = GameObject.Find("PlayerSpawner");
                _playerSpawnerCtrl = playerSpawner.GetComponent<PlayerSpawnerController>();
            }
        }
    }
}