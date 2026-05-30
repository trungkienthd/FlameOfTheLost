namespace FlameOfTheLost
{
    public sealed class PlayerSpawnState
    {
        private const int DEFAULT_SPAWN_POINT_ID = 1;

        public CharacterSpawnSourceType SpawnSourceType { get; private set; } = CharacterSpawnSourceType.DefaultSpawnPoint;

        public int ActiveSpawnPointId { get; private set; } = DEFAULT_SPAWN_POINT_ID;

        public string ActiveCheckpointId { get; private set; } = string.Empty;

        public bool HasActiveSpawnPoint => this.ActiveSpawnPointId >= 0;

        public bool HasActiveCheckpoint => !string.IsNullOrWhiteSpace(this.ActiveCheckpointId);

        public void SetSpawnPointSpawn(int spawnPointId)
        {
            if (spawnPointId < 0) return;
            
            this.SpawnSourceType = CharacterSpawnSourceType.DefaultSpawnPoint;
            this.ActiveSpawnPointId = spawnPointId;
            this.ActiveCheckpointId = string.Empty;
        }


        public void SetCheckpointSpawn(string checkpointId)
        {
            if (string.IsNullOrWhiteSpace(checkpointId)) return;
            
            this.SpawnSourceType = CharacterSpawnSourceType.Checkpoint;
            this.ActiveCheckpointId = checkpointId;
        }

        public void ClearCheckpointSpawn()
        {
            this.ActiveCheckpointId = string.Empty;
            this.SpawnSourceType = CharacterSpawnSourceType.DefaultSpawnPoint;
        }
    }
}
