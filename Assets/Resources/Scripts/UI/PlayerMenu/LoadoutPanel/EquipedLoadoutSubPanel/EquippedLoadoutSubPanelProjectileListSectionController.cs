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
        [SerializeField] private List<EquippedLoadoutQuantityTextSlotController> _projectileSlots;
        private IReadOnlyList<EquippedLoadoutQuantityTextSlotController> ProjectileSlots => _projectileSlots;

        //  2. Player's References
        private CharacterCombatLoadoutController _playerCombatLoadoutController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;

        // --- Events ---
        public event Action<EquippedLoadoutSlotController> OnSlotClicked;

        // --- Public APIs ---
        public void Initialize(CharacterCombatLoadoutController playerCombatLoadoutController)
        {
            _playerCombatLoadoutController = playerCombatLoadoutController;

            if (this.PlayerCombatLoadoutController != null && _projectileSlots == null)
            {
                int maxProjectileCapacity = this.PlayerCombatLoadoutController.MaxEquippedProjectileCapacity;
                _projectileSlots = new List<EquippedLoadoutQuantityTextSlotController>(maxProjectileCapacity);
            }
        }

        public void Refresh(EquippedLoadoutSlotController selectedSlot)
        {
            IReadOnlyList<InventoryItemEntry> equippedProjectiles = this.PlayerCombatLoadoutController != null
                ? this.PlayerCombatLoadoutController.EquippedProjectileEntries
                : null;

            for (int i = 0; i < this.ProjectileSlots.Count; i++)
            {
                EquippedLoadoutQuantityTextSlotController slot = this.ProjectileSlots[i];
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

        public bool ContainsSlot(EquippedLoadoutSlotController slot)
        {
            return slot != null && this.ProjectileSlots.Contains(slot);
        }

        // --- Helpers ---
        private void BindSlotEvents()
        {
            foreach (EquippedLoadoutQuantityTextSlotController slot in this.ProjectileSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
                slot.OnClicked += this.HandleSlotClicked;
            }
        }
        private void UnbindSlotEvents()
        {
            foreach (EquippedLoadoutQuantityTextSlotController slot in this.ProjectileSlots)
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
