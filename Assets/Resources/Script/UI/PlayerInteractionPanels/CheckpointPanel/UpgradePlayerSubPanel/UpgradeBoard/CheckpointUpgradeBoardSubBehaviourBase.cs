namespace FlameOfTheLost
{
    public abstract class CheckpointUpgradeBoardSubBehaviourBase : FOTLSubBehaviour<CheckpointUpgradeBoardController>
    {
        protected CheckpointUpgradeBoardController UpgradeBoardController => this.BrainBehaviour;

        protected CheckpointUpgradePlayerSubPanelController UpgradePlayerSubPanelController => this.UpgradeBoardController?.UpgradePlayerSubPanelController;
    }
}