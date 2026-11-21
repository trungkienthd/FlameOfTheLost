using System;
using System.Collections.Generic;

namespace FlameOfTheLost
{
    [Serializable]
    public class PlayerProgressionSaveData
    {
        public List<PlayerAttributeSaveEntry> Attributes = new();
    }
}