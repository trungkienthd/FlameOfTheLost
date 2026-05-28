using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerCarryLoadoutController))]
    public class PlayerInputHandler : CharacterInputHandler
    {
        [Header("References (UI")]
        [SerializeField] private PlayerMenuManager _playerMenuManager;
        [SerializeField] private PlayerInteractionPanelListManager _playerInteractionPanelListManager;
        private PlayerMenuManager PlayerMenuManager => _playerMenuManager;
        private PlayerInteractionPanelListManager PlayerInteractionPanelListManager => _playerInteractionPanelListManager;
        private PlayerCarryLoadoutController PlayerCarryLoadoutController
        {
            get 
            {
                var playerComponentController = this.CharacterComponentController as PlayerComponentController;
                return playerComponentController != null ? playerComponentController.PlayerCarryLoadoutController : null;
            }
        }
        private PlayerInteractionHandler PlayerInteractionHandler
        {
            get
            {
                var playerComponentController = this.CharacterComponentController as PlayerComponentController;
                return playerComponentController != null ? playerComponentController.PlayerInteractionHandler : null;
            }
        }

        [Header("References (Player Input")]
        [SerializeField] private PlayerInput _playerInput;
        private PlayerInput PlayerInput => _playerInput;

        // --- Public APIs ---
        //  1. Called by PlayerInput
        //      1.1. Movement
        public void OnMove(InputAction.CallbackContext ctx)
        {
            Vector2 moveInput = ctx.ReadValue<Vector2>();

            this.SetMoveInput(moveInput);
        }
        //      1.2. Attacking
        public void OnNormalAttackLeft(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            this.RequestNormalAttack(AttackSide.Left);
        }
        public void OnNormalAttackRight(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            this.RequestNormalAttack(AttackSide.Right);
        }
        public void OnSpecialAttackLeft(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            this.RequestSpecialAttack(AttackSide.Left);
        }
        public void OnSpecialAttackRight(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            this.RequestSpecialAttack(AttackSide.Right);
        }

        //      1.3. Loadout
        public void OnSwitchWeapon(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            int direction = this.ReadInputDirection(ctx);

            this.RequestCycleWeapon(direction);
        }

        public void OnQuickEquipWeapon(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            this.RequestQuickEquipWeapon();
        }

        public void OnSwitchConsumableItem(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            int direction = this.ReadInputDirection(ctx);

            this.RequestCycleConsumableItem(direction);
        }

        //      1.4. Other Actions
        public void OnConsumeConsumableItem(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            this.RequestConsumeConsumableItem();
        }

        //          1.4.1. Player-only actions
        public void OnCarryCarriableItem(InputAction.CallbackContext ctx)
        {
            if (this.PlayerCarryLoadoutController == null) return;

            if (ctx.started)
            {
                this.PlayerCarryLoadoutController.TrySetCarriableItemCarryingState(true);
            }
            else if (ctx.canceled)
            {
                this.PlayerCarryLoadoutController.TrySetCarriableItemCarryingState(false);
            }
        }

        //      1.5. Interaction
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (this.PlayerInteractionHandler == null) return;

            this.PlayerInteractionHandler.TryInteractNearest();
        }

        //      1.6. Player-only Menu Actions
        public void OnPause(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (this.PlayerMenuManager == null) return;

            this.PlayerMenuManager.OpenSettingsPanel();
        }

        public void OnOpenSettingPanel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (this.PlayerMenuManager == null) return;

            this.PlayerMenuManager.OpenSettingsPanel();
        }

        public void OnOpenInventoryPanel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (this.PlayerMenuManager == null) return;

            this.PlayerMenuManager.OpenInventoryPanel();
        }

        public void OnOpenLoadoutPanel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (this.PlayerMenuManager == null) return;

            this.PlayerMenuManager.OpenLoadoutPanel();
        }

        public void OnOpenQuestPanel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (this.PlayerMenuManager == null) return;

            this.PlayerMenuManager.OpenQuestPanel();
        }

        public void OnOpenMapPanel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (this.PlayerMenuManager == null) return;

            this.PlayerMenuManager.OpenMapPanel();
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (this.PlayerMenuManager == null) return;

            this.PlayerMenuManager.HandleCancel();
        }

        public void OnCloseInteractionPanel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (this.PlayerInteractionPanelListManager == null) return;

            this.PlayerInteractionPanelListManager.CloseCurrentPanel();

            this.PlayerInput.SwitchCurrentActionMap("Player");
        }

        //  2. Called by other Player scripts to block/unblock inputs when needed
        public void BlockAllPlayerInputs()
        {
            this.ClearMoveInput();

            if (this.PlayerInput == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.PlayerInputHandler] No PlayerInput found to block inputs.", this);
                return;
            }

            this.PlayerInput.DeactivateInput();
        }
        public void UnblockAllPlayerInputs()
        {
            this.ClearMoveInput();

            if (this.PlayerInput == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.PlayerInputHandler] No PlayerInput found to unblock inputs.", this);
                return;
            }

            this.PlayerInput.ActivateInput();
        }

        //  3. (Temporary) Debug Actions
        public void OnTest(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                // Debug.Log("Test input received!");
                SkillProfile skill = UnityEngine.Resources.Load<SkillProfile>("Data/Inventory/Skills/Ranged/SKR_DoubleShot");
                Debug.Log($"Loaded skill: {skill?.DisplayName ?? "null"}");
                if (skill != null) this.CombatLoadoutController.TrySetActiveEquippedWeaponActiveSpecialSkill(skill);
            }
        }

        // --- Helpers ---
        private int ReadInputDirection(InputAction.CallbackContext ctx)
        {
            int direction = +1;

            Type valueType = ctx.control?.valueType;

            if (valueType == typeof(float))
            {
                float value = ctx.ReadValue<float>();

                if (Mathf.Abs(value) > 0.01f)
                {
                    direction = value > 0 ? +1 : -1;
                }
            }
            else if (valueType == typeof(Vector2))
            {
                Vector2 value = ctx.ReadValue<Vector2>();

                float dominantAxisValue = Mathf.Abs(value.y) >= Mathf.Abs(value.x)
                    ? value.y
                    : value.x;

                if (Mathf.Abs(dominantAxisValue) > 0.01f)
                {
                    direction = dominantAxisValue > 0 ? +1 : -1;
                }
            }

            return direction;
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.PlayerMenuManager == null)
            {
                GameObject playerMenu = GameObject.Find("PlayerMenu");
                if (playerMenu != null) _playerMenuManager = playerMenu.GetComponent<PlayerMenuManager>();
            }

            if (this.PlayerInteractionPanelListManager == null)
            {
                GameObject playerInteractionPanelList = GameObject.Find("PlayerInteractionPanelList");
                if (playerInteractionPanelList != null) _playerInteractionPanelListManager = playerInteractionPanelList.GetComponent<PlayerInteractionPanelListManager>();
            }

            if (this.PlayerInput == null) _playerInput = this.GetComponent<PlayerInput>();
        }
    }
}
