using System;

namespace FlameOfTheLost
{
    [Serializable]
    public class PlayerSaveData
    {
        public PlayerProgressionSaveData Progression = new();
        public PlayerRewardSaveData Reward = new();
        public PlayerInventorySaveData Inventory = new();
        public PlayerCombatLoadoutSaveData CombatLoadout = new();
        public PlayerCarryLoadoutSaveData CarryLoadout = new();
    }
}
