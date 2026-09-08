namespace FlameOfTheLost
{
    public abstract class HarvestableObjectSubHandlerBase : FOTLSubBehaviour<HarvestableObjectController>
    {
        // --- References ---
        //  1. Brain Behaviour
        protected HarvestableObjectController HarvestableObjectController => this.BrainBehaviour;

        // --- Data / Configs ---
        protected HarvestableObjectProfile Profile => this.HarvestableObjectController?.Profile;
    }
}
