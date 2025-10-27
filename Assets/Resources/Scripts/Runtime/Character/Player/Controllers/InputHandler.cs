using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputHandler : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private MovementHandler _movement;
        [SerializeField] private CombatLoadoutController _loadout;
        public MovementHandler PlayerMovementHandler
        {
            get => _movement;
            private set => _movement = value;
        }
        public CombatLoadoutController PlayerCombatLoadoutController
        {
            get => _loadout;
            private set => _loadout = value;
        }

        private Vector2 _move;
        public Vector2 Move
        {
            get => _move;
            set => _move = value;
        }

        private void Awake()
        {
            this.PlayerMovementHandler = this.GetComponent<MovementHandler>();
            this.PlayerCombatLoadoutController = this.GetComponent<CombatLoadoutController>();
        }

        // Called by PlayerInput
        public void OnMove(InputAction.CallbackContext ctx)
        {
            this.Move = ctx.ReadValue<Vector2>();

            this.PlayerMovementHandler?.SetMoveInput(this.Move);
        }
        // ---------------- Attack (2-slot hotbar) ----------------
        public void OnNormalAttackLeft(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) this.PlayerCombatLoadoutController?.TryDoNormalAttack(AttackSlot.Left);
        }
        public void OnNormalAttackRight(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) this.PlayerCombatLoadoutController?.TryDoNormalAttack(AttackSlot.Right);
        }
        public void OnSpecialAttackLeft(InputAction.CallbackContext ctx) 
        {
            if (ctx.performed) this.PlayerCombatLoadoutController?.TryDoSpecialAttack(AttackSlot.Left);
        }
        public void OnSpecialAttackRight(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) this.PlayerCombatLoadoutController?.TryDoSpecialAttack(AttackSlot.Right);
        }
        // Optional: cycle which pool skill is assigned to each slot
        public void OnSwitchLeftSpecialSkillSlot(InputAction.CallbackContext ctx) 
        {
            if (ctx.performed) this.PlayerCombatLoadoutController?.CycleSpecialSkillSlot(AttackSlot.Left, +1); 
        }
        public void OnSwitchRightSpecialSkillSlot(InputAction.CallbackContext ctx) 
        {
            if (ctx.performed) this.PlayerCombatLoadoutController?.CycleSpecialSkillSlot(AttackSlot.Right, +1); 
        }
        // Optional: bind these to keys/buttons or UI
        public void OnSwitchStyle(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                switch (this.PlayerCombatLoadoutController?.CurrentAttackStyle)
                {
                    case AttackStyle.Melee:
                        this.PlayerCombatLoadoutController?.SetAttackStyle(AttackStyle.Ranged);
                        break;
                    case AttackStyle.Ranged:
                        this.PlayerCombatLoadoutController?.SetAttackStyle(AttackStyle.Melee);
                        break;
                    default:
                        Debug.LogWarning($"[InputController] Unknown attack style: {this.PlayerCombatLoadoutController?.CurrentAttackStyle}");
                        break;
                }
            }
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
        public void OnHit(InputAction.CallbackContext ctx)
        {   
            if (ctx.performed) this.GetComponent<HealthController>()?.TestHit();
        }
    }
}
