using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "ConsumableItemProfile", menuName = "Data/FlameOfTheLost/Items/ConsumableItem")]
    public class ConsumableItemProfile : ItemProfile
    {
        public override ItemType ItemType => ItemType.ConsumableItem;

        [Header("Use Cost")]
        [SerializeField] private ResourceCost _useCost = new();
        public ResourceCost UseResourceCost => _useCost;

        [Header("Status Effects")]
        [SerializeField] private List<StatusEffectApplication> _statusEffectApplications = new();
        public IReadOnlyList<StatusEffectApplication> StatusEffectApplications => _statusEffectApplications;
    }
}