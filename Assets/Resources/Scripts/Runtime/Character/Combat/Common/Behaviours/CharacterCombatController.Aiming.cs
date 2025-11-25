using UnityEngine;

namespace FlameOfTheLost
{
    public abstract partial class CharacterCombatController : MonoBehaviour
    {
        [Header("Aiming")]
        [SerializeField] private CombatAimingConfig _aimConfig;
        [SerializeField] private MonoBehaviour _aimProviderBehaviour;           // implemented IAimProvider
        private IAimProvider _aimProvider;
        public CombatAimingConfig AimConfig
        {
            get => _aimConfig;
            private set => _aimConfig = value;
        }
        public MonoBehaviour AimProviderBehaviour
        {
            get => _aimProviderBehaviour;
            private set => _aimProviderBehaviour = value;
        }
        public IAimProvider AimProvider
        {
            get => _aimProvider;
            private set => _aimProvider = value;
        }

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

        private void AwakeAiming()
        {
            if (!this.AimProviderBehaviour) this.AimProviderBehaviour = this.GetComponentInChildren<IAimProvider>(true) as MonoBehaviour;
            this.AimProvider = this.AimProviderBehaviour as IAimProvider;
        }

        protected void SetFacingLock(bool locked)
        {
            this.IsFacingLocked = locked;
        }

        protected void UnlockFacingAfterActiveWindow()
        {
            if (this.IsFacingLocked)
            {
                this.IsFacingLocked = false;
            }
        }

        // Set once when an attack starts (melee/ranged). Pass a normalized direction vector
        protected void SnapshotAttackDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude > Mathf.Epsilon) this.AttackSnapshotDirection = direction.normalized;
        }

        // Current best attack direction:
        //  1) Snapshot from attack start
        //  2) Live aim from IAimProvider
        //  3) Fallback to Animator Facing (+ flip) via AnimationHandler
        protected Vector2 GetCurrentAttackDirection()
        {
            if (this.AttackSnapshotDirection.sqrMagnitude > Mathf.Epsilon) return this.AttackSnapshotDirection;

            if (this.AimProvider != null && this.AimProvider.TryGetAimDirection(this.transform, out Vector2 aimDirection)) return aimDirection.normalized;

            // Fallback to AnimationHandler facing
            var facing = this.AnimationHandler ? this.AnimationHandler.FacingStatus : FacingStatus.Side;
            switch (facing)
            {
                case FacingStatus.Down: 
                    return Vector2.down;
                case FacingStatus.Up:
                    return Vector2.up;
                case FacingStatus.Side:
                    bool right = this.AnimationHandler ? this.AnimationHandler.IsFacingRight : true;
                    return right ? Vector2.right : Vector2.left;
                default:
                    return Vector2.right;
            }
        }

        // Call this every Update() or when it is needed to refresh aim/facing
        protected void UpdateAiming()
        {
            if (this.AimConfig == null || !this.AimConfig.AimingOverridesFacing) return;
            if (this.IsFacingLocked && this.AimConfig.LockFacingDuringActiveFrames) return;
            if (this.AimProvider == null) return;

            if (!this.AimProvider.TryGetAimDirection(this.transform, out Vector2 aimDirection)) return;

            // Process aim vector
            aimDirection = AimUtility.ApplyDeadzone(aimDirection, this.AimConfig.DeadzoneRadius);
            if (aimDirection == Vector2.zero) return;

            aimDirection = AimUtility.ClampLength(aimDirection, this.AimConfig.MaxAimVectorLength);
            var snappedDirection = AimUtility.SnapDirection(aimDirection, this.AimConfig.FacingSnapMode);
            int facing = AimUtility.FacingFromDirection(snappedDirection, this.AimConfig.FacingSnapMode);

            // Drive animation
            if (this.AnimationHandler)
            {
                this.AnimationHandler.ForceFacing(snappedDirection);
                this.AnimationHandler.SetFacing(facing);
            }
        }
    }
}
