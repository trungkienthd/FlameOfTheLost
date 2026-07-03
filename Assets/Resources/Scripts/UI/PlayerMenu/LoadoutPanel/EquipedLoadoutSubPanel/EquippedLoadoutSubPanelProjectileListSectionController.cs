using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EquippedLoadoutSubPanelProjectileListSectionController : FOTLMonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private List<EquippedLoadoutSlotUIController> _projectileSlots = new List<EquippedLoadoutSlotUIController>(1);
        private IReadOnlyList<EquippedLoadoutSlotUIController> ProjectileSlots => _projectileSlots;

        //  2. Player's References
        private CharacterCombatLoadoutController _playerCombatLoadoutController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;

        // --- Events ---
        public event Action<EquippedLoadoutSlotUIController> OnSlotClicked;

        // --- Public APIs ---
        public void Initialize(CharacterCombatLoadoutController playerCombatLoadoutController)
        {
            _playerCombatLoadoutController = playerCombatLoadoutController;
        }

        public void Refresh(EquippedLoadoutSlotUIController selectedSlot)
        {
            IReadOnlyList<InventoryItemEntry> equippedProjectiles = this.PlayerCombatLoadoutController != null
                ? this.PlayerCombatLoadoutController.EquippedProjectileEntries
                : null;

            for (int i = 0; i < this.ProjectileSlots.Count; i++)
            {
                EquippedLoadoutSlotUIController slot = this.ProjectileSlots[i];
                if (slot == null) continue;

                InventoryItemEntry entry = equippedProjectiles != null && i < equippedProjectiles.Count
                    ? equippedProjectiles[i]
                    : null;

                bool isSelected = selectedSlot == slot;

                bool slotInteractable =
                    this.PlayerCombatLoadoutController != null &&
                    i < this.PlayerCombatLoadoutController.MaxEquippedProjectiles;

                slot.BindProjectileItem
                (
                    projectileEntry: entry,
                    slotIndex: i,
                    isSelected: isSelected,
                    isMainButtonInteractable: slotInteractable
                );
            }
        }

        public bool ContainsSlot(EquippedLoadoutSlotUIController slot)
        {
            return slot != null && this.ProjectileSlots.Contains(slot);
        }

        // --- Helpers ---
        private void BindSlotEvents()
        {
            foreach (EquippedLoadoutSlotUIController slot in this.ProjectileSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
                slot.OnClicked += this.HandleSlotClicked;
            }
        }
        private void UnbindSlotEvents()
        {
            foreach (EquippedLoadoutSlotUIController slot in this.ProjectileSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
            }
        }

        private void HandleSlotClicked(EquippedLoadoutSlotUIController slot)
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
