using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class UICanvasHUDCombatLoadoutWeaponSlotHandler : UICanvasHUDCombatLoadoutSlotHandlerBase
    {
        // --- States ---
        [Header("References - Special Skill")]
        [SerializeField] private Image _activeSpecialSkillIcon;
        private Image ActiveSpecialSkillIcon => _activeSpecialSkillIcon;

        // --- Public APIs ---
        public override void RefreshSlot(CharacterCombatLoadoutController combatLoadoutController)
        {
            if (combatLoadoutController == null)
            {
                this.RefreshMainIcon(null);
                this.RefreshImage(this.ActiveSpecialSkillIcon, null);
                return;
            }

            Sprite weaponIcon = combatLoadoutController.GetActiveEquippedWeaponIcon();
            Sprite activeSpecialSkillIcon = combatLoadoutController.GetActiveSpecialSkillIcon();

            this.RefreshMainIcon(weaponIcon);
            this.RefreshImage(this.ActiveSpecialSkillIcon, activeSpecialSkillIcon);
        }

        // --- Helpers ---
        //  1. Initialization
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.ActiveSpecialSkillIcon == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDCombatLoadoutWeaponSlotHandler)}] Active Special Skill Icon is not set.");
            }
        }
    }
}