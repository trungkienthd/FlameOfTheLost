using UnityEngine;

namespace FlameOfTheLost
{
    public class NeutralCharacterBrainController : CharacterBehaviourControllerBase,
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- References ---
        protected CharacterVisualController VisualController => this.CharacterRootController.VisualController;
        private CharacterAITargetDetectionHandler TargetDetectionHandler => this.CharacterRootController is NeutralCharacterRootController neutralRoot
            ? neutralRoot.TargetDetectionHandler
            : null;
        protected NeutralCharacterInputHandler NeutralCharacterInputHandler => this.CharacterRootController is NeutralCharacterRootController neutralRoot
            ? neutralRoot.NeutralCharacterInputHandler
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

        //  2. Death Disable State (Runtime)
        [Header("Death Disable (Runtime)")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        // --- Public APIs ---
        //  1. Character Refresh
        public virtual int RefreshmentConfigurePriority => 90;
        public virtual void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;

            this.TargetDetectionHandler?.ClearAllTargets();
            this.NeutralCharacterInputHandler?.ClearMoveInput();

            this.OnBrainRefreshed(context);
        }

        //  2. Character Death Disable
        public virtual int DeathDisablePriority => 25;
        public virtual void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;

            this.TargetDetectionHandler?.ClearAllTargets();
            this.NeutralCharacterInputHandler?.ClearMoveInput();

            this.OnBrainDeathDisabled(context);
        }

        // --- Hooks ---
        protected virtual void OnBrainRefreshed(CharacterRefreshContext context)
        {
        }
        protected virtual void OnBrainDeathDisabled(CharacterDeathContext context)
        {
        }
    }
}
