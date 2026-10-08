using System;
using System.Collections.Generic;

namespace FlameOfTheLost
{
    [Serializable]
    public class PlayerCarryLoadoutSaveData
    {
        public List<string> CarriedItemEntryIds = new();
        public List<PlayerCarryLoadoutSlotSaveEntry> Slots = new();
    }
}