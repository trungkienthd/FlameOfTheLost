namespace FlameOfTheLost
{
    public abstract class GameConditionSubBehaviourBase : FOTLSubBehaviour<GameConditionController>
    {
        // --- References ---
        protected GameConditionController GameConditionController => this.BrainBehaviour;
    }
}
