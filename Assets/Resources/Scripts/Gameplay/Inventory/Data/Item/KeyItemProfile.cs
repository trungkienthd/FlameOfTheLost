using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "KeyItemProfile", menuName = "Data/FlameOfTheLost/Items/KeyItem")]
    public class KeyItemProfile : InteractableItemProfile
    {
        public override ItemType ItemType => ItemType.KeyItem;
    }
}