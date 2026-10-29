using UnityEngine;

namespace FlameOfTheLost
{
    public class SkirmisherEnemyBrainController : EnemyBrainController
    {
        // --- Data / Configs ---
        //  1. Skirmisher AI
        [Header("Skirmisher Config (Data)")]
        [SerializeField] private SkirmisherEnemyRoamingConfig _roamingConfig;
        [SerializeField] private SkirmisherEnemyEngagementConfig _engagementConfig;
        [SerializeField] private SkirmisherEnemyPositioningConfig _positioningConfig;
        [SerializeField] private SkirmisherEnemyCombatConfig _combatConfig;
        [SerializeField] private SkirmisherEnemyRetreatConfig _retreatConfig;
        public SkirmisherEnemyRoamingConfig RoamingConfig
        {
            get => _roamingConfig;
            private set => _roamingConfig = value;
        }
        public SkirmisherEnemyEngagementConfig EngagementConfig
        {
            get => _engagementConfig;
            private set => _engagementConfig = value;
        }
        public SkirmisherEnemyPositioningConfig PositioningConfig
        {
            get => _positioningConfig;
            private set => _positioningConfig = value;
        }
        public SkirmisherEnemyCombatConfig CombatConfig
        {
            get => _combatConfig;
            private set => _combatConfig = value;
        }
        public SkirmisherEnemyRetreatConfig RetreatConfig
        {
            get => _retreatConfig;
            private set => _retreatConfig = value;
        }

        // --- Runtime ---
        //  1. Runtime AI State
        [Header("AI State (Runtime)")]
        [SerializeField] private SkirmisherEnemyState _currentAIState = SkirmisherEnemyState.Idle;
        [SerializeField] private float _stateEnterTime;
        public SkirmisherEnemyState CurrentAIState
        {
            get => _currentAIState;
            private set => _currentAIState = value;
        }
        public float StateEnterTime
        {
            get => _stateEnterTime;
            private set => _stateEnterTime = value;
        }

        public float StateElapsedTime => Time.time - this.StateEnterTime;

        //  2. Home Area
        [Header("Home Area (Runtime)")]
        [SerializeField] private TerritorialEnemyHomeAreaEntry _homeAreaEntry;
        public TerritorialEnemyHomeAreaEntry HomeAreaEntry
        {
            get => _homeAreaEntry;
            private set => _homeAreaEntry = value;
        }

        //  3. Engagement Runtime
        [Header("Engagement (Runtime)")]
        [SerializeField] private CharacterRootController _engagedTarget;
        [SerializeField] private float _engagementStartTime;
        [SerializeField] private float _lastTargetDetectedTime;
        [SerializeField] private int _retreatCount;
        public CharacterRootController EngagedTarget
        {
            get => _engagedTarget;
            private set => _engagedTarget = value;
        }
        public float EngagementStartTime
        {
            get => _engagementStartTime;
            private set => _engagementStartTime = value;
        }
        public int RetreatCount
        {
            get => _retreatCount;
            private set => _retreatCount = value;
        }

        // 4. Roaming
        [Header("Roaming (Runtime)")]
        [SerializeField] private float _nextRoamPhaseTime;
        [SerializeField] private Vector2 _roamMoveDirection;

        // 5. Positioning
        [Header("Positioning (Runtime)")]
        [SerializeField] private int _strafeDirection = 1;
        [SerializeField] private float _nextStrafePhaseTime;

        // 6. Combat
        [Header("Combat (Runtime)")]
        [SerializeField] private float _nextAttackDecisionTime;
        [SerializeField] private float _attackStateEndTime;
        [SerializeField] private float _recoveryStateEndTime;

        // 7. Retreat
        [Header("Retreat (Runtime)")]
        [SerializeField] private float _retreatEndTime;

        // 8. Returning
        [Header("Returning (Runtime)")]
        [SerializeField] private bool _hasHomeReturnPoint;
        [SerializeField] private Vector2 _homeReturnPoint;

        // --- References ---
        private EnemyRootController EnemyRoot => this.CharacterRootController as EnemyRootController;
        private CharacterCombatController CombatController => this.CharacterRootController != null
            ? this.CharacterRootController.CombatController
            : null;
        private CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController != null
            ? this.CharacterRootController.CombatLoadoutController
            : null;
        private HPController HPController => this.CharacterRootController != null
            ? this.CharacterRootController.HPController
            : null;
        private CharacterAITargetDetectionHandler TargetHandler => this.EnemyRoot != null
            ? this.EnemyRoot.TargetDetectionHandler
            : null;

        private Vector2 CurrentPosition => this.CharacterRootController != null
            ? (Vector2)this.CharacterRootController.transform.position
            : (Vector2)this.transform.position;

        private bool HasValidAIConfig => this.RoamingConfig != null && this.EngagementConfig != null && this.PositioningConfig != null && this.CombatConfig != null && this.RetreatConfig != null && this.HomeAreaEntry.HasValidArea();

        // --- Unity Callbacks ---
        protected virtual void Update()
        {
            if (this.IsDeathDisabled) return;
            if (!this.HasValidAIConfig) return;

            this.TickAI();
        }

        // --- Public APIs ---
        //  1. AI Configuration
        public void ConfigureSkirmisherAI(SkirmisherEnemyRoamingConfig roamingConfig, SkirmisherEnemyEngagementConfig engagementConfig, SkirmisherEnemyPositioningConfig positioningConfig, SkirmisherEnemyCombatConfig combatConfig, SkirmisherEnemyRetreatConfig retreatConfig, TerritorialEnemyHomeAreaEntry homeAreaEntry)
        {
            if (roamingConfig == null || engagementConfig == null || positioningConfig == null || combatConfig == null || retreatConfig == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(SkirmisherEnemyBrainController)}] Missing one or more Skirmisher AI configurations.", this);
                return;
            }

            if (!homeAreaEntry.HasValidArea())
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(SkirmisherEnemyBrainController)}] Invalid home area.", this);
                return;
            }

            this.RoamingConfig      = roamingConfig;
            this.EngagementConfig   = engagementConfig;
            this.PositioningConfig  = positioningConfig;
            this.CombatConfig       = combatConfig;
            this.RetreatConfig      = retreatConfig;

            this.HomeAreaEntry = homeAreaEntry;

            this.ResetAIRuntime();
        }

        public void ConfigureHomeArea(TerritorialEnemyHomeAreaEntry homeAreaEntry)
        {
            if (!homeAreaEntry.HasValidArea())
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(SkirmisherEnemyBrainController)}] Invalid home area.", this);
                return;
            }

            this.HomeAreaEntry = homeAreaEntry;
        }

        // --- Helpers ---
        //  1. AI State Management
        private void SetAIState(SkirmisherEnemyState nextState)
        {
            if (this.CurrentAIState == nextState) return;

            this.CurrentAIState = nextState;
            this.StateEnterTime = Time.time;

            this.EnemyInputHandler?.ClearMoveInput();

            switch (nextState)
            {
                case SkirmisherEnemyState.Idle:
                    this.BeginIdlePhase();
                    break;

                case SkirmisherEnemyState.Wandering:
                    this.BeginWanderingPhase();
                    break;

                case SkirmisherEnemyState.Strafing:
                    this.BeginStrafingPhase();
                    break;

                case SkirmisherEnemyState.Retreating:
                    this.BeginRetreatPhase();
                    break;

                case SkirmisherEnemyState.Returning:
                    this.BeginReturningPhase();
                    break;
            }
        }
        private void TickAI()
        {
            if (this.CurrentAIState == SkirmisherEnemyState.Dead)
            {
                this.StopMoving();
                return;
            }

            // Returning must finish before new targets are acquired.
            if (this.CurrentAIState == SkirmisherEnemyState.Returning)
            {
                this.HandleReturningState();
                return;
            }

            // Attack states are controlled by the Combat lifecycle.
            if (this.CurrentAIState == SkirmisherEnemyState.NormalAttacking || this.CurrentAIState == SkirmisherEnemyState.SpecialAttacking)
            {
                this.HandleAttackingState();
                return;
            }

            if (this.CurrentAIState == SkirmisherEnemyState.Recovering)
            {
                this.HandleRecoveringState();
                return;
            }

            // Resolve the existing engagement or acquire a new target.
            if (this.EngagedTarget != null)
            {
                if (!this.UpdateEngagement()) return;
            }
            else
            {
                if (this.CurrentAIState == SkirmisherEnemyState.Retreating)
                {
                    this.SetAIState(SkirmisherEnemyState.Returning);
                    return;
                }

                if (!this.HomeAreaEntry.Contains(this.CurrentPosition))
                {
                    this.SetAIState(SkirmisherEnemyState.Returning);
                    return;
                }

                CharacterRootController target = this.FindEngagementTarget();

                if (target != null)
                {
                    this.BeginEngagement(target);
                }
            }

            // Retreat takes priority over normal positioning/combat.
            if (this.EngagedTarget != null && this.CurrentAIState != SkirmisherEnemyState.Retreating && this.ShouldRetreat())
            {
                this.SetAIState(SkirmisherEnemyState.Retreating);
            }

            switch (this.CurrentAIState)
            {
                case SkirmisherEnemyState.Idle:
                    this.HandleIdleState();
                    break;

                case SkirmisherEnemyState.Wandering:
                    this.HandleWanderingState();
                    break;

                case SkirmisherEnemyState.Alert:
                    this.HandleAlertState();
                    break;

                case SkirmisherEnemyState.Chasing:
                    this.HandleChasingState();
                    break;

                case SkirmisherEnemyState.Strafing:
                    this.HandleStrafingState();
                    break;

                case SkirmisherEnemyState.Retreating:
                    this.HandleRetreatingState();
                    break;
            }
        }

        //  2. Engagement Management
        private CharacterRootController FindEngagementTarget()
        {
            CharacterRootController target = this.CurrentAttackRangeTarget;

            if (this.IsValidEngagementTarget(target)) return target;

            target = this.CurrentAlertTarget;

            return this.IsValidEngagementTarget(target) ? target : null;
        }
        private bool IsValidEngagementTarget(CharacterRootController target)
        {
            if (target == null) return false;
            if (target == this.CharacterRootController) return false;
            if (!target.gameObject.activeInHierarchy) return false;

            if (target.HPController != null && target.HPController.IsDead) return false;

            return CharacterAITargetValidationUtility.CanTarget(this.CharacterRootController.Faction, target.Faction);
        }
        private void BeginEngagement(CharacterRootController target)
        {
            if (target == null) return;

            this.EngagedTarget = target;
            this.EngagementStartTime = Time.time;
            _lastTargetDetectedTime = Time.time;
            this.RetreatCount = 0;

            this.SetAIState(SkirmisherEnemyState.Alert);
        }
        private void EndEngagement()
        {
            this.EngagedTarget = null;
            this.EngagementStartTime = 0f;
            _lastTargetDetectedTime = 0f;
            this.RetreatCount = 0;
        }
        private bool UpdateEngagement()
        {
            if (!this.IsValidEngagementTarget(this.EngagedTarget))
            {
                this.SetAIState(SkirmisherEnemyState.Returning);
                return false;
            }

            // Maximum pursuit distance measured from the home area.
            // Moving outside the configured leash ends the engagement.
            Vector2 homeMin = this.HomeAreaEntry.GetMin();
            Vector2 homeMax = this.HomeAreaEntry.GetMax();

            Vector2 closestHomePoint = new Vector2(Mathf.Clamp(this.CurrentPosition.x, homeMin.x, homeMax.x), Mathf.Clamp(this.CurrentPosition.y, homeMin.y, homeMax.y));

            float distanceOutsideHome = Vector2.Distance(this.CurrentPosition, closestHomePoint);

            if (distanceOutsideHome > this.EngagementConfig.MaximumPursuitDistance)
            {
                this.SetAIState(SkirmisherEnemyState.Returning);
                return false;
            }

            // This method queries membership of the engaged target, rather than comparing against the nearest detected target.
            bool targetDetected = this.TargetHandler != null && this.TargetHandler.IsTargetDetected(this.EngagedTarget);

            if (targetDetected)
            {
                _lastTargetDetectedTime = Time.time;
            }
            else if (Time.time - _lastTargetDetectedTime > this.EngagementConfig.TargetLostGraceDuration)
            {
                this.SetAIState(SkirmisherEnemyState.Returning);
                return false;
            }

            return true;
        }
        private Vector2 GetDirectionToEngagedTarget()
        {
            if (this.EngagedTarget == null) return Vector2.zero;

            return (Vector2)this.EngagedTarget.transform.position - this.CurrentPosition;
        }

        // 3. Idle
        private void BeginIdlePhase()
        {
            _roamMoveDirection = Vector2.zero;

            _nextRoamPhaseTime = Time.time + this.RoamingConfig.GetRandomIdleDuration();
        }
        private void HandleIdleState()
        {
            this.StopMoving();

            if (Time.time < _nextRoamPhaseTime) return;

            this.SetAIState(SkirmisherEnemyState.Wandering);
        }

        // 4. Wandering
        private void BeginWanderingPhase()
        {
            Vector2 destination = this.HomeAreaEntry.GetRandomPointInside();

            _roamMoveDirection = (destination - this.CurrentPosition).normalized;

            _nextRoamPhaseTime = Time.time + this.RoamingConfig.GetRandomWalkDuration();
        }
        private void HandleWanderingState()
        {
            if (!this.HomeAreaEntry.Contains(this.CurrentPosition))
            {
                this.SetAIState(SkirmisherEnemyState.Returning);
                return;
            }

            if (Time.time >= _nextRoamPhaseTime)
            {
                this.SetAIState(SkirmisherEnemyState.Idle);
                return;
            }

            Vector2 projectedPosition = this.CurrentPosition + _roamMoveDirection * 0.25f;

            if (!this.HomeAreaEntry.Contains(projectedPosition))
            {
                Vector2 destination = this.HomeAreaEntry.GetRandomPointInside();

                _roamMoveDirection = (destination - this.CurrentPosition).normalized;
            }

            this.Move(_roamMoveDirection);
        }

        // 5. Alert
        private void HandleAlertState()
        {
            this.StopMoving();
            this.FaceEngagedTarget();

            if (this.StateElapsedTime < this.EngagementConfig.AlertDuration) return;

            this.SetAIState(SkirmisherEnemyState.Chasing);
        }

        // 6. Chasing
        private void HandleChasingState()
        {
            if (this.EngagedTarget == null)
            {
                this.SetAIState(SkirmisherEnemyState.Returning);
                return;
            }

            Vector2 toTarget = this.GetDirectionToEngagedTarget();

            this.FaceDirection(toTarget);

            if (toTarget.magnitude <= this.PositioningConfig.MaximumCombatDistance)
            {
                this.SetAIState(SkirmisherEnemyState.Strafing);
                return;
            }

            this.Move(toTarget, this.EngagementConfig.ChaseSpeedMultiplier);
        }

        // 7. Strafing
        private void BeginStrafingPhase()
        {
            _strafeDirection = Random.value < 0.5f ? -1 : 1;

            _nextStrafePhaseTime = Time.time +
                this.PositioningConfig.GetRandomStrafeDuration();

            _nextAttackDecisionTime = Time.time +
                this.CombatConfig.GetRandomAttackDecisionInterval();
        }

        private void HandleStrafingState()
        {
            if (this.EngagedTarget == null)
            {
                this.SetAIState(SkirmisherEnemyState.Returning);
                return;
            }

            Vector2 toTarget = this.GetDirectionToEngagedTarget();
            float distance = toTarget.magnitude;

            this.FaceDirection(toTarget);

            if (distance > this.PositioningConfig.MaximumCombatDistance)
            {
                this.SetAIState(SkirmisherEnemyState.Chasing);
                return;
            }

            if (Time.time >= _nextStrafePhaseTime)
            {
                if (Random.value < this.PositioningConfig.DirectionChangeChance)
                {
                    _strafeDirection *= -1;
                }

                _nextStrafePhaseTime = Time.time + this.PositioningConfig.GetRandomStrafeDuration();
            }

            Vector2 radialDirection = toTarget.sqrMagnitude > 0.0001f
                ? toTarget.normalized
                : Vector2.right;

            Vector2 tangentDirection = new Vector2(-radialDirection.y, radialDirection.x) * _strafeDirection;

            Vector2 moveDirection = tangentDirection;

            if (distance < this.PositioningConfig.MinimumCombatDistance)
            {
                moveDirection -= radialDirection;
            }
            else if (distance > this.PositioningConfig.PreferredCombatDistance)
            {
                moveDirection += radialDirection;
            }

            this.Move(moveDirection, this.PositioningConfig.StrafeSpeedMultiplier);

            if (Time.time < _nextAttackDecisionTime) return;

            _nextAttackDecisionTime = Time.time + this.CombatConfig.GetRandomAttackDecisionInterval();

            this.TryBeginAttack();
        }

        // 8. Combat
        private void TryBeginAttack()
        {
            if (this.EngagedTarget == null) return;
            if (this.CombatController == null) return;
            if (this.CombatLoadoutController == null) return;
            if (this.EnemyInputHandler == null) return;

            if (this.EnemyInputHandler.IsDeathDisabled || this.EnemyInputHandler.IsHitSequenceInputBlocked) return;

            if (this.CharacterRootController.DodgeController != null && this.CharacterRootController.DodgeController.ShouldBlockOtherInputs) return;

            if (this.CombatController.IsInAttackWindow) return;

            RuntimeWeapon weapon = this.CombatLoadoutController.ActiveEquippedRuntimeWeapon;
            if (weapon == null || weapon.Profile == null) return;

            bool canNormalAttack = weapon.NormalSkill != null && !this.CombatController.IsAttackLocked;

            bool canSpecialAttack = weapon.ActiveSpecialSkill != null && !this.CombatController.IsSpecialAttackLocked;

            int normalWeight = canNormalAttack ? this.CombatConfig.NormalAttackWeight : 0;
            int specialWeight = canSpecialAttack ? this.CombatConfig.SpecialAttackWeight : 0;

            int totalWeight = normalWeight + specialWeight;

            if (totalWeight <= 0) return;

            bool chooseSpecial = Random.Range(0, totalWeight) >= normalWeight;

            this.StopMoving();
            this.FaceEngagedTarget();

            bool attackStarted;

            if (chooseSpecial)
            {
                attackStarted = this.CombatController.TryDoSpecialAttack(this.CombatConfig.NormalAttackSide);

                if (!attackStarted) return;

                _attackStateEndTime = Time.time + this.CombatConfig.SpecialAttackStateDuration;

                this.SetAIState(SkirmisherEnemyState.SpecialAttacking);
            }
            else
            {
                attackStarted = this.CombatController.TryDoNormalAttack(this.CombatConfig.NormalAttackSide);

                if (!attackStarted) return;

                _attackStateEndTime = Time.time + this.CombatConfig.NormalAttackStateDuration;

                this.SetAIState(SkirmisherEnemyState.NormalAttacking);
            }
        }
        private void HandleAttackingState()
        {
            this.StopMoving();

            if (Time.time < _attackStateEndTime) return;

            // Allow the shared CombatController to complete the window.
            if (this.CombatController != null && this.CombatController.IsInAttackWindow) return;

            float recoveryDuration = this.CurrentAIState == SkirmisherEnemyState.SpecialAttacking
                ? this.CombatConfig.SpecialAttackRecoveryDuration
                : this.CombatConfig.NormalAttackRecoveryDuration;

            _recoveryStateEndTime = Time.time + recoveryDuration;

            this.SetAIState(SkirmisherEnemyState.Recovering);
        }

        // 9. Recovering
        private void HandleRecoveringState()
        {
            this.StopMoving();

            if (Time.time < _recoveryStateEndTime) return;

            if (!this.UpdateEngagement()) return;

            if (this.ShouldRetreat())
            {
                this.SetAIState(SkirmisherEnemyState.Retreating);
                return;
            }

            this.SetAIState(SkirmisherEnemyState.Chasing);
        }

        // 10. Retreating
        private bool ShouldRetreat()
        {
            if (this.EngagedTarget == null) return false;

            if (this.RetreatCount >= this.RetreatConfig.MaxRetreatsPerEngagement) return false;

            // Uses the isolated health reader below.
            if (!this.TryGetHealthRatio(out float healthRatio)) return false;

            return healthRatio <= this.RetreatConfig.HealthThreshold;
        }
        private void BeginRetreatPhase()
        {
            this.RetreatCount++;

            _retreatEndTime = Time.time + Mathf.Max(this.RetreatConfig.MinimumRetreatDuration, this.RetreatConfig.MaximumRetreatDuration);
        }
        private void HandleRetreatingState()
        {
            if (this.EngagedTarget == null)
            {
                this.SetAIState(SkirmisherEnemyState.Returning);
                return;
            }

            Vector2 toTarget = this.GetDirectionToEngagedTarget();
            float distance = toTarget.magnitude;

            this.FaceDirection(toTarget);

            bool minimumDurationCompleted = this.StateElapsedTime >= this.RetreatConfig.MinimumRetreatDuration;

            bool reachedSafeDistance = distance >= this.RetreatConfig.PreferredSafeDistance;

            if (Time.time >= _retreatEndTime || (minimumDurationCompleted && reachedSafeDistance))
            {
                this.SetAIState(SkirmisherEnemyState.Chasing);
                return;
            }

            Vector2 retreatDirection = -toTarget.normalized;

            // Do not retreat beyond the configured pursuit leash.
            Vector2 projectedPosition = this.CurrentPosition + retreatDirection * 0.25f;

            Vector2 homeMin = this.HomeAreaEntry.GetMin();
            Vector2 homeMax = this.HomeAreaEntry.GetMax();

            Vector2 closestHomePoint = new Vector2(Mathf.Clamp(projectedPosition.x, homeMin.x, homeMax.x), Mathf.Clamp(projectedPosition.y, homeMin.y, homeMax.y));

            float outsideDistance = Vector2.Distance(projectedPosition, closestHomePoint);

            if (outsideDistance > this.EngagementConfig.MaximumPursuitDistance)
            {
                this.SetAIState(SkirmisherEnemyState.Returning);
                return;
            }

            this.Move(retreatDirection, this.RetreatConfig.RetreatSpeedMultiplier);
        }

        // 11. Returning
        private void BeginReturningPhase()
        {
            this.EndEngagement();

            _hasHomeReturnPoint = false;
            _homeReturnPoint = Vector2.zero;
        }
        private void HandleReturningState()
        {
            if (!_hasHomeReturnPoint)
            {
                Vector2 min = this.HomeAreaEntry.GetMin();
                Vector2 max = this.HomeAreaEntry.GetMax();

                Vector2 center = (min + max) * 0.5f;

                Vector2 clamped = new Vector2(Mathf.Clamp(this.CurrentPosition.x, min.x, max.x), Mathf.Clamp(this.CurrentPosition.y, min.y, max.y));

                // Return to a valid interior position. If already inside, the current position is a valid return destination.
                _homeReturnPoint = Vector2.Lerp(clamped, center, 0.25f);
                _hasHomeReturnPoint = true;
            }

            Vector2 toHome = _homeReturnPoint - this.CurrentPosition;

            float reachDistance = Mathf.Max(0.05f,this.HomeAreaEntry.ReturnReachDistance);

            if (toHome.magnitude <= reachDistance && this.HomeAreaEntry.Contains(this.CurrentPosition))
            {
                _hasHomeReturnPoint = false;
                this.StopMoving();

                this.SetAIState(SkirmisherEnemyState.Idle);
                return;
            }

            this.Move(toHome);
        }

        // 12. Movement / Facing
        private void Move(Vector2 direction, float speedMultiplier = 1f)
        {
            if (direction.sqrMagnitude <= 0.0001f)
            {
                this.StopMoving();
                return;
            }

            // MovementHandler clamps RawMovementInput to magnitude 1.
            // Values > 1 require a separate movement-speed channel.
            float inputScale = Mathf.Clamp01(speedMultiplier);

            this.EnemyInputHandler?.SetMoveInput(direction.normalized * inputScale);
        }
        private void StopMoving()
        {
            this.EnemyInputHandler?.ClearMoveInput();
        }
        private void FaceEngagedTarget()
        {
            this.FaceDirection(this.GetDirectionToEngagedTarget());
        }
        private void FaceDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.0001f) return;

            this.VisualController?.ForceFacing(direction);
        }

        // 13. Health Reader
        private bool TryGetHealthRatio(out float ratio)
        {
            ratio = 1f;

            HPController hpController = this.HPController;

            if (hpController == null) return false;
            if (hpController.IsDead || hpController.IsDeathDisabled) return false;
            if (hpController.MaxValue <= 0) return false;

            ratio = Mathf.Clamp01((float)hpController.CurrentValue / hpController.MaxValue);

            return true;
        }

        // 14. Runtime Reset
        private void ResetAIRuntime()
        {
            this.EndEngagement();

            this.CurrentAIState = SkirmisherEnemyState.Idle;
            this.StateEnterTime = Time.time;

            _nextRoamPhaseTime = this.RoamingConfig != null
                ? Time.time + this.RoamingConfig.GetRandomIdleDuration()
                : Time.time;

            _roamMoveDirection = Vector2.zero;

            _strafeDirection = 1;
            _nextStrafePhaseTime = 0f;

            _nextAttackDecisionTime = 0f;
            _attackStateEndTime = 0f;
            _recoveryStateEndTime = 0f;

            _retreatEndTime = 0f;

            _homeReturnPoint = Vector2.zero;
            _hasHomeReturnPoint = false;

            this.StopMoving();
        }

        //  15. EnemyBrainController Overrides
        protected override void OnBrainRefreshed(CharacterRefreshContext context)
        {
            base.OnBrainRefreshed(context);

            this.ResetAIRuntime();
        }
        protected override void OnBrainDeathDisabled(CharacterDeathContext context)
        {
            base.OnBrainDeathDisabled(context);

            this.EndEngagement();

            this.CurrentAIState = SkirmisherEnemyState.Dead;
            this.StateEnterTime = Time.time;

            this.StopMoving();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ResetAIRuntime();
        }
        protected override void InitializeGameplay()
        {
            base.InitializeGameplay();

            this.CombatLoadoutController?.TryQuickEquipWeapon();
        }
    }
}
