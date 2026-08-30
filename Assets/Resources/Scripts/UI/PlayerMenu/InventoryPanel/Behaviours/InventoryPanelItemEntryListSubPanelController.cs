using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(InventoryPanelItemEntryListFilterHandler))]
    public class InventoryPanelItemEntryListSubPanelController : InventoryPanelSubBehaviourBase
    {
        // --- References ---
        //  1. Inventory (Player)
        [Header("References (Player-only)")]
        [SerializeField] private InventoryController _inventoryController;
        internal InventoryController PlayerInventoryController => _inventoryController;

        //  2. Sub-Behaviours
        [Header("References (Sub-Behaviours)")]
        [SerializeField] private InventoryPanelItemEntryListFilterHandler _filterHandler;
        [SerializeField] private InventoryItemEntrySubPanelSearchBarHandler _searchBarHandler;
        [SerializeField] private InventoryPanelItemEntryListDisplayHandler _displayHandler;
        private InventoryPanelItemEntryListFilterHandler FilterHandler => _filterHandler;
        private InventoryItemEntrySubPanelSearchBarHandler SearchBarHandler => _searchBarHandler;
        private InventoryPanelItemEntryListDisplayHandler DisplayHandler => _displayHandler;

        //  3. Shared UI Behaviours
        [Header("References (Shared UI)")]
        [SerializeField] private ItemEntryListPaginationHandler _paginationHandler;
        private ItemEntryListPaginationHandler PaginationHandler => _paginationHandler;

        // --- Runtime ---
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
            this.DisplayHandler?.RefreshSelection(selectedEntry);
        }

        // --- Helpers ---
        //  1. Event Handling
        private void HandleSelectedItemTypeChanged(ItemType itemType)
        {
            if (this.FilterHandler == null) return;

            this.FilterHandler.SetItemType(itemType);

            this.PaginationHandler?.SetCurrentPageWithoutNotify(1);
            this.RebuildAndRefresh();
        }
        private void HandleSearchKeywordChanged(string keyword)
        {
            if (this.FilterHandler == null) return;

            this.FilterHandler.SetSearchKeyword(keyword);

            this.PaginationHandler?.SetCurrentPageWithoutNotify(1);
            this.RebuildAndRefresh();
        }
        private void HandleCurrentPageChanged(int pageNumber)
        {
            this.RefreshVisibleEntries();
        }
        private void HandleEntrySlotClicked(InventoryItemEntry entry)
        {
            if (entry == null) return;
            if (!this.EnsureBound()) return;
            if (this.FilterHandler == null) return;

            this.PanelController.SetSelectedEntry(
                entry,
                this.FilterHandler.FilteredItemEntries);
        }

        //  2. Refresh
        private void RebuildAndRefresh()
        {
            if (this.FilterHandler == null) return;
            if (this.PaginationHandler == null) return;

            this.FilterHandler.Rebuild();

            IReadOnlyList<InventoryItemEntry> filteredEntries = this.FilterHandler.FilteredItemEntries;

            bool pageChanged = this.PaginationHandler.TrySetTotalEntryCount(filteredEntries.Count);

            if (this.EnsureBound())
            {
                this.PanelController.ValidateSelectedEntry(filteredEntries);
            }

            if (!pageChanged)
            {
                this.RefreshVisibleEntries();
            }
        }
        private void RefreshVisibleEntries()
        {
            if (this.FilterHandler == null) return;
            if (this.PaginationHandler == null) return;
            if (this.DisplayHandler == null) return;

            InventoryItemEntry selectedEntry = this.IsBound ? this.PanelController.CurrentSelectedEntry : null;

            this.DisplayHandler.RefreshEntries
            (
                entries: this.FilterHandler.FilteredItemEntries,
                pageNumber: this.PaginationHandler.CurrentPageNumber,
                slotsPerPage: this.PaginationHandler.SlotsPerPage,
                selectedEntry: selectedEntry
            );
        }

        //  4. FOTLSubBehaviour Overrides
        protected override void OnBound(InventoryPanelController panelController)
        {
            base.OnBound(panelController);

            InventoryNavigationSubPanelController navigationSubPanelController = panelController.NavigationSubPanelController;

            if (navigationSubPanelController != null)
            {
                this.FilterHandler?.SetItemType(navigationSubPanelController.CurrentSelectedItemType);

                navigationSubPanelController.OnSelectedItemTypeChanged += this.HandleSelectedItemTypeChanged;
            }
        }
        protected override void OnUnbinding(InventoryPanelController panelController)
        {
            InventoryNavigationSubPanelController navigationSubPanelController = panelController.NavigationSubPanelController;

            if (navigationSubPanelController != null)
            {
                navigationSubPanelController.OnSelectedItemTypeChanged -= this.HandleSelectedItemTypeChanged;
            }

            base.OnUnbinding(panelController);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.FilterHandler == null) _filterHandler = this.GetComponent<InventoryPanelItemEntryListFilterHandler>();
            if (this.SearchBarHandler == null)
            {
                Transform searchBar = this.transform.Find("SearchBar");
                if (searchBar != null) _searchBarHandler = searchBar.GetComponent<InventoryItemEntrySubPanelSearchBarHandler>();
            }
            if (this.DisplayHandler == null)
            {
                Transform itemEntryList = this.transform.Find("ItemEntryList");
                if (itemEntryList != null) _displayHandler = itemEntryList.GetComponent< InventoryPanelItemEntryListDisplayHandler>();
            }

            if (this.PaginationHandler == null)
            {
                Transform pagination = this.transform.Find("Pagination");
                if (pagination != null) _paginationHandler = pagination.GetComponent<ItemEntryListPaginationHandler>();
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.FilterHandler      == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelItemEntryListSubPanelController)}] {nameof(this.FilterHandler)} is missing.", this);
            if (this.SearchBarHandler   == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelItemEntryListSubPanelController)}] {nameof(this.SearchBarHandler)} is missing.", this);
            if (this.DisplayHandler     == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelItemEntryListSubPanelController)}] {nameof(this.DisplayHandler)} is missing.", this);

            if (this.PaginationHandler  == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelItemEntryListSubPanelController)}] {nameof(this.PaginationHandler)} is missing.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.FilterHandler?.Bind(this);
            this.SearchBarHandler?.Bind(this);
            this.DisplayHandler?.Bind(this);

            if (this.SearchBarHandler   != null) this.SearchBarHandler.OnSearchValueChanged     += this.HandleSearchKeywordChanged;
            if (this.DisplayHandler     != null) this.DisplayHandler.OnEntryClicked             += this.HandleEntrySlotClicked;

            if (this.PaginationHandler  != null) this.PaginationHandler.OnCurrentPageChanged    += this.HandleCurrentPageChanged;
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            if (this.DisplayHandler != null && this.PaginationHandler != null)
            {
                this.DisplayHandler.CreateSlots(this.PaginationHandler.SlotsPerPage, this.PaginationHandler.ColumnCount);
            }
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
            if (this.SearchBarHandler   != null) this.SearchBarHandler.OnSearchValueChanged     -= this.HandleSearchKeywordChanged;
            if (this.DisplayHandler     != null) this.DisplayHandler.OnEntryClicked             -= this.HandleEntrySlotClicked;

            if (this.PaginationHandler  != null) this.PaginationHandler.OnCurrentPageChanged    -= this.HandleCurrentPageChanged;

            this.FilterHandler?.Unbind(this);
            this.SearchBarHandler?.Unbind(this);
            this.DisplayHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}