namespace FlameOfTheLost
{
    public abstract class GameConditionProgressControllerBase : GameConditionSubBehaviourBase
    {
        // --- Public APIs ---
        public abstract bool BindGameplayReferences();
        public abstract void UnbindGameplayReferences();

        // --- FOTLSubBehaviour Overrides ---
        protected override void OnUnbinding(GameConditionController brainBehaviour)
        {
            this.UnbindGameplayReferences();

            base.OnUnbinding(brainBehaviour);
        }
    }
}