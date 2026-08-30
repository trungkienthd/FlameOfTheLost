namespace FlameOfTheLost
{
    public abstract class CraftingPanelSubHandlerBase : FOTLSubBehaviour<CraftingPanelController>
    {
        // --- References ---
        //  1. Brain Behaviour
        protected CraftingPanelController CraftingPanelController => this.BrainBehaviour;
    }
}
