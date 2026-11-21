using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class InventoryPanelItemEntryListDisplayHandler : InventoryPanelItemEntryListSubBehaviourBase
    {
        // --- References ---
        [Header("References (Prefab - Slot)")]
        [SerializeField] private GameObject _slotPrefab;
        private GameObject SlotPrefab => _slotPrefab;

        // --- Configs ---
        [Header("Configs (Layout)")]
        [SerializeField] private Vector2 _startPosition = new(-225f, 100f);
        [SerializeField] private Vector2 _step = new(150f, 150f);
        private Vector2 StartPosition => _startPosition;
        private Vector2 Step => _step;

        // --- Runtime ---
        private readonly List<ItemEntrySlotController> _slotControllers = new();
        private IReadOnlyList<ItemEntrySlotController> SlotControllers => _slotControllers;

        // --- Events ---
        public event Action<InventoryItemEntry> OnEntryClicked;

        // --- Public APIs ---
        public void CreateSlots(int slotCount, int columnCount)
        {
            if (this.SlotPrefab == null) return;
            if (this.SlotControllers.Count > 0) return;

            columnCount = Mathf.Max(1, columnCount);

            for (int i = 0; i < slotCount; i++)
            {
                GameObject slot = UnityEngine.Object.Instantiate(this.SlotPrefab,this.transform);

                ItemEntrySlotController slotController = slot.GetComponent<ItemEntrySlotController>();
                RectTransform rectTransform = slot.transform as RectTransform;

                int column = i % columnCount;
                int row = i / columnCount;

                float posX = this.StartPosition.x + (column * this.Step.x);
                float posY = this.StartPosition.y - (row * this.Step.y);

                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(posX, posY);
                }

                if (slotController == null) continue;

                slotController.OnClicked += this.HandleSlotClicked;
                slotController.ResetBindings();

                _slotControllers.Add(slotController);
            }
        }

        public void RefreshEntries(IReadOnlyList<InventoryItemEntry> entries, int pageNumber, int slotsPerPage, InventoryItemEntry selectedEntry)
        {
            entries ??= Array.Empty<InventoryItemEntry>();

            pageNumber = Mathf.Max(1, pageNumber);
            slotsPerPage = Mathf.Max(1, slotsPerPage);

            int startIndex = (pageNumber - 1) * slotsPerPage;

            for (int i = 0; i < this.SlotControllers.Count; i++)
            {
                ItemEntrySlotController slot = this.SlotControllers[i];

                if (slot == null) continue;

                int entryIndex = startIndex + i;

                InventoryItemEntry entry = entryIndex >= 0 && entryIndex < entries.Count
                    ? entries[entryIndex]
                    : null;

                if (entry == null)
                {
                    slot.ResetBindings();
                    continue;
                }

                slot.Bind(entry);

                slot.SetSelected(UnityEngine.Object.ReferenceEquals(entry, selectedEntry));
            }
        }

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
        private void HandleSlotClicked(ItemEntrySlotController clickedSlot)
        {
            if (clickedSlot == null) return;
            if (clickedSlot.BoundEntry == null) return;

            this.OnEntryClicked?.Invoke(clickedSlot.BoundEntry);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.SlotPrefab == null) _slotPrefab = Resources.Load<GameObject>("Prefabs/UI/Prefab_UI_ItemEntrySlot_Dark");
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.SlotPrefab == null) Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelItemEntryListDisplayHandler)}] {nameof(this.SlotPrefab)} is missing.", this);
        }
        protected override void UnbindInternalReferences()
        {
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
