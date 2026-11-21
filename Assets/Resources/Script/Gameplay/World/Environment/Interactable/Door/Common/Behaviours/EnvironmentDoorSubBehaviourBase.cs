namespace FlameOfTheLost
{
    public abstract class EnvironmentDoorSubBehaviourBase : FOTLSubBehaviour<EnvironmentDoorController>
    {
        // --- References ---
        protected EnvironmentDoorController DoorController => this.BrainBehaviour;
    }
}
