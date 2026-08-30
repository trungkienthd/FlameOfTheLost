namespace FlameOfTheLost
{
    public abstract class PlayerMenuPanelController : FOTLMonoBehaviour
    {
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
