using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class InventoryPanelController : PlayerMenuPanelController
    {
        // --- References ---
        //  1. Sub-Behaviours
        [SerializeField] private InventoryNavigationSubPanelController _navigationSubPanelController;
        [SerializeField] private InventoryPanelItemEntryListSubPanelController _itemEntryListSubPanelController;
        [SerializeField] private InventoryPanelSelectedItemEntryDisplayHandler _selectedItemEntryDisplayHandler;
        internal InventoryNavigationSubPanelController NavigationSubPanelController => _navigationSubPanelController;
        internal InventoryPanelItemEntryListSubPanelController ItemEntryListSubPanelController => _itemEntryListSubPanelController;
        internal InventoryPanelSelectedItemEntryDisplayHandler SelectedItemEntryDisplayHandler => _selectedItemEntryDisplayHandler;

        // --- Runtime ---
        public InventoryItemEntry CurrentSelectedEntry { get; private set; }

        // --- Public APIs ---
        //  1. Selection
        public void SetSelectedEntry(InventoryItemEntry entry, IReadOnlyList<InventoryItemEntry> validEntries)
        {
            if (entry == null || !this.ContainsEntry(validEntries, entry))
            {
                this.ClearSelectedEntry();
                return;
            }

            this.CurrentSelectedEntry = entry;
            this.RefreshSelectionPresentation();
        }
        public void ValidateSelectedEntry(IReadOnlyList<InventoryItemEntry> validEntries)
        {
            if (this.CurrentSelectedEntry == null) return;

            if (!this.ContainsEntry(validEntries, this.CurrentSelectedEntry))
            {
                this.ClearSelectedEntry();
                return;
            }

            this.RefreshSelectionPresentation();
        }
        public void ClearSelectedEntry()
        {
            this.CurrentSelectedEntry = null;
            this.RefreshSelectionPresentation();
        }

        // --- Helpers ---
        //  1. Selection
        private void RefreshSelectionPresentation()
        {
            this.SelectedItemEntryDisplayHandler?.DisplayEntry(this.CurrentSelectedEntry);
            this.ItemEntryListSubPanelController?.RefreshSelection(this.CurrentSelectedEntry);
        }
        private bool ContainsEntry(IReadOnlyList<InventoryItemEntry> entries, InventoryItemEntry target)
        {
            if (entries == null || target == null) return false;

            for (int i = 0; i < entries.Count; i++)
            {
                if (UnityEngine.Object.ReferenceEquals(entries[i], target)) return true;
            }

            return false;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.NavigationSubPanelController == null)
            {
                Transform navigationSubPanel = this.transform.Find("InventoryNavigationSubPanel");
                if (navigationSubPanel != null) _navigationSubPanelController = navigationSubPanel.GetComponent<InventoryNavigationSubPanelController>();
            }
            if (this.ItemEntryListSubPanelController == null)
            {
                Transform itemEntryListSubPanel = this.transform.Find("ItemEntryListSubPanel");
                if (itemEntryListSubPanel != null) _itemEntryListSubPanelController = itemEntryListSubPanel.GetComponent<InventoryPanelItemEntryListSubPanelController>();
            }
            if (this.SelectedItemEntryDisplayHandler == null)
            {
                Transform selectedItemEntryDisplay = this.transform.Find("SelectedItemEntryDisplay");
                if (selectedItemEntryDisplay != null) _selectedItemEntryDisplayHandler = selectedItemEntryDisplay.GetComponent<InventoryPanelSelectedItemEntryDisplayHandler>();
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.NavigationSubPanelController       == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelController)}] {nameof(this.NavigationSubPanelController)} is missing.", this);
            if (this.ItemEntryListSubPanelController    == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelController)}] {nameof(this.ItemEntryListSubPanelController)} is missing.", this);
            if (this.SelectedItemEntryDisplayHandler    == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelController)}] {nameof(this.SelectedItemEntryDisplayHandler)} is missing.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.NavigationSubPanelController?.Bind(this);
            this.ItemEntryListSubPanelController?.Bind(this);
            this.SelectedItemEntryDisplayHandler?.Bind(this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.CurrentSelectedEntry = null;
        }
        protected override void InitializeGameplay()
        {
            base.InitializeGameplay();

            this.RefreshSelectionPresentation();
        }
        protected override void UnbindInternalReferences()
        {
            this.NavigationSubPanelController?.Unbind(this);
            this.ItemEntryListSubPanelController?.Unbind(this);
            this.SelectedItemEntryDisplayHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
