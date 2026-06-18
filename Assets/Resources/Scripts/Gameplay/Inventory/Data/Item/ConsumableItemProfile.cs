using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "ConsumableItemProfile", menuName = "Data/FlameOfTheLost/Inventory/Item/ConsumableItem")]
    public class ConsumableItemProfile : ItemProfile
    {
        public override ItemType ItemType => ItemType.ConsumableItem;

        [Header("Use Cost")]
        [SerializeField] private ResourceCostEntry _useCost = new();
        public ResourceCostEntry UseResourceCost => _useCost;

        [Header("Status Effects")]
        [SerializeField] private List<StatusEffectApplicationEntry> _statusEffectApplications = new();
        public IReadOnlyList<StatusEffectApplicationEntry> StatusEffectApplications => _statusEffectApplications;
    }
}