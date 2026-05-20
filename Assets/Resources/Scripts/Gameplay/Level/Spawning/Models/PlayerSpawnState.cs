namespace FlameOfTheLost
{
    public sealed class PlayerSpawnState
    {
        private const string DEFAULT_SPAWN_POINT_ID = "001_MAIN_ENTRANCE";

        public PlayerSpawnSourceType SpawnSourceType { get; private set; } = PlayerSpawnSourceType.SpawnPoint;

        public string ActiveSpawnPointId { get; private set; } = DEFAULT_SPAWN_POINT_ID;

        public string ActiveCheckpointId { get; private set; } = string.Empty;

        public bool HasActiveSpawnPoint => !string.IsNullOrWhiteSpace(this.ActiveSpawnPointId);

        public bool HasActiveCheckpoint => !string.IsNullOrWhiteSpace(this.ActiveCheckpointId);

        public void SetSpawnPointSpawn(string spawnPointId)
        {
            if (string.IsNullOrWhiteSpace(spawnPointId)) return;
            
            this.SpawnSourceType = PlayerSpawnSourceType.SpawnPoint;
            this.ActiveSpawnPointId = spawnPointId;
            this.ActiveCheckpointId = string.Empty;
        }


        public void SetCheckpointSpawn(string checkpointId)
        {
            if (string.IsNullOrWhiteSpace(checkpointId)) return;
            
            this.SpawnSourceType = PlayerSpawnSourceType.CheckpointSpawn;
            this.ActiveCheckpointId = checkpointId;
        }

        public void ClearCheckpointSpawn()
        {
            this.ActiveCheckpointId = string.Empty;
            this.SpawnSourceType = PlayerSpawnSourceType.SpawnPoint;
        }
    }
}
