using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(AnimalRoamingHandler))]
    [RequireComponent(typeof(AnimalThreatHandler))]
    [RequireComponent(typeof(AnimalFleeHandler))]
    public class AnimalBrainController : NeutralCharacterBrainController
    {
        // --- Data / Configs ---
        [Header("Configs - Animal AI")]
        [SerializeField] private AnimalRoamingConfig _roamingConfig;
        [SerializeField] private AnimalFleeConfig _fleeConfig;
        public AnimalRoamingConfig RoamingConfig
        {
            get => _roamingConfig;
            private set => _roamingConfig = value;
        }

        public AnimalFleeConfig FleeConfig
        {
            get => _fleeConfig;
            private set => _fleeConfig = value;
        }

        // --- Runtime ---
        //  1. AI State
        [Header("Animal State (Runtime)")]
        [SerializeField] private AnimalState _currentState = AnimalState.Idle;
        public AnimalState CurrentState
        {
            get => _currentState;
            private set => _currentState = value;
        }

        //  2. Passive State Timing
        [Header("Passive State (Runtime)")]
        [SerializeField] private float _stateEndTime;
        private float StateEndTime
        {
            get => _stateEndTime;
            set => _stateEndTime = value;
        }

        //  3. Stand Up Transition
        [SerializeField] private AnimalState _stateAfterStandUp = AnimalState.Idle;
        private AnimalState StateAfterStandUp
        {
            get => _stateAfterStandUp;
            set => _stateAfterStandUp = value;
        }

        //  4. Flee Boundary Block
        [Header("Fleeing (Runtime)")]
        [SerializeField] private bool _isFleeBlockedByHomeBoundary;
        private bool IsFleeBlockedByHomeBoundary
        {
            get => _isFleeBlockedByHomeBoundary;
            set => _isFleeBlockedByHomeBoundary = value;
        }

        //  5. Home Area
        [Header("Home Area (Runtime)")]
        [SerializeField] private AnimalHomeAreaEntry _homeAreaEntry;
        public AnimalHomeAreaEntry HomeAreaEntry
        {
            get => _homeAreaEntry;
            private set => _homeAreaEntry = value;
        }

        // --- References ---
        [Header("References - Animal Brain Sub-Behaviours")]
        [SerializeField] private AnimalRoamingHandler _roamingHandler;
        [SerializeField] private AnimalThreatHandler _threatHandler;
        [SerializeField] private AnimalFleeHandler _fleeHandler;
        public AnimalRoamingHandler RoamingHandler => _roamingHandler;
        public AnimalThreatHandler ThreatHandler => _threatHandler;
        public AnimalFleeHandler FleeHandler => _fleeHandler;

        public AnimalRootController AnimalRootController => this.CharacterRootController as AnimalRootController;

        public new NeutralCharacterInputHandler NeutralCharacterInputHandler => this.AnimalRootController != null
            ? this.AnimalRootController.NeutralCharacterInputHandler
            : null;

        private CharacterAITargetDetectionHandler TargetDetectionHandler => this.AnimalRootController != null
            ? this.AnimalRootController.TargetDetectionHandler
            : null;

        // --- Unity Callbacks ---
        protected virtual void Update()
        {
            if (this.IsDeathDisabled) return;

            this.TickAI();
        }

        // --- Public APIs ---
        public void ConfigureAnimalAI(AnimalRoamingConfig roamingConfig, AnimalFleeConfig fleeConfig, AnimalHomeAreaEntry homeAreaEntry)
        {
            if (roamingConfig == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(AnimalBrainController)}] Missing {nameof(AnimalRoamingConfig)}.", this);
                return;
            }

            if (fleeConfig == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(AnimalBrainController)}] Missing {nameof(AnimalFleeConfig)}.", this);
                return;
            }

            if (!homeAreaEntry.HasValidArea())
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(AnimalBrainController)}] Invalid animal home area.", this);
                return;
            }

            this.RoamingConfig = roamingConfig;
            this.FleeConfig = fleeConfig;
            this.HomeAreaEntry = homeAreaEntry;

            this.ResetAnimalRuntime();
        }

        public void ConfigureHomeArea(AnimalHomeAreaEntry homeAreaEntry)
        {
            if (!homeAreaEntry.HasValidArea())
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(AnimalBrainController)}] Invalid home area provided.", this);
                return;
            }

            this.HomeAreaEntry = homeAreaEntry;
        }

        public void NotifyThreatRegistered(CharacterRootController threat)
        {
            if (this.IsDeathDisabled) return;
            if (threat == null) return;

            if (this.IsReclinedState(this.CurrentState))
            {
                this.BeginStandUp(AnimalState.Fleeing);
                return;
            }

            if (this.CurrentState == AnimalState.StandUp)
            {
                this.StateAfterStandUp = AnimalState.Fleeing;
                return;
            }

            this.SetState(AnimalState.Fleeing);
        }

        private void TickAI()
        {
            if (this.CurrentState == AnimalState.Dead)
            {
                this.HandleDeadState();
                return;
            }

            CharacterRootController nearTarget = this.TargetDetectionHandler != null
                ? this.TargetDetectionHandler.CurrentAttackRangeTarget
                : null;

            CharacterRootController alertTarget = this.TargetDetectionHandler != null
                ? this.TargetDetectionHandler.CurrentAlertTarget
                : null;

            this.RefreshDamageThreat();

            CharacterRootController damageThreat = this.ThreatHandler != null
                ? this.ThreatHandler.CurrentThreat
                : null;

            // Once the target leaves Detection 2, the Cow may flee again
            // if the target re-enters later.
            if (nearTarget == null)
            {
                this.IsFleeBlockedByHomeBoundary = false;
            }

            // --- Highest Priority: Detection 2 / direct threat ---
            CharacterRootController fleeTarget =
                nearTarget != null
                    ? nearTarget
                    : damageThreat;

            if (fleeTarget != null)
            {
                this.HandleFleeIntent(fleeTarget);
                return;
            }

            // --- Detection 1 ---
            if (alertTarget != null)
            {
                this.HandleAlertIntent(alertTarget);
                return;
            }

            // --- Outside all detection ---
            this.HandlePassiveBehaviour();
        }

        // --- Helpers ----
        //  1. Detection Behaviour
        private void HandleAlertIntent(CharacterRootController target)
        {
            if (target == null) return;

            if (this.IsReclinedState(this.CurrentState))
            {
                this.BeginStandUp(AnimalState.Alert);
                return;
            }

            if (this.CurrentState == AnimalState.StandUp)
            {
                this.StateAfterStandUp = AnimalState.Alert;
                this.TickStandUp();
                return;
            }

            this.StopAllMovement();
            this.SetState(AnimalState.Alert);
            this.FaceTargetHorizontally(target);
        }

        private void HandleFleeIntent(CharacterRootController target)
        {
            if (target == null) return;

            if (this.IsReclinedState(this.CurrentState))
            {
                this.BeginStandUp(AnimalState.Fleeing);
                return;
            }

            if (this.CurrentState == AnimalState.StandUp)
            {
                this.StateAfterStandUp = AnimalState.Fleeing;
                this.TickStandUp();
                return;
            }

            if (this.IsFleeBlockedByHomeBoundary)
            {
                this.StopAllMovement();
                this.SetState(AnimalState.Alert);
                this.FaceTargetHorizontally(target);
                return;
            }

            this.SetState(AnimalState.Fleeing);

            bool blockedByBoundary = false;

            bool isFleeing = this.FleeHandler != null && this.FleeHandler.TickFlee(target, this.HomeAreaEntry, this.FleeConfig, out blockedByBoundary);

            if (isFleeing) return;

            if (blockedByBoundary)
            {
                this.IsFleeBlockedByHomeBoundary = true;

                this.SetState(AnimalState.Alert);
                this.FaceTargetHorizontally(target);
            }
        }

        //  2. Passive Behaviour
        private void HandlePassiveBehaviour()
        {
            switch (this.CurrentState)
            {
                case AnimalState.Idle:
                    this.TickIdle();
                    return;

                case AnimalState.Wandering:
                    this.TickWandering();
                    return;

                case AnimalState.Graze:
                    this.TickGraze();
                    return;

                case AnimalState.LieDown:
                    this.TickLieDown();
                    return;

                case AnimalState.Rest:
                    this.TickRest();
                    return;

                case AnimalState.Sleep:
                    this.TickSleep();
                    return;

                case AnimalState.StandUp:
                    this.TickStandUp();
                    return;

                case AnimalState.Alert:
                case AnimalState.Fleeing:
                default:
                    this.BeginIdle();
                    return;
            }
        }

        private void TickIdle()
        {
            this.StopAllMovement();

            if (Time.time < this.StateEndTime) return;

            AnimalState nextState = this.RoamingConfig != null
                ? this.RoamingConfig.GetRandomPassiveState()
                : AnimalState.Idle;

            switch (nextState)
            {
                case AnimalState.Wandering:
                    this.BeginWandering();
                    break;

                case AnimalState.Graze:
                    this.BeginGraze();
                    break;

                case AnimalState.LieDown:
                    this.BeginLieDown();
                    break;

                case AnimalState.Idle:
                default:
                    this.BeginIdle();
                    break;
            }
        }

        private void TickWandering()
        {
            if (Time.time >= this.StateEndTime)
            {
                this.BeginIdle();
                return;
            }

            bool canContinue = this.RoamingHandler != null && this.RoamingHandler.TickWandering(this.HomeAreaEntry);
            if (!canContinue)
            {
                this.BeginIdle();
            }
        }

        private void TickGraze()
        {
            this.StopAllMovement();

            if (Time.time >= this.StateEndTime)
            {
                this.BeginIdle();
            }
        }

        private void TickLieDown()
        {
            this.StopAllMovement();

            if (Time.time >= this.StateEndTime)
            {
                this.BeginRest();
            }
        }

        private void TickRest()
        {
            this.StopAllMovement();

            if (Time.time >= this.StateEndTime)
            {
                this.BeginSleep();
            }
        }

        private void TickSleep()
        {
            this.StopAllMovement();

            if (Time.time >= this.StateEndTime)
            {
                this.BeginStandUp(AnimalState.Idle);
            }
        }

        private void TickStandUp()
        {
            this.StopAllMovement();

            if (Time.time < this.StateEndTime) return;

            AnimalState nextState = this.StateAfterStandUp;

            this.StateAfterStandUp = AnimalState.Idle;

            switch (nextState)
            {
                case AnimalState.Alert:
                    this.SetState(AnimalState.Alert);
                    break;

                case AnimalState.Fleeing:
                    this.SetState(AnimalState.Fleeing);
                    break;

                case AnimalState.Idle:
                default:
                    this.BeginIdle();
                    break;
            }
        }

        //  3. State Starters
        private void BeginIdle()
        {
            this.StopAllMovement();

            this.SetState(AnimalState.Idle);

            float idleDuration = this.RoamingConfig != null ? this.RoamingConfig.GetRandomIdleDuration() : 0f;
            this.StateEndTime = Time.time + idleDuration;
        }

        private void BeginWandering()
        {
            bool started =
                this.RoamingHandler != null &&
                this.RoamingHandler.BeginWandering(this.HomeAreaEntry);

            if (!started)
            {
                this.BeginIdle();
                return;
            }

            this.SetState(AnimalState.Wandering);

            float walkDuration = this.RoamingConfig != null ? this.RoamingConfig.GetRandomWalkDuration() : 0f;
            this.StateEndTime = Time.time + walkDuration;
        }

        private void BeginGraze()
        {
            this.StopAllMovement();

            this.SetState(AnimalState.Graze);

            float grazeDuration = this.RoamingConfig != null ? this.RoamingConfig.GetRandomGrazeDuration() : 0f;
            this.StateEndTime = Time.time + grazeDuration;
        }

        private void BeginLieDown()
        {
            this.StopAllMovement();

            this.SetState(AnimalState.LieDown);

            float lieDownDuration = this.RoamingConfig != null ? this.RoamingConfig.LieDownDuration : 0f;
            this.StateEndTime = Time.time + lieDownDuration;
        }

        private void BeginRest()
        {
            this.StopAllMovement();

            this.SetState(AnimalState.Rest);

            float restDuration = this.RoamingConfig != null ? this.RoamingConfig.GetRandomRestDuration() : 0f;
            this.StateEndTime = Time.time + restDuration;
        }

        private void BeginSleep()
        {
            this.StopAllMovement();

            this.SetState(AnimalState.Sleep);

            float sleepDuration = this.RoamingConfig != null ? this.RoamingConfig.GetRandomSleepDuration() : 0f;
            this.StateEndTime = Time.time + sleepDuration;
        }

        private void BeginStandUp(AnimalState stateAfterStandUp)
        {
            this.StopAllMovement();

            this.StateAfterStandUp = stateAfterStandUp;
            this.SetState(AnimalState.StandUp);

            float standUpDuration = this.RoamingConfig != null ? this.RoamingConfig.StandUpDuration : 0f;
            this.StateEndTime = Time.time + standUpDuration;
        }

        //  4. Threat
        private void RefreshDamageThreat()
        {
            if (this.ThreatHandler == null) return;
            if (!this.ThreatHandler.HasThreat) return;

            float forgetDuration = this.FleeConfig != null
                ? this.FleeConfig.DamageThreatForgetDuration
                : 0f;

            if (!this.ThreatHandler.HasThreatExpired(forgetDuration)) return;

            this.ThreatHandler.ClearThreat();
        }

        //  5. Others
        private void SetState(AnimalState nextState)
        {
            if (this.CurrentState == nextState) return;

            this.CurrentState = nextState;
        }

        private bool IsReclinedState(AnimalState state)
        {
            return state == AnimalState.LieDown ||
                   state == AnimalState.Rest    ||
                   state == AnimalState.Sleep;
        }

        private void FaceTargetHorizontally(CharacterRootController target)
        {
            if (target == null) return;
            if (this.VisualController == null) return;

            float deltaX = target.transform.position.x - this.transform.position.x;
            if (Mathf.Abs(deltaX) <= 0.001f) return;

            Vector2 horizontalDirection = deltaX < 0f
                ? Vector2.left
                : Vector2.right;

            this.VisualController.ForceFacing(horizontalDirection);
        }

        private void StopAllMovement()
        {
            this.RoamingHandler?.StopWandering();
            this.FleeHandler?.StopFleeing();

            this.NeutralCharacterInputHandler?.ClearMoveInput();
        }

        private void HandleDeadState()
        {
            this.StopAllMovement();
        }

        private void ResetAnimalRuntime()
        {
            this.StateEndTime = 0f;
            this.StateAfterStandUp = AnimalState.Idle;
            this.IsFleeBlockedByHomeBoundary = false;

            this.ThreatHandler?.ResetRuntime();
            this.FleeHandler?.ResetRuntime();
            this.RoamingHandler?.ResetRuntime();

            this.BeginIdle();
        }

        //  6. Hooks
        protected override void OnBrainRefreshed(CharacterRefreshContext context)
        {
            base.OnBrainRefreshed(context);

            this.ResetAnimalRuntime();
        }

        protected override void OnBrainDeathDisabled(CharacterDeathContext context)
        {
            base.OnBrainDeathDisabled(context);

            this.ThreatHandler?.ResetRuntime();
            this.FleeHandler?.ResetRuntime();
            this.RoamingHandler?.ResetRuntime();

            this.SetState(AnimalState.Dead);

            this.StopAllMovement();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.RoamingHandler == null)
            {
                _roamingHandler =
                    this.GetComponentInChildren<AnimalRoamingHandler>(true);
            }

            if (this.ThreatHandler == null)
            {
                _threatHandler =
                    this.GetComponentInChildren<AnimalThreatHandler>(true);
            }

            if (this.FleeHandler == null)
            {
                _fleeHandler =
                    this.GetComponentInChildren<AnimalFleeHandler>(true);
            }
        }

        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.RoamingHandler?.Bind(this);
            this.ThreatHandler?.Bind(this);
            this.FleeHandler?.Bind(this);
        }

        protected override void UnbindInternalReferences()
        {
            this.FleeHandler?.Unbind(this);
            this.ThreatHandler?.Unbind(this);
            this.RoamingHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }

        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.RoamingHandler == null)
            {
                Debug.LogError(
                    $"[{this.CharacterName}.{nameof(AnimalBrainController)}] " +
                    $"{nameof(AnimalRoamingHandler)} is missing.",
                    this);
            }

            if (this.ThreatHandler == null)
            {
                Debug.LogError(
                    $"[{this.CharacterName}.{nameof(AnimalBrainController)}] " +
                    $"{nameof(AnimalThreatHandler)} is missing.",
                    this);
            }

            if (this.FleeHandler == null)
            {
                Debug.LogError(
                    $"[{this.CharacterName}.{nameof(AnimalBrainController)}] " +
                    $"{nameof(AnimalFleeHandler)} is missing.",
                    this);
            }
        }

        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ResetAnimalRuntime();
        }
    }
}