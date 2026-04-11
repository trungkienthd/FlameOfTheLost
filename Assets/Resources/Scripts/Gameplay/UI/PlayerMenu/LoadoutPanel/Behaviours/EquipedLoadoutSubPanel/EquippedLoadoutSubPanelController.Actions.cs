using UnityEngine;

namespace FlameOfTheLost
{
    public partial class EquippedLoadoutSubPanelController : MonoBehaviour
    {
        private void HandleTooltipUnequipClicked(EquippedLoadoutSlotUIController slot)
        {
            if (slot == null) return;

            bool success = false;
            switch (slot.SlotType)
            {
                case LoadoutPanelSlotType.Weapon:
                    success = this.TryUnequipWeapon(slot);
                    break;

                case LoadoutPanelSlotType.Consumable:
                    success = this.TryUnequipConsumable(slot);
                    break;

                case LoadoutPanelSlotType.InteractableItem:
                    success = this.TryUnequipInteractable(slot);
                    break;
            }

            if (!success) return;

            this.ClearSelection();
            this.HideCandidatePanel();
        }

        private bool TryUnequipWeapon(EquippedLoadoutSlotUIController slot)
        {
            if (slot == null || this.PlayerCombatLoadoutController == null) return false;

            // Replace this with your real API name if needed.
            return this.PlayerCombatLoadoutController.TryUnequipWeaponEntry(slot.BoundWeaponEntry);
        }

        private bool TryUnequipConsumable(EquippedLoadoutSlotUIController slot)
        {
            if (slot == null || this.PlayerCombatLoadoutController == null) return false;

            // Replace this with your real API name if needed.
            return this.PlayerCombatLoadoutController.TryUnequipEquippedConsumableItemEntry(slot.BoundConsumableEntry);
        }

        private bool TryUnequipInteractable(EquippedLoadoutSlotUIController slot)
        {
            if (slot == null || this.PlayerInteractionLoadoutController == null) return false;

            // Replace this with your real API name if needed.
            return this.PlayerInteractionLoadoutController.TryUnequipInteractableEntry();
        }

        private void HandleTooltipReplaceClicked(EquippedLoadoutSlotUIController slot)
        {
            if (slot == null || this.CandidateLoadoutSubPanelController == null) return;

            this.SelectedSlot = slot;
            this.HideTooltip();
            this.RefreshAll();

            switch (slot.SlotType)
            {
                case LoadoutPanelSlotType.Weapon:
                    this.CandidateLoadoutSubPanelController.ShowWeaponCandidates(slot);
                    break;

                case LoadoutPanelSlotType.Consumable:
                    this.CandidateLoadoutSubPanelController.ShowConsumableCandidates(slot);
                    break;

                case LoadoutPanelSlotType.InteractableItem:
                    this.CandidateLoadoutSubPanelController.ShowInteractableCandidates(slot);
                    break;
            }
        }

        private void HandleTooltipChangeSkillClicked(EquippedLoadoutSlotUIController slot)
        {
            if (slot == null || slot.BoundWeaponEntry == null) return;
            
            this.SelectedSlot = slot;
            this.HideTooltip();
            this.RefreshAll();

            this.CandidateLoadoutSubPanelController?.ShowSpecialSkillCandidates(slot);
        }
    }
}