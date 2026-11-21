namespace FlameOfTheLost
{
    public abstract class PlayerCraftingSubHandlerBase : FOTLSubBehaviour<PlayerCraftingController>
    {
        // --- References ---
        //  1. Brain Behaviour
        protected PlayerCraftingController CraftingController => this.BrainBehaviour;
    }
}
