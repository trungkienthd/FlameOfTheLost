using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EquippedLoadoutSubPanelCarriableItemSectionController : MonoBehaviour
    {
        // --- States ---
        //  1. References
        //      1.1. Refs - Interactable Section
        [Header("References")]
        [SerializeField] private EquippedLoadoutSlotUIController _interactableSlot;
        private EquippedLoadoutSlotUIController InteractableItemSlot => _interactableSlot;

        //      1.2. Refs - Player
        private PlayerCarryLoadoutController _playerInteractionLoadoutController;
        private PlayerCarryLoadoutController PlayerInteractionLoadoutController => _playerInteractionLoadoutController;

        //  2. Events
        public event Action<EquippedLoadoutSlotUIController> OnSlotClicked;

        // --- Unity Callbacks ---
        private void Awake()
        {
            this.CacheReferences();

            this.BindSlotEvents();
        }

        private void OnDestroy()
        {
            this.UnbindSlotEvents();
        }

        // --- Public APIs ---
        public void Initialize(PlayerCarryLoadoutController playerInteractionLoadoutController)
        {
            _playerInteractionLoadoutController = playerInteractionLoadoutController;
        }

        public void Refresh(EquippedLoadoutSlotUIController selectedSlot)
        {
            if (this.InteractableItemSlot == null) return;

            InventoryItemEntry interactableEntry = this.PlayerInteractionLoadoutController != null
                ? this.PlayerInteractionLoadoutController.EquippedCarriableItemEntry
                : null;

            bool isSelected = selectedSlot == this.InteractableItemSlot;
            this.InteractableItemSlot.BindCarriableItem
            (
                carriableEntry: interactableEntry,
                isSelected: isSelected,
                isMainButtonInteractable: this.PlayerInteractionLoadoutController != null
            );
        }

        public bool ContainsSlot(EquippedLoadoutSlotUIController slot)
        {
            return slot != null && this.InteractableItemSlot == slot;
        }

        // --- Helpers ---
        private void CacheReferences()
        {
            if (this.InteractableItemSlot == null)
            {
                _interactableSlot = this.GetComponentInChildren<EquippedLoadoutSlotUIController>(true);
            }
        }
        private void BindSlotEvents()
        {
            if (this.InteractableItemSlot == null) return;

            this.InteractableItemSlot.OnClicked -= this.HandleSlotClicked;
            this.InteractableItemSlot.OnClicked += this.HandleSlotClicked;
        }
        private void UnbindSlotEvents()
        {
            if (this.InteractableItemSlot == null) return;

            this.InteractableItemSlot.OnClicked -= this.HandleSlotClicked;
        }
        private void HandleSlotClicked(EquippedLoadoutSlotUIController slot)
        {
            this.OnSlotClicked?.Invoke(slot);
        }
    }
}