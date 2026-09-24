using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class EnemyBrainController : CharacterBehaviourControllerBase, 
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- References ---
        protected CharacterVisualController VisualController => this.CharacterRootController.VisualController;
        private CharacterAITargetDetectionHandler TargetDetectionHandler => this.CharacterRootController is EnemyRootController enemyRoot 
            ? enemyRoot.TargetDetectionHandler 
            : null;
        protected EnemyInputHandler EnemyInputHandler => this.CharacterRootController is EnemyRootController enemyRoot 
            ? enemyRoot.EnemyInputHandler 
            : null;

        // --- Runtime ---
        //  1. Current Target Info
        protected CharacterRootController CurrentTarget
        {
            get
            {
                if (this.IsDeathDisabled) return null;

                return this.TargetDetectionHandler != null
                    ? this.TargetDetectionHandler.CurrentTarget
                    : null;
            }
        }
        protected CharacterRootController CurrentAlertTarget
        {
            get
            {
                if (this.IsDeathDisabled) return null;

                return this.TargetDetectionHandler != null
                    ? this.TargetDetectionHandler.CurrentAlertTarget
                    : null;
            }
        }
        protected CharacterRootController CurrentAttackRangeTarget
        {
            get
            {
                if (this.IsDeathDisabled) return null;

                return this.TargetDetectionHandler != null
                    ? this.TargetDetectionHandler.CurrentAttackRangeTarget
                    : null;
            }
        }

        //  2. Death Disable State (Runtime)
        [Header("Death Disable (Runtime)")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        // --- Public APIs ---
        //  1. ICharacterPlacementRefreshable implementation
        public virtual int RefreshmentConfigurePriority => 90;
        public virtual void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;

            this.TargetDetectionHandler?.ClearAllTargets();
            this.EnemyInputHandler?.ClearMoveInput();

            this.OnBrainRefreshed(context);
        }

        //  2. ICharacterDeathDisableable implementation
        public virtual int DeathDisablePriority => 25;
        public virtual void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            _isDeathDisabled = true;

            this.TargetDetectionHandler?.ClearAllTargets();
            this.EnemyInputHandler?.ClearMoveInput();

            this.OnBrainDeathDisabled(context);
        }

        // --- Helpers ---
        protected virtual void OnBrainDeathDisabled(CharacterDeathContext context)
        {
        }
        protected virtual void OnBrainRefreshed(CharacterRefreshContext context)
        {
        }
    }
}
