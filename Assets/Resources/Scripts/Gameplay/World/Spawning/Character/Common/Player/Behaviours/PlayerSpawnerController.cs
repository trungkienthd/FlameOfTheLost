using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(PlayerSpawnLocationHandler))]
    [RequireComponent(typeof(PlayerRespawnHandler))]
    [RequireComponent(typeof(PlayerSpawnSessionHandler))]
    [RequireComponent(typeof(PlayerSpawnAutoSaveHandler))]
    public class PlayerSpawnerController : FOTLMonoBehaviour, ISaveDataHandler<PlayerSpawnSaveData>
    {
        // --- References ---
        //  1. Player
        [Header("References - Player")]
        [SerializeField] private PlayerRootController _playerRoot;
        internal PlayerRootController PlayerRoot => _playerRoot;
        internal PlayerInputHandler PlayerInputHandler => this.PlayerRoot != null ? this.PlayerRoot.PlayerInputHandler : null;
        internal CharacterRefreshHandler PlayerRefreshHandler => this.PlayerRoot != null ? this.PlayerRoot.RefreshHandler : null;

        //  2. World Refresh
        [Header("References - World")]
        [SerializeField] private WorldRefreshController _worldRefreshController;
        internal WorldRefreshController WorldRefreshController => _worldRefreshController;

        //  3. Game Save
        [Header("References - Game Save")]
        [SerializeField] private GameSaveController _gameSaveController;
        internal GameSaveController GameSaveController => _gameSaveController;

        //  4. Sub-Behaviours
        [Header("References - Sub-behaviours")]
        [SerializeField] private PlayerSpawnLocationHandler _locationHandler;
        [SerializeField] private PlayerRespawnHandler       _respawnHandler;
        [SerializeField] private PlayerSpawnSessionHandler  _sessionHandler;
        [SerializeField] private PlayerSpawnAutoSaveHandler _autoSaveHandler;
        internal PlayerSpawnLocationHandler LocationHandler => _locationHandler;
        internal PlayerRespawnHandler       RespawnHandler  => _respawnHandler;
        internal PlayerSpawnSessionHandler  SessionHandler  => _sessionHandler;
        internal PlayerSpawnAutoSaveHandler AutoSaveHandler => _autoSaveHandler;

        // --- Runtime ---
        //  1. Spawn Record
        [Header("Runtime - Recorded Player Spawn State(s)")]
        [SerializeField] private PlayerSpawnRecord _spawnRecord = new();
        public PlayerSpawnRecord PlayerSpawnRecord => _spawnRecord;

        //  2. Shared State
        public bool IsSessionInitializing => this.SessionHandler != null && this.SessionHandler.IsSessionInitializing;
        public bool IsRespawning => this.RespawnHandler != null && this.RespawnHandler.IsRespawning;
        public bool HasEnteredGameplay => this.SessionHandler != null && this.SessionHandler.HasEnteredGameplay;

        // --- Events ---
        public event Action OnPlayerSpawned;
        public event Action OnPlayerRespawned;
        public event Action OnPlayerReloaded;
        public event Action OnPlayerSpawnedOrRespawned;

        // --- Public APIs ---
        //  1. Initial Spawn
        public void SpawnPlayer()
        {
            if (!this.PlacePlayer()) return;

            this.OnPlayerSpawned?.Invoke();
            this.OnPlayerSpawnedOrRespawned?.Invoke();
        }

        //  2. Death Respawn
        public void RespawnPlayer()
        {
            this.RespawnHandler?.RespawnPlayer();
        }

        //  3. Save Reload
        public void ReloadPlayerFromSave()
        {
            if (!this.PlacePlayer()) return;

            this.OnPlayerReloaded?.Invoke();
        }

        //  4. World Location Placement
        public bool PlacePlayerAtWorldLocation(Vector3 position, CharacterRefreshOrigin refreshOrigin, CharacterFacingStatus facing = CharacterFacingStatus.Side)
        {
            if (this.PlayerRefreshHandler == null)
            {
                Debug.LogError($"[{nameof(PlayerSpawnerController)}] {nameof(PlayerRefreshHandler)} is missing.", this);
                return false;
            }

            CharacterRefreshTarget target = new CharacterRefreshTarget(position, Quaternion.identity, facing);

            this.PlayerRefreshHandler.Refresh(target, refreshOrigin);

            return true;
        }

        //  5. Spawn Point Queries
        public PlayerDefaultSpawnPointController GetFirstDefaultSpawnPoint()
        {
            return this.LocationHandler != null
                ? this.LocationHandler.GetFirstDefaultSpawnPoint()
                : null;
        }
        public PlayerSpawnSaveData CreateSceneDefaultSpawnSaveData()
        {
            PlayerDefaultSpawnPointController spawnPoint = this.GetFirstDefaultSpawnPoint();

            return new PlayerSpawnSaveData
            {
                SpawnPlacementSourceType = CharacterRefreshOrigin.DefaultSpawnPoint,
                ActiveSpawnPointId = spawnPoint != null ? spawnPoint.Id : -1,
                ActiveCheckpointId = -1
            };
        }

        //  6. Activation
        public void ActivateSpawnPoint(PlayerDefaultSpawnPointController spawnPoint)
        {
            if (spawnPoint == null) return;

            this.PlayerSpawnRecord.SetSpawnPointSpawn(spawnPoint.Id);

            this.AutoSaveHandler?.TryAutoSaveAfterActivation();
        }
        public void ActivateCheckpoint(CheckpointController checkpoint)
        {
            if (checkpoint == null) return;

            this.PlayerSpawnRecord.SetCheckpointSpawn(checkpoint.Id);

            this.AutoSaveHandler?.TryAutoSaveAfterActivation();
        }

        //  7. ISaveDataHandler<PlayerSpawnSaveData>
        public PlayerSpawnSaveData CaptureSaveData()
        {
            return this.PlayerSpawnRecord?.CaptureSaveData() ?? new PlayerSpawnSaveData();
        }
        public void RestoreSaveData(PlayerSpawnSaveData saveData)
        {
            if (this.PlayerSpawnRecord == null) _spawnRecord = new PlayerSpawnRecord();

            this.PlayerSpawnRecord.RestoreSaveData(saveData);

            this.LocationHandler?.ValidateRestoredSpawnRecord();
        }

        //  8. Session Initialization
        public void CompleteSessionInitialization()
        {
            this.SessionHandler?.CompleteSessionInitialization();
        }

        // --- Internal APIs ---
        //  1. Sub-Behaviour Coordination
        internal bool PlacePlayer()
        {
            if (this.PlayerRefreshHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No {nameof(PlayerRefreshHandler)} assigned.", this);
                return false;
            }

            if (this.LocationHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Location handler is missing.", this);
                return false;
            }

            CharacterRefreshTarget target = this.LocationHandler.ResolveSpawn();

            this.PlayerRefreshHandler.Refresh(target, this.PlayerSpawnRecord.SpawnPlacementSourceType);

            return true;
        }

        internal void NotifyPlayerRespawned()
        {
            this.OnPlayerRespawned?.Invoke();
            this.OnPlayerSpawnedOrRespawned?.Invoke();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.LocationHandler    == null) _locationHandler   = this.GetComponent<PlayerSpawnLocationHandler>();
            if (this.RespawnHandler     == null) _respawnHandler    = this.GetComponent<PlayerRespawnHandler>();
            if (this.SessionHandler     == null) _sessionHandler    = this.GetComponent<PlayerSpawnSessionHandler>();
            if (this.AutoSaveHandler    == null) _autoSaveHandler   = this.GetComponent<PlayerSpawnAutoSaveHandler>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.LocationHandler    == null) Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] {nameof(PlayerSpawnLocationHandler)} is missing.", this);
            if (this.RespawnHandler     == null) Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] {nameof(PlayerRespawnHandler)} is missing.", this);
            if (this.SessionHandler     == null) Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] {nameof(PlayerSpawnSessionHandler)} is missing.", this);
            if (this.AutoSaveHandler    == null) Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] {nameof(PlayerSpawnAutoSaveHandler)} is missing.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.LocationHandler?.Bind(this);
            this.RespawnHandler?.Bind(this);
            this.SessionHandler?.Bind(this);
            this.AutoSaveHandler?.Bind(this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            if (this.PlayerSpawnRecord == null)
            {
                _spawnRecord = new PlayerSpawnRecord();
            }

            this.SessionHandler?.InitializeSessionState();
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

            // Ensure references are available before gameplay initialization.
            this.LocationHandler?.EnsureSpawnPointReferences();
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
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.SessionHandler?.InitializePregameplay();
        }
        protected override void InitializeGameplay()
        {
            base.InitializeGameplay();

            this.SessionHandler?.StartGameplay();
        }
        protected override void UnbindInternalReferences()
        {
            this.AutoSaveHandler?.Unbind(this);
            this.SessionHandler?.Unbind(this);
            this.RespawnHandler?.Unbind(this);
            this.LocationHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
