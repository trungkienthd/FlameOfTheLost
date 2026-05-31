using UnityEngine;

namespace FlameOfTheLost
{
    public class EnemyBrainController : CharacterBehaviourBaseController, ICharacterRespawnRefreshable
    {
        // --- State ---
        //  1. Runtime AI State
        [Header("AI State (Runtime)")]
        [SerializeField] private CharacterAIState _currentAIState = CharacterAIState.Idle;
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

        //  2. References
        private EnemyTargetDetectionHandler EnemyTargetDetectionHandler
        {
            get
            {
                if (this.CharacterRootController is EnemyRootController enemyRoot)
                {
                    return enemyRoot.EnemyTargetDetectionHandler;
                }
                return null;
            }
        }
        private EnemyAIInputHandler EnemyAIInputHandler
        {
            get
            {
                if (this.CharacterRootController is EnemyRootController enemyRoot)
                {
                    return enemyRoot.EnemyAIInputHandler;
                }
                return null;
            }
        }

        //  3. Current Target Info
        public Transform CurrentTargetTransform => this.EnemyTargetDetectionHandler != null
            ? this.EnemyTargetDetectionHandler.CurrentTargetTransform
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
            this.SetState(CharacterAIState.Idle);

            this.EnemyTargetDetectionHandler?.ClearAllTargets();
            this.EnemyAIInputHandler?.ClearAIMoveInput();
            this.EnemyAIInputHandler?.UnblockAIInput();
        }

        // --- Helpers ---
        protected virtual void TickAI()
        {
            if (this.EnemyTargetDetectionHandler == null)
            {
                this.SetState(CharacterAIState.Idle);
                return;
            }

            if (this.EnemyTargetDetectionHandler.HasAttackRangeTarget)
            {
                this.SetState(CharacterAIState.ReadyToAttack);
                this.HandleReadyToAttackState();
                return;
            }

            if (this.EnemyTargetDetectionHandler.HasAlertTarget)
            {
                this.SetState(CharacterAIState.Alert);
                this.HandleAlertState();
                return;
            }

            this.SetState(CharacterAIState.Idle);
            this.HandleIdleState();
        }

        protected virtual void HandleIdleState()
        {
            this.EnemyAIInputHandler?.ClearAIMoveInput();
        }

        protected virtual void HandleAlertState()
        {
            this.EnemyAIInputHandler?.ClearAIMoveInput();
        }

        protected virtual void HandleReadyToAttackState()
        {
            this.EnemyAIInputHandler?.ClearAIMoveInput();
        }

        protected void SetState(CharacterAIState nextState)
        {
            if (this.CurrentAIState == nextState) return;

            _currentAIState = nextState;
        }
    }
}
