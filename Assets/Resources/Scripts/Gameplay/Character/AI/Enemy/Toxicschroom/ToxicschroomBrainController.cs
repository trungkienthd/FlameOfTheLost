using FlameOfTheLost;
using UnityEngine;

public class ToxicschroomBrainController : EnemyBrainController
{
    // --- States ---
    //  1. Home Area
    [Header("Home Area")]
    [SerializeField] private Vector2 _homeAreaBottomLeftPos;
    [SerializeField] private Vector2 _homeAreaTopRightPos;
    [SerializeField] private float _homeReturnReachDistance = 0.2f;
    private Vector2 HomeAreaBottomLeftPos => _homeAreaBottomLeftPos;
    private Vector2 HomeAreaTopRightPos => _homeAreaTopRightPos;
    private float HomeReturnReachDistance => _homeReturnReachDistance;

    //  2. Roaming Config
    [Header("Roaming")]
    [SerializeField] private float _minRoamWalkDuration = 1.2f;
    [SerializeField] private float _maxRoamWalkDuration = 2.5f;
    [SerializeField] private float _minRoamIdleDuration = 0.8f;
    [SerializeField] private float _maxRoamIdleDuration = 1.8f;

    private float MinRoamWalkDuration => _minRoamWalkDuration;
    private float MaxRoamWalkDuration => _maxRoamWalkDuration;
    private float MinRoamIdleDuration => _minRoamIdleDuration;
    private float MaxRoamIdleDuration => _maxRoamIdleDuration;

    //  3. Attack Config
    [Header("Attack")]
    [SerializeField] private float _idealAttackRange = 1.4f;
    [SerializeField] private float _attackDecisionCooldown = 1.5f;
    [SerializeField] private float _attackingStateDuration = 0.7f;
    [SerializeField] private AttackSide _normalAttackSide = AttackSide.Left;
    private float IdealAttackRange => _idealAttackRange;
    private float AttackDecisionCooldown => _attackDecisionCooldown;
    private float AttackingStateDuration => _attackingStateDuration;
    private AttackSide NormalAttackSide => _normalAttackSide;

    //  4. Runtime Roaming State
    [Header("Roaming Runtime")]
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

    //  5. Runtime Attack State
    [Header("Attack Runtime")]
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

    //  6. References 
    private HPController HPController => this.CharacterRootController.HPController;
    private CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController.CombatLoadoutController;

    // --- Unity Callbacks ---
    private void Start()
    {
        this.SubscribeToHPDeathEvent();
        this.ResetRoamingRuntime();

        this.CombatLoadoutController?.TryQuickEquipWeapon();   
    }
    protected virtual void OnDestroy()
    {
        this.UnsubscribeFromHPDeathEvent();
    }

    // --- Public APIs ---
    //  1. ICharacterRespawnRefreshable implementation
    public override void RefreshAfterRespawn(CharacterRespawnRefreshContext context)
    {
        base.RefreshAfterRespawn(context);

        this.ResetRoamingRuntime();
        this.ResetAttackRuntime();

        this.SetState(CharacterAIState.Roaming);

        this.EnemyInputHandler?.UnblockAIInput();
        this.EnemyInputHandler?.ClearAIMoveInput();

        this.SubscribeToHPDeathEvent();
    }

    // --- Helpers ---
    protected override void TickAI()
    {
        if (this.CurrentAIState == CharacterAIState.Dead)
        {
            this.HandleDeadState();
            return;
        }

        if (this.CurrentAIState == CharacterAIState.Attacking)
        {
            this.HandleAttackingState();
            return;
        }

        if (this.CurrentAttackRangeTarget != null)
        {
            this.SetState(CharacterAIState.ReadyToAttack);
            this.HandleReadyToAttackState();
            return;
        }

        if (this.CurrentAlertTarget != null)
        {
            this.SetState(CharacterAIState.Alert);
            this.HandleAlertState();
            return;
        }

        this.SetState(CharacterAIState.Roaming);
        this.HandleRoamingState();
    }

    protected override void HandleRoamingState()
    {
        if (!this.HasValidHomeArea())
        {
            this.EnemyInputHandler?.ClearAIMoveInput();
            return;
        }

        Vector2 currentPosition = this.transform.position;

        if (!this.IsInsideHomeArea(currentPosition) || this.HasHomeReturnPoint)
        {
            this.HandleReturnToHomeArea(currentPosition);
            return;
        }

        this.HandleRandomRoaming(currentPosition);
    }

    protected override void HandleAlertState()
    {
        this.EnemyInputHandler?.ClearAIMoveInput();

        Transform targetTransform = this.CurrentTarget.transform;
        if (targetTransform == null) return;

        this.FaceTarget(targetTransform);
    }

    protected override void HandleReadyToAttackState()
    {
        Transform targetTransform = this.CurrentTarget.transform;
        if (targetTransform == null)
        {
            this.EnemyInputHandler?.ClearAIMoveInput();
            this.SetState(CharacterAIState.Roaming);
            return;
        }

        Vector2 currentPosition = this.transform.position;
        Vector2 targetPosition = targetTransform.position;
        Vector2 toTarget = targetPosition - currentPosition;

        this.FaceDirection(toTarget);

        float distanceToTarget = toTarget.magnitude;

        if (distanceToTarget > this.IdealAttackRange)
        {
            Vector2 moveDirection = toTarget.normalized;
            this.EnemyInputHandler?.SetAIMoveInput(moveDirection);
            return;
        }

        this.EnemyInputHandler?.ClearAIMoveInput();

        if (Time.time < this.NextAllowedAttackTime) return;

        this.EnemyInputHandler?.RequestAINormalAttack(this.NormalAttackSide);

        this.NextAllowedAttackTime = Time.time + this.AttackDecisionCooldown;
        this.AttackingStateEndTime = Time.time + this.AttackingStateDuration;

        this.SetState(CharacterAIState.Attacking);
    }

    protected override void HandleAttackingState()
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
            this.SetState(CharacterAIState.ReadyToAttack);
            return;
        }

        if (this.CurrentAlertTarget != null)
        {
            this.SetState(CharacterAIState.Alert);
            return;
        }

        this.SetState(CharacterAIState.Roaming);
    }

    protected override void HandleDeadState()
    {
        this.EnemyInputHandler?.BlockAIInput();
        this.EnemyInputHandler?.ClearAIMoveInput();
    }

    //  2. Roaming Helpers
    private void HandleRandomRoaming(Vector2 currentPosition)
    {
        if (Time.time >= _nextRoamPhaseTime)
        {
            this.BeginNextRoamPhase(currentPosition);
        }

        if (!this.IsRoamWalking)
        {
            this.EnemyInputHandler?.ClearAIMoveInput();
            return;
        }

        Vector2 projectedPosition = currentPosition + this.RoamMoveDirection * 0.25f;

        if (!this.IsInsideHomeArea(projectedPosition))
        {
            Vector2 randomPoint = this.GetRandomPointInsideHomeArea();
            this.RoamMoveDirection = (randomPoint - currentPosition).normalized;
        }

        this.EnemyInputHandler?.SetAIMoveInput(this.RoamMoveDirection);
    }
    private void BeginNextRoamPhase(Vector2 currentPosition)
    {
        this.IsRoamWalking = !this.IsRoamWalking;

        if (this.IsRoamWalking)
        {
            Vector2 randomPoint = this.GetRandomPointInsideHomeArea();
            Vector2 direction = randomPoint - currentPosition;

            if (direction.sqrMagnitude <= 0.01f)
            {
                direction = Random.insideUnitCircle.normalized;
            }

            this.RoamMoveDirection = direction.normalized;
            this.NextRoamPhaseTime = Time.time + Random.Range(this.MinRoamWalkDuration, this.MaxRoamWalkDuration);
        }
        else
        {
            this.RoamMoveDirection = Vector2.zero;
            this.NextRoamPhaseTime = Time.time + Random.Range(this.MinRoamIdleDuration, this.MaxRoamIdleDuration);
        }
    }
    private void HandleReturnToHomeArea(Vector2 currentPosition)
    {
        if (!this.HasHomeReturnPoint)
        {
            this.HomeReturnPoint = this.GetRandomPointInsideHomeArea();
            this.HasHomeReturnPoint = true;
        }

        Vector2 toReturnPoint = this.HomeReturnPoint - currentPosition;

        if (toReturnPoint.magnitude <= this.HomeReturnReachDistance)
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
        this.NextRoamPhaseTime = Time.time + Random.Range(this.MinRoamIdleDuration, this.MaxRoamIdleDuration);
    }
    private void ResetRoamingRuntime()
    {
        this.IsRoamWalking = false;
        this.RoamMoveDirection = Vector2.zero;
        this.NextRoamPhaseTime = Time.time + Random.Range(this.MinRoamIdleDuration, this.MaxRoamIdleDuration);

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
        return this.HomeAreaBottomLeftPos != null && this.HomeAreaTopRightPos != null;
    }
    private bool IsInsideHomeArea(Vector2 position)
    {
        Vector2 min = this.GetHomeAreaMin();
        Vector2 max = this.GetHomeAreaMax();

        return position.x >= min.x &&
               position.x <= max.x &&
               position.y >= min.y &&
               position.y <= max.y;
    }
    private Vector2 GetRandomPointInsideHomeArea()
    {
        Vector2 min = this.GetHomeAreaMin();
        Vector2 max = this.GetHomeAreaMax();

        return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }
    private Vector2 GetHomeAreaMin()
    {
        Vector2 a = this.HomeAreaBottomLeftPos;
        Vector2 b = this.HomeAreaTopRightPos;

        return new Vector2( Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
    }

    private Vector2 GetHomeAreaMax()
    {
        Vector2 a = this.HomeAreaBottomLeftPos;
        Vector2 b = this.HomeAreaTopRightPos;

        return new Vector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
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
        this.SetState(CharacterAIState.Dead);
        this.EnemyInputHandler?.BlockAIInput();
        this.EnemyInputHandler?.ClearAIMoveInput();

        this.ResetRoamingRuntime();
        this.ResetAttackRuntime();
    }
}
