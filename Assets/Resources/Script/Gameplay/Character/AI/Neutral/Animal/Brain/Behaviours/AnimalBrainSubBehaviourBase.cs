namespace FlameOfTheLost
{
    public abstract class AnimalBrainSubBehaviourBase : FOTLSubBehaviour<AnimalBrainController>
    {
        protected AnimalBrainController AnimalBrainController => this.BrainBehaviour;

        protected AnimalRootController AnimalRootController => this.AnimalBrainController != null
                ? this.AnimalBrainController.AnimalRootController
                : null;

        protected NeutralCharacterInputHandler NeutralCharacterInputHandler => this.AnimalBrainController != null
                ? this.AnimalBrainController.NeutralCharacterInputHandler
                : null;
    }
}