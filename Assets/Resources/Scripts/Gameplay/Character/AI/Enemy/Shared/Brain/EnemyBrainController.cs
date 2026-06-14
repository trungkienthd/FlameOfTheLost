namespace FlameOfTheLost
{
    public abstract class EnemyBrainController : CharacterBehaviourControllerBase, ICharacterRefreshable
    {
        // --- State ---
        //  1. References
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

        // --- Public APIs ---
        //  1. ICharacterPlacementRefreshable implementation
        public virtual int RefreshmentConfigurePriority => 90;
        public virtual void RefreshCharacter(CharacterPlacementContext context)
        {
            this.TargetDetectionHandler?.ClearAllTargets();
        }
    }
}
