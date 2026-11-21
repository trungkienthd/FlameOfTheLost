using UnityEngine;

namespace FlameOfTheLost
{
    public static class InventoryItemProfileResolver
    {
        public static ItemProfile GetById(int id)
        {
            ItemProfile[] profiles = Resources.LoadAll<ItemProfile>("Data/Inventory/Items");

            for (int i = 0; i < profiles.Length; i++)
            {
                ItemProfile profile = profiles[i];

                if (profile != null && profile.Id == id)
                {
                    return profile;
                }
            }

            return null;
        }
    }
}