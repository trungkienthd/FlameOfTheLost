using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EquippedLoadoutSubPanelConsumableItemListSectionController : MonoBehaviour
    {
        // --- States ---
        //  1. References
        //      1.1. Refs - Consumable Section
        [Header("References")]
        [SerializeField] private List<EquippedLoadoutSlotUIController> _consumableSlots = new List<EquippedLoadoutSlotUIController>(8);
        private IReadOnlyList<EquippedLoadoutSlotUIController> ConsumableItemSlots => _consumableSlots;

        //      1.2. Refs - Player
        private CharacterCombatLoadoutController _playerCombatLoadoutController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;

        //  2. Events
        public event Action<EquippedLoadoutSlotUIController> OnSlotClicked;

        // --- Unity Callbacks ---
        private void Awake()
        {
            this.BindSlotEvents();
        }
        private void OnDestroy()
        {
            this.UnbindSlotEvents();
        }

        // --- Public APIs ---
        public void Initialize(CharacterCombatLoadoutController playerCombatLoadoutController)
        {
            _playerCombatLoadoutController = playerCombatLoadoutController;
        }

        public void Refresh(EquippedLoadoutSlotUIController selectedSlot)
        {
            IReadOnlyList<InventoryItemEntry> equippedConsumables = this.PlayerCombatLoadoutController != null
                ? this.PlayerCombatLoadoutController.EquippedConsumableItemEntries
                : null;

            for (int i = 0; i < this.ConsumableItemSlots.Count; i++)
            {
                EquippedLoadoutSlotUIController slot = this.ConsumableItemSlots[i];
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
        public bool ContainsSlot(EquippedLoadoutSlotUIController slot)
        {
            return slot != null && _consumableSlots.Contains(slot);
        }
        private void BindSlotEvents()
        {
            foreach (EquippedLoadoutSlotUIController slot in this.ConsumableItemSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
                slot.OnClicked += this.HandleSlotClicked;
            }
        }

        private void UnbindSlotEvents()
        {
            foreach (EquippedLoadoutSlotUIController slot in this.ConsumableItemSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
            }
        }
        private void HandleSlotClicked(EquippedLoadoutSlotUIController slot)
        {
            this.OnSlotClicked?.Invoke(slot);
        }
    }
}

