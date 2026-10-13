using System;
using System.Collections.Generic;

namespace FlameOfTheLost
{
    [Serializable]
    public class GameSaveData
    {
        // --- Constants ---
        public const int CurrentVersion = 3;

        // --- Data ---
        public int Version = GameSaveData.CurrentVersion;

        //  1. Game Conditions
        public GameConditionSaveData GameCondition = new();

        //  2. Player Spawn
        // Legacy field retained for compatibility.
        public PlayerSpawnSaveData PlayerSpawn = new();

        // Scene-specific spawn records.
        public List<PlayerSceneSpawnSaveEntry> ScenePlayerSpawns = new();

        //  3. Player
        public PlayerSaveData Player = new();

        //  4. World Location
        public PlayerWorldLocationSaveData WorldLocation = new();
    }
}