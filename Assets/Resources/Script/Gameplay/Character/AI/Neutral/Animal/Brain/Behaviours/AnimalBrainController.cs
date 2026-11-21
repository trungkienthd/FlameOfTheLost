using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(AnimalRoamingHandler))]
    [RequireComponent(typeof(AnimalDamageReactionHandler))]
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

        // --- References ---
        [Header("References - Animal Brain Sub-Behaviours")]
        [SerializeField] private AnimalRoamingHandler _roamingHandler;
        [SerializeField] private AnimalDamageReactionHandler _damageReactionHandler;
        [SerializeField] private AnimalFleeHandler _fleeHandler;
        public AnimalRoamingHandler RoamingHandler => _roamingHandler;
        public AnimalDamageReactionHandler DamageReactionHandler => _damageReactionHandler;
        public AnimalFleeHandler FleeHandler => _fleeHandler;

        public AnimalRootController AnimalRootController => this.CharacterRootController as AnimalRootController;
        public new NeutralCharacterInputHandler NeutralCharacterInputHandler => this.AnimalRootController != null ? this.AnimalRootController.NeutralCharacterInputHandler : null;
        private CharacterAITargetDetectionHandler TargetDetectionHandler => this.AnimalRootController != null ? this.AnimalRootController.TargetDetectionHandler : null;

        // --- Runtime ---
        //  1. AI Animal State
        [Header("Animal State (Runtime)")]
        [SerializeField] private AnimalState _currentState = AnimalState.Idle;
        public AnimalState CurrentState
        {
            get => _currentState;
            private set => _currentState = value;
        }
        [SerializeField] private AnimalLieDownReason _lieDownReason = AnimalLieDownReason.None;
        public AnimalLieDownReason LieDownReason
        {
            get => _lieDownReason;
            private set => _lieDownReason = value;
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

        //  4. Home Area
        [Header("Home Area (Runtime)")]
        [SerializeField] private AnimalHomeAreaEntry _homeAreaEntry;
        public AnimalHomeAreaEntry HomeAreaEntry
        {
            get => _homeAreaEntry;
            private set => _homeAreaEntry = value;
        }

        //  6. Convenience State
        public bool IsSurrendering => this.CurrentState == AnimalState.LieDown && this.LieDownReason == AnimalLieDownReason.Surrender;
        private bool IsInsideHomeAreaHorizontally
        {
            get
            {
                if (!this.HomeAreaEntry.HasValidArea()) return true;
                if (this.AnimalRootController == null) return true;

                float x = this.AnimalRootController.transform.position.x;

                return this.HomeAreaEntry.ContainsHorizontalPosition(x);
            }
        }

        // --- Unity Callbacks ---
        protected virtual void Update()
        {
            if (this.IsDeathDisabled) return;

            this.TickAI();
        }

        // --- Public APIs ---
        //  1. COnfiguration
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

            this.RoamingConfig  = roamingConfig;
            this.FleeConfig     = fleeConfig;
            this.HomeAreaEntry  = homeAreaEntry;

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

        //  2. Damage
        public void NotifyDamageReceived()
        {
            if (this.IsDeathDisabled) return;
            if (this.CurrentState == AnimalState.Dead) return;

            this.BeginSurrender();
        }

        // --- Helpers ----
        //  1. AI Tick
        private void TickAI()
        {
            if (this.CurrentState == AnimalState.Dead)
            {
                this.HandleDeadState();
                return;
            }

            CharacterRootController alertTarget = this.TargetDetectionHandler != null
                ? this.TargetDetectionHandler.CurrentAlertTarget
                : null;
            CharacterRootController fleeTarget  = this.TargetDetectionHandler != null 
                ? this.TargetDetectionHandler.CurrentFleeRangeTarget
                : null;

            // a. Priority 1 - Surrender
            if (this.IsSurrendering)
            {
                this.TickSurrender(alertTarget);
                return;
            }

            // b. Priority 2 - Fleeing outside Home Area
            if (this.CurrentState == AnimalState.Fleeing && !this.IsInsideHomeAreaHorizontally)
            {
                this.BeginSurrender();
                return;
            }

            // c. Priority 3 - Flee Range
            if (fleeTarget != null)
            {
                this.HandleFleeIntent(fleeTarget);
                return;
            }

            // d. Priority 4 - Alert Range
            if (alertTarget != null)
            {
                this.HandleAlertIntent(alertTarget);
                return;
            }

            // e. Priority 5 - No Target
            this.HandleNoDetectedTarget();
        }

        //  2. Detection Behaviour
        private void HandleAlertIntent(CharacterRootController target)
        {
            if (target == null) return;

            // Passive reclined Cow must first stand up.
            if (this.IsPassiveReclinedState(this.CurrentState))
            {
                this.BeginStandUp(AnimalState.Alert);
                return;
            }

            // Already standing up: change its destination state.
            if (this.CurrentState == AnimalState.StandUp)
            {
                this.StateAfterStandUp = AnimalState.Alert;

                this.TickStandUp();
                return;
            }

            // Includes transition: Fleeing -> Alert
            this.StopAllMovement();

            this.LieDownReason = AnimalLieDownReason.None;

            this.SetState(AnimalState.Alert);

            this.FaceTargetHorizontally(target);
        }
        private void HandleFleeIntent(CharacterRootController target)
        {
            if (target == null) return;

            // Reclined -> StandUp -> Fleeing
            if (this.IsPassiveReclinedState(this.CurrentState))
            {
                this.BeginStandUp(AnimalState.Fleeing);
                return;
            }

            // If already standing up, change destination.
            if (this.CurrentState == AnimalState.StandUp)
            {
                this.StateAfterStandUp = AnimalState.Fleeing;
                this.TickStandUp();
                return;
            }

            this.LieDownReason = AnimalLieDownReason.None;

            this.SetState(AnimalState.Fleeing);

            this.RoamingHandler?.StopWandering();

            bool isFleeing = this.FleeHandler != null && this.FleeHandler.TickFlee(target, this.HomeAreaEntry, this.FleeConfig);
            if (!isFleeing)
            {
                this.StopAllMovement();
            }
        }
        private void HandleNoDetectedTarget()
        {
            // A reactive StandUp loses its target before standing completely. Finish standing, but go to Idle.
            if (this.CurrentState == AnimalState.StandUp)
            {
                if (this.StateAfterStandUp == AnimalState.Alert || this.StateAfterStandUp == AnimalState.Fleeing)
                {
                    this.StateAfterStandUp = AnimalState.Idle;
                }

                this.TickStandUp();
                return;
            }

            // Alert/Fleeing ends completely when the target leaves the broad AlertRange.
            if (this.CurrentState == AnimalState.Alert || this.CurrentState == AnimalState.Fleeing)
            {
                this.BeginIdle();
                return;
            }

            this.HandlePassiveBehaviour();
        }

        //  2. Surrender
        private void BeginSurrender()
        {
            this.StopAllMovement();

            this.LieDownReason = AnimalLieDownReason.Surrender;

            this.StateAfterStandUp = AnimalState.Idle;

            this.SetState(AnimalState.LieDown);

            // Surrender is not timer driven.
            this.StateEndTime = 0f;
        }
        private void TickSurrender(CharacterRootController alertTarget)
        {
            this.StopAllMovement();

            // Remain surrendered while the target is anywhere within the broad AlertRange.
            if (alertTarget != null) return;

            // AlertRange is clear.
            this.LieDownReason = AnimalLieDownReason.None;

            this.BeginIdle();
        }

        //  3. Passive Behaviour
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

            this.BeginWandering();
        }
        private void TickWandering()
        {
            // Outside Home Area: remain Wandering and return toward it.
            if (!this.IsInsideHomeAreaHorizontally)
            {
                // Pause the activity timer while correcting position.
                this.StateEndTime += Time.deltaTime;

                bool returning = this.RoamingHandler != null && this.RoamingHandler.TickReturnHome(this.HomeAreaEntry);
                if (!returning)
                {
                    this.BeginIdle();
                }

                return;
            }

            if (Time.time >= this.StateEndTime)
            {
                this.BeginGraze();
                return;
            }

            bool continuing = this.RoamingHandler != null && this.RoamingHandler.TickWandering();
            if (!continuing)
            {
                this.BeginIdle();
            }
        }

        private void TickGraze()
        {
            this.StopAllMovement();

            if (Time.time < this.StateEndTime) return;

            this.BeginPassiveLieDown();
        }
        private void TickLieDown()
        {
            this.StopAllMovement();

            // Surrender is handled before passive states inside TickAI()
            if (this.LieDownReason == AnimalLieDownReason.Surrender) return;
            if (Time.time < this.StateEndTime) return;

            bool shouldRest = this.RoamingConfig == null || this.RoamingConfig.ShouldRestAfterLieDown();
            if (shouldRest)
            {
                this.BeginRest();
            }
            else
            {
                this.BeginSleep();
            }
        }
        private void TickRest()
        {
            this.StopAllMovement();

            if (Time.time < this.StateEndTime) return;

            this.BeginStandUp(AnimalState.Idle);
        }
        private void TickSleep()
        {
            this.StopAllMovement();

            if (Time.time < this.StateEndTime) return;

            this.BeginStandUp(AnimalState.Idle);
        }
        private void TickStandUp()
        {
            this.StopAllMovement();

            if (Time.time < this.StateEndTime) return;

            AnimalState nextState = this.StateAfterStandUp;

            this.StateAfterStandUp = AnimalState.Idle;

            this.LieDownReason = AnimalLieDownReason.None;

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

        //  4. Passive State Starters
        private void BeginIdle()
        {
            this.StopAllMovement();

            this.LieDownReason = AnimalLieDownReason.None;

            this.SetState(AnimalState.Idle);

            float idleDuration = this.RoamingConfig != null ? this.RoamingConfig.GetRandomIdleDuration() : 0f;
            this.StateEndTime = Time.time + idleDuration;
        }
        private void BeginWandering()
        {
            bool started = this.RoamingHandler != null && this.RoamingHandler.BeginWandering(this.HomeAreaEntry);
            if (!started)
            {
                this.BeginIdle();
                return;
            }

            this.LieDownReason = AnimalLieDownReason.None;

            this.SetState(AnimalState.Wandering);

            float walkDuration = this.RoamingConfig != null ? this.RoamingConfig.GetRandomWalkDuration() : 0f;
            this.StateEndTime = Time.time + walkDuration;
        }
        private void BeginGraze()
        {
            this.StopAllMovement();

            this.LieDownReason = AnimalLieDownReason.None;

            this.SetState(AnimalState.Graze);

            float grazeDuration = this.RoamingConfig != null ? this.RoamingConfig.GetRandomGrazeDuration() : 0f;
            this.StateEndTime = Time.time + grazeDuration;
        }
        private void BeginPassiveLieDown()
        {
            this.StopAllMovement();

            this.LieDownReason = AnimalLieDownReason.Passive;

            this.SetState(AnimalState.LieDown);

            float lieDownDuration = this.RoamingConfig != null ? this.RoamingConfig.LieDownDuration : 0f;
            this.StateEndTime = Time.time + lieDownDuration;
        }
        private void BeginRest()
        {
            this.StopAllMovement();

            this.LieDownReason = AnimalLieDownReason.None;

            this.SetState(AnimalState.Rest);

            float restDuration = this.RoamingConfig != null ? this.RoamingConfig.GetRandomRestDuration() : 0f;
            this.StateEndTime = Time.time + restDuration;
        }
        private void BeginSleep()
        {
            this.StopAllMovement();

            this.LieDownReason = AnimalLieDownReason.None;

            this.SetState(AnimalState.Sleep);

            float sleepDuration = this.RoamingConfig != null ? this.RoamingConfig.GetRandomSleepDuration() : 0f;
            this.StateEndTime = Time.time + sleepDuration;
        }
        private void BeginStandUp(AnimalState stateAfterStandUp)
        {
            this.StopAllMovement();

            this.LieDownReason = AnimalLieDownReason.None;

            this.StateAfterStandUp = stateAfterStandUp;

            this.SetState(AnimalState.StandUp);

            float standUpDuration = this.RoamingConfig != null ? this.RoamingConfig.StandUpDuration : 0f;
            this.StateEndTime = Time.time + standUpDuration;
        }

        //  5. Others
        private void SetState(AnimalState nextState)
        {
            if (this.CurrentState == nextState) return;

            this.CurrentState = nextState;
        }

        private bool IsPassiveReclinedState(AnimalState state)
        {
            return (state == AnimalState.LieDown && this.LieDownReason != AnimalLieDownReason.Surrender) ||
                   state == AnimalState.Rest ||
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

            this.LieDownReason = AnimalLieDownReason.None;

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

            this.FleeHandler?.ResetRuntime();
            this.RoamingHandler?.ResetRuntime();

            this.LieDownReason = AnimalLieDownReason.None;

            this.SetState(AnimalState.Dead);

            this.StopAllMovement();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.RoamingHandler         == null) _roamingHandler        = this.GetComponentInChildren<AnimalRoamingHandler>(true);
            if (this.FleeHandler            == null) _fleeHandler           = this.GetComponentInChildren<AnimalFleeHandler>(true);
            if (this.DamageReactionHandler  == null) _damageReactionHandler = this.GetComponentInChildren<AnimalDamageReactionHandler>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.RoamingHandler         == null) Debug.LogError($"[{this.CharacterName}.{nameof(AnimalBrainController)}] {nameof(AnimalRoamingHandler)} is missing.", this);
            if (this.FleeHandler            == null) Debug.LogError($"[{this.CharacterName}.{nameof(AnimalBrainController)}] {nameof(AnimalFleeHandler)} is missing.", this);
            if (this.DamageReactionHandler  == null) Debug.LogError($"[{this.CharacterName}.{nameof(AnimalBrainController)}] {nameof(AnimalDamageReactionHandler)} is missing.", this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ResetAnimalRuntime();
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.RoamingHandler?.Bind(this);
            this.DamageReactionHandler?.Bind(this);
            this.FleeHandler?.Bind(this);
        }
        protected override void UnbindInternalReferences()
        {
            this.FleeHandler?.Unbind(this);
            this.DamageReactionHandler?.Unbind(this);
            this.RoamingHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}