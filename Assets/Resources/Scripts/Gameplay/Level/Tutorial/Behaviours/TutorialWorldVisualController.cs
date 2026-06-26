using TMPro;
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
        [SerializeField] private GameObject _instructionVisual;
        [SerializeField] private TMP_Text _instructionText;
        private GameObject InstructionVisual => _instructionVisual;
        private TMP_Text InstructionText => _instructionText;

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
            if (this.InstructionVisual == null) return;

            this.InstructionVisual.SetActive(isVisible);

            if (isVisible)
            {
                this.InstructionText.text = this.Description;
            }
        }

        // --- Helpers ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.InstructionVisual == null) _instructionVisual = this.transform.Find("WorldSpaceTutorialVisual")?.gameObject;
            if (this.InstructionText == null)
            {
                if (this.InstructionVisual != null)
                {
                    _instructionText = this.InstructionVisual.transform.Find("InstructionText")?.GetComponent<TMP_Text>();
                }
            }
        }
    }
}
