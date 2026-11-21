using System.Collections;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterDodgeController : CharacterConfiguredBehaviourBase<CharacterDodgeConfig>,
        ICharacterHitInvulnerabilityProvider,
        IStatusEffectFlagConsumer,
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- References ---
        private Rigidbody2D Rigidbody2D => this.CharacterRootController.Rigidbody2D;
        private Collider2D MainCollider2D => this.CharacterRootController.MainCollider2D;
        private IFacingProvider FacingProvider => this.CharacterRootController.VisualController;
        private CharacterResourcesController ResourcesController => this.CharacterRootController.ResourcesController;
        private CharacterVisualController VisualController => this.CharacterRootController.VisualController;

        // --- Data / Config ---
        public CharacterDodgeConfig DodgeConfig => this.Config;

        // --- Runtime ---
        //  1. Dodge State
        [Header("Runtime - Dodge State")]
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

        //  2. Collider Disable
        [Header("Runtime - Collider Disable")]
        [SerializeField] private bool _isMainColliderDisabledByDodge;
        [SerializeField] private bool _mainColliderWasEnabledBeforeDodge;
        private bool IsMainColliderDisabledByDodge
        {
            get => _isMainColliderDisabledByDodge;
            set => _isMainColliderDisabledByDodge = value;
        }
        private bool MainColliderWasEnabledBeforeDodge
        {
            get => _mainColliderWasEnabledBeforeDodge;
            set => _mainColliderWasEnabledBeforeDodge = value;
        }
        public bool ShouldBlockOtherInputs => this.IsDodging;

        //  3. Last Resource Gate Result
        public ResourceGateResponseRecord LastDodgeCostGateResponse
        {
            get;
            private set;
        }

        //  4. Status Effects Flags
        [Header("Runtime - Status Effects Flags")]
        [SerializeField] private bool _cannotDodge = false;
        [SerializeField] private bool _ignoreDodgeLock = false;
        private bool CannotDodge
        {
            get => _cannotDodge;
            set => _cannotDodge = value;
        }
        private bool IgnoreDodgeLock
        {
            get => _ignoreDodgeLock;
            set => _ignoreDodgeLock = value;
        }
        public bool DodgeBlocked => this.CannotDodge && !this.IgnoreDodgeLock;

        //  5. ICharacterHitInvulnerabilityProvider implementation
        public bool IsHitInvulnerable
        {
            get
            {
                if (this.DodgeConfig == null) return false;

                if (this.IsDodging)
                {
                    return this.DodgeConfig.IsHitInvulnerableDuringDodge;
                }

                if (this.IsInRecovery)
                {
                    return this.DodgeConfig.IsHitInvulnerableDuringRecovery;
                }

                return false;
            }
        }

        //  6. Coroutine
        private Coroutine _dodgeRoutine;
        private Coroutine DodgeRoutine
        {
            get => _dodgeRoutine;
            set => _dodgeRoutine = value;
        }

        //  7. Death Disable
        [Header("Runtime - Death Disable")]
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
            if (this.DodgeBlocked) return false;
            if (this.DodgeConfig == null) return false;
            if (this.DodgeState != CharacterDodgeState.None) return false;

            Vector2 dodgeDirection = this.ResolveDodgeDirection(requestedDirection);
            if (dodgeDirection.sqrMagnitude < 0.0001f) return false;

            // Pay immediately before the dodge starts.
            // If this fails, the dodge does not begin.
            if (!this.TryPayDodgeCost())
            {
                return false;
            }

            this.StartDodge(dodgeDirection);
            return true;
        }
        public bool TryDodge()
        {
            return this.TryDodge(Vector2.zero);
        }

        //  2. IStatusEffectFlagConsumer implementation
        public void SetStatusEffectFlag(StatusEffectFlagTarget flagType, bool value)
        {
            switch (flagType)
            {
                case StatusEffectFlagTarget.CannotDodge:
                    this.CannotDodge = value;
                    return;

                case StatusEffectFlagTarget.IgnoreDodgeLock:
                    this.IgnoreDodgeLock = value;
                    return;

                default:
                    // Do nothing for other flags.
                    return;
            }
        }

        //  3. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 35;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;
            this.LastDodgeCostGateResponse = default;
            this.ClearDodgeState();
        }

        //  4. ICharacterDeathDisableable implementation
        public int DeathDisablePriority => 20;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;
            this.LastDodgeCostGateResponse = default;
            this.ClearDodgeState();
        }

        // --- Helpers ---
        private bool TryPayDodgeCost()
        {
            this.LastDodgeCostGateResponse = default;

            if (this.DodgeConfig == null) return false;

            CharacterResourcesController resourcesController = this.ResourcesController;
            if (resourcesController == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterDodgeController)}] Missing {nameof(CharacterResourcesController)}. Dodge cost cannot be paid.", this);
                return false;
            }

            ResourceGateResponseRecord response = resourcesController.TryPayCost(this.DodgeConfig.DodgeCost);
            this.LastDodgeCostGateResponse = response;

            return response.IsAccepted;
        }

        private void DisableMainColliderForDodge()
        {
            Collider2D mainCollider = this.MainCollider2D;
            if (mainCollider == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterDodgeController)}] Missing main Collider2D. Dodge will not disable root collision.", this);
                return;
            }

            if (this.IsMainColliderDisabledByDodge) return;

            this.MainColliderWasEnabledBeforeDodge = mainCollider.enabled;
            this.IsMainColliderDisabledByDodge = true;

            if (mainCollider.enabled)
            {
                mainCollider.enabled = false;
            }
        }

        private void RestoreMainColliderAfterDodge()
        {
            if (!this.IsMainColliderDisabledByDodge) return;

            Collider2D mainCollider = this.MainCollider2D;
            if (mainCollider != null)
            {
                mainCollider.enabled = this.MainColliderWasEnabledBeforeDodge;
            }

            this.IsMainColliderDisabledByDodge = false;
            this.MainColliderWasEnabledBeforeDodge = false;
        }

        private void StartDodge(Vector2 dodgeDirection)
        {
            this.ClearDodgeRoutineOnly();

            this.CurrentDodgeDirection = dodgeDirection.normalized;

            // Gameplay state should begin immediately when dodge is accepted.
            this.DodgeState = CharacterDodgeState.Dodging;

            this.DisableMainColliderForDodge();

            // Visual should react only after the real dodge has started.
            if (this.VisualController != null)
            {
                this.VisualController.TriggerDodge(this.CurrentDodgeDirection);
            }

            this.DodgeRoutine = this.StartCoroutine(this.CoHandleDodge());
        }

        private IEnumerator CoHandleDodge()
        {
            //this.DodgeState = CharacterDodgeState.Dodging;

            float dodgeDuration = this.DodgeConfig != null ? this.DodgeConfig.DodgeDuration : 0f;
            float dodgeSpeed = this.DodgeConfig != null ? this.DodgeConfig.DodgeSpeed : 0f;

            if (this.Rigidbody2D != null)
            {
                this.Rigidbody2D.linearVelocity = this.CurrentDodgeDirection * dodgeSpeed;
            }

            if (dodgeDuration > 0f)
            {
                yield return new WaitForSeconds(dodgeDuration);
            }

            if (this.Rigidbody2D != null)
            {
                this.Rigidbody2D.linearVelocity = Vector2.zero;
            }

            this.RestoreMainColliderAfterDodge();

            float recoveryDuration = this.DodgeConfig != null ? this.DodgeConfig.RecoveryDuration : 0f;

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

            if (this.DodgeConfig == null || !this.DodgeConfig.UseLastFacingDirectionWhenNoInput)
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

            this.RestoreMainColliderAfterDodge();
        }

        private void ClearDodgeRoutineOnly()
        {
            if (this.DodgeRoutine == null) return;

            this.StopCoroutine(this.DodgeRoutine);
            this.DodgeRoutine = null;
        }
    }
}