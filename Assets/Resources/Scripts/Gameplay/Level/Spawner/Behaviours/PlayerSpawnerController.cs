using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerSpawnerController : FOTLMonoBehaviour
    {
        // --- States ---
        [Header("References - Player")]
        [SerializeField] private PlayerRootController _playerRoot;
        private PlayerRootController PlayerRoot => _playerRoot;
        private PlayerInputHandler PlayerInputHandler => this.PlayerRoot != null ? this.PlayerRoot.PlayerInputHandler : null;
        private CharacterPlacementHandler PlayerPlacementHandler => this.PlayerRoot != null ? this.PlayerRoot.PlacementHandler : null;

        [Header("References - Spawn Points (Controllers)")]
        [SerializeField] private List<PlayerDefaultSpawnPointController> _defaultSpawnPointCtrls = new();
        private IReadOnlyList<PlayerDefaultSpawnPointController> DefaultSpawnPointCtrls => _defaultSpawnPointCtrls;

        [Header("References - Checkpoints (Controllers)")]
        [SerializeField] private List<CheckpointController> _checkpointCtrls = new();
        private IReadOnlyList<CheckpointController> CheckpointCtrls => _checkpointCtrls;

        [Header("Spawn/Respawn Settings")]
        [SerializeField] private bool _spawnPlayerOnStart = true;
        [SerializeField] private float _respawnDelaySeconds = 1.0f;
        private bool SpawnPlayerOnStart => _spawnPlayerOnStart;
        private float RespawnDelaySeconds => _respawnDelaySeconds;

        private bool _isRespawning;
        private bool IsRespawning
        {
            get => _isRespawning;
            set => _isRespawning = value;
        }

        private readonly PlayerSpawnState _spawnState = new();
        public PlayerSpawnState SpawnState => _spawnState;

        // --- Events ---
        public event Action OnPlayerSpawned;
        public event Action OnPlayerRespawned;
        public event Action OnPlayerSpawnedOrRespawned;

        // --- Unity Callbacks ---
        private void Reset()
        {
            this.CacheReferences();
        }

        private void OnValidate()
        {
            this.CacheReferences();
        }
        private void Start()
        {
            this.InitializeFirstDefaultSpawnPointIfNeeded();

            if (this.SpawnPlayerOnStart)
            {
                this.SpawnPlayer();
            }
        }

        // --- Public APIs ---
        public void SpawnPlayer()
        {
            this.PlacePlayer();

            this.OnPlayerSpawned?.Invoke();
            this.OnPlayerSpawnedOrRespawned?.Invoke();
        }

        public void RespawnPlayer()
        {
            if (this.IsRespawning) return;

            this.StartCoroutine(this.RespawnPlayerRoutine());
        }

        public PlayerDefaultSpawnPointController GetFirstDefaultSpawnPoint()
        {
            return this.DefaultSpawnPointCtrls.FirstOrDefault(spawnPoint => spawnPoint != null);
        }
        public void ActivateSpawnPoint(PlayerDefaultSpawnPointController spawnPoint)
        {
            if (spawnPoint == null) return;

            this.SpawnState.SetSpawnPointSpawn(spawnPoint.Id);

            Debug.Log($"[{this.gameObject.name}.PlayerSpawnerController] Activated spawn point: {spawnPoint.DisplayName}", spawnPoint);
        }

        public void ActivateCheckpoint(CheckpointController checkpoint)
        {
            // //TODO
            //if (checkpoint == null) return;            

            //checkpoint.Activate();

            //this.SpawnState.SetCheckpointSpawn(checkpoint.CheckpointId);

            //Debug.Log($"[{this.gameObject.name}.PlayerSpawnerController] Activated checkpoint: {checkpoint.CheckpointId}", checkpoint);
        }

        // --- Helpers ---
        private void PlacePlayer()
        {
            if (this.PlayerPlacementHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No PlayerPlacementHandler assigned.", this);
                return;
            }

            SpawnResolveResult spawnResult = this.ResolveSpawn();

            this.PlayerPlacementHandler.PlaceCharacter(spawnResult, this.SpawnState.SpawnSourceType);
        }

        private IEnumerator RespawnPlayerRoutine()
        {
            this.IsRespawning = true;

            this.DisablePlayerBeforeRespawn();

            yield return new WaitForSeconds(this.RespawnDelaySeconds);

            this.PlacePlayer();

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

        private void InitializeFirstDefaultSpawnPointIfNeeded()
        {
            if (this.SpawnState.SpawnSourceType == CharacterPlacementSourceType.Checkpoint) return;

            PlayerDefaultSpawnPointController firstDefaultSpawnPoint = this.GetFirstDefaultSpawnPoint();

            if (firstDefaultSpawnPoint == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No default spawn point found during initialization.", this);
                return;
            }

            this.ActivateSpawnPoint(firstDefaultSpawnPoint);
            
        }

        private SpawnResolveResult ResolveSpawn()
        {
            if (this.SpawnState.SpawnSourceType == CharacterPlacementSourceType.Checkpoint)
            {
                CheckpointController checkpointCtrl = this.FindCheckpointController(this.SpawnState.ActiveCheckpointId);

                if (checkpointCtrl != null)
                {
                    return new SpawnResolveResult
                    (
                        checkpointCtrl.SpawnPosition,
                        checkpointCtrl.SpawnRotation,
                        checkpointCtrl.DefaultFacing
                    );
                }

                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Checkpoint '{this.SpawnState.ActiveCheckpointId}' was not found. Falling back to default spawn.", this);
            }

            PlayerDefaultSpawnPointController defaultSpawnPointCtrl = this.FindDefaultSpawnPointController(this.SpawnState.ActiveSpawnPointId);

            if (defaultSpawnPointCtrl != null)
            {
                return new SpawnResolveResult
                (
                    defaultSpawnPointCtrl.SpawnPosition,
                    defaultSpawnPointCtrl.SpawnRotation,
                    defaultSpawnPointCtrl.DefaultFacing
                );
            }

            if (this.DefaultSpawnPointCtrls.Count > 0 && this.DefaultSpawnPointCtrls[0] != null)
            {
                PlayerDefaultSpawnPointController fallbackSpawnPoint = this.DefaultSpawnPointCtrls[0];

                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] Default spawn point '{this.SpawnState.ActiveSpawnPointId}' was not found. Using first default spawn point.", this);

                return new SpawnResolveResult
                (
                    fallbackSpawnPoint.SpawnPosition,
                    fallbackSpawnPoint.SpawnRotation,
                    fallbackSpawnPoint.DefaultFacing
                );
            }

            Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerSpawnerController)}] No valid spawn point was found. Using Player's current transform.", this);
            if (this.PlayerRoot != null)
            {
                return new SpawnResolveResult
                (
                    this.PlayerRoot.transform.position,
                    this.PlayerRoot.transform.rotation,
                    CharacterFacingStatus.Side
                );
            }

            return new SpawnResolveResult
            (
                Vector3.zero,
                Quaternion.identity,
                CharacterFacingStatus.Side
            );
        }

        private PlayerDefaultSpawnPointController FindDefaultSpawnPointController(int spawnPointId)
        {
            if (spawnPointId < 0) return null;

            return this.DefaultSpawnPointCtrls.FirstOrDefault(spawnPoint => spawnPoint != null && spawnPoint.Id == spawnPointId);
        }

        private CheckpointController FindCheckpointController(string checkpointId)
        {
            if (string.IsNullOrWhiteSpace(checkpointId)) return null;

            return this.CheckpointCtrls.FirstOrDefault(checkpoint => checkpoint != null && checkpoint.CheckpointId == checkpointId);
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.PlayerRoot == null)
            {
                GameObject player = GameObject.Find("Player");
                _playerRoot = player != null ? player.GetComponent<PlayerRootController>() : null;
            }

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
    }
}
