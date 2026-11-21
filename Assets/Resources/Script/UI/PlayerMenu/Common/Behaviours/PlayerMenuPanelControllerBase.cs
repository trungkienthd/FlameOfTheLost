namespace FlameOfTheLost
{
    public abstract class PlayerMenuPanelControllerBase : FOTLMonoBehaviour
    {
        // --- Runtime ---
        public bool IsPanelShown => this.gameObject.activeSelf;

        // --- Public APIs ---
        public virtual void ShowPanel()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void HidePanel()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void TogglePanel()
        {
            if (this.gameObject.activeSelf)
            {
                this.HidePanel();
            }
            else
            {
                this.ShowPanel();
            }
        }
    }
}
