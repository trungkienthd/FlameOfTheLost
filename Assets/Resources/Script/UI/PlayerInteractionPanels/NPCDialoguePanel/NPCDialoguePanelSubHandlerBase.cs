namespace FlameOfTheLost
{
    public abstract class NPCDialoguePanelSubHandlerBase : FOTLSubBehaviour<NPCDialoguePanelController>
    {
        // --- References ---
        //  1. Brain Behaviour
        protected NPCDialoguePanelController PanelController => this.BrainBehaviour;
    }
}