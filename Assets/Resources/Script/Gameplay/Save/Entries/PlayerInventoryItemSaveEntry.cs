using System;

namespace FlameOfTheLost
{
    [Serializable]
    public class PlayerInventoryItemSaveEntry
    {
        public string RuntimeId;
        public int ItemProfileId;
        public int Quantity;
    }
}
