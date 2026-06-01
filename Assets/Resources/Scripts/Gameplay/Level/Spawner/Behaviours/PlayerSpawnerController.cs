using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerSpawnerController : FOTLMonoBehaviour
    {
        // --- States ---
        [Header("Player")]
        [SerializeField] private GameObject _player;
        private GameObject Player => _player;

        [Header("Spawn Settings")]
        [SerializeField] private bool _spawnPlayerOnStart = true;
        private bool SpawnPlayerOnStart => _spawnPlayerOnStart;

        [Header("References - Spawn Points (Controllers)")]
        [SerializeField] private List<PlayerDefaultSpawnPointController> _defaultSpawnPointCtrls = new();
        private IReadOnlyList<PlayerDefaultSpawnPointController> DefaultSpawnPointCtrls => _defaultSpawnPointCtrls;

        [Header("References - Checkpoints (Controllers)")]
        [SerializeField] private List<CheckpointController> _checkpointCtrls = new();
        private IReadOnlyList<CheckpointController> CheckpointCtrls => _checkpointCtrls;

        private readonly PlayerSpawnState _spawnState = new();
        public PlayerSpawnState SpawnState => _spawnState;

        // --- Events ---
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
        public PlayerDefaultSpawnPointController GetFirstDefaultSpawnPoint()
        {
            return this.DefaultSpawnPointCtrls.FirstOrDefault(spawnPoint => spawnPoint != null);
        }

        public void SpawnPlayer()
        {
            this.RespawnPlayer();

            //this.OnPlayerSpawnedOrRespawned?.Invoke();
        }

        public void RespawnPlayer()
        {
            if (this.Player == null)
            {
                Debug.LogError($"[{this.gameObject.name}.PlayerSpawnerController] No Player assigned.", this);
                return;
            }

            SpawnResolveResult spawnResult = this.ResolveSpawn();
            CharacterRespawnUtility.RespawnCharacter
            (
                character: this.Player,
                position: spawnResult.Position,
                rotation: spawnResult.Rotation,
                facing: spawnResult.Facing,
                spawnSourceType: this.SpawnState.SpawnSourceType
            );

            this.OnPlayerSpawnedOrRespawned?.Invoke();
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
        private void InitializeFirstDefaultSpawnPointIfNeeded()
        {
            if (this.SpawnState.SpawnSourceType == CharacterSpawnSourceType.Checkpoint) return;

            PlayerDefaultSpawnPointController firstDefaultSpawnPoint = this.GetFirstDefaultSpawnPoint();

            if (firstDefaultSpawnPoint != null)
            {
                this.ActivateSpawnPoint(firstDefaultSpawnPoint);
            }
        }

        private SpawnResolveResult ResolveSpawn()
        {
            if (this.SpawnState.SpawnSourceType == CharacterSpawnSourceType.Checkpoint)
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

                Debug.LogWarning($"[{this.gameObject.name}.PlayerSpawnerController] Checkpoint '{this.SpawnState.ActiveCheckpointId}' was not found. Falling back to default spawn.", this);
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

                Debug.LogWarning($"[{this.gameObject.name}.PlayerSpawnerController] Default spawn point '{this.SpawnState.ActiveSpawnPointId}' was not found. Using first default spawn point.", this);

                return new SpawnResolveResult
                (
                    fallbackSpawnPoint.SpawnPosition,
                    fallbackSpawnPoint.SpawnRotation,
                    fallbackSpawnPoint.DefaultFacing
                );
            }

            Debug.LogError($"[{this.gameObject.name}.PlayerSpawnerController] No valid spawn point was found. Using Player's current transform.", this);

            return new SpawnResolveResult
            (
                this.Player.transform.position,
                this.Player.transform.rotation,
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

            if (this.Player == null)
            {
                PlayerRespawnHandler playerRespawnHandler = UnityEngine.Object.FindFirstObjectByType<PlayerRespawnHandler>();

                if (playerRespawnHandler != null) _player = playerRespawnHandler.gameObject;
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
