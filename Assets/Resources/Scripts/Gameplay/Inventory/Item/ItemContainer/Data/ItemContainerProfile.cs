using UnityEngine;
using System.Collections.Generic;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "ItmCtn_New", menuName = "Data/FlameOfTheLost/Level/Environment/Interactable/ItemContainerProfile")]
    public class ItemContainerProfile : ScriptableObject,
        IIdentityProvider,
        IIconProvider
    {
        // --- Data / Configs
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id = 32000;
        [SerializeField] private string _key;
        [SerializeField] private string _displayName = "Chest";
        [SerializeField, TextArea(5, 5)] private string _description;
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Display
        [Header("Display")]
        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        //  3. Inventory
        [Header("Inventory")]
        [SerializeField] private List<InventoryItemEntry> _initialItems = new();
        [SerializeField, Min(1)] private int _maximumEntryCount = 20;
        public IReadOnlyList<InventoryItemEntry> InitialItems => _initialItems;
        public int MaximumEntryCount => Mathf.Max(1, _maximumEntryCount);
    }
}
