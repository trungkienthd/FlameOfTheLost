using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class EquippedLoadoutSubIconSlotController : EquippedLoadoutSlotController
    {
        // --- States ---
        [Header("References - Sub Icon Slot")]
        [SerializeField] private Image _subSlotIcon;
        private Image SubSlotIcon => _subSlotIcon;

        // --- Public APIs ---
        public void BindWeapon(InventoryItemEntry weaponEntry, SkillProfile activeSpecialSkill, int slotIndex, bool isActive, bool isSelected, bool isMainButtonInteractable, bool isSubSlotButtonInteractable)
        {
            this.ResetBindings();

            this.SlotType = LoadoutPanelSlotType.Weapon;
            this.SlotIndex = slotIndex;
            this.BoundWeaponEntry = weaponEntry;
            this.BoundWeaponSpecialSkill = activeSpecialSkill;

            this.IsActive = isActive;
            this.IsSelected = isSelected;

            bool hasWeapon =
                weaponEntry != null &&
                weaponEntry.Item != null;

            this.SetMainIcon
            (
                sprite: hasWeapon ? weaponEntry.Icon : null,
                hasContent: hasWeapon
            );

            this.SetSubSlotVisible(true);

            this.ConfigureSubIcon
            (
                sprite: activeSpecialSkill != null ? activeSpecialSkill.Icon : null,
                hasContent: activeSpecialSkill != null
            );

            this.SetMainButtonInteractable(isMainButtonInteractable);

            bool subSlotInteractable = hasWeapon && isSubSlotButtonInteractable;
            this.SetSubSlotButtonInteractable
            (
                value: subSlotInteractable,
                keepDisabledVisualAsNormal: isMainButtonInteractable
            );

            this.RefreshSlotColor();
        }

        // --- Helpers ---
        protected override void ClearSubSlotContent()
        {
            base.ClearSubSlotContent();

            if (this.SubSlotIcon == null) return;

            this.SubSlotIcon.sprite = null;
            this.SubSlotIcon.enabled = false;
        }

        protected override bool CanSubmitSubSlotClick()
        {
            return this.SlotType == LoadoutPanelSlotType.Weapon &&
                   this.BoundWeaponEntry != null &&
                   this.BoundWeaponEntry.Item != null;
        }

        private void ConfigureSubIcon(Sprite sprite, bool hasContent)
        {
            if (this.SubSlotIcon == null) return;

            this.SubSlotIcon.sprite = sprite;
            this.SubSlotIcon.enabled = hasContent;
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.SubSlotIcon == null && this.SubSlotRoot != null)
            {
                Transform iconTransform = this.SubSlotRoot.transform.Find("Icon");
                if (iconTransform != null)
                {
                    _subSlotIcon = iconTransform.GetComponent<Image>();
                }
            }
        }

    }
}
