using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMenuManager _playerMenuManager;

        [SerializeField] private MovementHandler _movementHandler;
        [SerializeField] private CharacterCombatLoadoutController _combatLoadoutController;
        [SerializeField] private CharacterCombatAttackController _combatAttackController;
        [SerializeField] private PlayerInteractionLoadoutController _playerInteractionLoadoutController;
        private PlayerMenuManager PlayerMenuManager => _playerMenuManager;
        private MovementHandler PlayerMovementHandler => _movementHandler;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _combatLoadoutController;
        private CharacterCombatAttackController PlayerCombatAttackController => _combatAttackController;
        private PlayerInteractionLoadoutController PlayerInteractionLoadoutController => _playerInteractionLoadoutController;

        private Vector2 _move;
        public Vector2 Move
        {
            get => _move;
            set => _move = value;
        }

        private void Awake()
        {
            if (this.PlayerMenuManager == null)
            {
                GameObject playerMenu = GameObject.Find("PlayerMenu");
                if (playerMenu != null)
                {
                    _playerMenuManager = playerMenu.GetComponent<PlayerMenuManager>();
                }
            }
            if (this.PlayerMovementHandler == null) _movementHandler = this.GetComponent<MovementHandler>();
            if (this.PlayerCombatLoadoutController == null) _combatLoadoutController = this.GetComponent<CharacterCombatLoadoutController>();
            if (this.PlayerCombatAttackController == null)  _combatAttackController = this.GetComponent<CharacterCombatAttackController>();
            if (this.PlayerInteractionLoadoutController == null) _playerInteractionLoadoutController = this.GetComponent<PlayerInteractionLoadoutController>();
        }

        // Called by PlayerInput
        public void OnMove(InputAction.CallbackContext ctx)
        {
            this.Move = ctx.ReadValue<Vector2>();

            this.PlayerMovementHandler.RawMovementInput = this.Move;
        }
        // ---------------- Attack (2-slot hotbar) ----------------
        public void OnNormalAttackLeft(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) this.PlayerCombatAttackController?.TryDoNormalAttack(AttackSideType.Left);
        }
        public void OnNormalAttackRight(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) this.PlayerCombatAttackController?.TryDoNormalAttack(AttackSideType.Right);
        }
        public void OnSpecialAttackLeft(InputAction.CallbackContext ctx) 
        {
            if (ctx.performed) this.PlayerCombatAttackController?.TryDoSpecialAttack(AttackSideType.Left);
        }
        public void OnSpecialAttackRight(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) this.PlayerCombatAttackController?.TryDoSpecialAttack(AttackSideType.Right);
        }
        public void OnSwitchWeapon(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                int dir = +1; // Default for a simple button press

                // If this action is bound to an axis (e.g., mouse scroll, stick), use its sign:
                var t = ctx.control?.valueType;
                if (t == typeof(float))
                {
                    float v = ctx.ReadValue<float>();
                    if (Mathf.Abs(v) > 0.01f) dir = v > 0 ? +1 : -1;
                }
                else if (t == typeof(Vector2))
                {
                    // prefer vertical for mouse wheel; fall back to horizontal if needed
                    Vector2 v2 = ctx.ReadValue<Vector2>();
                    float mag = Mathf.Abs(v2.y) >= Mathf.Abs(v2.x) ? v2.y : v2.x;
                    if (Mathf.Abs(mag) > 0.01f) dir = mag > 0 ? +1 : -1;
                }

                // Cycle weapons for the CURRENT style
                this.PlayerCombatLoadoutController?.TryCycleWeaponEntry(dir);
            }
        }
        public void OnQuickEquipWeapon(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            { 
                this.PlayerCombatLoadoutController.TryQuickEquipWeapon();
            }
        }
        public void OnSwitchConsumableItem(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                int dir = +1; // Default for a simple button press

                // If this action is bound to an axis (e.g., mouse scroll, stick), use its sign:
                var t = ctx.control?.valueType;
                if (t == typeof(float))
                {
                    float v = ctx.ReadValue<float>();
                    if (Mathf.Abs(v) > 0.01f) dir = v > 0 ? +1 : -1;
                }
                else if (t == typeof(Vector2))
                {
                    // prefer vertical for mouse wheel; fall back to horizontal if needed
                    Vector2 v2 = ctx.ReadValue<Vector2>();
                    float mag = Mathf.Abs(v2.y) >= Mathf.Abs(v2.x) ? v2.y : v2.x;
                    if (Mathf.Abs(mag) > 0.01f) dir = mag > 0 ? +1 : -1;
                }

                // Cycle weapons for the CURRENT style
                this.PlayerCombatLoadoutController?.TryCycleConsumableItemEntry(dir);
            }
        }
        public void OnConsumeConsumableItem(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                this.PlayerCombatLoadoutController.TryConsumeConsumableItem();
            }
        }
        public void OnCarryInteractable(InputAction.CallbackContext ctx)
        {
            if (this.PlayerInteractionLoadoutController == null) return;

            if (ctx.started)
            {
                // Button pressed → start carrying
                this.PlayerInteractionLoadoutController.TrySetInteractableItemCarryingState(true);
            }
            else if (ctx.canceled)
            {
                // Button released → stop carrying
                this.PlayerInteractionLoadoutController.TrySetInteractableItemCarryingState(false);
            }
        }

        public void OnTest(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                // Debug.Log("Test input received!");
                SkillProfile skill = UnityEngine.Resources.Load<SkillProfile>("Data/Inventory/Skills/Ranged/SKR_DoubleShot");
                Debug.Log($"Loaded skill: {skill?.DisplayName ?? "null"}");
                if (skill != null) this.PlayerCombatLoadoutController.TrySetActiveEquippedWeaponActiveSpecialSkill(skill);
            }
        }

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
    }
}
