namespace FlameOfTheLost
{
    public abstract class SceneTransitionPointSubBehaviourBase : FOTLSubBehaviour<SceneTransitionPointController>
    {
        // --- References ---
        protected SceneTransitionPointController SceneTransitionPoint => this.BrainBehaviour;
    }
}