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
        [SerializeField]
        private AnimalState _currentState = AnimalState.Roaming;
        public AnimalState CurrentState
        {
            get => _currentState;
            private set => _currentState = value;
        }

        //  2. Home Area
        [Header("Home Area (Runtime)")]
        [SerializeField] private AnimalHomeAreaEntry _homeAreaEntry;
        public AnimalHomeAreaEntry HomeAreaEntry
        {
            get => _homeAreaEntry;
            private set => _homeAreaEntry = value;
        }

        //  3. Flee Runtime
        [Header("Fleeing (Runtime)")]
        [SerializeField] private float _fleeStartTime;
        private float FleeStartTime
        {
            get => _fleeStartTime;
            set => _fleeStartTime = value;
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

            this.FleeStartTime = Time.time;
            this.SetState(AnimalState.Fleeing);
        }

        // --- AI ---
        private void TickAI()
        {
            switch (this.CurrentState)
            {
                case AnimalState.Dead:
                    this.HandleDeadState();
                    return;

                case AnimalState.Fleeing:
                    this.HandleFleeingState();
                    return;

                case AnimalState.Returning:
                    this.HandleReturningState();
                    return;

                case AnimalState.Roaming:
                default:
                    this.HandleRoamingState();
                    return;
            }
        }

        private void HandleRoamingState()
        {
            if (this.ThreatHandler != null && this.ThreatHandler.HasThreat)
            {
                this.FleeStartTime = Time.time;
                this.SetState(AnimalState.Fleeing);
                return;
            }

            if (!this.HomeAreaEntry.HasValidArea())
            {
                this.NeutralCharacterInputHandler?.ClearMoveInput();
                return;
            }

            Vector2 currentPosition = this.transform.position;

            if (!this.HomeAreaEntry.Contains(currentPosition))
            {
                this.SetState(AnimalState.Returning);
                return;
            }

            this.RoamingHandler?.TickRoaming
            (
                this.RoamingConfig,
                this.HomeAreaEntry
            );
        }

        private void HandleFleeingState()
        {
            CharacterRootController threat = this.ThreatHandler != null
                ? this.ThreatHandler.CurrentThreat
                : null;

            if (threat == null)
            {
                this.BeginReturning();
                return;
            }

            this.FleeHandler?.TickFlee(threat);

            if (this.FleeConfig == null) return;

            bool minimumDurationPassed =
                Time.time >=
                this.FleeStartTime +
                this.FleeConfig.MinimumFleeDuration;

            if (!minimumDurationPassed) return;

            float distanceToThreat = this.FleeHandler != null
                ? this.FleeHandler.GetDistanceToThreat(threat)
                : float.PositiveInfinity;

            bool safeDistanceReached =
                distanceToThreat >=
                this.FleeConfig.MinimumSafeDistance;

            bool threatExpired = this.ThreatHandler == null || this.ThreatHandler.HasThreatExpired(this.FleeConfig.ThreatForgetDuration);

            if (!safeDistanceReached && !threatExpired) return;

            this.ThreatHandler?.ClearThreat();
            this.BeginReturning();
        }

        private void HandleReturningState()
        {
            if (this.ThreatHandler != null && this.ThreatHandler.HasThreat)
            {
                this.FleeStartTime = Time.time;
                this.SetState(AnimalState.Fleeing);
                return;
            }

            bool returned = this.RoamingHandler != null && this.RoamingHandler.TickReturnToHome(this.HomeAreaEntry);

            if (!returned) return;

            this.RoamingHandler?.BeginIdlePhase(this.RoamingConfig);
            this.SetState(AnimalState.Roaming);
        }

        private void HandleDeadState()
        {
            this.NeutralCharacterInputHandler?.ClearMoveInput();
        }

        // --- State Helpers ---
        private void SetState(AnimalState nextState)
        {
            if (this.CurrentState == nextState) return;

            this.CurrentState = nextState;
        }

        private void BeginReturning()
        {
            this.FleeHandler?.StopFleeing();

            if (this.HomeAreaEntry.HasValidArea())
            {
                this.SetState(AnimalState.Returning);
            }
            else
            {
                this.SetState(AnimalState.Roaming);
                this.NeutralCharacterInputHandler?.ClearMoveInput();
            }
        }

        private void ResetAnimalRuntime()
        {
            this.FleeStartTime = 0f;

            this.ThreatHandler?.ResetRuntime();
            this.FleeHandler?.ResetRuntime();
            this.RoamingHandler?.ResetRuntime(this.RoamingConfig);

            this.SetState(AnimalState.Roaming);

            this.NeutralCharacterInputHandler?.ClearMoveInput();
        }

        // --- Hooks ---
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
            this.RoamingHandler?.ResetRuntime(this.RoamingConfig);

            this.SetState(AnimalState.Dead);

            this.NeutralCharacterInputHandler?.ClearMoveInput();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.RoamingHandler == null) _roamingHandler    = this.GetComponentInChildren<AnimalRoamingHandler>(true);
            if (this.ThreatHandler  == null) _threatHandler     = this.GetComponentInChildren<AnimalThreatHandler>(true);
            if (this.FleeHandler    == null) _fleeHandler       = this.GetComponentInChildren<AnimalFleeHandler>(true);
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

            if (this.RoamingHandler == null) Debug.LogError($"[{this.CharacterName}.{nameof(AnimalBrainController)}] {nameof(AnimalRoamingHandler)} is missing.", this);
            if (this.ThreatHandler  == null) Debug.LogError($"[{this.CharacterName}.{nameof(AnimalBrainController)}] {nameof(AnimalThreatHandler)} is missing.", this);
            if (this.FleeHandler    == null) Debug.LogError($"[{this.CharacterName}.{nameof(AnimalBrainController)}] {nameof(AnimalFleeHandler)} is missing.", this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ResetAnimalRuntime();
        }
    }
}