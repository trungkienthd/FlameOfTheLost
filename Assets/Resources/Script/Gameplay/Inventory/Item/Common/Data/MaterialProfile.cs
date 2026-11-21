using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Mtr_New", menuName = "Data/FlameOfTheLost/Inventory/Item/Material")]
    public class MaterialProfile : ItemProfile
    {
        public override ItemType ItemType => ItemType.Material;
    }
}
