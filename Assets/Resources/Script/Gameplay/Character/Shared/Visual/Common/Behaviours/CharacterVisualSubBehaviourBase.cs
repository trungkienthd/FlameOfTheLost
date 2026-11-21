namespace FlameOfTheLost
{
    public abstract class CharacterVisualSubBehaviourBase : FOTLSubBehaviour<CharacterVisualController>
    {
        protected CharacterVisualController VisualController => this.BrainBehaviour;

        protected CharacterVisualConfig VisualConfig => this.VisualController != null
            ? this.VisualController.VisualConfig
            : null;
    }
}
