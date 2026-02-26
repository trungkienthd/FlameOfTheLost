using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MovementHandler _movement;
        [SerializeField] private CharacterCombatLoadoutController _combatLoadoutController;
        [SerializeField] private CharacterCombatAttackController _combatAttackController;
        private MovementHandler PlayerMovementHandler
        {
            get => _movement;
            set => _movement = value;
        }
        private CharacterCombatLoadoutController PlayerCombatLoadoutController
        {
            get => _combatLoadoutController;
            set => _combatLoadoutController = value;
        }
        private CharacterCombatAttackController PlayerCombatAttackController
        {
            get => _combatAttackController;
            set => _combatAttackController = value;
        }

        private Vector2 _move;
        public Vector2 Move
        {
            get => _move;
            set => _move = value;
        }

        private void Awake()
        {
            if (this.PlayerMovementHandler == null) this.PlayerMovementHandler = this.GetComponent<MovementHandler>();
            if (this.PlayerCombatLoadoutController == null) this.PlayerCombatLoadoutController  = this.GetComponent<CharacterCombatLoadoutController>();
            if (this.PlayerCombatAttackController == null)  this.PlayerCombatAttackController   = this.GetComponent<CharacterCombatAttackController>();
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
                this.PlayerCombatLoadoutController?.CycleWeapon(dir);
            }
        }
    }
}
