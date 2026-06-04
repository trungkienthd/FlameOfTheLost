using FlameOfTheLost;
using UnityEngine;

public class TerritorialEnemyBrainController : EnemyBrainController
{
    // --- States ---
    //  1. Data
    [Header("Territorial Config Data")]
    [SerializeField] private TerritorialEnemyRoamingConfig _roamingConfig;
    [SerializeField] private TerritorialEnemyAttackConfig _attackConfig;
    private TerritorialEnemyRoamingConfig RoamingConfig => _roamingConfig;
    private TerritorialEnemyAttackConfig AttackConfig => _attackConfig;

    //  1. Runtime AI State
    [Header("AI State (Runtime)")]
    [SerializeField] private TerritorialEnemyState _currentAIState = TerritorialEnemyState.Roaming;
    public TerritorialEnemyState CurrentState => _currentAIState;

    //  2. Home Area
    [Header("Home Area (Runtime)")]
    [SerializeField] private TerritorialEnemyHomeAreaEntry _homeAreaEntry;
    private TerritorialEnemyHomeAreaEntry HomeAreaEntry
    {
        get => _homeAreaEntry;
        set => _homeAreaEntry = value;
    }

    //  3. Runtime Roaming State
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

    //  4. Runtime Attack State
    [Header("Attack (Runtime)")]
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

    //  5. References 
    private HPController HPController => this.CharacterRootController.HPController;
    private CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController.CombatLoadoutController;

    // --- Unity Callbacks ---
    private void Start()
    {
        this.SubscribeToHPDeathEvent();
        this.ResetRoamingRuntime();

        this.CombatLoadoutController?.TryQuickEquipWeapon();   
    }
    protected virtual void Update()
    {
        this.TickAI();
    }
    protected virtual void OnDestroy()
    {
        this.UnsubscribeFromHPDeathEvent();
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

    //  1. ICharacterPlacementRefreshable implementation
    public override void RefreshAfterPlace(CharacterPlacementContext context)
    {
        base.RefreshAfterPlace(context);

        this.ResetRoamingRuntime();
        this.ResetAttackRuntime();

        this.SetState(TerritorialEnemyState.Roaming);

        this.EnemyInputHandler?.UnblockAIInput();
        this.EnemyInputHandler?.ClearAIMoveInput();

        this.SubscribeToHPDeathEvent();

        this.CombatLoadoutController?.TryQuickEquipWeapon();
    }

    // --- Public APIs ---
    //  1. ICharacterPlacementRefreshable implementation

    // --- Helpers ---
    private void SetState(TerritorialEnemyState nextState)
    {
        if (this.CurrentState == nextState) return;

        _currentAIState = nextState;
    }
    private void TickAI()
    {
        if (this.CurrentState == TerritorialEnemyState.Dead)
        {
            this.HandleDeadState();
            return;
        }

        if (this.CurrentState == TerritorialEnemyState.Attacking)
        {
            this.HandleAttackingState();
            return;
        }

        if (this.CurrentAttackRangeTarget != null)
        {
            this.SetState(TerritorialEnemyState.ReadyToAttack);
            this.HandleReadyToAttackState();
            return;
        }

        if (this.CurrentAlertTarget != null)
        {
            this.SetState(TerritorialEnemyState.Alert);
            this.HandleAlertState();
            return;
        }

        this.SetState(TerritorialEnemyState.Roaming);
        this.HandleRoamingState();
    }

    private void HandleRoamingState()
    {
        if (!this.HasValidHomeArea())
        {
            this.EnemyInputHandler?.ClearAIMoveInput();
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
        this.EnemyInputHandler?.ClearAIMoveInput();

        Transform targetTransform = this.CurrentTarget.transform;
        if (targetTransform == null) return;

        this.FaceTarget(targetTransform);
    }
    private void HandleReadyToAttackState()
    {
        Transform targetTransform = this.CurrentTarget.transform;
        if (targetTransform == null)
        {
            this.EnemyInputHandler?.ClearAIMoveInput();
            this.SetState(TerritorialEnemyState.Roaming);
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
            this.EnemyInputHandler?.SetAIMoveInput(moveDirection);
            return;
        }

        this.EnemyInputHandler?.ClearAIMoveInput();

        if (Time.time < this.NextAllowedAttackTime) return;

        this.EnemyInputHandler?.RequestAINormalAttack(this.AttackConfig.NormalAttackSide);

        this.NextAllowedAttackTime = Time.time + this.AttackConfig.AttackDecisionCooldown;
        this.AttackingStateEndTime = Time.time + this.AttackConfig.AttackingStateDuration;

        this.SetState(TerritorialEnemyState.Attacking);
    }
    private void HandleAttackingState()
    {
        this.EnemyInputHandler?.ClearAIMoveInput();

        Transform targetTransform = this.CurrentTarget.transform;
        if (targetTransform != null)
        {
            this.FaceTarget(targetTransform);
        }

        if (Time.time < this.AttackingStateEndTime) return;

        if (this.CurrentAttackRangeTarget != null)
        {
            this.SetState(TerritorialEnemyState.ReadyToAttack);
            return;
        }

        if (this.CurrentAlertTarget != null)
        {
            this.SetState(TerritorialEnemyState.Alert);
            return;
        }

        this.SetState(TerritorialEnemyState.Roaming);
    }
    private void HandleDeadState()
    {
        this.EnemyInputHandler?.BlockAIInput();
        this.EnemyInputHandler?.ClearAIMoveInput();
    }

    //  2. Roaming Helpers
    private void HandleRandomRoaming(Vector2 currentPosition)
    {
        if (this.RoamingConfig == null)
        {
            this.EnemyInputHandler?.ClearAIMoveInput();

            Debug.LogWarning($"[{this.CharacterName}.{nameof(TerritorialEnemyBrainController)}] Missing roaming config.", this);
            return;
        }

        if (Time.time >= this.NextRoamPhaseTime)
        {
            this.BeginNextRoamPhase(currentPosition);
        }

        if (!this.IsRoamWalking)
        {
            this.EnemyInputHandler?.ClearAIMoveInput();
            return;
        }

        Vector2 projectedPosition = currentPosition + this.RoamMoveDirection * 0.25f;

        if (!this.HomeAreaEntry.Contains(projectedPosition))
        {
            Vector2 randomPoint = this.HomeAreaEntry.GetRandomPointInside();
            this.RoamMoveDirection = (randomPoint - currentPosition).normalized;
        }

        this.EnemyInputHandler?.SetAIMoveInput(this.RoamMoveDirection);
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
            this.EnemyInputHandler?.ClearAIMoveInput();
            this.BeginIdleRoamPhase();
            return;
        }

        Vector2 moveDirection = toReturnPoint.normalized;
        this.EnemyInputHandler?.SetAIMoveInput(moveDirection);
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

    //  5. Death Helpers 
    private void SubscribeToHPDeathEvent()
    {
        if (this.HPController == null) return;

        this.HPController.OnDeath -= this.HandleDeath;
        this.HPController.OnDeath += this.HandleDeath;
    }
    private void UnsubscribeFromHPDeathEvent()
    {
        if (this.HPController == null) return;

        this.HPController.OnDeath -= this.HandleDeath;
    }
    private void HandleDeath()
    {
        this.SetState(TerritorialEnemyState.Dead);
        this.EnemyInputHandler?.BlockAIInput();
        this.EnemyInputHandler?.ClearAIMoveInput();

        this.ResetRoamingRuntime();
        this.ResetAttackRuntime();
    }

    //  6. Cache & Validate References
    protected override void ValidateReferences()
    {
        base.ValidateReferences();

        if (this.RoamingConfig == null)
        {
            Debug.LogError($"[{this.CharacterName}.{nameof(TerritorialEnemyBrainController)}] Roaming config is not assigned.", this);
        }

        if (this.AttackConfig == null)
        {
            Debug.LogError($"[{this.CharacterName}.{nameof(TerritorialEnemyBrainController)}] Attack config is not assigned.", this);
        }
    }
}
