using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EquippedLoadoutSubPanelWeaponListSectionController : EquippedLoadoutSubPanelSubBehaviourBase
    {
        // --- States ---
        //  1. References
        //      1.1. Refs - Weapons Section
        [Header("References")]
        [SerializeField] private List<EquippedLoadoutSubIconSlotController> _weaponSlots;
        private IReadOnlyList<EquippedLoadoutSubIconSlotController> WeaponSlots => _weaponSlots;

        // --- Events ---
        public event Action<EquippedLoadoutSlotController> OnSlotClicked;
        public event Action<EquippedLoadoutSlotController> OnSubSlotClicked;

        // --- Public APIs ---
        public void Refresh(EquippedLoadoutSlotController selectedSlot)
        {
            if (!this.EnsureBound()) return;
            if (this.WeaponSlots == null) return;

            IReadOnlyList<InventoryItemEntry> equippedWeaponEntries = this.PlayerCombatLoadoutController != null
                ? this.PlayerCombatLoadoutController.EquippedWeaponEntries
                : null;

            WeaponProfile activeEquippedWeapon = this.PlayerCombatLoadoutController != null && this.PlayerCombatLoadoutController.ActiveEquippedRuntimeWeapon != null
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
            return slot != null && this.WeaponSlots != null && this.WeaponSlots.Contains(slot);
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

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (_weaponSlots == null || this.WeaponSlots.Count <= 0) _weaponSlots = new List<EquippedLoadoutSubIconSlotController>(this.GetComponentsInChildren<EquippedLoadoutSubIconSlotController>(true));
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
