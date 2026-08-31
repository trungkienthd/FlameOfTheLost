namespace FlameOfTheLost
{
    public abstract class LoadoutPanelSubBehaviourBase : FOTLSubBehaviour<LoadoutPanelController>
    {
        protected LoadoutPanelController LoadoutPanelController => this.BrainBehaviour;
    }
}
