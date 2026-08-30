using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class InventoryPanelItemEntryListSubPanelController : InventoryPanelSubBehaviourBase
    {
        // --- References ---
        [Header("References")]

        //  1. Inventory (Player)
        [SerializeField] private InventoryController _inventoryController;
        private InventoryController PlayerInventoryController => _inventoryController;

        //  2. UI Behaviours
        [SerializeField] private InventoryNavigationSubPanelController _navigationSubPanelController;
        [SerializeField] private InventoryItemEntrySubPanelSearchBarHandler _searchBarHandler;
        [SerializeField] private ItemEntryListPaginationHandler _paginationHandler;
        private InventoryNavigationSubPanelController NavigationSubPanelController => _navigationSubPanelController;
        private InventoryItemEntrySubPanelSearchBarHandler SearchBarHandler => _searchBarHandler;
        private ItemEntryListPaginationHandler PaginationHandler => _paginationHandler;

        //  3. Entry List
        [SerializeField] private RectTransform _itemEntryListRoot;
        [SerializeField] private GameObject _slotPrefab;
        private RectTransform ItemEntryListRoot => _itemEntryListRoot;
        private GameObject SlotPrefab => _slotPrefab;

        // --- Configs ---
        [Header("Configs (Pagination)")]
        [SerializeField] private Vector2 _startPosition = new(-450f, 200f);
        [SerializeField] private Vector2 _step = new(150f, 150f);
        private Vector2 StartPosition => _startPosition;
        private Vector2 Step => _step;

        // --- Runtime ---
        private readonly List<ItemEntrySlotController> _slotControllers = new();
        private readonly List<InventoryItemEntry> _filteredItemEntries = new();
        private IReadOnlyList<ItemEntrySlotController> SlotControllers => _slotControllers;
        private IReadOnlyList<InventoryItemEntry> FilteredItemEntries => _filteredItemEntries;

        private ItemType _currentItemType = ItemType.Weapon;
        private ItemType CurrentItemType
        {
            get => _currentItemType;
            set => _currentItemType = value;
        }

        private string _currentSearchKeyword = string.Empty;
        private string CurrentSearchKeyword
        {
            get => _currentSearchKeyword;
            set => _currentSearchKeyword = value ?? string.Empty;
        }

        private bool _hasInitializedGameplay;
        private bool HasInitializedGameplay
        {
            get => _hasInitializedGameplay;
            set => _hasInitializedGameplay = value;
        }

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            if (!this.HasInitializedGameplay) return;

            this.RebuildAndRefresh();
        }

        // --- Public APIs ---
        public void RefreshSelection(InventoryItemEntry selectedEntry)
        {
            for (int i = 0; i < this.SlotControllers.Count; i++)
            {
                ItemEntrySlotController slot = this.SlotControllers[i];
                if (slot == null) continue;

                slot.SetSelected(UnityEngine.Object.ReferenceEquals(slot.BoundEntry, selectedEntry));
            }
        }

        // --- Helpers ---
        //  1. Event Handling
        private void HandleSelectedItemTypeChanged(ItemType itemType)
        {
            this.CurrentItemType = itemType;
            this.PaginationHandler?.SetCurrentPageWithoutNotify(1);
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
        private void HandleSlotClicked(ItemEntrySlotController clickedSlot)
        {
            if (clickedSlot == null) return;
            if (!this.EnsureBound()) return;

            this.PanelController.SetSelectedEntry(clickedSlot.BoundEntry, this.FilteredItemEntries);
            //this.RefreshVisibleSlots();
        }

        //  2. Refresh
        private void RebuildAndRefresh()
        {
            this.RebuildFilteredEntries();

            if (this.PaginationHandler == null) return;

            bool pageChanged = this.PaginationHandler.TrySetTotalEntryCount(this.FilteredItemEntries.Count);

            if (this.EnsureBound())
            {
                this.PanelController.ValidateSelectedEntry(this.FilteredItemEntries);
            }

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
        private void RefreshVisibleSlots()
        {
            if (this.PaginationHandler == null) return;

            int currentPage = this.PaginationHandler.CurrentPageNumber;
            int startIndex = (currentPage - 1) * this.PaginationHandler.SlotsPerPage;

            InventoryItemEntry selectedEntry = this.IsBound ? this.PanelController.CurrentSelectedEntry : null;

            for (int i = 0; i < this.SlotControllers.Count; i++)
            {
                ItemEntrySlotController slot = this.SlotControllers[i];
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

        //  2. Entries
        private IReadOnlyList<InventoryItemEntry> GetItemEntriesByType(ItemType itemType)
        {
            List<InventoryItemEntry> result = new();
            if (this.PlayerInventoryController == null) return result;

            switch (itemType)
            {
                case ItemType.Weapon:
                    result.AddRange(this.PlayerInventoryController.AllWeaponEntries);
                    break;
                case ItemType.Projectile:
                    result.AddRange(this.PlayerInventoryController.AllProjectileEntries);
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

            //Debug.Log($"[{this.gameObject.name}.{nameof(InventoryPanelItemEntryListSubPanelController)}] Retrieved {result.Count} entries for item type {itemType}", this);
            return result;
        }
        private bool MatchesSearch(InventoryItemEntry entry, string keyword)
        {
            if (entry == null || entry.Item == null) return false;
            if (string.IsNullOrWhiteSpace(keyword)) return true;

            string displayName = entry.Item.DisplayName ?? string.Empty;
            return displayName.IndexOf(keyword.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
        }

        //  3. Slots 
        private void CreateSlots()
        {
            if (this.SlotPrefab == null) return;
            if (this.ItemEntryListRoot == null) return;
            if (this.PaginationHandler == null) return;
            if (this.SlotControllers.Count > 0) return;

            int columnCount = Mathf.Max(1, this.PaginationHandler.ColumnCount);

            for (int i = 0; i < this.PaginationHandler.SlotsPerPage; i++)
            {
                GameObject slot = UnityEngine.Object.Instantiate(this.SlotPrefab, this.ItemEntryListRoot);
                ItemEntrySlotController slotController = slot.GetComponent<ItemEntrySlotController>();
                RectTransform rectTransform = slot.transform as RectTransform;

                int column = i % Mathf.Max(1, columnCount);
                int row = i / Mathf.Max(1, columnCount);

                float posX = this.StartPosition.x + (column * this.Step.x);
                float posY = this.StartPosition.y - (row * this.Step.y);

                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(posX, posY);
                }

                if (slotController != null)
                {
                    slotController.OnClicked += this.HandleSlotClicked;
                    slotController.ResetBindings();

                    _slotControllers.Add(slotController);
                }
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.NavigationSubPanelController == null)
            {
                Transform inventoryNavigationSubPanel = this.transform.parent != null ? this.transform.parent.Find("InventoryNavigationSubPanel") : null;
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
                if (pagination != null) _paginationHandler = pagination.GetComponent<ItemEntryListPaginationHandler>();
            }
            if (this.ItemEntryListRoot == null)
            {
                Transform itemEntriesParent = this.transform.Find("ItemEntryList");
                if (itemEntriesParent != null) _itemEntryListRoot = itemEntriesParent.GetComponent<RectTransform>();
            }

            if (this.SlotPrefab == null) _slotPrefab = Resources.Load<GameObject>("Prefabs/UI/InventoryPanelItemEntryListSubPanelSlot");
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.NavigationSubPanelController   == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelItemEntryListSubPanelController)}] {nameof(this.NavigationSubPanelController)} is missing.", this);
            if (this.SearchBarHandler               == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelItemEntryListSubPanelController)}] {nameof(this.SearchBarHandler)} is missing.", this);
            if (this.PaginationHandler              == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelItemEntryListSubPanelController)}] {nameof(this.PaginationHandler)} is missing.", this);
            if (this.ItemEntryListRoot              == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelItemEntryListSubPanelController)}] {nameof(this.ItemEntryListRoot)} is missing.", this);
            if (this.SlotPrefab                     == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelItemEntryListSubPanelController)}] {nameof(this.SlotPrefab)} is missing.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.NavigationSubPanelController   != null) this.NavigationSubPanelController.OnSelectedItemTypeChanged    += this.HandleSelectedItemTypeChanged;
            if (this.SearchBarHandler               != null) this.SearchBarHandler.OnSearchValueChanged                     += this.HandleSearchKeywordChanged;
            if (this.PaginationHandler              != null) this.PaginationHandler.OnCurrentPageChanged                    += this.HandleCurrentPageChanged;
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.CreateSlots();
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.PlayerInventoryController != null) return;

            GameObject player = GameObject.Find("Player");
            if (player != null) _inventoryController = player.GetComponentInChildren<InventoryController>();
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.PlayerInventoryController == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelItemEntryListSubPanelController)}] {nameof(this.PlayerInventoryController)} is missing.", this);
        }
        protected override void InitializeGameplay()
        {
            base.InitializeGameplay();

            this.HasInitializedGameplay = true;
            this.RebuildAndRefresh();
        }

        protected override void UnbindInternalReferences()
        {
            if (this.NavigationSubPanelController   != null) this.NavigationSubPanelController.OnSelectedItemTypeChanged    -= this.HandleSelectedItemTypeChanged;
            if (this.SearchBarHandler               != null) this.SearchBarHandler.OnSearchValueChanged                     -= this.HandleSearchKeywordChanged;
            if (this.PaginationHandler              != null) this.PaginationHandler.OnCurrentPageChanged                    -= this.HandleCurrentPageChanged;

            for (int i = 0; i < this.SlotControllers.Count; i++)
            {
                ItemEntrySlotController slot = this.SlotControllers[i];
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
            }

            base.UnbindInternalReferences();
        }
    }
}