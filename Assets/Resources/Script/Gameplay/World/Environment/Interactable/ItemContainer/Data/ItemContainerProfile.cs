using UnityEngine;
using System.Collections.Generic;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "ItmCtn_New", menuName = "Data/FlameOfTheLost/World/Environment/Interactable/ItemContainerProfile")]
    public class ItemContainerProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        //  1. Inventory
        [Header("Inventory")]
        [SerializeField] private List<InventoryItemEntry> _initialItems = new();
        [SerializeField, Min(1)] private int _maximumEntryCount = 20;
        public IReadOnlyList<InventoryItemEntry> InitialItems => _initialItems;
        public int MaximumEntryCount => Mathf.Max(1, _maximumEntryCount);
    }
}
