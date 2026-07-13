using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterCombatAimingHandler : CharacterBehaviourControllerBase
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private MonoBehaviour _aimingProviderBehaviour;
        private MonoBehaviour AimingProviderBehaviour => _aimingProviderBehaviour;
        private ICombatAimingProvider AimingProvider => this.AimingProviderBehaviour as ICombatAimingProvider;

        private CharacterVisualController VisualController => this.CharacterRootController.VisualController;

        private Transform AimingOrigin => this.CharacterRootController != null
            ? this.CharacterRootController.transform
            : this.transform;

        //  2. Runtime State
        private bool _isFacingLocked = false;                 // Runtime aim/facing
        protected bool IsFacingLocked
        {
            get => _isFacingLocked;
            private set => _isFacingLocked = value;
        }

        private Vector2 _attackSnapshotDir = Vector2.zero; // used if Hitbox needs direction
        protected Vector2 AttackSnapshotDirection
        {
            get => _attackSnapshotDir;
            private set => _attackSnapshotDir = value;
        }

        //  3. Aiming Config
        [Header("References (Aiming Config)")]
        [SerializeField] private CombatAimingConfig _aimingConfig;
        public CombatAimingConfig AimingConfig => _aimingConfig;

        // --- Public APIs ---
        //  1. Attack Window
        public void ResolveOnAttackWindowStart()
        {
            this.LockFacing();
        }
        public void ResolveOnAttackWindowEnd()
        {
            this.UnlockFacing();
            this.ClearAttackSnapshotDirection();
        }
        public void ResolveOnAttackWindowInterrupt()
        {
            this.ResolveOnAttackWindowEnd();
        }

        //  2. Direction Resolution
        public Vector2 GetCurrentAttackDirection()
        {
            // Use recorded snapshot first, if one exists
            if (this.AttackSnapshotDirection.sqrMagnitude > Mathf.Epsilon)
            {
                return this.AttackSnapshotDirection;
            }

            if (this.TryResolveLiveAimDirection(out Vector2 aimDirection))
            {
                return aimDirection;
            }

            // c. Fallback to animation facing
            return this.GetFallbackDirectionFromFacing();
        }

        //  3. Attack Preparation
        public void PrepareAimingForAttack()
        {
            // Clear any stale direction left by an abnormal previous sequence.
            this.ClearAttackSnapshotDirection();

            Vector2 attackDirection;

            if (this.TryResolveLiveAimDirection(out Vector2 liveAimDirection))
            {
                attackDirection = liveAimDirection;
            }
            else
            {
                attackDirection = this.GetFallbackDirectionFromFacing();
            }

            this.SnapshotAttackDirection(attackDirection);
            this.ApplyFacingFromAimDirection(attackDirection);

            // Lock immediately after applying the attack direction.
            // This happens before TriggerAttack(), so movement cannot replace
            // the facing on the attack animation's first frame.
            this.LockFacing();
        }

        //  5. Refresh Character
        public void ClearAimingStateAfterCharacterRefresh()
        {
            this.UnlockFacing();
            this.ClearAttackSnapshotDirection();
        }

        // --- Helpers ---
        //  1. Set/Unset facing lock
        //      1.1. Lock facing at the start of attack animation to prevent aim changes during active frames
        private void LockFacing()
        {
            if (this.IsFacingLocked) return;

            this.IsFacingLocked = true;
            this.VisualController?.LockFacing();
        }
        //      1.2 Unlock facing after attack active window or attack is interrupted
        private void UnlockFacing()
        {
            this.IsFacingLocked = false;
            this.VisualController?.UnlockFacing();
        }

        //  2. Live Direction
        private bool TryResolveLiveAimDirection(out Vector2 aimDirection)
        {
            aimDirection = Vector2.zero;

            if (this.AimingProvider == null)
            {
                return false;
            }

            if (!this.AimingProvider.TryGetAimDirection(this.AimingOrigin, out Vector2 providerDirection))
            {
                return false;
            }

            if (providerDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                return false;
            }

            float deadzone = this.AimingConfig != null ? this.AimingConfig.DeadzoneRadius : 0.1f;
            providerDirection = AimingUtility.ApplyDeadzone(providerDirection, deadzone);

            if (providerDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                return false;
            }

            if (this.AimingConfig != null && this.AimingConfig.MaxAimVectorLength > 0f)
            {
                providerDirection = AimingUtility.ClampLength(providerDirection, this.AimingConfig.MaxAimVectorLength);
            }

            aimDirection = providerDirection.normalized;
            return true;
        }

        //  3. Apply Facing
        private void ApplyFacingFromAimDirection(Vector2 aimDirection)
        {
            if (this.AimingConfig == null || !this.AimingConfig.AimingOverridesFacing) return;

            if (aimDirection.sqrMagnitude <= Mathf.Epsilon) return;

            Vector2 snappedDirection = AimingUtility.SnapDirection(aimDirection, this.AimingConfig.FacingSnapType);

            if (snappedDirection.sqrMagnitude <= Mathf.Epsilon) return;

            this.VisualController?.ForceFacing(snappedDirection);
        }

        //  4. Snapshot: Set once when an attack starts (melee/ranged). Pass a normalized direction vector.
        private void SnapshotAttackDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude <= Mathf.Epsilon) return;

            this.AttackSnapshotDirection = direction.normalized;
        }
        //      4.2. Clear snapshot after attack active frames end
        private void ClearAttackSnapshotDirection()
        {
            this.AttackSnapshotDirection = Vector2.zero;
        }

        //  5. Fallback direction based on animation facing
        private Vector2 GetFallbackDirectionFromFacing()
        {
            CharacterFacingStatus facingStatus = CharacterFacingStatus.Side;
            bool isFacingRight = true;

            if (this.VisualController != null)
            {
                facingStatus = this.VisualController.FacingStatus;
                isFacingRight = this.VisualController.IsFacingRight;
            }

            switch (facingStatus)
            {
                case CharacterFacingStatus.Down:
                    return Vector2.down;
                case CharacterFacingStatus.Up:
                    return Vector2.up;
                case CharacterFacingStatus.Side:
                    return isFacingRight ? Vector2.right : Vector2.left;
                default:
                    Debug.LogWarning($"[{this.CharacterName}.CharacterCombatAimingHandler] Unknown FacingStatusType '{facingStatus}', defaulting to Vector2.right");
                    return Vector2.right;
            }
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.AimingProviderBehaviour == null)
            {
                Debug.LogError($"[{this.CharacterName}.CharacterCombatAimingHandler] No AimingProvider assigned, aiming will not work!", this);
            }

            if (this.AimingConfig == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.CharacterCombatAimingHandler] No AimingConfig assigned, generic config will be loaded instead.");
                _aimingConfig = Resources.Load<CombatAimingConfig>("Data/Combat/GenericCombatAimingConfig");
            }
        }
    }
}
