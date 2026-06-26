using FlameOfTheLost;
using UnityEngine;

public class TerritorialEnemyBrainController : EnemyBrainController
{
    // --- States ---
    //  1. Data
    [Header("Territorial Config (Data)")]
    [SerializeField] private TerritorialEnemyRoamingConfig _roamingConfig;
    [SerializeField] private TerritorialEnemyAttackConfig _attackConfig;
    private TerritorialEnemyRoamingConfig RoamingConfig => _roamingConfig;
    private TerritorialEnemyAttackConfig AttackConfig => _attackConfig;

    //  2. Runtime AI State
    [Header("AI State (Runtime)")]
    [SerializeField] private TerritorialEnemyState _currentAIState = TerritorialEnemyState.Roaming;
    public TerritorialEnemyState CurrentAIState
    {
        get => _currentAIState;
        private set => _currentAIState = value;
    }

    //  3. Home Area (Runtime)
    [Header("Home Area (Runtime)")]
    [SerializeField] private TerritorialEnemyHomeAreaEntry _homeAreaEntry;
    private TerritorialEnemyHomeAreaEntry HomeAreaEntry
    {
        get => _homeAreaEntry;
        set => _homeAreaEntry = value;
    }

    //  4. Roaming State (Runtime)
    [Header("Roaming (Runtime)")]
    [SerializeField] private bool _isRoamWalking;
    [SerializeField] private Vector2 _roamMoveDirection;
    [SerializeField] private float _nextRoamPhaseTime;
    [SerializeField] private bool _hasHomeReturnPoint;
    [SerializeField] private Vector2 _homeReturnPoint;
    private bool IsRoamWalking
    {
        get => _isRoamWalking;
        set => _isRoamWalking = value;
    }
    private Vector2 RoamMoveDirection
    {
        get => _roamMoveDirection;
        set => _roamMoveDirection = value;
    }
    private float NextRoamPhaseTime
    {
        get => _nextRoamPhaseTime;
        set => _nextRoamPhaseTime = value;
    }
    private bool HasHomeReturnPoint
    {
        get => _hasHomeReturnPoint;
        set => _hasHomeReturnPoint = value;
    }
    private Vector2 HomeReturnPoint
    {
        get => _homeReturnPoint;
        set => _homeReturnPoint = value;
    }

    //  5. Attack States (Runtime)
    [Header("Attack & PreAttack (Runtime)")]
    [SerializeField] private float _nextAllowedAttackTime;
    [SerializeField] private float _attackingStateEndTime;
    private float NextAllowedAttackTime
    {
        get => _nextAllowedAttackTime;
        set => _nextAllowedAttackTime = value;
    }
    private float AttackingStateEndTime
    {
        get => _attackingStateEndTime;
        set => _attackingStateEndTime = value;
    }

    //      5.1. PreAttack State (Runtime)
    [SerializeField] private float _preAttackStateEndTime;
    private float PreAttackStateEndTime
    {
        get => _preAttackStateEndTime;
        set => _preAttackStateEndTime = value;
    }

    //  6. References 
    private CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController.CombatLoadoutController;

    // --- Unity Callbacks ---
    protected override void Start()
    {
        this.InitializeBrainRuntimeAfterBinding();
    }
    protected virtual void Update()
    {
        if (this.IsDeathDisabled) return;

        this.TickAI();
    }

    // --- Public APIs ---
    public void ConfigureHomeArea(TerritorialEnemyHomeAreaEntry homeAreaEntry)
    {
        if (!homeAreaEntry.HasValidArea())
        {
            Debug.LogWarning($"[{this.CharacterName}.{nameof(TerritorialEnemyBrainController)}] Invalid home area entry provided for {this.name}. Home area not configured.");
            return;
        }
        this.HomeAreaEntry = homeAreaEntry;
    }

    // --- Helpers ---
    private void SetAIState(TerritorialEnemyState nextState)
    {
        if (this.CurrentAIState == nextState) return;

        this.CurrentAIState = nextState;
    }
    private void TickAI()
    {
        if (this.IsDeathDisabled) return;

        if (this.CurrentAIState == TerritorialEnemyState.Dead)
        {
            this.HandleDeadState();
            return;
        }

        if (this.CurrentAIState == TerritorialEnemyState.PreAttack)
        {
            this.HandlePreAttackState();
            return;
        }

        if (this.CurrentAIState == TerritorialEnemyState.Attacking)
        {
            this.HandleAttackingState();
            return;
        }

        if (this.CurrentAttackRangeTarget != null)
        {
            this.SetAIState(TerritorialEnemyState.ReadyToAttack);
            this.HandleReadyToAttackState();
            return;
        }

        if (this.CurrentAlertTarget != null)
        {
            this.SetAIState(TerritorialEnemyState.Alert);
            this.HandleAlertState();
            return;
        }

        this.SetAIState(TerritorialEnemyState.Roaming);
        this.HandleRoamingState();
    }

    private void HandleRoamingState()
    {
        if (!this.HasValidHomeArea())
        {
            this.EnemyInputHandler?.ClearMoveInput();
            return;
        }

        Vector2 currentPosition = this.transform.position;
        if (!this.HomeAreaEntry.Contains(currentPosition) || this.HasHomeReturnPoint)
        {
            this.HandleReturnToHomeArea(currentPosition);
            return;
        }

        this.HandleRandomRoaming(currentPosition);
    }
    private void HandleAlertState()
    {
        this.EnemyInputHandler?.ClearMoveInput();

        Transform targetTransform = this.CurrentTarget?.transform;
        if (targetTransform == null) return;

        this.FaceTarget(targetTransform);
    }
    private void HandleReadyToAttackState()
    {
        Transform targetTransform = this.CurrentTarget?.transform;
        if (targetTransform == null)
        {
            this.EnemyInputHandler?.ClearMoveInput();
            this.SetAIState(TerritorialEnemyState.Roaming);
            return;
        }

        Vector2 currentPosition = this.transform.position;
        Vector2 targetPosition = targetTransform.position;
        Vector2 toTarget = targetPosition - currentPosition;

        this.FaceDirection(toTarget);

        float distanceToTarget = toTarget.magnitude;

        if (distanceToTarget > this.AttackConfig.IdealAttackRange)
        {
            Vector2 moveDirection = toTarget.normalized;
            this.EnemyInputHandler?.SetMoveInput(moveDirection);
            return;
        }

        this.EnemyInputHandler?.ClearMoveInput();

        if (Time.time < this.NextAllowedAttackTime) return;

        this.PreAttackStateEndTime = Time.time + this.AttackConfig.PreAttackDuration;
        this.SetAIState(TerritorialEnemyState.PreAttack);
    }
    private void HandlePreAttackState()
    {
        this.EnemyInputHandler?.ClearMoveInput();

        Transform targetTransform = this.CurrentTarget?.transform;
        if (targetTransform != null)
        {
            this.FaceTarget(targetTransform);
        }

        if (Time.time < this.PreAttackStateEndTime) return;

        this.EnemyInputHandler?.RequestNormalAttack(this.AttackConfig.NormalAttackSide);

        this.NextAllowedAttackTime = Time.time + this.AttackConfig.AttackDecisionCooldown;
        this.AttackingStateEndTime = Time.time + this.AttackConfig.AttackingStateDuration;

        this.SetAIState(TerritorialEnemyState.Attacking);
    }
    private void HandleAttackingState()
    {
        this.EnemyInputHandler?.ClearMoveInput();

        Transform targetTransform = this.CurrentTarget?.transform;
        if (targetTransform != null)
        {
            this.FaceTarget(targetTransform);
        }

        if (Time.time < this.AttackingStateEndTime) return;

        if (this.CurrentAttackRangeTarget != null)
        {
            this.SetAIState(TerritorialEnemyState.ReadyToAttack);
            return;
        }

        if (this.CurrentAlertTarget != null)
        {
            this.SetAIState(TerritorialEnemyState.Alert);
            return;
        }

        this.SetAIState(TerritorialEnemyState.Roaming);
    }
    private void HandleDeadState()
    {
        this.EnemyInputHandler?.ClearMoveInput();
    }

    //  2. Roaming Helpers
    private void HandleRandomRoaming(Vector2 currentPosition)
    {
        if (this.RoamingConfig == null)
        {
            this.EnemyInputHandler?.ClearMoveInput();

            Debug.LogWarning($"[{this.CharacterName}.{nameof(TerritorialEnemyBrainController)}] Missing roaming config.", this);
            return;
        }

        if (Time.time >= this.NextRoamPhaseTime)
        {
            this.BeginNextRoamPhase(currentPosition);
        }

        if (!this.IsRoamWalking)
        {
            this.EnemyInputHandler?.ClearMoveInput();
            return;
        }

        Vector2 projectedPosition = currentPosition + this.RoamMoveDirection * 0.25f;

        if (!this.HomeAreaEntry.Contains(projectedPosition))
        {
            Vector2 randomPoint = this.HomeAreaEntry.GetRandomPointInside();
            this.RoamMoveDirection = (randomPoint - currentPosition).normalized;
        }

        this.EnemyInputHandler?.SetMoveInput(this.RoamMoveDirection);
    }
    private void BeginNextRoamPhase(Vector2 currentPosition)
    {
        this.IsRoamWalking = !this.IsRoamWalking;

        if (this.IsRoamWalking)
        {
            Vector2 randomPoint = this.HomeAreaEntry.GetRandomPointInside();
            Vector2 direction = randomPoint - currentPosition;

            if (direction.sqrMagnitude <= 0.01f)
            {
                direction = Random.insideUnitCircle.normalized;
            }

            this.RoamMoveDirection = direction.normalized;
            this.NextRoamPhaseTime = Time.time + Random.Range(this.RoamingConfig.MinRoamWalkDuration, this.RoamingConfig.MaxRoamWalkDuration);
        }
        else
        {
            this.RoamMoveDirection = Vector2.zero;
            this.NextRoamPhaseTime = Time.time + Random.Range(this.RoamingConfig.MinRoamIdleDuration, this.RoamingConfig.MaxRoamIdleDuration);
        }
    }
    private void HandleReturnToHomeArea(Vector2 currentPosition)
    {
        if (!this.HasHomeReturnPoint)
        {
            this.HomeReturnPoint = this.HomeAreaEntry.GetRandomPointInside();
            this.HasHomeReturnPoint = true;
        }

        Vector2 toReturnPoint = this.HomeReturnPoint - currentPosition;

        if (toReturnPoint.magnitude <= this.HomeAreaEntry.ReturnReachDistance)
        {
            this.HasHomeReturnPoint = false;
            this.EnemyInputHandler?.ClearMoveInput();
            this.BeginIdleRoamPhase();
            return;
        }

        Vector2 moveDirection = toReturnPoint.normalized;
        this.EnemyInputHandler?.SetMoveInput(moveDirection);
    }
    private void BeginIdleRoamPhase()
    {
        this.IsRoamWalking = false;
        this.RoamMoveDirection = Vector2.zero;
        this.NextRoamPhaseTime = Time.time + this.RoamingConfig.GetRandomIdleDuration();
    }
    private void ResetRoamingRuntime()
    {
        this.IsRoamWalking = false;
        this.RoamMoveDirection = Vector2.zero;
        this.NextRoamPhaseTime = Time.time + this.RoamingConfig.GetRandomIdleDuration();

        this.HasHomeReturnPoint = false;
        this.HomeReturnPoint = Vector2.zero;
    }
    private void ResetAttackRuntime()
    {
        this.NextAllowedAttackTime = 0f;
        this.PreAttackStateEndTime = 0f;
        this.AttackingStateEndTime = 0f;
    }

    //  3. Home Area Helpers
    private bool HasValidHomeArea()
    {
        return this.HomeAreaEntry.HasValidArea();
    }

    //  4. Facing Helpers
    private void FaceTarget(Transform targetTransform)
    {
        if (targetTransform == null) return;

        Vector2 direction = targetTransform.position - this.transform.position;
        this.FaceDirection(direction);
    }
    private void FaceDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0.001f) return;

        this.VisualController?.ForceFacing(direction);
    }

    //  5. Hooks
    protected override void OnBrainDeathDisabled(CharacterDeathContext context)
    {
        this.SetAIState(TerritorialEnemyState.Dead);

        this.EnemyInputHandler?.ClearMoveInput();

        this.ResetRoamingRuntime();
        this.ResetAttackRuntime();
    }
    protected override void OnBrainRefreshed(CharacterRefreshContext context)
    {
        this.ResetRoamingRuntime();
        this.ResetAttackRuntime();

        this.SetAIState(TerritorialEnemyState.Roaming);

        this.EnemyInputHandler?.ClearMoveInput();

        this.CombatLoadoutController?.TryQuickEquipWeapon();
    }

    //  6. Other Initializations 
    private void InitializeBrainRuntimeAfterBinding()
    {
        this.CombatLoadoutController?.TryQuickEquipWeapon();
    }

    //  7. FOTL Base Behavior Overrides
    protected override void ValidateInternalReferences()
    {
        base.ValidateInternalReferences();

        if (this.RoamingConfig == null)
        {
            Debug.LogError($"[{this.CharacterName}.{nameof(TerritorialEnemyBrainController)}] Roaming config is not assigned.", this);
        }

        if (this.AttackConfig == null)
        {
            Debug.LogError($"[{this.CharacterName}.{nameof(TerritorialEnemyBrainController)}] Attack config is not assigned.", this);
        }
    }
    protected override void InitializeInternalState()
    {
        base.InitializeInternalState();

        this.ResetRoamingRuntime();
        this.ResetAttackRuntime();
        this.SetAIState(TerritorialEnemyState.Roaming);
    }
}
