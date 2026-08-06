using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "KeyItemProfile", menuName = "Data/FlameOfTheLost/Inventory/Item/KeyItem")]
    public class KeyItemProfile : CarriableItemProfile
    {
        public override ItemType ItemType => ItemType.KeyItem;
    }
}