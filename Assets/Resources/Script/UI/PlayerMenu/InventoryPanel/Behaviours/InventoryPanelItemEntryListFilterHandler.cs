using System;
using System.Collections.Generic;

namespace FlameOfTheLost
{
    public class InventoryPanelItemEntryListFilterHandler : InventoryPanelItemEntryListSubBehaviourBase
    {
        // --- Runtime ---
        private readonly List<InventoryItemEntry> _filteredItemEntries = new();
        public IReadOnlyList<InventoryItemEntry> FilteredItemEntries => _filteredItemEntries;

        public ItemType? CurrentItemType { get; private set; }

        public string CurrentSearchKeyword { get; private set; } = string.Empty;

        // --- Public APIs ---
        public void SetItemType(ItemType? itemType)
        {
            this.CurrentItemType = itemType;
        }

        public void SetSearchKeyword(string keyword)
        {
            this.CurrentSearchKeyword = keyword ?? string.Empty;
        }

        public void Rebuild()
        {
            _filteredItemEntries.Clear();

            if (!this.EnsureBound()) return;

            IReadOnlyList<InventoryItemEntry> sourceEntries = this.GetItemEntriesByType(this.CurrentItemType);

            for (int i = 0; i < sourceEntries.Count; i++)
            {
                InventoryItemEntry entry = sourceEntries[i];

                if (entry == null || entry.Item == null) continue;
                if (entry.Quantity <= 0) continue;
                if (!this.MatchesSearch(entry, this.CurrentSearchKeyword)) continue;

                _filteredItemEntries.Add(entry);
            }
        }

        // --- Helpers ---
        private IReadOnlyList<InventoryItemEntry> GetItemEntriesByType(ItemType? itemType)
        {
            List<InventoryItemEntry> result = new();

            if (!this.EnsureBound()) return result;

            InventoryController inventoryController = this.ItemEntryListController.PlayerInventoryController;

            if (inventoryController == null) return result;

            if (!itemType.HasValue)
            {
                result.AddRange(inventoryController.Items);
                return result;
            }

            switch (itemType)
            {
                case ItemType.Weapon:
                    result.AddRange(inventoryController.AllWeaponEntries);
                    break;

                case ItemType.Projectile:
                    result.AddRange(inventoryController.AllProjectileEntries);
                    break;

                case ItemType.ConsumableItem:
                    result.AddRange(inventoryController.AllConsumableItemEntries);
                    break;

                case ItemType.LightSource:
                    result.AddRange(inventoryController.AllLightSourceEntries);
                    break;

                case ItemType.KeyItem:
                    result.AddRange(inventoryController.AllKeyItemEntries);
                    break;

                case ItemType.Material:
                    result.AddRange(inventoryController.AllMaterialEntries);
                    break;
            }

            return result;
        }

        private bool MatchesSearch(InventoryItemEntry entry, string keyword)
        {
            if (entry == null || entry.Item == null) return false;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                return true;
            }

            string displayName = entry.Item.DisplayName ?? string.Empty;

            return displayName.IndexOf(keyword.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
