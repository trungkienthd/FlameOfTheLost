using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class ItemContainerPanelItemEntryListSubPanelController : ItemContainerPanelSubHandlerBase
    {
        // --- References ---
        //  1. Pagination
        [Header("References (Pagination)")]
        [SerializeField] private ItemEntryListPaginationHandler _paginationHandler;
        private ItemEntryListPaginationHandler PaginationHandler => _paginationHandler;

        //  2. Entry List
        [Header("References (Entry List)")]
        [SerializeField] private RectTransform _itemEntryListParent;
        [SerializeField] private GameObject _slotPrefab;
        private RectTransform ItemEntryListParent => _itemEntryListParent;
        private GameObject SlotPrefab => _slotPrefab;

        // --- Configs ---
        [SerializeField] private Vector2 _startPosition = new(-450f, 200f);
        [SerializeField] private Vector2 _step = new(150f, 150f);
        private Vector2 StartPosition => _startPosition;
        private Vector2 Step => _step;

        // --- Runtime ---
        private readonly List<ItemEntrySlotController> _slotControllers = new();
        private readonly List<InventoryItemEntry> _visibleEntries = new();
        private IReadOnlyList<ItemEntrySlotController> SlotControllers => _slotControllers;
        private IReadOnlyList<InventoryItemEntry> VisibleEntries => _visibleEntries;

        // --- Public APIs ---
        public void ResetPagination()
        {
            if (this.PaginationHandler == null) return;

            this.PaginationHandler.SetCurrentPageWithoutNotify(1);
        }

        public void RefreshEntries()
        {
            this.RebuildVisibleEntries();

            bool pageChanged = false;

            if (this.PaginationHandler != null)
            {
                pageChanged = this.PaginationHandler.TrySetTotalEntryCount(this.VisibleEntries.Count);
            }

            if (!pageChanged)
            {
                this.RefreshVisibleSlots();
            }
        }

        public void RefreshSelection()
        {
            this.RefreshVisibleSlots();
        }

        public void Clear()
        {
            _visibleEntries.Clear();

            if (this.PaginationHandler != null)
            {
                this.PaginationHandler.SetCurrentPageWithoutNotify(1);
                this.PaginationHandler.TrySetTotalEntryCount(0);
            }

            this.RefreshVisibleSlots();
        }

        public bool ContainsEntry(InventoryItemEntry entry)
        {
            return entry != null && _visibleEntries.Contains(entry);
        }

        // --- Helpers ---
        private void RebuildVisibleEntries()
        {
            _visibleEntries.Clear();

            if (!this.EnsureBound()) return;

            InventoryController sourceInventoryController = this.ItemContainerPanelController.SourceInventoryController;

            if (sourceInventoryController == null) return;

            IReadOnlyList<InventoryItemEntry> sourceEntries = sourceInventoryController.Items;

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
                GameObject slotObject = UnityEngine.Object.Instantiate(this.SlotPrefab, this.ItemEntryListParent);

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

            InventoryItemEntry selectedEntry = this.IsBound
                ? this.ItemContainerPanelController.CurrentSelectedEntry
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

        private void HandleCurrentPageChanged(int pageNumber)
        {
            this.RefreshVisibleSlots();
        }

        private void HandleSlotClicked(ItemEntrySlotController clickedSlot)
        {
            if (!this.EnsureBound()) return;
            if (clickedSlot == null) return;
            if (clickedSlot.BoundEntry == null) return;

            this.ItemContainerPanelController.SelectEntry(clickedSlot.BoundEntry);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PaginationHandler == null) _paginationHandler = this.GetComponentInChildren<ItemEntryListPaginationHandler>(true);

            if (this.ItemEntryListParent == null)
            {
                Transform itemEntryListTransform = this.transform.Find("ItemEntryList");
                if (itemEntryListTransform != null) _itemEntryListParent = itemEntryListTransform.GetComponent<RectTransform>();
             }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.PaginationHandler != null)
            {
                this.PaginationHandler.OnCurrentPageChanged += this.HandleCurrentPageChanged;
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.CreateSlots();
            this.Clear();
        }
        protected override void UnbindInternalReferences()
        {
            if (this.PaginationHandler != null)
            {
                this.PaginationHandler.OnCurrentPageChanged -= this.HandleCurrentPageChanged;
            }

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
