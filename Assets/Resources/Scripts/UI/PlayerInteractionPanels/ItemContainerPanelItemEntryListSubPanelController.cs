using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class ItemContainerPanelItemEntryListSubPanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ItemEntryListPaginationHandler _paginationHandler;
        [SerializeField] private ItemContainerPanelSelectedItemEntryDisplayHandler _selectedDisplayHandler;
        private ItemEntryListPaginationHandler PaginationHandler => _paginationHandler;
        private ItemContainerPanelSelectedItemEntryDisplayHandler SelectedEntryDisplayHandler => _selectedDisplayHandler;

        [Header("Entry List")]
        [SerializeField] private RectTransform _itemEntryListParent;
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private Vector2 _startPosition = new(-450f, 200f);
        [SerializeField] private Vector2 _step = new(150f, 150f);
        private RectTransform ItemEntryListParent => _itemEntryListParent;
        private GameObject SlotPrefab => _slotPrefab;
        private Vector2 StartPosition => _startPosition;
        private Vector2 Step => _step;

        private readonly List<ItemEntrySlotController> _slotControllers = new();
        private readonly List<InventoryItemEntry> _visibleEntries = new();
        private IReadOnlyList<ItemEntrySlotController> SlotControllers => _slotControllers;
        private IReadOnlyList<InventoryItemEntry> VisibleEntries => _visibleEntries;

        private InventoryController _sourceInventoryController;
        private InventoryController _playerInventoryController;
        private InventoryController SourceInventoryController => _sourceInventoryController;
        private InventoryController PlayerInventoryController => _playerInventoryController;

        private void Awake()
        {
            this.CacheReferences();
            this.CreateSlots();

            if (this.PaginationHandler != null)
            {
                this.PaginationHandler.OnCurrentPageChanged += this.HandleCurrentPageChanged;
            }

            if (this.SelectedEntryDisplayHandler != null)
            {
                this.SelectedEntryDisplayHandler.OnPickUpClicked += this.HandlePickUpClicked;
            }
        }

        private void OnDestroy()
        {
            if (this.PaginationHandler != null)
            {
                this.PaginationHandler.OnCurrentPageChanged -= this.HandleCurrentPageChanged;
            }

            if (this.SelectedEntryDisplayHandler != null)
            {
                this.SelectedEntryDisplayHandler.OnPickUpClicked -= this.HandlePickUpClicked;
            }

            for (int i = 0; i < _slotControllers.Count; i++)
            {
                ItemEntrySlotController slot = this.SlotControllers[i];
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
            }
        }

        public void Bind(InventoryController sourceInventoryController, InventoryController playerInventoryController)
        {
            _sourceInventoryController = sourceInventoryController;
            _playerInventoryController = playerInventoryController;

            if (this.PaginationHandler != null)
            {
                this.PaginationHandler.SetCurrentPageWithoutNotify(1);
            }

            this.RebuildAndRefresh();
        }

        public void Clear()
        {
            _sourceInventoryController = null;
            _playerInventoryController = null;

            _visibleEntries.Clear();

            if (this.SelectedEntryDisplayHandler != null)
            {
                this.SelectedEntryDisplayHandler.ClearSelectedEntry();
            }

            this.RefreshVisibleSlots();
        }

        private void HandleCurrentPageChanged(int pageNumber)
        {
            this.RefreshVisibleSlots();
        }

        private void HandleSlotClicked(ItemEntrySlotController clickedSlot)
        {
            if (clickedSlot == null) return;
            if (this.SelectedEntryDisplayHandler == null) return;

            this.SelectedEntryDisplayHandler.SetSelectedEntry(clickedSlot.BoundEntry, _visibleEntries);
            this.RefreshVisibleSlots();
        }

        private void HandlePickUpClicked(InventoryItemEntry selectedEntry)
        {
            if (selectedEntry == null) return;
            if (this.SourceInventoryController == null) return;
            if (this.PlayerInventoryController == null) return;
            if (selectedEntry.Item == null) return;
            if (selectedEntry.Quantity <= 0) return;

            ItemProfile item = selectedEntry.Item;
            int quantity = selectedEntry.Quantity;

            bool removedFromSource = this.SourceInventoryController.TryRemoveFromEntry(selectedEntry, quantity);
            if (!removedFromSource) return;

            bool addedToPlayer = this.PlayerInventoryController.TryAddItem(item, quantity);

            if (!addedToPlayer)
            {
                this.SourceInventoryController.TryAddItem(item, quantity);
                return;
            }

            this.RebuildAndRefresh();
        }

        private void RebuildAndRefresh()
        {
            this.RebuildVisibleEntries();

            bool pageChanged = false;

            if (this.PaginationHandler != null)
            {
                pageChanged = this.PaginationHandler.TrySetTotalEntryCount(this.VisibleEntries.Count);
            }

            if (this.SelectedEntryDisplayHandler != null)
            {
                this.SelectedEntryDisplayHandler.ValidateSelection(_visibleEntries);
            }

            if (!pageChanged)
            {
                this.RefreshVisibleSlots();
            }
        }

        private void RebuildVisibleEntries()
        {
            _visibleEntries.Clear();

            if (this.SourceInventoryController == null) return;

            IReadOnlyList<InventoryItemEntry> sourceEntries = this.SourceInventoryController.Items;

            for (int i = 0; i < sourceEntries.Count; i++)
            {
                InventoryItemEntry entry = sourceEntries[i];
                if (entry == null) continue;
                if (entry.Item == null) continue;
                if (entry.Quantity <= 0) continue;

                _visibleEntries.Add(entry);
            }
        }

        private void CreateSlots()
        {
            if (this.SlotPrefab == null || this.ItemEntryListParent == null) return;
            if (this.SlotControllers.Count > 0) return;
            if (this.PaginationHandler == null) return;

            for (int i = 0; i < this.PaginationHandler.SlotsPerPage; i++)
            {
                GameObject slotObject = Object.Instantiate(this.SlotPrefab, this.ItemEntryListParent);

                ItemEntrySlotController slotController = slotObject.GetComponent<ItemEntrySlotController>();
                RectTransform rectTransform = slotObject.transform as RectTransform;

                int column = i % Mathf.Max(1, this.PaginationHandler.ColumnCount);
                int row = i / Mathf.Max(1, this.PaginationHandler.ColumnCount);

                float posX = this.StartPosition.x + column * this.Step.x;
                float posY = this.StartPosition.y - row * this.Step.y;

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

        private void RefreshVisibleSlots()
        {
            int currentPage = this.PaginationHandler != null ? this.PaginationHandler.CurrentPageNumber : 1;
            int slotsPerPage = this.PaginationHandler != null ? this.PaginationHandler.SlotsPerPage : this.SlotControllers.Count;
            int startIndex = (currentPage - 1) * slotsPerPage;

            InventoryItemEntry selectedEntry = this.SelectedEntryDisplayHandler != null
                ? this.SelectedEntryDisplayHandler.CurrentSelectedEntry
                : null;

            for (int i = 0; i < this.SlotControllers.Count; i++)
            {
                ItemEntrySlotController slot = this.SlotControllers[i];
                if (slot == null) continue;

                int entryIndex = startIndex + i;

                InventoryItemEntry entry = entryIndex >= 0 && entryIndex < this.VisibleEntries.Count
                    ? this.VisibleEntries[entryIndex]
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

        private void CacheReferences()
        {
            if (this.PaginationHandler == null) _paginationHandler = this.GetComponentInChildren<ItemEntryListPaginationHandler>(true);
            if (this.SelectedEntryDisplayHandler == null) _selectedDisplayHandler = this.GetComponentInChildren<ItemContainerPanelSelectedItemEntryDisplayHandler>(true);

            if (this.ItemEntryListParent == null)
            {
                Transform itemEntries = this.transform.Find("ItemEntryList");
                if (itemEntries != null)
                {
                    _itemEntryListParent = itemEntries.GetComponent<RectTransform>();
                }
            }
        }
    }
}
