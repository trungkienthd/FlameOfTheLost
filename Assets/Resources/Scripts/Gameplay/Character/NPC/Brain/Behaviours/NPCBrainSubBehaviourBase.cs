namespace FlameOfTheLost
{
    public class NPCBrainSubBehaviourBase : FOTLSubBehaviour<NPCBrainController>
    {
        // --- References ---
        protected NPCBrainController NPCBrainController => this.BrainBehaviour;

        protected NPCRootController NPCRootController => this.NPCBrainController?.NPCRootController;

        protected NPCProfile NPCProfile => this.NPCRootController?.NPCProfile;
    }
}
