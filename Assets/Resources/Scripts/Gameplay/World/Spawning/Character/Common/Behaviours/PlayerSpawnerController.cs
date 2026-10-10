using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerSpawnerController : FOTLMonoBehaviour, ISaveDataHandler<PlayerSpawnSaveData>
    {
        // --- References ---
        //  1. Player
        [Header("References - Player")]
        [SerializeField] private PlayerRootController _playerRoot;
        private PlayerRootController PlayerRoot => _playerRoot;
        private PlayerInputHandler PlayerInputHandler => this.PlayerRoot != null ? this.PlayerRoot.PlayerInputHandler : null;
        private CharacterRefreshHandler PlayerRefreshHandler => this.PlayerRoot != null ? this.PlayerRoot.RefreshHandler : null;

        //  2. World Refresh
        [Header("References - World")]
        [SerializeField]
        private WorldRefreshController _worldRefreshController;
        private WorldRefreshController WorldRefreshController => _worldRefreshController;

        //  3. Game Save
        [Header("References - Game Save")]
        [SerializeField] private GameSaveController _gameSaveController;
        private GameSaveController GameSaveController => _gameSaveController;

        //  4. Default Spawn Points
        [Header("References - Default Spawn Points (Controllers)")]
        [SerializeField] private List<PlayerDefaultSpawnPointController> _defaultSpawnPointCtrls = new();
        private IReadOnlyList<PlayerDefaultSpawnPointController> DefaultSpawnPointCtrls => _defaultSpawnPointCtrls;

        //  5. Checkpoints
        [Header("References - Checkpoints (Controllers)")]
        [SerializeField] private List<CheckpointController> _checkpointCtrls = new();
        private IReadOnlyList<CheckpointController> CheckpointCtrls => _checkpointCtrls;

        // --- Data / Configs ---
        [Header("Configs - Spawn/Respawn")]
        [SerializeField] private bool _spawnPlayerOnStart = true;
        [SerializeField] private float _respawnDelaySeconds = 1.0f;
        private bool SpawnPlayerOnStart => _spawnPlayerOnStart;
        private float RespawnDelaySeconds => _respawnDelaySeconds;

        // --- Runtime ---
        //  1. Respawning
        private bool _isRespawning;

        private bool IsRespawning
        {
            get => _isRespawning;
            set => _isRespawning = value;
        }

        //  2. Spawn Record
        [Header("Runtime - Recorded Player Spawn State(s)")]
        [SerializeField] private PlayerSpawnRecord _spawnRecord = new();
        public PlayerSpawnRecord PlayerSpawnRecord => _spawnRecord;

        //  3. Gameplay Initialization
        private bool _hasEnteredGameplay;

        // --- Events ---
        public event Action OnPlayerSpawned;
        public event Action OnPlayerRespawned;
        public event Action OnPlayerReloaded;
        public event Action OnPlayerSpawnedOrRespawned;

        // --- Public APIs ---
        //  1. Initial Spawn
        public void SpawnPlayer()
        {
            this.PlacePlayer();

            this.OnPlayerSpawned?.Invoke();
            this.OnPlayerSpawnedOrRespawned?.Invoke();
        }

        //  2. Death Respawn
        public void RespawnPlayer()
        {
            if (this.IsRespawning) return;

            this.StartCoroutine(this.RespawnPlayerRoutine());
        }

        //  3. Save Reload
        public void ReloadPlayerFromSave()
        {
            /*
             * Persistent Player systems must already have been restored before calling this.
             *
             * PlacePlayer() then refreshes every transient ICharacterRefreshable system.
             */
            this.PlacePlayer();

            this.OnPlayerReloaded?.Invoke();
        }

        //  4. Spawn Point Queries
        public PlayerDefaultSpawnPointController GetFirstDefaultSpawnPoint()
        {
            return this.DefaultSpawnPointCtrls.FirstOrDefault(spawnPoint => spawnPoint != null);
        }

        //  5. Activation
        public void ActivateSpawnPoint(PlayerDefaultSpawnPointController spawnPoint)
        {
            if (spawnPoint == null) return;

            this.PlayerSpawnRecord.SetSpawnPointSpawn(spawnPoint.Id);

            //Debug.Log($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Activated spawn point: {spawnPoint.DisplayName}", spawnPoint);

            // Automatically save after updating the active spawn point.
            this.TryAutoSaveAfterActivation();
        }
        public void ActivateCheckpoint(CheckpointController checkpoint)
        {
            if (checkpoint == null) return;

            this.PlayerSpawnRecord.SetCheckpointSpawn(checkpoint.Id);

            //Debug.Log($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Activated checkpoint: {checkpoint.CheckpointId}", checkpoint);

            // Automatically save after updating the active spawn point.
            this.TryAutoSaveAfterActivation();
        }

        //  6. ISaveDataHandler<PlayerSpawnSaveData>
        public PlayerSpawnSaveData CaptureSaveData()
        {
            return this.PlayerSpawnRecord?.CaptureSaveData() ?? new PlayerSpawnSaveData();
        }
        public void RestoreSaveData(PlayerSpawnSaveData saveData)
        {
            if (this.PlayerSpawnRecord == null) _spawnRecord = new PlayerSpawnRecord();

            this.PlayerSpawnRecord.RestoreSaveData(saveData);

            this.ValidateRestoredSpawnRecord();
        }

        // --- Helpers ---
        //  1. Placement
        private void PlacePlayer()
        {
            if (this.PlayerRefreshHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No {nameof(this.PlayerRefreshHandler)} assigned.", this);
                return;
            }

            CharacterRefreshTarget spawnResult = this.ResolveSpawn();

            this.PlayerRefreshHandler.Refresh(spawnResult, this.PlayerSpawnRecord.SpawnPlacementSourceType);
        }

        //  2. Death Respawn
        private IEnumerator RespawnPlayerRoutine()
        {
            this.IsRespawning = true;

            this.DisablePlayerBeforeRespawn();

            yield return new WaitForSeconds(this.RespawnDelaySeconds);

            // Refresh Player and place at current recorded spawn.
            this.PlacePlayer();

            // Reconstruct ordinary world content.
            this.WorldRefreshController?.RefreshForPlayerRespawn();

            this.EnablePlayerAfterRespawn();

            this.IsRespawning = false;

            this.OnPlayerRespawned?.Invoke();
            this.OnPlayerSpawnedOrRespawned?.Invoke();
        }
        private void DisablePlayerBeforeRespawn()
        {
            if (this.PlayerInputHandler == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No PlayerInputHandler found to block player input.", this);
                return;
            }

            this.PlayerInputHandler.BlockAllPlayerInputs();
        }
        private void EnablePlayerAfterRespawn()
        {
            if (this.PlayerInputHandler == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No PlayerInputHandler found to unblock player input.", this);
                return;
            }

            this.PlayerInputHandler.UnblockAllPlayerInputs();
        }

        //  3. Spawn Record Initialization
        private void InitializeFirstDefaultSpawnPointIfNeeded()
        {
            if (this.PlayerSpawnRecord.SpawnPlacementSourceType == CharacterRefreshOrigin.Checkpoint) return;

            PlayerDefaultSpawnPointController firstDefaultSpawnPoint = this.GetFirstDefaultSpawnPoint();
            if (firstDefaultSpawnPoint == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No default spawn point found during initialization.", this);
                return;
            }

            this.ActivateSpawnPoint(firstDefaultSpawnPoint);
        }

        //  4. Restored Record Validation
        private void ValidateRestoredSpawnRecord()
        {
            if (this.PlayerSpawnRecord == null) return;

            // Prefer the requested checkpoint if it exists.
            if (this.PlayerSpawnRecord.SpawnPlacementSourceType == CharacterRefreshOrigin.Checkpoint)
            {
                CheckpointController checkpoint = this.FindCheckpointController(this.PlayerSpawnRecord.ActiveCheckpointId);
                if (checkpoint != null) return;

                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Saved checkpoint ID '{this.PlayerSpawnRecord.ActiveCheckpointId}' could not be resolved. Falling back to a default spawn point.", this);
            }
            else
            {
                PlayerDefaultSpawnPointController spawnPoint = this.FindDefaultSpawnPointController(this.PlayerSpawnRecord.ActiveSpawnPointId);
                if (spawnPoint != null) return;
                
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Saved default spawn point ID '{this.PlayerSpawnRecord.ActiveSpawnPointId}' could not be resolved.", this);
            }

            // Try the stored default spawn point even if the saved active source was a missing checkpoint.
            PlayerDefaultSpawnPointController recordedDefaultSpawnPoint = this.FindDefaultSpawnPointController(this.PlayerSpawnRecord.ActiveSpawnPointId);
            if (recordedDefaultSpawnPoint != null)
            {
                this.PlayerSpawnRecord.ClearCheckpointSpawn();
                this.PlayerSpawnRecord.SetSpawnPointSpawn(recordedDefaultSpawnPoint.Id);
                this.PlayerSpawnRecord.TrySetSpawnPlacementSource(CharacterRefreshOrigin.DefaultSpawnPoint);

                return;
            }

            // Final fallback.
            PlayerDefaultSpawnPointController fallback = this.GetFirstDefaultSpawnPoint();
            if (fallback == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No valid default spawn point exists for restored Player spawn data.", this);
                return;
            }

            this.PlayerSpawnRecord.ClearCheckpointSpawn();
            this.PlayerSpawnRecord.SetSpawnPointSpawn(fallback.Id);
            this.PlayerSpawnRecord.TrySetSpawnPlacementSource(CharacterRefreshOrigin.DefaultSpawnPoint);
        }

        //  5. Spawn Resolution
        private CharacterRefreshTarget ResolveSpawn()
        {
            if (this.PlayerSpawnRecord.SpawnPlacementSourceType == CharacterRefreshOrigin.Checkpoint)
            {
                CheckpointController checkpointCtrl = this.FindCheckpointController(this.PlayerSpawnRecord.ActiveCheckpointId);
                if (checkpointCtrl != null) return this.CreateSpawnTarget(checkpointCtrl);

                //Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Checkpoint '{this.PlayerSpawnRecord.ActiveCheckpointId}' was not found. Falling back to default spawn.", this);
            }

            PlayerDefaultSpawnPointController defaultSpawnPointCtrl = this.FindDefaultSpawnPointController(this.PlayerSpawnRecord.ActiveSpawnPointId);
            if (defaultSpawnPointCtrl != null) return this.CreateSpawnTarget(defaultSpawnPointCtrl);

            PlayerDefaultSpawnPointController fallbackDefaultSpawnPoint = this.DefaultSpawnPointCtrls.FirstOrDefault(spawnPoint => spawnPoint != null);
            if (fallbackDefaultSpawnPoint != null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Default spawn point '{this.PlayerSpawnRecord.ActiveSpawnPointId}' was not found. Using first default spawn point.", this);
                return this.CreateSpawnTarget(fallbackDefaultSpawnPoint);
            }

            Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No valid spawn point was found. Using Player's current transform.", this);
            if (this.PlayerRoot != null)
            {
                return new CharacterRefreshTarget
                (
                    this.PlayerRoot.transform.position,
                    this.PlayerRoot.transform.rotation,
                    CharacterFacingStatus.Side
                );
            }

            return new CharacterRefreshTarget
            (
                Vector3.zero,
                Quaternion.identity,
                CharacterFacingStatus.Side
            );
        }
        private CharacterRefreshTarget CreateSpawnTarget(CharacterSpawnPointController spawnPoint)
        {
            return new CharacterRefreshTarget
            (
                spawnPoint.SpawnPosition,
                spawnPoint.SpawnRotation,
                spawnPoint.DefaultFacing
            );
        }

        //  6. Queries
        private PlayerDefaultSpawnPointController FindDefaultSpawnPointController(int spawnPointId)
        {
            if (spawnPointId < 0) return null;

            return this.DefaultSpawnPointCtrls.FirstOrDefault(spawnPoint => spawnPoint != null && spawnPoint.Id == spawnPointId);
        }
        private CheckpointController FindCheckpointController(int checkpointId)
        {
            if (checkpointId < 0) return null;

            return this.CheckpointCtrls.FirstOrDefault(checkpoint => checkpoint != null && checkpoint.Id == checkpointId);
        }

        //  7. Automatic Saving
        private void TryAutoSaveAfterActivation()
        {
            // Do not save during initial scene initialization or respawning.
            if (!_hasEnteredGameplay || this.IsRespawning) return;

            if (this.GameSaveController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Cannot autosave: {nameof(GameSaveController)} is missing.", this);
                return;
            }

            if (!this.GameSaveController.SaveGame())
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Autosave failed after spawn/checkpoint activation.", this);
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.IsRespawning = false;
            _hasEnteredGameplay = false;

            if (this.PlayerSpawnRecord == null)
            {
                _spawnRecord = new PlayerSpawnRecord();
            }
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.PlayerRoot == null)
            {
                GameObject player = GameObject.Find("Player");
                _playerRoot = player != null ? player.GetComponent<PlayerRootController>() : null;
            }

            if (this.WorldRefreshController == null) _worldRefreshController = UnityEngine.Object.FindFirstObjectByType<WorldRefreshController>();

            if (this.GameSaveController == null) _gameSaveController = UnityEngine.Object.FindFirstObjectByType<GameSaveController>();

            if (_defaultSpawnPointCtrls == null || _defaultSpawnPointCtrls.Count == 0)
            {
                _defaultSpawnPointCtrls = UnityEngine.Object.FindObjectsByType<PlayerDefaultSpawnPointController>(FindObjectsSortMode.None)
                    .Where(spawnPoint => spawnPoint != null)
                    .ToList();
            }
            if (_checkpointCtrls == null || this.CheckpointCtrls.Count == 0)
            {
                _checkpointCtrls = UnityEngine.Object.FindObjectsByType<CheckpointController>(FindObjectsSortMode.None)
                    .Where(checkpoint => checkpoint != null)
                    .ToList();
            }
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.PlayerRoot == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] PlayerRoot reference is missing.", this);
            }

            if (this.PlayerRefreshHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Player's {nameof(CharacterRefreshHandler)} reference is missing.", this);
            }

            if (this.PlayerInputHandler == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] {nameof(PlayerInputHandler)} reference is missing. Respawn input blocking will not work.", this);
            }

            if (this.DefaultSpawnPointCtrls == null || this.DefaultSpawnPointCtrls.Count == 0)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No Player default spawn points found.", this);
            }
            if (this.CheckpointCtrls == null || this.CheckpointCtrls.Count == 0)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No Checkpoints found.", this);
            }
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.InitializeFirstDefaultSpawnPointIfNeeded();
        }
        protected override void InitializeGameplay()
        {
            base.InitializeGameplay();

            if (this.SpawnPlayerOnStart)
            {
                this.SpawnPlayer();
            }

            // Allow automatic saving only after initial gameplay setup.
            _hasEnteredGameplay = true;
        }
    }
}
