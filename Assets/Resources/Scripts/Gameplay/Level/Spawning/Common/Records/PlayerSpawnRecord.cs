using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class PlayerSpawnRecord
    {
        private const int DEFAULT_SPAWN_POINT_ID = 1;

        [SerializeField] private CharacterRefreshOrigin _spawnPlacementSourceType = CharacterRefreshOrigin.DefaultSpawnPoint;
        public CharacterRefreshOrigin SpawnPlacementSourceType 
        {
            get => _spawnPlacementSourceType;
            private set => _spawnPlacementSourceType = value;
        }

        [SerializeField] private int _activeSpawnPointId = DEFAULT_SPAWN_POINT_ID;
        public int ActiveSpawnPointId 
        {
            get => _activeSpawnPointId; 
            private set => _activeSpawnPointId = value; 
        }

        [SerializeField] private string _activeCheckpointId = string.Empty;
        public string ActiveCheckpointId
        {
            get => _activeCheckpointId;
            private set => _activeCheckpointId = value;
        }

        public bool HasActiveSpawnPoint => this.ActiveSpawnPointId >= 0;

        public bool HasActiveCheckpoint => !string.IsNullOrWhiteSpace(this.ActiveCheckpointId);

        public void SetSpawnPointSpawn(int spawnPointId)
        {
            if (spawnPointId < 0) return;
            
            this.SpawnPlacementSourceType = CharacterRefreshOrigin.DefaultSpawnPoint;
            this.ActiveSpawnPointId = spawnPointId;
            this.ActiveCheckpointId = string.Empty;
        }


        public void SetCheckpointSpawn(string checkpointId)
        {
            if (string.IsNullOrWhiteSpace(checkpointId)) return;
            
            this.SpawnPlacementSourceType = CharacterRefreshOrigin.Checkpoint;
            this.ActiveCheckpointId = checkpointId;
        }

        public void ClearCheckpointSpawn()
        {
            this.ActiveCheckpointId = string.Empty;
            this.SpawnPlacementSourceType = CharacterRefreshOrigin.DefaultSpawnPoint;
        }
    }
}
