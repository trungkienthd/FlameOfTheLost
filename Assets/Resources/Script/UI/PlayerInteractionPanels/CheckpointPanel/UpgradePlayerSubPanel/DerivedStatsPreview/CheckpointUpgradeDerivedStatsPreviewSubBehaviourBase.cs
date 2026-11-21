namespace FlameOfTheLost
{
    namespace FlameOfTheLost
    {
        public abstract class CheckpointUpgradeDerivedStatsPreviewSubBehaviourBase : FOTLSubBehaviour<CheckpointUpgradeDerivedStatsPreviewController>
        {
            protected CheckpointUpgradeDerivedStatsPreviewController DerivedStatsPreviewController => this.BrainBehaviour;

            protected CheckpointUpgradePlayerSubPanelController UpgradePlayerSubPanelController => this.DerivedStatsPreviewController?.UpgradePlayerSubPanelController;
        }
    }
}
