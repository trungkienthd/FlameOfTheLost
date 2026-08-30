namespace FlameOfTheLost
{
    public abstract class InventoryPanelSubBehaviourBase : FOTLSubBehaviour<InventoryPanelController>
    {
        protected InventoryPanelController PanelController => this.BrainBehaviour;
    }
}