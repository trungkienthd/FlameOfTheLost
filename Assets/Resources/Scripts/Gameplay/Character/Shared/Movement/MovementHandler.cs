using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementHandler : CharacterBehaviourControllerBase, 
        IStatusEffectFlagConsumer, 
        IStatusEffectStatModifierMultiplierConsumer,
        ICharacterRefreshable
    {
        // --- States ---
        //  1. References
        [Header("References (Movement)")]
        [SerializeField] private Rigidbody2D _rigidbody2D;
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public CharacterVisualController VisualController => this.CharacterRootController.VisualController;

        //  2. Tuning Config
        [Header("Tuning")]
        [SerializeField] private float _movementSpeed = 6f;
        [SerializeField] private float _accel = 60f;
        [SerializeField] private float _decel = 80f;
        private float MovementSpeed => _movementSpeed;
        private float Accel => _accel;
        private float Decel => _decel;

        //  3. Status Effects states
        [Header("Status Effects Flags (Runtime)")]
        [SerializeField] private bool _cannotMove = false;
        [SerializeField] private bool _ignoreMovementLock = false;
        private bool CannotMove
        {
            get => _cannotMove;
            set => _cannotMove = value;
        }
        private bool IgnoreMovementLock
        {
            get => _ignoreMovementLock;
            set => _ignoreMovementLock = value;
        }
        public bool MovementBlocked => this.CannotMove && !this.IgnoreMovementLock;

        [Header("Status Effects Stat Modifier (Runtime)")]
        [SerializeField] private float _movementSpeedScale = 1f; // 1 = no change
        public float MovementSpeedScale
        {
            get => _movementSpeedScale;
            private set => _movementSpeedScale = Mathf.Max(0f, value);
        }

        //  4. Internal State
        private Vector2 _raw;
        private Vector2 _desired;
        private Vector2 _vel;
        public Vector2 RawMovementInput
        {
            get => _raw;
            set => _raw = value;
        }
        private Vector2 DesiredMovement
        {
            get => _desired;
            set => _desired = value;
        }
        private Vector2 Velocity
        {
            get => _vel;
            set => _vel = value;
        }

        // --- Unity Callbacks --- 
        private void FixedUpdate()
        {
            // IMPORTANT: Status Effects apply here via MovementSpeedScale (0 if blocked)
            float finalMovementSpeed = this.MovementBlocked ? 0f : (this.MovementSpeed * this.MovementSpeedScale);

            this.DesiredMovement = Vector2.ClampMagnitude(this.RawMovementInput, 1f) * finalMovementSpeed;

            // Current velocity
            this.Velocity = this.Rigidbody2D.linearVelocity;
            var vel = this.Velocity;

            // --- Acceleration / Deceleration
            if (this.MovementBlocked)
            {
                // Hard stop when stunned
                vel.x = Mathf.MoveTowards(vel.x, 0f, this.Decel * Time.fixedDeltaTime);
                vel.y = Mathf.MoveTowards(vel.y, 0f, this.Decel * Time.fixedDeltaTime);
            }
            else
            {
                vel.x = Mathf.MoveTowards(this.Velocity.x, this.DesiredMovement.x, (Mathf.Abs(this.DesiredMovement.x) > 0.01f ? this.Accel : this.Decel) * Time.fixedDeltaTime);
                vel.y = Mathf.MoveTowards(this.Velocity.y, this.DesiredMovement.y, (Mathf.Abs(this.DesiredMovement.y) > 0.01f ? this.Accel : this.Decel) * Time.fixedDeltaTime);
            }

            this.Velocity = vel;
            this.Rigidbody2D.linearVelocity = this.Velocity;

            // Feed animator (Speed uses world velocity magnitude)
            this.VisualController?.FeedMovement(this.RawMovementInput);
        }

        // --- Public APIs ---
        //  1. Status Effect Flag consumption
        public void SetStatusEffectFlag(StatusEffectFlagTarget flagType, bool value)
        {
            switch (flagType)
            {
                case StatusEffectFlagTarget.CannotMove:
                    this.CannotMove = value;
                    return;
                case StatusEffectFlagTarget.IgnoreMovementLock:
                    this.IgnoreMovementLock = value;
                    return;
                default:
                    // Do nothing for other flags
                    return;
            }
        }

        //  2. Status Effect Stat Modifier consumption
        public void SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTarget target, float multiplier)
        {
            if (target != StatusEffectStatModifierTarget.MovementSpeedScale) return;

            // Do NOT clamp to config here; ModifierHandler should do global clamps.
            this.MovementSpeedScale = multiplier;
        }

        //  3. ICharacterPlacementRefreshable implementation
        public int RefreshmentConfigurePriority => 30;
        public void RefreshCharacter(CharacterRefreshmentContext context)
        {
            this.RawMovementInput = Vector2.zero;
            this.DesiredMovement = Vector2.zero;
            this.Velocity = Vector2.zero;

            if (this.Rigidbody2D != null)
            {
                this.Rigidbody2D.linearVelocity = Vector2.zero;
                this.Rigidbody2D.angularVelocity = 0f;
            }

            this.VisualController?.FeedMovement(Vector2.zero);
        }

        // --- Helpers ---
        protected override void CacheReferences()
        {
            base.CacheReferences();
            if (this.Rigidbody2D == null) _rigidbody2D = this.GetComponent<Rigidbody2D>();
        }
    }
}
