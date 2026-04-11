using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public partial class EquippedLoadoutSubPanelController : MonoBehaviour
    {
        public void RefreshAll()
        {
            this.RefreshWeapons();
            this.RefreshConsumableItems();
            this.RefreshInteractable();
        }

        private void RefreshWeapons()
        {
            IReadOnlyList<InventoryItemEntry> equippedWeaponEntries = this.PlayerCombatLoadoutController != null
                ? this.PlayerCombatLoadoutController.EquippedWeaponEntries
                : null;

            WeaponProfile activeEquippedWeapon = this.PlayerCombatLoadoutController != null && this.PlayerCombatLoadoutController.ActiveEquippedRuntimeWeapon != null
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

                bool isActive = weaponEntry != null && UnityEngine.Object.ReferenceEquals(weapon, activeEquippedWeapon);
                bool isSelected = this.SelectedSlot == slot;
                bool slotInteractable = this.PlayerCombatLoadoutController != null && i < this.PlayerCombatLoadoutController.MaxEquippedWeapons;

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

        private void RefreshConsumableItems()
        {
            IReadOnlyList<InventoryItemEntry> equippedConsumables = this.PlayerCombatLoadoutController != null ? this.PlayerCombatLoadoutController.EquippedConsumableItemEntries : null;

            for (int i = 0; i < this.ConsumableItemSlots.Count; i++)
            {
                EquippedLoadoutSlotUIController slot = this.ConsumableItemSlots[i];
                if (slot == null) continue;

                InventoryItemEntry entry = equippedConsumables != null && i < equippedConsumables.Count
                    ? equippedConsumables[i]
                    : null;

                bool isActive = entry != null && this.PlayerCombatLoadoutController != null && UnityEngine.Object.ReferenceEquals(entry, this.PlayerCombatLoadoutController.ActiveEquippedConsumableItemEntry);
                bool isSelected = this.SelectedSlot == slot;
                bool slotInteractable = this.PlayerCombatLoadoutController != null && i < this.PlayerCombatLoadoutController.MaxEquippedConsumables;

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

        private void RefreshInteractable()
        {
            if (this.InteractableItemSlot == null) return;

            InventoryItemEntry interactableEntry = this.PlayerInteractionLoadoutController != null ? this.PlayerInteractionLoadoutController.EquippedInteractableItemEntry : null;

            bool isSelected = this.SelectedSlot == this.InteractableItemSlot;
            this.InteractableItemSlot.BindInteractableItem
            (
                interactableEntry: interactableEntry,
                isSelected: isSelected,
                isMainButtonInteractable: this.PlayerInteractionLoadoutController != null
            );
        }
    }
}