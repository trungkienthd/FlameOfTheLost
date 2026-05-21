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

        public void Bind(InventoryController sourceInventoryController, InventoryController playerInventoryController)
        {
            if (sourceInventoryController == null) return;
            if (playerInventoryController == null) return;
        }

        public void Clear()
        {

        }
    }
}
