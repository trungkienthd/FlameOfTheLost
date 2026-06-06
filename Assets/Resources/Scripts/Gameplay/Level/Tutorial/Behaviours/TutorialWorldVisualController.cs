using UnityEngine;

namespace FlameOfTheLost
{
    public class TutorialWorldVisualController : FOTLMonoBehaviour, IIdentityProvider
    {
        [Header("Identity")]
        [SerializeField] private int _id = 4000;
        [SerializeField] private string _key = "TUTORIAL_WORLD_VISUAL";
        [SerializeField] private string _displayName = "Tutorial World Visual";
        [SerializeField, TextArea(5, 5)] private string _description = "Tutorial world visual.";
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        [Header("References")]
        [SerializeField] private GameObject _visualRoot;
        private GameObject VisualRoot => _visualRoot != null ? _visualRoot : this.gameObject;

        [Header("Settings")]
        [SerializeField] private bool _visibleOnStart = true;
        private bool VisibleOnStart => _visibleOnStart;

        private void Start()
        {
            this.SetVisible(this.VisibleOnStart);
        }

        public void Show()
        {
            this.SetVisible(true);
        }

        public void Hide()
        {
            this.SetVisible(false);
        }

        public void SetVisible(bool isVisible)
        {
            if (this.VisualRoot == null) return;

            this.VisualRoot.SetActive(isVisible);
        }
    }
}
