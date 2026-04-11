using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public partial class CandidateLoadoutSubPanelController : MonoBehaviour
    {
        private void HandleCandidateSlotClicked(CandidateLoadoutSlotUIController slot)
        {
            if (slot == null) return;

            bool success = this.TryApplyCandidate(slot);
            if (!success) return;

            this.EquippedLoadoutSubPanelController?.RefreshAll();

            if (this.CurrentMode == CandidateLoadoutSelectionModeType.ChangeWeaponSpecialSkill)
            {
                this.RefreshSpecialSkillCandidatesForCurrentTarget();
                return;
            }

            this.HidePanel();
        }

        private bool TryApplyCandidate(CandidateLoadoutSlotUIController slot)
        {
            switch (this.CurrentMode)
            {
                case CandidateLoadoutSelectionModeType.ReplaceWeapon:
                    return this.TryApplyWeaponCandidate(slot.BoundWeaponEntry);

                case CandidateLoadoutSelectionModeType.ReplaceConsumableItem:
                    return this.TryApplyConsumableCandidate(slot.BoundConsumableEntry);

                case CandidateLoadoutSelectionModeType.ReplaceInteractableItem:
                    return this.TryApplyInteractableCandidate(slot.BoundInteractableItem);

                case CandidateLoadoutSelectionModeType.ChangeWeaponSpecialSkill:
                    return this.TryApplySpecialSkillCandidate(slot.BoundSpecialSkill);
            }

            return false;
        }

        private bool TryApplyWeaponCandidate(InventoryItemEntry weaponEntry)
        {
            if (weaponEntry == null || this.TargetSlot == null || this.PlayerCombatLoadoutController == null) return false;
            if (this.TargetSlot.SlotType != LoadoutPanelSlotType.Weapon) return false;
            
            return this.PlayerCombatLoadoutController.TryReplaceEquippedWeaponEntry(this.TargetSlot.SlotIndex, weaponEntry, out _);
        }

        private bool TryApplyConsumableCandidate(InventoryItemEntry consumableEntry)
        {
            if (consumableEntry == null || this.TargetSlot == null || this.PlayerCombatLoadoutController == null) return false;
            if (this.TargetSlot.SlotType != LoadoutPanelSlotType.Consumable) return false; 

            return this.PlayerCombatLoadoutController.TryReplaceEquippedConsumableItemEntry(this.TargetSlot.SlotIndex, consumableEntry, out _);
        }

        private bool TryApplyInteractableCandidate(InventoryItemEntry interactableEntry)
        {
            if (interactableEntry == null || this.TargetSlot == null || this.PlayerInteractionLoadoutController == null) return false;
            if (this.TargetSlot.SlotType != LoadoutPanelSlotType.InteractableItem) return false;

            return this.PlayerInteractionLoadoutController.TryReplaceEquippedInteractabletemEntry(interactableEntry, out _);
        }

        private bool TryApplySpecialSkillCandidate(SkillProfile skill)
        {
            if (skill == null || this.TargetSlot == null || this.PlayerCombatLoadoutController == null) return false;
            if (this.TargetSlot.BoundWeaponEntry == null) return false;

            return this.PlayerCombatLoadoutController.TrySetEquippedWeaponActiveSpecialSkill(this.TargetSlot.BoundWeaponEntry, skill);
        }

        private void RefreshSpecialSkillCandidatesForCurrentTarget()
        {
            if (this.TargetSlot == null || this.TargetSlot.BoundWeaponEntry == null || this.PlayerInventoryController == null)
            {
                this.HidePanel();
                return;
            }

            this.ShowSpecialSkillCandidates(this.TargetSlot);
        }
    }
}
