using System.Collections;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerRespawnHandler : PlayerSpawnerSubBehaviourBase
    {
        // --- Configs ---
        [Header("Configs - Respawn")]
        [SerializeField] private float _respawnDelaySeconds = 1.0f;
        private float RespawnDelaySeconds => _respawnDelaySeconds;

        // --- Runtime ---
        private bool _isRespawning;
        public bool IsRespawning => _isRespawning;

        // --- Public APIs ---
        //  1. Death Respawn
        public void RespawnPlayer()
        {
            if (!this.EnsureBound()) return;
            if (this.IsRespawning) return;

            this.StartCoroutine(this.RespawnPlayerRoutine());
        }

        // --- Helpers ---
        //  1. Respawn Routine
        private IEnumerator RespawnPlayerRoutine()
        {
            _isRespawning = true;

            this.SetInputBlocked(true);

            yield return new WaitForSeconds(this.RespawnDelaySeconds);

            // Refresh Player and place at the current recorded spawn.
            bool placementSucceeded = this.PlayerSpawner.PlacePlayer();
            if (!placementSucceeded)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerRespawnHandler)}] Player respawn failed because Player placement was unsuccessful.", this);

                this.SetInputBlocked(false);

                _isRespawning = false;

                yield break;
            }

            // Reconstruct ordinary world content.
            this.PlayerSpawner.WorldRefreshController?.RefreshForPlayerRespawn();

            this.SetInputBlocked(false);

            _isRespawning = false;

            this.PlayerSpawner.NotifyPlayerRespawned();
        }

        //  2. Player Input
        private void SetInputBlocked(bool blocked)
        {
            PlayerInputHandler inputHandler = this.PlayerSpawner.PlayerInputHandler;

            if (inputHandler == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No {nameof(PlayerInputHandler)} found to {(blocked ? "block" : "unblock")} player input.", this);
                return;
            }

            if (blocked)
            {
                inputHandler.BlockAllPlayerInputs();
            }
            else
            {
                inputHandler.UnblockAllPlayerInputs();
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CleanupStates()
        {
            this.StopAllCoroutines();

            if (this.IsRespawning && this.IsBound)
            {
                this.SetInputBlocked(false);
            }

            _isRespawning = false;

            base.CleanupStates();
        }
    }
}