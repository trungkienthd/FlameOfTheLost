namespace FlameOfTheLost
{
    public abstract class KeyItemInteractableSubHandlerBase : FOTLSubBehaviour<KeyItemInteractableController>
    {
        // --- References ---
        //  1. Brain Behaviour
        protected KeyItemInteractableController KeyItemInteractableController => this.BrainBehaviour;

        //  2. Data / Configs
        protected KeyItemInteractableProfile Profile => this.KeyItemInteractableController != null
            ? this.KeyItemInteractableController.Profile
            : null;
    }
}