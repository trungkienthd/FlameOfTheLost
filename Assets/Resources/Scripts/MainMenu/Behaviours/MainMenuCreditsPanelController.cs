namespace FlameOfTheLost
{
    public class MainMenuCreditsPanelController : MainMenuSubBehaviourBase
    {
        // --- Public APIs ---
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
