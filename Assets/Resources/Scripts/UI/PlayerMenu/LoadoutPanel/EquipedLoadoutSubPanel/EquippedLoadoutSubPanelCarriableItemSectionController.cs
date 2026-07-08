using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EquippedLoadoutSubPanelCarriableItemSectionController : FOTLMonoBehaviour
    {
        // --- States ---
        //  1. References
        //      1.1. Refs - Carriable Section
        [Header("References")]
        [SerializeField] private EquippedLoadoutSlotController _carriableSlot;
        private EquippedLoadoutSlotController CarriableSlot => _carriableSlot;

        //      1.2. Refs - Player
        private PlayerCarryLoadoutController _playerInteractionLoadoutController;
        private PlayerCarryLoadoutController PlayerInteractionLoadoutController => _playerInteractionLoadoutController;

        // --- Events ---
        public event Action<EquippedLoadoutSlotController> OnSlotClicked;

        // --- Public APIs ---
        public void Initialize(PlayerCarryLoadoutController playerInteractionLoadoutController)
        {
            _playerInteractionLoadoutController = playerInteractionLoadoutController;
        }

        public void Refresh(EquippedLoadoutSlotController selectedSlot)
        {
            if (this.CarriableSlot == null) return;

            InventoryItemEntry interactableEntry = this.PlayerInteractionLoadoutController != null
                ? this.PlayerInteractionLoadoutController.EquippedCarriableItemEntry
                : null;

            bool isSelected = selectedSlot == this.CarriableSlot;
            this.CarriableSlot.BindCarriableItem
            (
                carriableEntry: interactableEntry,
                isSelected: isSelected,
                isMainButtonInteractable: this.PlayerInteractionLoadoutController != null
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

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.CarriableSlot == null)
            {
                _carriableSlot = this.GetComponentInChildren<EquippedLoadoutSlotController>(true);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.BindSlotEvents();
        }
        protected override void UnbindInternalReferences()
        {
            base.UnbindInternalReferences();

            this.UnbindSlotEvents();
        }
    }
}