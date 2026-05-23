using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerSpawnerController : MonoBehaviour
    {
        // --- States ---
        [Header("Player")]
        [SerializeField] private GameObject _player;
        private GameObject Player => _player;

        [Header("Spawn Points (Controllers)")]
        [SerializeField] private List<PlayerDefaultSpawnPointController> _defaultSpawnPointCtrls = new();
        private IReadOnlyList<PlayerDefaultSpawnPointController> DefaultSpawnPointCtrls => _defaultSpawnPointCtrls;

        [Header("Checkpoints (Controllers)")]
        [SerializeField] private List<CheckpointController> _checkpointCtrls = new();
        private IReadOnlyList<CheckpointController> CheckpointCtrls => _checkpointCtrls;

        private readonly PlayerSpawnState _spawnState = new();
        public PlayerSpawnState SpawnState => _spawnState;

        // --- Unity Callbacks ---
        private void Reset()
        {
            this.CacheSpawnPointsInScene();
        }

        private void OnValidate()
        {
            this.CacheSpawnPointsInScene();
        }
        private void Awake()
        {
            this.InitializeSpawnState();
        }
        private void Start()
        {
            this.SpawnPlayer();
        }

        // --- Public APIs ---
        public PlayerDefaultSpawnPointController GetFirstDefaultSpawnPoint()
        {
            return this.DefaultSpawnPointCtrls.FirstOrDefault(spawnPoint => spawnPoint != null);
        }
        public void SpawnPlayer()
        {
            if (this.Player == null)
            {
                Debug.LogError($"[{this.gameObject.name}.PlayerSpawnerController] No Player assigned.", this);
                return;
            }

            SpawnResolveResult spawnResult = this.ResolveSpawn();

            this.PlacePlayer(spawnResult.Position, spawnResult.Rotation);
            this.ApplyPlayerFacing(spawnResult.Facing);
            this.RestorePlayerAfterRespawn(spawnResult);
        }

        public void RespawnPlayer()
        {
            if (this.Player == null)
            {
                Debug.LogError($"[{this.gameObject.name}.PlayerSpawnerController] No Player assigned.", this);
                return;
            }

            SpawnResolveResult spawnResult = this.ResolveSpawn();

            this.PlacePlayer(spawnResult.Position, spawnResult.Rotation);
            this.ApplyPlayerFacing(spawnResult.Facing);
            this.RestorePlayerAfterRespawn(spawnResult);
        }

        public void ActivateSpawnPoint(PlayerDefaultSpawnPointController spawnPoint)
        {
            if (spawnPoint == null) return;

            this.SpawnState.SetSpawnPointSpawn(spawnPoint.SpawnPointId);

            Debug.Log($"[{this.gameObject.name}.PlayerSpawnerController] Activated spawn point: {spawnPoint.SpawnPointId}", spawnPoint);
        }

        public void ActivateCheckpoint(CheckpointController checkpoint)
        {
            if (checkpoint == null) return;            

            checkpoint.Activate();

            this.SpawnState.SetCheckpointSpawn(checkpoint.CheckpointId);

            Debug.Log($"[{this.gameObject.name}.PlayerSpawnerController] Activated checkpoint: {checkpoint.CheckpointId}", checkpoint);
        }

        // --- Helpers ---
        private void InitializeSpawnState()
        {
        }

        private SpawnResolveResult ResolveSpawn()
        {
            if (this.SpawnState.SpawnSourceType == PlayerSpawnSourceType.CheckpointSpawn)
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
                CharacterFacingStatusType.Side
            );
        }

        private PlayerDefaultSpawnPointController FindDefaultSpawnPointController(string spawnPointId)
        {
            if (string.IsNullOrWhiteSpace(spawnPointId)) return null;

            return this.DefaultSpawnPointCtrls.FirstOrDefault(spawnPoint => spawnPoint != null && spawnPoint.SpawnPointId == spawnPointId);
        }

        private CheckpointController FindCheckpointController(string checkpointId)
        {
            if (string.IsNullOrWhiteSpace(checkpointId)) return null;

            return this.CheckpointCtrls.FirstOrDefault(checkpoint => checkpoint != null && checkpoint.CheckpointId == checkpointId);
        }

        private void PlacePlayer(Vector3 position, Quaternion rotation)
        {
            // Important for Rigidbody2D-based characters.
            Rigidbody2D rb = this.Player.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.position = (Vector2)position;
                rb.rotation = rotation.eulerAngles.z;
            }
            else
            {
                this.Player.transform.SetPositionAndRotation(position, rotation);
            }
        }

        private void ApplyPlayerFacing(CharacterFacingStatusType facing)
        {
            CharacterVisualController animHandler = this.Player.GetComponent<CharacterVisualController>();
            if (animHandler != null)
            {
                animHandler.SetFacing(facing);
            }
        }

        private void RestorePlayerAfterRespawn(SpawnResolveResult spawnResult)
        {
            CharacterRespawnRefreshContext context = new CharacterRespawnRefreshContext
            (
                character: this.Player,
                respawnPosition: spawnResult.Position,
                respawnRotation: spawnResult.Rotation,
                respawnFacing: spawnResult.Facing,
                spawnSourceType: this.SpawnState.SpawnSourceType
            );

            IEnumerable<ICharacterRespawnRefreshable> refreshableSystems = this.Player
                .GetComponentsInChildren<MonoBehaviour>(true)
                .OfType<ICharacterRespawnRefreshable>()
                .OrderBy(system => system.RespawnRefreshPriority);

            foreach (ICharacterRespawnRefreshable refreshableSystem in refreshableSystems)
            {
                refreshableSystem.RefreshAfterRespawn(context);
            }
        }

        private void CacheSpawnPointsInScene()
        {
            if (_defaultSpawnPointCtrls != null && _defaultSpawnPointCtrls.Count > 0) return;

            _defaultSpawnPointCtrls = FindObjectsByType<PlayerDefaultSpawnPointController>(FindObjectsSortMode.None)
                .Where(spawnPoint => spawnPoint != null)
                .ToList();
        }
    }
}
