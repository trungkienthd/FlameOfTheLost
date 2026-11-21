using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(ItemContainerPanelSelectionHandler))]
    public class ItemContainerPanelController : PlayerInteractionBasePanelController
    {
        // --- References ---
        [Header("References")]

        //  1. Sub-behaviours
        [SerializeField] private ItemContainerPanelSelectionHandler _selectionHandler;
        [SerializeField] private ItemContainerPanelBackgroundClickClearAreaHandler _backgroundClickClearAreaHandler;
        [SerializeField] private ItemContainerPanelItemEntryListSubPanelController _itemEntryListSubPanelController;
        [SerializeField] private ItemContainerPanelSelectedItemEntryDisplayHandler _selectedItemEntryDisplayHandler;
        private ItemContainerPanelSelectionHandler SelectionHandler => _selectionHandler;
        private ItemContainerPanelBackgroundClickClearAreaHandler BackgroundClickClearAreaHandler => _backgroundClickClearAreaHandler;
        private ItemContainerPanelItemEntryListSubPanelController ItemEntryListSubPanelController => _itemEntryListSubPanelController;
        private ItemContainerPanelSelectedItemEntryDisplayHandler SelectedItemEntryDisplayHandler => _selectedItemEntryDisplayHandler;

        // --- Runtime ---
        //  1. Active Container / Inventories
        public ItemContainerController CurrentContainerCtrl { get; private set; }
        public InventoryController SourceInventoryController { get; private set; }
        public InventoryController PlayerInventoryController { get; private set; }

        //  2. Selection
        public InventoryItemEntry CurrentSelectedEntry => this.SelectionHandler != null ? this.SelectionHandler.CurrentSelectedEntry : null;

        // --- Public APIs ---
        public void Open(ItemContainerController sourceContainer, InventoryController sourceInventoryController, InventoryController playerInventoryController)
        {
            if (sourceContainer == null) return;
            if (sourceInventoryController == null) return;
            if (playerInventoryController == null) return;

            base.Open();

            this.CurrentContainerCtrl = sourceContainer;
            this.SourceInventoryController = sourceInventoryController;
            this.PlayerInventoryController = playerInventoryController;

            this.ClearSelection();

            if (this.ItemEntryListSubPanelController != null)
            {
                this.ItemEntryListSubPanelController.ResetPagination();
                this.ItemEntryListSubPanelController.RefreshEntries();
            }
        }
        public override void Close()
        {
            this.ClearSelection();

            if (this.ItemEntryListSubPanelController != null)
            {
                this.ItemEntryListSubPanelController.Clear();
            }

            this.CurrentContainerCtrl = null;
            this.SourceInventoryController = null;
            this.PlayerInventoryController = null;

            base.Close();
        }

        //  1. Selection
        public void SelectEntry(InventoryItemEntry entry)
        {
            if (this.SelectionHandler == null) return;

            this.SelectionHandler.Select(entry);
        }
        public void ClearSelection()
        {
            if (this.SelectionHandler == null) return;

            this.SelectionHandler.Clear();
        }

        //  2. Item Transfer
        public bool TryPickUpSelectedEntry()
        {
            InventoryItemEntry selectedEntry = this.CurrentSelectedEntry;

            if (selectedEntry == null) return false;
            if (selectedEntry.Item == null) return false;
            if (selectedEntry.Quantity <= 0) return false;
            if (this.SourceInventoryController == null) return false;
            if (this.PlayerInventoryController == null) return false;

            ItemProfile item = selectedEntry.Item;
            int quantity = selectedEntry.Quantity;

            bool removedFromSource = this.SourceInventoryController.TryRemoveFromEntry(selectedEntry, quantity);

            if (!removedFromSource) return false;

            bool addedToPlayer = this.PlayerInventoryController.TryAddItem(item, quantity);

            if (!addedToPlayer)
            {
                this.SourceInventoryController.TryAddItem(item, quantity);

                this.RefreshEntriesAndValidateSelection();
                return false;
            }

            this.RefreshEntriesAndValidateSelection();
            return true;
        }

        // --- Internal APIs ---
        //  1. Selection Support
        internal bool ContainsEntry(InventoryItemEntry entry)
        {
            if (this.ItemEntryListSubPanelController == null) return false;

            return this.ItemEntryListSubPanelController.ContainsEntry(entry);
        }
        internal void RefreshSelectionVisuals()
        {
            if (this.ItemEntryListSubPanelController != null)
            {
                this.ItemEntryListSubPanelController.RefreshSelection();
            }

            if (this.SelectedItemEntryDisplayHandler == null) return;

            if (this.CurrentSelectedEntry != null)
            {
                this.SelectedItemEntryDisplayHandler.RefreshDisplay(this.CurrentSelectedEntry);
            }
            else
            {
                this.SelectedItemEntryDisplayHandler.ClearDisplay();
            }
        }

        // --- Helpers ---
        private void RefreshEntriesAndValidateSelection()
        {
            if (this.ItemEntryListSubPanelController != null)
            {
                this.ItemEntryListSubPanelController.RefreshEntries();
            }

            if (this.SelectionHandler != null)
            {
                this.SelectionHandler.ValidateSelection();
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.SelectionHandler                   == null) _selectionHandler                  = this.GetComponentInChildren<ItemContainerPanelSelectionHandler>(true);
            if (this.BackgroundClickClearAreaHandler    == null) _backgroundClickClearAreaHandler   = this.GetComponentInChildren<ItemContainerPanelBackgroundClickClearAreaHandler>(true);
            if (this.ItemEntryListSubPanelController    == null) _itemEntryListSubPanelController   = this.GetComponentInChildren<ItemContainerPanelItemEntryListSubPanelController>(true);
            if (this.SelectedItemEntryDisplayHandler    == null) _selectedItemEntryDisplayHandler   = this.GetComponentInChildren<ItemContainerPanelSelectedItemEntryDisplayHandler>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.SelectionHandler                   == null) Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(ItemContainerPanelSelectionHandler)} reference is missing.", this);
            if (this.BackgroundClickClearAreaHandler    == null) Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(ItemContainerPanelBackgroundClickClearAreaHandler)} reference is missing.", this);
            if (this.ItemEntryListSubPanelController    == null) Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(ItemContainerPanelItemEntryListSubPanelController)} reference is missing.", this);
            if (this.SelectedItemEntryDisplayHandler    == null) Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(ItemContainerPanelSelectedItemEntryDisplayHandler)} reference is missing.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.SelectionHandler?.Bind(this);
            this.BackgroundClickClearAreaHandler?.Bind(this);
            this.ItemEntryListSubPanelController?.Bind(this);
            this.SelectedItemEntryDisplayHandler?.Bind(this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.CurrentContainerCtrl = null;
            this.SourceInventoryController = null;
            this.PlayerInventoryController = null;
        }
        protected override void UnbindInternalReferences()
        {
            this.SelectionHandler?.Unbind(this);
            this.BackgroundClickClearAreaHandler?.Unbind(this);
            this.ItemEntryListSubPanelController?.Unbind(this);
            this.SelectedItemEntryDisplayHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
