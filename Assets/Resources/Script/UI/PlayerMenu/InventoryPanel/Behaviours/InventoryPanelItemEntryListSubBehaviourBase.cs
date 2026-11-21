namespace FlameOfTheLost
{
    public abstract class InventoryPanelItemEntryListSubBehaviourBase : FOTLSubBehaviour<InventoryPanelItemEntryListSubPanelController>
    {
        protected InventoryPanelItemEntryListSubPanelController ItemEntryListController => this.BrainBehaviour;
    }
}