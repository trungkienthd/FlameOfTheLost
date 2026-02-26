using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterCombatAimingHandler : MonoBehaviour
    {
        // --- References --- 
        [Header("References")]
        [SerializeField] private CharacterAnimationHandler _animationHandler;
        [SerializeField] private AimingHandler _aimingHandler; 
        private CharacterAnimationHandler AnimationHandler
        {
            get => _animationHandler;
            set => _animationHandler = value;
        }
        private AimingHandler AimingHandler
        {
            get => _aimingHandler;
            set => _aimingHandler = value;
        }

        // --- Runtime State ---
        private bool _isFacingLocked = false;                 // Runtime aim/facing
        protected bool IsFacingLocked
        {
            get => _isFacingLocked;
            private set => _isFacingLocked = value;
        }

        private Vector2 _attackSnapshotDir = Vector2.right; // used if Hitbox needs direction
        protected Vector2 AttackSnapshotDirection
        {
            get => _attackSnapshotDir;
            private set => _attackSnapshotDir = value;
        }

        // --- Aiming Config ---
        [SerializeField] private CombatAimingConfig _aimingConfig;
        public CombatAimingConfig AimingConfig => _aimingConfig;

        // --- Unity methods ---
        private void Awake()
        {
            if (!this.AnimationHandler) this.AnimationHandler = this.GetComponent<CharacterAnimationHandler>();
            if (this.AimingHandler == null) this.AimingHandler = this.GetComponent<AimingHandler>();
        }
        private void Update()
        {
            this.UpdateAiming();
        }

        // --- Public APIs ---
        //  1.1 Set/Unset facing lock
        public void LockFacing()
        {
            this.IsFacingLocked = true;
        }
        //  1.2 Unlock facing after attack active window or attack is interrupted
        public void UnlockFacing()
        {
            if (this.IsFacingLocked) this.IsFacingLocked = false;
        }

        //  2. Best direction to use for hitbox/projectile:
        //      a) snapshot from attack start
        //      b) live aim from IAimProvider
        //      c) fallback to animation facing
        public Vector2 GetCurrentAttackDirection()
        {
            // Fallback to snapshot
            if (this.AttackSnapshotDirection.sqrMagnitude > Mathf.Epsilon) return this.AttackSnapshotDirection;

            if (this.AimingHandler != null && this.AimingHandler.TryGetAimDirection(this.transform, out var aimDirection))
            {
                return aimDirection.sqrMagnitude > Mathf.Epsilon ? aimDirection.normalized : Vector2.right;
            }

            // Fallback to animation facing
            return this.GetFallbackDirectionFromFacing();
        }

        //  3. Call right before triggering attack animation, so facing is correct at frame 0.
        public void PrepareAimingForAttack()
        {
            this.UpdateAiming();
            this.SnapshotAttackDirection(this.GetCurrentAttackDirection());
        }

        // --- Helpers ---
        //  1. Update aiming state (called in Update and before attack)
        private void UpdateAiming()
        {
            if (this.AimingConfig == null || !this.AimingConfig.AimingOverridesFacing) return;
            if (this.IsFacingLocked && this.AimingConfig.LockFacingDuringActiveFrames) return;
            if (this.AimingHandler == null) return;

            if (!this.AimingHandler.TryGetAimDirection(this.transform, out var aimDirection)) return;

            // Deadzone check
            aimDirection = AimingUtility.ApplyDeadzone(aimDirection, this.AimingConfig.DeadzoneRadius);
            if (aimDirection.sqrMagnitude <= Mathf.Epsilon) return;

            // Snap
            var snapType = this.AimingConfig.FacingSnapType;
            var snappedDirection = AimingUtility.SnapDirection(aimDirection, snapType);

            // Facing (Down/Side/Up)
            int facing = AimingUtility.FacingFromDirection(snappedDirection);

            // Drive animation handler
            if (this.AnimationHandler)
            {
                this.AnimationHandler.ForceFacing(snappedDirection);
                this.AnimationHandler.SetFacing(facing);
            }
        }

        //  2. Set once when an attack starts (melee/ranged). Pass a normalized direction vector.
        private void SnapshotAttackDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude > Mathf.Epsilon) this.AttackSnapshotDirection = direction.normalized;
        }

        //  3. Fallback direction based on animation facing
        private Vector2 GetFallbackDirectionFromFacing()
        {
            FacingStatusType facingStatus = FacingStatusType.Side;
            bool isFacingRight = true;

            if (this.AnimationHandler != null)
            {
                facingStatus = this.AnimationHandler.FacingStatus;
                isFacingRight = this.AnimationHandler.IsFacingRight;
            }

            switch (facingStatus)
            {
                case FacingStatusType.Down:
                    return Vector2.down;
                case FacingStatusType.Up:
                    return Vector2.up;
                case FacingStatusType.Side:
                    return isFacingRight ? Vector2.right : Vector2.left;
                default:
                    Debug.LogWarning($"[CharacterCombatAimingHandler] Unknown FacingStatusType '{facingStatus}', defaulting to Vector2.right");
                    return Vector2.right;
            }
        }
    }
}
