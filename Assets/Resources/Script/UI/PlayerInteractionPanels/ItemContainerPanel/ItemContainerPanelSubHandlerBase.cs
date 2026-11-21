namespace FlameOfTheLost
{
    public abstract class ItemContainerPanelSubHandlerBase : FOTLSubBehaviour<ItemContainerPanelController>
    {
        // --- References ---
        //  1. Brain Behaviour
        protected ItemContainerPanelController ItemContainerPanelController => this.BrainBehaviour;
    }
}