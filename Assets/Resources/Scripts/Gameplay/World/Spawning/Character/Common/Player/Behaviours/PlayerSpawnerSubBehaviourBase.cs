namespace FlameOfTheLost
{
    public abstract class PlayerSpawnerSubBehaviourBase : FOTLSubBehaviour<PlayerSpawnerController>
    {
        protected PlayerSpawnerController PlayerSpawner => this.BrainBehaviour;
    }
}