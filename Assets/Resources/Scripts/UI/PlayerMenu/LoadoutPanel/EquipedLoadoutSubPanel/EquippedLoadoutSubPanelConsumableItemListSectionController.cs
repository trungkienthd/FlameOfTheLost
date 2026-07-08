using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EquippedLoadoutSubPanelConsumableItemListSectionController : FOTLMonoBehaviour
    {
        // --- States ---
        //  1. References
        //      1.1. Refs - Consumable Section
        [Header("References")]
        [SerializeField] private List<EquippedLoadoutQuantityTextSlotController> _consumableSlots;
        private IReadOnlyList<EquippedLoadoutQuantityTextSlotController> ConsumableItemSlots => _consumableSlots;

        //      1.2. Refs - Player
        private CharacterCombatLoadoutController _playerCombatLoadoutController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;

        // --- Events ---
        public event Action<EquippedLoadoutSlotController> OnSlotClicked;

        // --- Public APIs ---
        public void Initialize(CharacterCombatLoadoutController playerCombatLoadoutController)
        {
            _playerCombatLoadoutController = playerCombatLoadoutController;

            if (this.PlayerCombatLoadoutController != null && _consumableSlots == null)
            {
                int maxConsumableCapacity = this.PlayerCombatLoadoutController.MaxEquippedConsumableCapacity;
                _consumableSlots = new List<EquippedLoadoutQuantityTextSlotController>(maxConsumableCapacity);
            }
        }

        public void Refresh(EquippedLoadoutSlotController selectedSlot)
        {
            IReadOnlyList<InventoryItemEntry> equippedConsumables = this.PlayerCombatLoadoutController != null
                ? this.PlayerCombatLoadoutController.EquippedConsumableItemEntries
                : null;

            for (int i = 0; i < this.ConsumableItemSlots.Count; i++)
            {
                EquippedLoadoutQuantityTextSlotController slot = this.ConsumableItemSlots[i];
                if (slot == null) continue;

                InventoryItemEntry entry = equippedConsumables != null && i < equippedConsumables.Count
                    ? equippedConsumables[i]
                    : null;

                bool isActive =
                    entry != null &&
                    this.PlayerCombatLoadoutController != null &&
                    UnityEngine.Object.ReferenceEquals(entry, this.PlayerCombatLoadoutController.ActiveEquippedConsumableItemEntry);

                bool isSelected = selectedSlot == slot;

                bool slotInteractable =
                    this.PlayerCombatLoadoutController != null &&
                    i < this.PlayerCombatLoadoutController.MaxEquippedConsumables;

                slot.BindConsumableItem
                (
                    consumableEntry: entry,
                    slotIndex: i,
                    isActive: isActive,
                    isSelected: isSelected,
                    isMainButtonInteractable: slotInteractable
                );
            }
        }

        // --- Helpers ---
        public bool ContainsSlot(EquippedLoadoutQuantityTextSlotController slot)
        {
            return slot != null && _consumableSlots.Contains(slot);
        }
        private void BindSlotEvents()
        {
            foreach (EquippedLoadoutQuantityTextSlotController slot in this.ConsumableItemSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
                slot.OnClicked += this.HandleSlotClicked;
            }
        }

        private void UnbindSlotEvents()
        {
            foreach (EquippedLoadoutQuantityTextSlotController slot in this.ConsumableItemSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
            }
        }

        private void HandleSlotClicked(EquippedLoadoutSlotController slot)
        {
            this.OnSlotClicked?.Invoke(slot);
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

