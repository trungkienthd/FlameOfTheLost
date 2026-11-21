namespace FlameOfTheLost
{
    public abstract class PlayerInteractionSubHandlerBase : FOTLSubBehaviour<PlayerInteractionHandler>
    {
        // --- References ---
        //  1. Brain Behaviour
        protected PlayerInteractionHandler InteractionHandler => this.BrainBehaviour;
    }
}
