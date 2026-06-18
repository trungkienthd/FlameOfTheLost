using System.Collections;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterDodgeController : CharacterBehaviourControllerBase,
        ICharacterHitInvulnerabilityProvider,
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- States ---
        //  1. References
        [Header("References - Config")]
        [SerializeField] private CharacterDodgeConfig _config;
        private CharacterDodgeConfig Config => _config;

        [Header("References - Body")]
        [SerializeField] private Rigidbody2D _rigidbody2D;
        private Rigidbody2D Rigidbody2D => _rigidbody2D;

        private IFacingProvider FacingProvider => this.CharacterRootController.VisualController;

        //  2. Dodge State
        [Header("Dodge State (Runtime)")]
        [SerializeField] private CharacterDodgeState _dodgeState = CharacterDodgeState.None;
        [SerializeField] private Vector2 _currentDodgeDirection = Vector2.zero;
        public CharacterDodgeState DodgeState
        {
            get => _dodgeState;
            private set => _dodgeState = value;
        }

        public bool IsDodging => this.DodgeState == CharacterDodgeState.Dodging;
        public bool IsInRecovery => this.DodgeState == CharacterDodgeState.Recovery;

        private Vector2 CurrentDodgeDirection
        {
            get => _currentDodgeDirection;
            set => _currentDodgeDirection = value;
        }

        //  3. ICharacterHitInvulnerabilityProvider implementation
        public bool IsHitInvulnerable
        {
            get
            {
                if (this.Config == null) return false;

                if (this.IsDodging)
                {
                    return this.Config.IsHitInvulnerableDuringDodge;
                }

                if (this.IsInRecovery)
                {
                    return this.Config.IsHitInvulnerableDuringRecovery;
                }

                return false;
            }
        }

        //  4. Coroutine
        private Coroutine _dodgeRoutine;
        private Coroutine DodgeRoutine
        {
            get => _dodgeRoutine;
            set => _dodgeRoutine = value;
        }

        //  5. Death Disable
        [Header("Death Disable (Runtime)")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        // --- Public APIs ---
        //  1. Dodge
        public bool TryDodge(Vector2 requestedDirection)
        {
            if (this.IsDeathDisabled) return false;
            if (this.Config == null) return false;
            if (this.DodgeState != CharacterDodgeState.None) return false;

            Vector2 dodgeDirection = this.ResolveDodgeDirection(requestedDirection);
            if (dodgeDirection.sqrMagnitude < 0.0001f) return false;

            this.StartDodge(dodgeDirection);
            return true;
        }
        public bool TryDodge()
        {
            return this.TryDodge(Vector2.zero);
        }

        //  2. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 35;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;
            this.ClearDodgeState();
        }

        //  3. ICharacterDeathDisableable implementation
        public int DeathDisablePriority => 20;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;
            this.ClearDodgeState();
        }

        // --- Helpers ---
        private void StartDodge(Vector2 dodgeDirection)
        {
            this.ClearDodgeRoutineOnly();

            this.CurrentDodgeDirection = dodgeDirection.normalized;
            this.DodgeRoutine = this.StartCoroutine(this.CoHandleDodge());
        }

        private IEnumerator CoHandleDodge()
        {
            this.DodgeState = CharacterDodgeState.Dodging;

            float dodgeDuration = this.Config != null ? this.Config.DodgeDuration : 0f;
            float dodgeSpeed = this.Config != null ? this.Config.DodgeSpeed : 0f;

            if (this.Rigidbody2D != null) this.Rigidbody2D.linearVelocity = this.CurrentDodgeDirection * dodgeSpeed;

            if (dodgeDuration > 0f)
            {
                yield return new WaitForSeconds(dodgeDuration);
            }

            if (this.Rigidbody2D != null) this.Rigidbody2D.linearVelocity = Vector2.zero;

            float recoveryDuration = this.Config != null ? this.Config.RecoveryDuration : 0f;

            if (recoveryDuration > 0f)
            {
                this.DodgeState = CharacterDodgeState.Recovery;
                yield return new WaitForSeconds(recoveryDuration);
            }

            this.DodgeState = CharacterDodgeState.None;
            this.CurrentDodgeDirection = Vector2.zero;
            this.DodgeRoutine = null;
        }

        private Vector2 ResolveDodgeDirection(Vector2 requestedDirection)
        {
            if (requestedDirection.sqrMagnitude > 0.0001f)
            {
                return requestedDirection.normalized;
            }

            if (this.Config == null || !this.Config.UseLastFacingDirectionWhenNoInput)
            {
                return Vector2.zero;
            }

            return this.GetFacingDirection();
        }

        private Vector2 GetFacingDirection()
        {
            IFacingProvider facingProvider = this.FacingProvider;

            if (facingProvider == null)
            {
                return Vector2.down;
            }

            switch (facingProvider.FacingStatus)
            {
                case CharacterFacingStatus.Down:
                    return Vector2.down;

                case CharacterFacingStatus.Up:
                    return Vector2.up;

                case CharacterFacingStatus.Side:
                    return facingProvider.IsFacingRight ? Vector2.right : Vector2.left;

                default:
                    return Vector2.down;
            }
        }

        private void ClearDodgeState()
        {
            this.ClearDodgeRoutineOnly();

            this.DodgeState = CharacterDodgeState.None;
            this.CurrentDodgeDirection = Vector2.zero;

            if (this.Rigidbody2D != null)
            {
                this.Rigidbody2D.linearVelocity = Vector2.zero;
            }
        }

        private void ClearDodgeRoutineOnly()
        {
            if (this.DodgeRoutine == null) return;

            this.StopCoroutine(this.DodgeRoutine);
            this.DodgeRoutine = null;
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.Config == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterDodgeController)}] No dodge config assigned. Loading default config.", this);
                _config = Resources.Load<CharacterDodgeConfig>("Data/Character/Shared/Dodging/Ddg_Generic");
            }

            if (this.Rigidbody2D == null) _rigidbody2D = this.GetComponent<Rigidbody2D>();
        }

        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.Config == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterDodgeController)}] No dodge config assigned.", this);
            }

            if (this.Rigidbody2D == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterDodgeController)}] Missing Rigidbody2D.", this);
            }
        }
    }
}