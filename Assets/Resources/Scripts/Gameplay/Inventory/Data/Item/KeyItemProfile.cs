using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "KeyItemProfile", menuName = "Data/FlameOfTheLost/Items/KeyItem")]
    public class KeyItemProfile : CarriableItemProfile
    {
        public override ItemType ItemType => ItemType.KeyItem;
    }
}