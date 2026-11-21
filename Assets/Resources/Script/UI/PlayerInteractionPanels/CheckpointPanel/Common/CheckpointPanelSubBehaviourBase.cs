namespace FlameOfTheLost
{
    public abstract class CheckpointPanelSubBehaviourBase : FOTLSubBehaviour<CheckpointPanelController>
    {
        // --- References ---
        //  1. Brain Behaviour
        protected CheckpointPanelController CheckpointPanelController => this.BrainBehaviour;
    }
}