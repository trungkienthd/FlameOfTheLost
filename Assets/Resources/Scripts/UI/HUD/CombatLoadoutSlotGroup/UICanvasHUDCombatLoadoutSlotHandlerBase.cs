using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public abstract class UICanvasHUDCombatLoadoutSlotHandlerBase : FOTLMonoBehaviour
    {
        // --- States ---
        [Header("References - Main Icon")]
        [SerializeField] private Image _mainIcon;
        protected Image MainIcon => _mainIcon;

        // --- Public APIs ---
        public abstract void RefreshSlot(CharacterCombatLoadoutController combatLoadoutController);

        // --- Helpers ---
        //  1. Refresh Main Icon
        protected void RefreshMainIcon(Sprite icon)
        {
            this.RefreshImage(this.MainIcon, icon);
        }

        //  2. Refresh Image
        protected void RefreshImage(Image image, Sprite sprite)
        {
            if (image == null) return;

            bool hasSprite = sprite != null;

            image.enabled = hasSprite;
            image.sprite = sprite;
        }

        //  3. Initialization
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.MainIcon == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Main Icon is not set.");
            }
        }
    }
}
