using System;

namespace FlameOfTheLost
{
    [Serializable]
    public class PlayerSpawnSaveData
    {
        public CharacterRefreshOrigin SpawnPlacementSourceType;

        public int ActiveSpawnPointId = -1;
        public int ActiveCheckpointId = -1;
    }
}