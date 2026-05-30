using System.Collections;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerRespawnHandler : CharacterRespawnHandler
    {
        // --- States ---
        [Header("References")]
        [SerializeField] private PlayerInputHandler _playerInputHandler;
        private PlayerInputHandler PlayerInputHandler => _playerInputHandler;

        [Header("Spawner")]
        [SerializeField] private PlayerSpawnerController _playerSpawnerCtrl;
        private PlayerSpawnerController PlayerSpawnerController => _playerSpawnerCtrl;

        [Header("Respawn Settings")]
        [SerializeField] private float _respawnDelaySeconds = 1.0f;
        private float RespawnDelaySeconds => _respawnDelaySeconds;  

        private bool _isRespawning;
        private bool IsRespawning
        {
            get => _isRespawning;
            set => _isRespawning = value;
        }

        // -- Unity Callbacks ---
        private void Start()
        {
            PlayerDefaultSpawnPointController firstDefaultSpawnPoint = this.PlayerSpawnerController.GetFirstDefaultSpawnPoint();
            if (firstDefaultSpawnPoint != null) this.RegisterVisitedSpawnPoint(firstDefaultSpawnPoint);
        }

        // --- Public APIs ---
        public void RegisterVisitedSpawnPoint(PlayerDefaultSpawnPointController spawnPoint)
        {
            if (spawnPoint == null) return;

            if (this.PlayerSpawnerController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.PlayerRespawnHandler] No PlayerSpawnerController assigned.", this);
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
                Debug.LogError($"[{this.gameObject.name}.PlayerRespawnHandler] No PlayerSpawnerController assigned.", this);
            }

            this.EnablePlayerAfterRespawn();

            this.IsRespawning = false;
        }

        private void DisablePlayerBeforeRespawn()
        {
            if (this.PlayerInputHandler == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.PlayerRespawnHandler] No PlayerInputHandler found to block inputs.", this);
                return;
            }

            this.PlayerInputHandler.BlockAllPlayerInputs();
        }

        private void EnablePlayerAfterRespawn()
        {
            if (this.PlayerInputHandler == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.PlayerRespawnHandler] No PlayerInputHandler found to unblock inputs.", this);
                return;
            }

            this.PlayerInputHandler.UnblockAllPlayerInputs();
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.PlayerInputHandler == null) _playerInputHandler = this.GetComponent<PlayerInputHandler>();
        }
    }
}