namespace FlameOfTheLost
{
    public abstract class MainMenuSubBehaviourBase : FOTLSubBehaviour<MainMenuController>
    {
        // --- References ---
        //  1. Brain Behaviour
        protected MainMenuController MainMenuController
            => this.BrainBehaviour;
    }
}