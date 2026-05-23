using UnityEngine;

namespace FlameOfTheLost
{
    public class TutorialWorldVisualController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _visualRoot;
        private GameObject VisualRoot => _visualRoot != null ? _visualRoot : this.gameObject;

        [Header("Settings")]
        [SerializeField] private bool _visibleOnStart = true;
        private bool VisibleOnStart => _visibleOnStart;

        private void Awake()
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
