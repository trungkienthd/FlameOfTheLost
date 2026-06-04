using System;

namespace FlameOfTheLost
{
    public class PlayerLocomotionVisualModeProvider : CharacterBehaviourBaseController, ICharacterLocomotionVisualModeProvider
    {
        // --- States ---
        private PlayerCarryLoadoutController PlayerCarryLoadoutController
        {
            get
            {
                if (this.CharacterRootController is not PlayerRootController playerComponentController) return null;

                return playerComponentController.PlayerCarryLoadoutController;
            }
        }

        public event Action OnLocomotionVisualModeChanged;

        // --- Unity Methods ---
        private void OnEnable()
        {
            this.BindEvents();
        }

        private void OnDisable()
        {
            this.UnbindEvents();
        }

        // --- Public APIs ---
        public CharacterLocomotionMode GetLocomotionMode()
        {
            if (this.PlayerCarryLoadoutController != null && this.PlayerCarryLoadoutController.IsCarryingCarriableItem)
            {
                return CharacterLocomotionMode.Carry;
            }

            return CharacterLocomotionMode.Default;
        }

        public CharacterCarryItemType GetCarryItemType()
        {
            return this.PlayerCarryLoadoutController != null
                ? this.PlayerCarryLoadoutController.GetCharacterCarryItemType()
                : CharacterCarryItemType.None;
        }

        public LightSourceType GetLightSourceType()
        {
            return this.PlayerCarryLoadoutController != null
                ? this.PlayerCarryLoadoutController.GetCurrentLightSourceType()
                : LightSourceType.None;
        }

        // --- Helpers ---
        private void BindEvents()
        {
            if (this.PlayerCarryLoadoutController == null) return;

            this.PlayerCarryLoadoutController.OnCarriableItemCarryingStateChanged += this.OnCarryingStateChanged;
            this.PlayerCarryLoadoutController.OnEquippedCarriableItemChanged += this.OnEquippedCarriableItemChanged;
        }

        private void UnbindEvents()
        {
            if (this.PlayerCarryLoadoutController == null) return;

            this.PlayerCarryLoadoutController.OnCarriableItemCarryingStateChanged -= this.OnCarryingStateChanged;
            this.PlayerCarryLoadoutController.OnEquippedCarriableItemChanged -= this.OnEquippedCarriableItemChanged;
        }

        private void OnCarryingStateChanged(bool isCarrying)
        {
            this.OnLocomotionVisualModeChanged?.Invoke();
        }

        private void OnEquippedCarriableItemChanged(InventoryItemEntry carriableEntry)
        {
            this.OnLocomotionVisualModeChanged?.Invoke();
        }
    }
}
