using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class EquippedLoadoutSubPanelWeaponListSectionController : MonoBehaviour
    {
        // --- States ---
        //  1. References
        //      1.1. Refs - Weapons Section
        [Header("References")]
        [SerializeField] private List<EquippedLoadoutSlotUIController> _weaponSlots = new List<EquippedLoadoutSlotUIController>(4);
        private IReadOnlyList<EquippedLoadoutSlotUIController> WeaponSlots => _weaponSlots;

        //      1.2. Refs - Player
        private CharacterCombatLoadoutController _playerCombatLoadoutController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;

        //  2. Events
        public event Action<EquippedLoadoutSlotUIController> OnSlotClicked;
        public event Action<EquippedLoadoutSlotUIController> OnSubSlotClicked;

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
            IReadOnlyList<InventoryItemEntry> equippedWeaponEntries = this.PlayerCombatLoadoutController != null
                ? this.PlayerCombatLoadoutController.EquippedWeaponEntries
                : null;

            WeaponProfile activeEquippedWeapon = this.PlayerCombatLoadoutController != null &&
                                                 this.PlayerCombatLoadoutController.ActiveEquippedRuntimeWeapon != null
                ? this.PlayerCombatLoadoutController.ActiveEquippedRuntimeWeapon.Profile
                : null;

            for (int i = 0; i < this.WeaponSlots.Count; i++)
            {
                EquippedLoadoutSlotUIController slot = this.WeaponSlots[i];
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

        public bool ContainsSlot(EquippedLoadoutSlotUIController slot)
        {
            return slot != null && _weaponSlots.Contains(slot);
        }


        // --- Helpers ---
        private void BindSlotEvents()
        {
            foreach (EquippedLoadoutSlotUIController slot in this.WeaponSlots)
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
            foreach (EquippedLoadoutSlotUIController slot in this.WeaponSlots)
            {
                if (slot == null) continue;

                slot.OnClicked -= this.HandleSlotClicked;
                slot.OnSubSlotClicked -= this.HandleSubSlotClicked;
            }
        }

        private void HandleSlotClicked(EquippedLoadoutSlotUIController slot)
        {
            this.OnSlotClicked?.Invoke(slot);
        }
        private void HandleSubSlotClicked(EquippedLoadoutSlotUIController slot)
        {
            this.OnSubSlotClicked?.Invoke(slot);
        }
    }
}
