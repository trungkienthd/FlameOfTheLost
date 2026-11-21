namespace FlameOfTheLost
{
    public abstract class MapTransitionPointSubBehaviourBase : FOTLSubBehaviour<MapTransitionPointController>
    {
        // --- References ---
        protected MapTransitionPointController SceneTransitionPoint => this.BrainBehaviour;
    }
}