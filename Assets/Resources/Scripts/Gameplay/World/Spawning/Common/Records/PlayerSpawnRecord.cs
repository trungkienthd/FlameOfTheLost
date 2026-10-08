using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class PlayerSpawnRecord : ISaveDataHandler<PlayerSpawnSaveData>
    {
        // --- Constants ---
        private const int DEFAULT_SPAWN_POINT_ID = 20001;
        private const int INVALID_CHECKPOINT_ID = -1;

        // --- Runtime ---
        [SerializeField] private CharacterRefreshOrigin _spawnPlacementSourceType = CharacterRefreshOrigin.DefaultSpawnPoint;
        public CharacterRefreshOrigin SpawnPlacementSourceType 
        {
            get => _spawnPlacementSourceType;
            private set => _spawnPlacementSourceType = value;
        }

        [SerializeField] private int _activeSpawnPointId = PlayerSpawnRecord.DEFAULT_SPAWN_POINT_ID;
        public int ActiveSpawnPointId 
        {
            get => _activeSpawnPointId; 
            private set => _activeSpawnPointId = value; 
        }

        [SerializeField] private int _activeCheckpointId = PlayerSpawnRecord.INVALID_CHECKPOINT_ID;
        public int ActiveCheckpointId
        {
            get => _activeCheckpointId;
            private set => _activeCheckpointId = value;
        }

        public bool HasActiveSpawnPoint => this.ActiveSpawnPointId >= 0;

        public bool HasActiveCheckpoint => this.ActiveCheckpointId >= 0;

        // --- Public APIs ---
        //  1. Activation
        public void SetSpawnPointSpawn(int spawnPointId)
        {
            if (spawnPointId < 0) return;
            
            this.ActiveSpawnPointId = spawnPointId;

            if (!this.HasActiveCheckpoint)
            {
                this.SpawnPlacementSourceType = CharacterRefreshOrigin.DefaultSpawnPoint;
            }
        }

        public void SetCheckpointSpawn(int checkpointId)
        {
            if (checkpointId < 0) return;

            this.ActiveCheckpointId = checkpointId;

            this.SpawnPlacementSourceType = CharacterRefreshOrigin.Checkpoint;
        }

        //  2. Spawn Placement Source
        public bool TrySetSpawnPlacementSource(CharacterRefreshOrigin sourceType)
        {
            switch (sourceType)
            {
                case CharacterRefreshOrigin.DefaultSpawnPoint:
                    if (!this.HasActiveSpawnPoint) return false;
                    break;

                case CharacterRefreshOrigin.Checkpoint:
                    if (!this.HasActiveCheckpoint) return false;
                    break;

                default:
                    return false;
            }

            this.SpawnPlacementSourceType = sourceType;
            return true;
        }

        public void ClearCheckpointSpawn()
        {
            this.ActiveCheckpointId = PlayerSpawnRecord.INVALID_CHECKPOINT_ID;

            if (this.HasActiveSpawnPoint)
            {
                this.SpawnPlacementSourceType = CharacterRefreshOrigin.DefaultSpawnPoint;
            }
        }

        //  3. ISaveDataHandler implementation
        public PlayerSpawnSaveData CaptureSaveData()
        {
            return new PlayerSpawnSaveData
            {
                SpawnPlacementSourceType = this.SpawnPlacementSourceType,
                ActiveSpawnPointId = this.ActiveSpawnPointId,
                ActiveCheckpointId = this.ActiveCheckpointId
            };
        }

        public void RestoreSaveData(PlayerSpawnSaveData saveData)
        {
            if (saveData == null) return;

            // Restore the complete record first.
            this.ActiveSpawnPointId = saveData.ActiveSpawnPointId >= 0
                ? saveData.ActiveSpawnPointId
                : PlayerSpawnRecord.DEFAULT_SPAWN_POINT_ID;

            this.ActiveCheckpointId = saveData.ActiveCheckpointId >= 0
                ? saveData.ActiveCheckpointId
                : PlayerSpawnRecord.INVALID_CHECKPOINT_ID;

            // Restore the preferred source when possible.
            if (this.TrySetSpawnPlacementSource(saveData.SpawnPlacementSourceType)) return;

            // Defensive fallback for invalid / older save data.
            if (this.HasActiveSpawnPoint)
            {
                this.SpawnPlacementSourceType = CharacterRefreshOrigin.DefaultSpawnPoint;
                return;
            }

            if (this.HasActiveCheckpoint)
            {
                this.SpawnPlacementSourceType = CharacterRefreshOrigin.Checkpoint;
            }
        }
    }
}
