using System;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class PlayerSceneSpawnSaveEntry
    {
        public string SceneName;

        public PlayerSpawnSaveData Spawn = new();
    }
}