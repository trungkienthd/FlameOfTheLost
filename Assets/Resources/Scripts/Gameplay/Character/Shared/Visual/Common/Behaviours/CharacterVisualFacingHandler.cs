using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterVisualFacingHandler : CharacterVisualSubBehaviourBase
    {
        // --- Data & Configs ---
        private float SpeedForOne  => this.VisualConfig != null
            ? this.VisualConfig.SpeedForOne
            : 10f;

        private float SpeedDampTime => this.VisualConfig != null
            ? this.VisualConfig.SpeedDampTime
            : 0.05f;

        // --- Runtime ---
        [Header("Runtime")]
        [SerializeField] private Vector2 _lastDirection = Vector2.down;
        [SerializeField] private int _facingSignX = +1;
        [SerializeField] private bool _isFacingLocked;
        public Vector2 LastDirection
        {
            get => _lastDirection;
            private set => _lastDirection = value;
        }
        public int FacingSignX
        {
            get => _facingSignX;
            private set => _facingSignX = value;
        }
        public bool IsFacingLocked
        {
            get => _isFacingLocked;
            private set => _isFacingLocked = value;
        }

        // --- Public APIs ---
        //  1. Facing APIs
        public CharacterFacingStatus FacingStatus
        {
            get
            {
                if (this.VisualController == null || this.VisualController.Animator == null) return CharacterFacingStatus.Down;

                int facingInt = this.VisualController.Animator.GetInteger(CharacterVisualController.FacingID);
                return (CharacterFacingStatus)facingInt;
            }
        }
        public bool IsFacingRight => this.FacingSignX >= 0;

        //  2. Movement
        public void FeedMovement(Vector2 worldVelocity)
        {
            if (!this.EnsureBound()) return;
            if (this.VisualController.Animator == null) return;

            float speed01 = this.SpeedForOne > 0f
                ? Mathf.Clamp01(worldVelocity.magnitude / this.SpeedForOne)
                : 0f;

            this.VisualController.Animator.SetFloat(CharacterVisualController.SpeedID, speed01, this.SpeedDampTime, Time.deltaTime);

            // Movement may continue during an attack, but it must not replace
            // the facing direction selected by the combat aiming system.
            if (this.IsFacingLocked) return;

            if (worldVelocity.sqrMagnitude > 0.0001f)
            {
                this.LastDirection = worldVelocity.normalized;
            }

            this.ApplyDirectionToAnimatorAndFlip(this.LastDirection);
        }

        //  2. Speed
        public void SetSpeedImmediate(float speed01)
        {
            if (!this.EnsureBound()) return;
            if (this.VisualController.Animator == null) return;

            this.VisualController.Animator.SetFloat(CharacterVisualController.SpeedID, Mathf.Clamp01(speed01));
        }

        //  3. Facing
        //      3.1. Facing Parameter
        public void SetFacingParameter(int facing)
        {
            if (!this.EnsureBound()) return;
            if (this.VisualController.Animator == null) return;

            this.VisualController.Animator.SetInteger(CharacterVisualController.FacingID, facing);
        }
        public void SetFacingParameter(CharacterFacingStatus facingStatus)
        {
            this.SetFacingParameter((int)facingStatus);
        }

        //      3.2. Force Facing
        public void ForceFacing(Vector2 direction)
        {
            if (!this.EnsureBound()) return;
            if (direction.sqrMagnitude <= 0.0001f) return;

            this.LastDirection = direction.normalized;
            this.ApplyDirectionToAnimatorAndFlip(this.LastDirection);
        }

        //      3.3. Facing Lock
        public void LockFacing()
        {
            this.IsFacingLocked = true;
        }
        public void UnlockFacing()
        {
            this.IsFacingLocked = false;
        }

        //  4. Character Refresh
        public void SetFacingDuringRefresh(CharacterFacingStatus facingStatus)
        {
            if (!this.EnsureBound()) return;

            this.UnlockFacing();

            Vector2 direction = facingStatus switch
            {
                CharacterFacingStatus.Up    => Vector2.up,
                CharacterFacingStatus.Side  => Vector2.right,
                CharacterFacingStatus.Down  => Vector2.down,
                _ => Vector2.down
            };

            this.LastDirection = direction;

            this.ApplyDirectionToAnimatorAndFlip(this.LastDirection, forceFacingRightForNonSide: true);
        }

        // --- Helpers ---
        private void ApplyDirectionToAnimatorAndFlip(Vector2 direction, bool forceFacingRightForNonSide = false)
        {
            if (this.VisualController == null || this.VisualController.Animator == null) return;

            this.VisualController.Animator.SetFloat(CharacterVisualController.DirXID, direction.x);
            this.VisualController.Animator.SetFloat(CharacterVisualController.DirYID, direction.y);

            CharacterFacingStatus facingStatus = this.ResolveFacing(direction);
            this.SetFacingParameter(facingStatus);

            if (this.VisualController.CharacterFlipGroup != null)
            {
                if (facingStatus == CharacterFacingStatus.Side)
                {
                    this.FacingSignX = direction.x >= 0f ? +1 : -1;
                }
                else if (forceFacingRightForNonSide)
                {
                    this.FacingSignX = +1;
                }

                Vector3 localScale = this.VisualController.CharacterFlipGroup.localScale;
                localScale.x = Mathf.Abs(localScale.x) * this.FacingSignX;
                this.VisualController.CharacterFlipGroup.localScale = localScale;
            }

            if (this.VisualController.SpriteRenderer != null)
            {
                this.VisualController.SpriteRenderer.flipX = false;
            }
        }

        private CharacterFacingStatus ResolveFacing(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y)) return CharacterFacingStatus.Side;

            return direction.y > 0f
                ? CharacterFacingStatus.Up
                : CharacterFacingStatus.Down;
        }
    }
}
