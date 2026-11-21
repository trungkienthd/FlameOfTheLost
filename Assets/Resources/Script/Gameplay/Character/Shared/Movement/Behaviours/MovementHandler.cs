using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementHandler : CharacterConfiguredBehaviourBase<CharacterMovementConfig>, 
        IStatusEffectFlagConsumer, 
        IStatusEffectStatModifierMultiplierConsumer,
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- References ---
        public Rigidbody2D Rigidbody2D => this.CharacterRootController.Rigidbody2D;
        public CharacterVisualController VisualController => this.CharacterRootController.VisualController;

        // --- Data / Config ---
        public CharacterMovementConfig MovementConfig => this.Config;
        private float MovementSpeed => this.MovementConfig.MovementSpeed;
        private float Accel => this.MovementConfig.Accel;
        private float Decel => this.MovementConfig.Decel;

        // --- Runtime ---
        //  0. Death Disable State (Runtime)
        [Header("Runtime - Death Disable")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        //  1. Status Effects states
        [Header("Runtime - Status Effects Flags")]
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

        [Header("Runtime - Status Effects Stat Modifier")]
        [SerializeField] private float _movementSpeedScale = 1f; // 1 = no change
        public float MovementSpeedScale
        {
            get => _movementSpeedScale;
            private set => _movementSpeedScale = Mathf.Max(0f, value);
        }

        //  2. Internal State
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

        //  3. Optional AI Movement Speed Scale
        [Header("Runtime - AI Movement")]
        [SerializeField, Min(0f)] private float _aiMovementSpeedMultiplier = 1f;
        public float AIMovementSpeedMultiplier => _aiMovementSpeedMultiplier;

        // --- Unity Callbacks --- 
        private void FixedUpdate()
        {
            if (this.IsDeathDisabled)
            {
                if (this.Rigidbody2D != null)
                {
                    //this.Rigidbody2D.linearVelocity = Vector2.zero;
                    //this.Rigidbody2D.angularVelocity = 0f;
                }

                this.VisualController?.FeedMovement(Vector2.zero);
                return;
            }

            // IMPORTANT: Status Effects apply here via MovementSpeedScale (0 if blocked)
            float finalMovementSpeed = this.MovementBlocked
                ? 0f
                : this.MovementSpeed * this.MovementSpeedScale * this.AIMovementSpeedMultiplier;

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
        //      0.1. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 30;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;

            this.RawMovementInput = Vector2.zero;
            this.DesiredMovement = Vector2.zero;
            this.Velocity = Vector2.zero;

            this.SetAIMovementSpeedMultiplier(1f);

            if (this.Rigidbody2D != null)
            {
                this.Rigidbody2D.linearVelocity = Vector2.zero;
                this.Rigidbody2D.angularVelocity = 0f;
            }

            this.VisualController?.FeedMovement(Vector2.zero);
        }

        //      0.2. ICharacterDeathDisableable implementation
        public int DeathDisablePriority => 20;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;

            this.RawMovementInput = Vector2.zero;
            this.DesiredMovement = Vector2.zero;
            // this.Velocity = Vector2.zero;

            this.SetAIMovementSpeedMultiplier(1f);

            if (this.Rigidbody2D != null)
            {
                // this.Rigidbody2D.linearVelocity = Vector2.zero;
                // this.Rigidbody2D.angularVelocity = 0f;
            }

            this.VisualController?.FeedMovement(Vector2.zero);
        }

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

        //  3. AI Movement Speed Scale Setter
        public void SetAIMovementSpeedMultiplier(float multiplier)
        {
            _aiMovementSpeedMultiplier = Mathf.Max(0f, multiplier);
        }
    }
}
