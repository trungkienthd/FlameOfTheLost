using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class EnemyBrainController : CharacterBehaviourBaseController, ICharacterRespawnRefreshable
    {
        // --- State ---
        //  1. Runtime AI State
        [Header("AI State (Runtime)")]
        [SerializeField] private CharacterAIState _currentAIState = CharacterAIState.Roaming;
        public CharacterAIState CurrentAIState => _currentAIState;
       
        public bool IsAlert
        {
            get
            {
                return this.CurrentAIState == CharacterAIState.Alert ||
                       this.CurrentAIState == CharacterAIState.ReadyToAttack ||
                       this.CurrentAIState == CharacterAIState.Attacking ||
                       this.CurrentAIState == CharacterAIState.Recovering;
            }
        }

        public bool IsDead => this.CurrentAIState == CharacterAIState.Dead;

        //  2. References
        protected CharacterVisualController VisualController => this.CharacterRootController.VisualController;
        private CharacterAITargetDetectionHandler TargetDetectionHandler => this.CharacterRootController is EnemyRootController enemyRoot 
            ? enemyRoot.TargetDetectionHandler 
            : null;
        protected EnemyInputHandler EnemyInputHandler => this.CharacterRootController is EnemyRootController enemyRoot 
            ? enemyRoot.EnemyInputHandler 
            : null;

        //  3. Current Target Info
        protected CharacterRootController CurrentTarget => this.TargetDetectionHandler != null
            ? this.TargetDetectionHandler.CurrentTarget
            : null;
        protected CharacterRootController CurrentAlertTarget => this.TargetDetectionHandler != null
            ? this.TargetDetectionHandler.CurrentAlertTarget
            : null;
        protected CharacterRootController CurrentAttackRangeTarget => this.TargetDetectionHandler != null
            ? this.TargetDetectionHandler.CurrentAttackRangeTarget
            : null;

        // --- Unity Callbacks ---
        protected virtual void Update()
        {
            this.TickAI();
        }

        // --- Public APIs ---
        //  1. ICharacterRespawnRefreshable implementation
        public virtual int RespawnRefreshPriority => 90;
        public virtual void RefreshAfterRespawn(CharacterRespawnRefreshContext context)
        {
            this.SetState(CharacterAIState.Roaming);

            this.TargetDetectionHandler?.ClearAllTargets();
            this.EnemyInputHandler?.ClearAIMoveInput();
            this.EnemyInputHandler?.UnblockAIInput();
        }

        // --- Helpers ---
        protected virtual void TickAI()
        {
            if (this.IsDead)
            {
                this.HandleDeadState();
                return;
            }

            if (this.TargetDetectionHandler == null)
            {
                this.SetState(CharacterAIState.Roaming);
                this.HandleRoamingState();
                return;
            }

            if (this.TargetDetectionHandler.HasAttackRangeTarget)
            {
                this.SetState(CharacterAIState.ReadyToAttack);
                this.HandleReadyToAttackState();
                return;
            }

            if (this.TargetDetectionHandler.HasAlertTarget)
            {
                this.SetState(CharacterAIState.Alert);
                this.HandleAlertState();
                return;
            }

            this.SetState(CharacterAIState.Roaming);
            this.HandleRoamingState();
        }

        protected virtual void HandleRoamingState()
        {
            this.EnemyInputHandler?.ClearAIMoveInput();
        }

        protected virtual void HandleAlertState()
        {
            this.EnemyInputHandler?.ClearAIMoveInput();
        }

        protected virtual void HandleReadyToAttackState()
        {
            this.EnemyInputHandler?.ClearAIMoveInput();
        }

        protected virtual void HandleAttackingState()
        {
            this.EnemyInputHandler?.ClearAIMoveInput();
        }

        protected virtual void HandleDeadState()
        {
            this.EnemyInputHandler?.BlockAIInput();
        }

        protected void SetState(CharacterAIState nextState)
        {
            if (this.CurrentAIState == nextState) return;

            _currentAIState = nextState;
        }
    }
}
