using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public partial class EquippedLoadoutSubPanelController : MonoBehaviour
    {
        private Vector2 _lastPointerScreenPosition;
        private Vector2 LastPointerScreenPosition
        {
            get => _lastPointerScreenPosition;
            set => _lastPointerScreenPosition = value;
        }

        private void HandleSlotClicked(EquippedLoadoutSlotUIController slot)
        {
            if (slot == null) return;

            this.LastPointerScreenPosition = this.GetCurrentPointerScreenPosition();

            if (this.SelectedSlot == slot)
            {
                this.SelectedSlot = null;
                this.HideTooltip();
                this.HideCandidatePanel();
                this.RefreshAll();
                return;
            }

            this.SelectedSlot = slot;
            this.HideCandidatePanel();

            // Refresh first so selection visuals are up to date before the tooltip is positioned/shown.
            this.RefreshAll();

            this.ShowTooltipForSelectedSlot();
        }

        private void HandleWeaponSubSlotClicked(EquippedLoadoutSlotUIController slot)
        {
            if (slot == null || slot.BoundWeaponEntry == null)
            {
                this.HideCandidatePanel();
                return;
            }

            this.LastPointerScreenPosition = this.GetCurrentPointerScreenPosition();

            this.SelectedSlot = slot;
            this.HideTooltip();
            this.RefreshAll();

            this.CandidateLoadoutSubPanelController?.ShowSpecialSkillCandidates(slot);
        }


        private void ShowTooltipForSelectedSlot()
        {
            if (this.SlotActionTooltip == null) return;

            if (this.SelectedSlot == null)
            {
                this.HideTooltip();
                return;
            }

            RectTransform parentRect = this.TooltipParent != null
                ? this.TooltipParent
                : this.transform.parent as RectTransform;

            if (parentRect == null)
            {
                this.HideTooltip();
                return;
            }

            bool isWeapon = this.SelectedSlot.SlotType == LoadoutPanelSlotType.Weapon;
            this.SlotActionTooltip.ShowForSlotAtScreenPosition
            (
                slot: this.SelectedSlot, 
                tooltipParent: parentRect, 
                screenPosition: this.LastPointerScreenPosition,
                offset: this.TooltipOffset,
                changeSkillBtnIsActive: isWeapon
            );
        }

        private void HideTooltip()
        {
            if (this.SlotActionTooltip == null) return;
            this.SlotActionTooltip.Hide();
        }

        private Vector2 GetCurrentPointerScreenPosition()
        {
            if (Mouse.current != null)
            {
                return Mouse.current.position.ReadValue();
            }

            return Input.mousePosition;
        }

        private void HideCandidatePanel()
        {
            if (this.CandidateLoadoutSubPanelController == null) return;
            this.CandidateLoadoutSubPanelController.HidePanel();
        }
    }
}