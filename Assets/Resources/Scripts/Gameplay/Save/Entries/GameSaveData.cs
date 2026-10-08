using System;

namespace FlameOfTheLost
{
    [Serializable]
    public class GameSaveData
    {
        public const int CurrentVersion = 1;

        public int Version = GameSaveData.CurrentVersion;

        public GameConditionSaveData GameCondition = new();
        public PlayerSpawnSaveData PlayerSpawn = new();
        public PlayerSaveData Player = new();
    }
}