using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerSpawnLocationHandler : PlayerSpawnerSubBehaviourBase
    {
        // --- References ---
        //  1. Default Spawn Points
        [Header("References - Default Spawn Points (Controllers)")]
        [SerializeField] private List<PlayerDefaultSpawnPointController> _defaultSpawnPointCtrls = new();
        private IReadOnlyList<PlayerDefaultSpawnPointController> DefaultSpawnPointCtrls => _defaultSpawnPointCtrls;

        //  2. Checkpoints
        [Header("References - Checkpoints (Controllers)")]
        [SerializeField] private List<CheckpointController> _checkpointCtrls = new();
        private IReadOnlyList<CheckpointController> CheckpointCtrls => _checkpointCtrls;

        // --- Public APIs ---
        //  1. Spawn Point Queries
        public PlayerDefaultSpawnPointController GetFirstDefaultSpawnPoint()
        {
            return this.DefaultSpawnPointCtrls.FirstOrDefault(spawnPoint => spawnPoint != null);
        }

        //  2. Restored Record Validation
        public void ValidateRestoredSpawnRecord()
        {
            if (!this.EnsureBound()) return;

            PlayerSpawnRecord record = this.PlayerSpawner.PlayerSpawnRecord;

            if (record == null) return;

            // Prefer the requested checkpoint if it exists.
            if (record.SpawnPlacementSourceType == CharacterRefreshOrigin.Checkpoint)
            {
                CheckpointController checkpoint = this.FindCheckpointController(record.ActiveCheckpointId);

                if (checkpoint != null) return;

                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Saved checkpoint ID '{record.ActiveCheckpointId}' could not be resolved. Falling back to a default spawn point.", this);
            }
            else
            {
                PlayerDefaultSpawnPointController spawnPoint = this.FindDefaultSpawnPointController(record.ActiveSpawnPointId);

                if (spawnPoint != null) return;

                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Saved default spawn point ID '{record.ActiveSpawnPointId}' could not be resolved.", this);
            }

            // Try the recorded default spawn point.
            PlayerDefaultSpawnPointController recordedDefaultSpawnPoint = this.FindDefaultSpawnPointController(record.ActiveSpawnPointId);

            if (recordedDefaultSpawnPoint != null)
            {
                record.ClearCheckpointSpawn();
                record.SetSpawnPointSpawn(recordedDefaultSpawnPoint.Id);

                record.TrySetSpawnPlacementSource(CharacterRefreshOrigin.DefaultSpawnPoint);

                return;
            }

            // Final fallback.
            PlayerDefaultSpawnPointController fallback = this.GetFirstDefaultSpawnPoint();

            if (fallback == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No valid default spawn point exists for restored Player spawn data.", this);
                return;
            }

            record.ClearCheckpointSpawn();
            record.SetSpawnPointSpawn(fallback.Id);

            record.TrySetSpawnPlacementSource(CharacterRefreshOrigin.DefaultSpawnPoint);
        }

        //  3. Spawn Resolution
        public CharacterRefreshTarget ResolveSpawn()
        {
            if (!this.EnsureBound())
            {
                return new CharacterRefreshTarget(Vector3.zero, Quaternion.identity, CharacterFacingStatus.Side);
            }

            PlayerSpawnRecord record = this.PlayerSpawner.PlayerSpawnRecord;

            if (record.SpawnPlacementSourceType == CharacterRefreshOrigin.Checkpoint)
            {
                CheckpointController checkpoint = this.FindCheckpointController(record.ActiveCheckpointId);

                if (checkpoint != null)
                {
                    return this.CreateSpawnTarget(checkpoint);
                }
            }

            PlayerDefaultSpawnPointController defaultSpawnPoint = this.FindDefaultSpawnPointController(record.ActiveSpawnPointId);
            if (defaultSpawnPoint != null)
            {
                return this.CreateSpawnTarget(defaultSpawnPoint);
            }

            PlayerDefaultSpawnPointController fallback = this.GetFirstDefaultSpawnPoint();
            if (fallback != null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Default spawn point '{record.ActiveSpawnPointId}' was not found. Using first default spawn point.", this);
                return this.CreateSpawnTarget(fallback);
            }

            Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No valid spawn point was found. Using Player's current transform.", this);

            PlayerRootController player = this.PlayerSpawner.PlayerRoot;

            return player != null
                ? new CharacterRefreshTarget(player.transform.position, player.transform.rotation, CharacterFacingStatus.Side)
                : new CharacterRefreshTarget(Vector3.zero, Quaternion.identity, CharacterFacingStatus.Side);
        }

        // --- Internal APIs ---
        internal void EnsureSpawnPointReferences()
        {
            if (_defaultSpawnPointCtrls == null || _defaultSpawnPointCtrls.Count == 0)
            {
                _defaultSpawnPointCtrls = UnityEngine.Object.FindObjectsByType<PlayerDefaultSpawnPointController>(FindObjectsSortMode.None).Where(spawnPoint => spawnPoint != null).ToList();
            }
            if (_checkpointCtrls == null || _checkpointCtrls.Count == 0)
            {
                _checkpointCtrls = UnityEngine.Object.FindObjectsByType<CheckpointController>(FindObjectsSortMode.None).Where(checkpoint => checkpoint != null).ToList();
            }
        }

        // --- Helpers ---
        //  1. Spawn Target Creation
        private CharacterRefreshTarget CreateSpawnTarget(CharacterSpawnPointController spawnPoint)
        {
            return new CharacterRefreshTarget(spawnPoint.SpawnPosition, spawnPoint.SpawnRotation, spawnPoint.DefaultFacing);
        }

        //  2. Default Spawn Point Queries
        private PlayerDefaultSpawnPointController FindDefaultSpawnPointController(int spawnPointId)
        {
            if (spawnPointId < 0) return null;

            return this.DefaultSpawnPointCtrls.FirstOrDefault(spawnPoint => spawnPoint != null && spawnPoint.Id == spawnPointId);
        }

        //  3. Checkpoint Queries
        private CheckpointController FindCheckpointController(int checkpointId)
        {
            if (checkpointId < 0) return null;

            return this.CheckpointCtrls.FirstOrDefault(checkpoint => checkpoint != null && checkpoint.Id == checkpointId);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            this.EnsureSpawnPointReferences();
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.DefaultSpawnPointCtrls == null || this.DefaultSpawnPointCtrls.Count == 0)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No Player default spawn points found.", this);
            }
            if (this.CheckpointCtrls == null || this.CheckpointCtrls.Count == 0)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No Checkpoints found.", this);
            }
        }
    }
}