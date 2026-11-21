using System;
using System.Collections.Generic;

namespace FlameOfTheLost
{
    [Serializable]
    public class PlayerInventorySaveData
    {
        //  1. Items
        public List<PlayerInventoryItemSaveEntry> Entries = new();

        //  2. Learned Skills
        public List<int> LearnedSpecialSkillIds = new();
    }
}
