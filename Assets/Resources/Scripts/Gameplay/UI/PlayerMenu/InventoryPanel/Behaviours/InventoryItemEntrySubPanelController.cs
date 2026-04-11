using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public partial class InventoryItemEntrySubPanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryController _inventoryController;
        [SerializeField] private InventoryNavigationSubPanelController _navigationSubPanelController;
        [SerializeField] private InventoryItemEntrySubPanelSearchBarHandler _searchBarHandler;
        [SerializeField] private InventoryItemEntrySubPanelPaginationHandler _paginationHandler;
        [SerializeField] private SelectedInventoryItemEntryDisplayHandler _selectedDisplayHandler;
        private InventoryController PlayerInventoryController => _inventoryController;
        private InventoryNavigationSubPanelController NavigationSubPanelController => _navigationSubPanelController;
        private InventoryItemEntrySubPanelSearchBarHandler SearchBarHandler => _searchBarHandler;
        private InventoryItemEntrySubPanelPaginationHandler PaginationHandler => _paginationHandler;
        private SelectedInventoryItemEntryDisplayHandler SelectedDisplayHandler => _selectedDisplayHandler;

        [Header("Entries")]
        [SerializeField] private RectTransform _itemEntriesParent;
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private Vector2 _startPosition = new(-450f, 200f);
        [SerializeField] private Vector2 _step = new(150f, 150f);
        private RectTransform ItemEntriesParent => _itemEntriesParent;
        private GameObject SlotPrefab => _slotPrefab;
        private Vector2 StartPosition => _startPosition;
        private Vector2 Step => _step;

        private readonly List<InventoryItemEntrySlotUIController> _slotControllers = new();
        private readonly List<InventoryItemEntry> _filteredItemEntries = new();
        private IReadOnlyList<InventoryItemEntrySlotUIController> SlotControllers => _slotControllers;
        private IReadOnlyList<InventoryItemEntry> FilteredItemEntries => _filteredItemEntries;

        private ItemType _currentItemType = ItemType.Weapon;
        private string _currentSearchKeyword = string.Empty;
        private ItemType CurrentItemType
        {
            get => _currentItemType;
            set => _currentItemType = value;
        }
        private string CurrentSearchKeyword
        {
            get => _currentSearchKeyword;
            set => _currentSearchKeyword = value ?? string.Empty;
        }

        private void Awake()
        {
            if (this.PlayerInventoryController == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null) _inventoryController = player.GetComponent<InventoryController>();  
            }
            if (this.NavigationSubPanelController == null)
            {
                Transform inventoryNavigationSubPanel = this.transform.parent.Find("InventoryNavigationSubPanel");
                if (inventoryNavigationSubPanel != null) _navigationSubPanelController = inventoryNavigationSubPanel.GetComponent<InventoryNavigationSubPanelController>();
            }
            if (this.SearchBarHandler == null)
            {
                Transform searchBar = this.transform.Find("SearchBar");
                if (searchBar != null) _searchBarHandler = searchBar.GetComponent<InventoryItemEntrySubPanelSearchBarHandler>();
            }
            if (this.PaginationHandler == null)
            {
                Transform pagination = this.transform.Find("Pagination");
                if (pagination != null) _paginationHandler = pagination.GetComponent<InventoryItemEntrySubPanelPaginationHandler>();
            }
            if (this.SelectedDisplayHandler == null)
            {
                Transform selectedItemEntryDisplay = this.transform.Find("SelectedItemEntryDisplay");
                if (selectedItemEntryDisplay != null) _selectedDisplayHandler = selectedItemEntryDisplay.GetComponent<SelectedInventoryItemEntryDisplayHandler>();
            }

            if (this.ItemEntriesParent == null)
            {
                Transform itemEntriesParent = this.transform.Find("ItemEntries");
                if (itemEntriesParent != null) _itemEntriesParent = itemEntriesParent.GetComponent<RectTransform>();
            }
            if (this.SlotPrefab == null) _slotPrefab = Resources.Load<GameObject>("Prefabs/UI/InventoryItemEntrySubPanelSlot");

            this.CreateSlots();

            if (this.NavigationSubPanelController != null) this.NavigationSubPanelController.OnSelectedItemTypeChanged += this.HandleSelectedItemTypeChanged;
            if (this.SearchBarHandler != null) this.SearchBarHandler.OnSearchValueChanged += this.HandleSearchKeywordChanged;
            if (this.PaginationHandler != null) this.PaginationHandler.OnCurrentPageChanged += this.HandleCurrentPageChanged;
        }

        private void OnEnable()
        {
            this.RebuildAndRefresh();
        }

        private void OnDestroy()
        {
            if (this.NavigationSubPanelController != null) this.NavigationSubPanelController.OnSelectedItemTypeChanged -= this.HandleSelectedItemTypeChanged;
            if (this.SearchBarHandler != null) this.SearchBarHandler.OnSearchValueChanged -= this.HandleSearchKeywordChanged;
            if (this.PaginationHandler != null) this.PaginationHandler.OnCurrentPageChanged -= this.HandleCurrentPageChanged;
            
            for (int i = 0; i < this.SlotControllers.Count; i++)
            {
                InventoryItemEntrySlotUIController slot = this.SlotControllers[i];
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
            }
        }

        private void HandleSelectedItemTypeChanged(ItemType itemType)
        {
            this.CurrentItemType = itemType;
            this.RebuildAndRefresh();
        }

        private void HandleSearchKeywordChanged(string keyword)
        {
            this.CurrentSearchKeyword = keyword ?? string.Empty;
            this.PaginationHandler?.SetCurrentPageWithoutNotify(1);
            this.RebuildAndRefresh();
        }

        private void HandleCurrentPageChanged(int pageNumber)
        {
            this.RefreshVisibleSlots();
        }

        private void HandleSlotClicked(InventoryItemEntrySlotUIController clickedSlot)
        {
            if (clickedSlot == null) return;
            if (this.SelectedDisplayHandler == null) return;

            this.SelectedDisplayHandler.SetSelectedEntry(clickedSlot.BoundEntry, this.FilteredItemEntries);
            this.RefreshVisibleSlots();
        }

        private void RebuildAndRefresh()
        {
            this.RebuildFilteredEntries();

            if (this.PaginationHandler == null) return;
            if (this.SelectedDisplayHandler == null) return;

            bool pageChanged = this.PaginationHandler.TrySetTotalEntryCount(this.FilteredItemEntries.Count);
            this.SelectedDisplayHandler.ValidateSelection(this.FilteredItemEntries);

            if (!pageChanged)
            {
                this.RefreshVisibleSlots();
            }
        }

        private void RebuildFilteredEntries()
        {
            _filteredItemEntries.Clear();

            IReadOnlyList<InventoryItemEntry> sourceEntries = this.GetItemEntriesByType(this.CurrentItemType);
            for (int i = 0; i < sourceEntries.Count; i++)
            {
                InventoryItemEntry entry = sourceEntries[i];
                if (!this.MatchesSearch(entry, this.CurrentSearchKeyword)) continue;

                _filteredItemEntries.Add(entry);
            }
        }

        private IReadOnlyList<InventoryItemEntry> GetItemEntriesByType(ItemType itemType)
        {
            List<InventoryItemEntry> result = new();
            if (this.PlayerInventoryController == null) return result;

            switch (itemType)
            {
                case ItemType.Weapon:
                    result.AddRange(this.PlayerInventoryController.AllWeaponEntries);
                    break;
                case ItemType.ConsumableItem:
                    result.AddRange(this.PlayerInventoryController.AllConsumableItemEntries);
                    break;
                case ItemType.LightSource:
                    result.AddRange(this.PlayerInventoryController.AllLightSourceEntries);
                    break;
                case ItemType.KeyItem:
                    result.AddRange(this.PlayerInventoryController.AllKeyItemEntries);
                    break;
            }

            return result;
        }

        private bool MatchesSearch(InventoryItemEntry entry, string keyword)
        {
            if (entry == null || entry.Item == null) return false;
            if (string.IsNullOrWhiteSpace(keyword)) return true;

            string displayName = entry.Item.DisplayName ?? string.Empty;
            return displayName.IndexOf(keyword.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void CreateSlots()
        {
            if (this.SlotPrefab == null || this.ItemEntriesParent == null) return;
            if (this.SlotControllers.Count > 0) return;
            if (this.PaginationHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.InventoryItemEntrySubPanelController] PaginationHandler is required", this);
                return;
            }

            for (int i = 0; i < this.PaginationHandler.SlotsPerPage; i++)
            {
                GameObject slot = UnityEngine.Object.Instantiate(this.SlotPrefab, this.ItemEntriesParent);
                InventoryItemEntrySlotUIController slotController = slot.GetComponent<InventoryItemEntrySlotUIController>();
                RectTransform rectTransform = slot.transform as RectTransform;

                int column = i % Mathf.Max(1, this.PaginationHandler.ColumnCount);
                int row = i / Mathf.Max(1, this.PaginationHandler.ColumnCount);

                float posX = this.StartPosition.x + (column * this.Step.x);
                float posY = this.StartPosition.y - (row * this.Step.y);

                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(posX, posY);
                }

                slotController.OnClicked += this.HandleSlotClicked;
                slotController.ResetBindings();

                _slotControllers.Add(slotController);
            }
        }

        private void RefreshVisibleSlots()
        {
            int currentPage = this.PaginationHandler != null ? this.PaginationHandler.CurrentPageNumber : 1;
            int startIndex = (currentPage - 1) * this.PaginationHandler.SlotsPerPage;
            InventoryItemEntry selectedEntry = this.SelectedDisplayHandler != null
                ? this.SelectedDisplayHandler.CurrentSelectedEntry
                : null;

            for (int i = 0; i < this.SlotControllers.Count; i++)
            {
                InventoryItemEntrySlotUIController slot = this.SlotControllers[i];
                if (slot == null) continue;

                int entryIndex = startIndex + i;
                InventoryItemEntry entry = entryIndex >= 0 && entryIndex < this.FilteredItemEntries.Count
                    ? this.FilteredItemEntries[entryIndex]
                    : null;

                if (entry != null)
                {
                    slot.Bind(entry);
                    slot.SetSelected(ReferenceEquals(entry, selectedEntry));
                }
                else
                {
                    slot.ResetBindings();
                }
            }
        }
    }
}