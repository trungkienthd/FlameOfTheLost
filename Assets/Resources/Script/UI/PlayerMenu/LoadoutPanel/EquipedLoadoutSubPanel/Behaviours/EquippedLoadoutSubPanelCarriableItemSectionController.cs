using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EquippedLoadoutSubPanelCarriableItemSectionController : EquippedLoadoutSubPanelSubBehaviourBase
    {
        // --- States ---
        //  1. References
        //      1.1. Refs - Carriable Section
        [Header("References")]
        [SerializeField] private EquippedLoadoutSlotController _carriableSlot;
        private EquippedLoadoutSlotController CarriableSlot => _carriableSlot;

        // --- Events ---
        public event Action<EquippedLoadoutSlotController> OnSlotClicked;

        // --- Public APIs ---
        public void Refresh(EquippedLoadoutSlotController selectedSlot)
        {
            if (!this.EnsureBound()) return;
            if (this.CarriableSlot == null) return;

            InventoryItemEntry carriableEntry = this.PlayerCarryLoadoutController != null
                ? this.PlayerCarryLoadoutController.EquippedCarriableItemEntry
                : null;

            bool isSelected = selectedSlot == this.CarriableSlot;
            this.CarriableSlot.BindCarriableItem
            (
                carriableEntry: carriableEntry,
                isSelected: isSelected,
                isMainButtonInteractable: this.PlayerCarryLoadoutController != null
            );
        }

        public bool ContainsSlot(EquippedLoadoutSlotController slot)
        {
            return slot != null && this.CarriableSlot == slot;
        }

        // --- Helpers ---
        private void BindSlotEvents()
        {
            if (this.CarriableSlot == null) return;

            this.CarriableSlot.OnClicked -= this.HandleSlotClicked;
            this.CarriableSlot.OnClicked += this.HandleSlotClicked;
        }
        private void UnbindSlotEvents()
        {
            if (this.CarriableSlot == null) return;

            this.CarriableSlot.OnClicked -= this.HandleSlotClicked;
        }

        private void HandleSlotClicked(EquippedLoadoutSlotController slot)
        {
            this.OnSlotClicked?.Invoke(slot);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.CarriableSlot == null) _carriableSlot = this.GetComponentInChildren<EquippedLoadoutSlotController>(true);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.BindSlotEvents();
        }
        protected override void UnbindInternalReferences()
        {
            this.UnbindSlotEvents();

            base.UnbindInternalReferences();
        }
    }
}