namespace FlameOfTheLost
{
    public class CheckpointUpgradePlayerSubPanelController : CheckpointPanelSubBehaviourBase
    {
        // --- Public APIs ---
        //  1. Visibility
        public void Show()
        {
            this.gameObject.SetActive(true);
        }
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}