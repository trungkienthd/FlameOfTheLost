namespace FlameOfTheLost
{
    public class MilestoneProgressController : GameConditionProgressControllerBase
    {
        // --- Public APIs ---
        //  1. Gameplay Reference Lifecycle
        public override bool BindGameplayReferences()
        {
            if (!this.EnsureBound()) return false;

            return true;
        }
        public override void UnbindGameplayReferences()
        {
        }
    }
}
