using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EquippedLoadoutSubPanelWeaponListSectionController : FOTLMonoBehaviour
    {
        // --- States ---
        //  1. References
        //      1.1. Refs - Weapons Section
        [Header("References")]
        [SerializeField] private List<EquippedLoadoutSubIconSlotController> _weaponSlots;
        private IReadOnlyList<EquippedLoadoutSubIconSlotController> WeaponSlots => _weaponSlots;

        //      1.2. Refs - Player
        private CharacterCombatLoadoutController _playerCombatLoadoutController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;

        // --- Events ---
        public event Action<EquippedLoadoutSlotController> OnSlotClicked;
        public event Action<EquippedLoadoutSlotController> OnSubSlotClicked;

        // --- Public APIs ---
        public void Initialize(CharacterCombatLoadoutController playerCombatLoadoutController)
        {
            _playerCombatLoadoutController = playerCombatLoadoutController;

            if (this.PlayerCombatLoadoutController != null && _weaponSlots == null)
            {
                int maxEntryCapacity = this.PlayerCombatLoadoutController.MaxEquippedWeaponCapacity;
                _weaponSlots = new List<EquippedLoadoutSubIconSlotController>(maxEntryCapacity);
            }
        }

        public void Refresh(EquippedLoadoutSlotController selectedSlot)
        {
            IReadOnlyList<InventoryItemEntry> equippedWeaponEntries = this.PlayerCombatLoadoutController != null
                ? this.PlayerCombatLoadoutController.EquippedWeaponEntries
                : null;

            WeaponProfile activeEquippedWeapon = this.PlayerCombatLoadoutController != null &&
                                                 this.PlayerCombatLoadoutController.ActiveEquippedRuntimeWeapon != null
                ? this.PlayerCombatLoadoutController.ActiveEquippedRuntimeWeapon.Profile
                : null;

            for (int i = 0; i < this.WeaponSlots.Count; i++)
            {
                EquippedLoadoutSubIconSlotController slot = this.WeaponSlots[i];
                if (slot == null) continue;

                InventoryItemEntry weaponEntry = equippedWeaponEntries != null && i < equippedWeaponEntries.Count
                    ? equippedWeaponEntries[i]
                    : null;

                WeaponProfile weapon = weaponEntry != null
                    ? weaponEntry.Item as WeaponProfile
                    : null;

                SkillProfile activeSpecialSkill = weaponEntry != null && this.PlayerCombatLoadoutController != null
                    ? this.PlayerCombatLoadoutController.GetWeaponActiveSpecialSkill(weaponEntry) ?? weapon?.InnateSpecialSkill
                    : null;

                bool isActive =
                    weaponEntry != null &&
                    UnityEngine.Object.ReferenceEquals(weapon, activeEquippedWeapon);

                bool isSelected = selectedSlot == slot;

                bool slotInteractable =
                    this.PlayerCombatLoadoutController != null &&
                    i < this.PlayerCombatLoadoutController.MaxEquippedWeapons;

                slot.BindWeapon
                (
                    weaponEntry: weaponEntry,
                    activeSpecialSkill: activeSpecialSkill,
                    slotIndex: i,
                    isActive: isActive,
                    isSelected: isSelected,
                    isMainButtonInteractable: slotInteractable,
                    isSubSlotButtonInteractable: slotInteractable
                );
            }
        }

        public bool ContainsSlot(EquippedLoadoutSubIconSlotController slot)
        {
            return slot != null && _weaponSlots.Contains(slot);
        }


        // --- Helpers ---
        private void BindSlotEvents()
        {
            foreach (EquippedLoadoutSubIconSlotController slot in this.WeaponSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
                slot.OnClicked += this.HandleSlotClicked;

                slot.OnSubSlotClicked -= this.HandleSubSlotClicked;
                slot.OnSubSlotClicked += this.HandleSubSlotClicked;
            }
        }
        private void UnbindSlotEvents()
        {
            foreach (EquippedLoadoutSubIconSlotController slot in this.WeaponSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
                slot.OnSubSlotClicked -= this.HandleSubSlotClicked;
            }
        }

        private void HandleSlotClicked(EquippedLoadoutSlotController slot)
        {
            this.OnSlotClicked?.Invoke(slot);
        }
        private void HandleSubSlotClicked(EquippedLoadoutSlotController slot)
        {
            this.OnSubSlotClicked?.Invoke(slot);
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
