using TMPro;
using UnityEngine;

namespace FlameOfTheLost
{
    public class TutorialObjectController : FOTLMonoBehaviour
    {
        // --- Data / Configs ---
        [Header("Data / Configs")]

        //  1. Profile
        [SerializeField] private TutorialObjectProfile _profile;
        private TutorialObjectProfile Profile => _profile;

        //  2. Identity
        private string Description => this.Profile != null ? this.Profile.Description : "";

        //  3. Settings
        private bool VisibleOnStart => this.Profile != null ? this.Profile.VisibleOnStart : true;

        // --- References ---
        [Header("References")]
        [SerializeField] private GameObject _instructionVisual;
        [SerializeField] private TMP_Text _instructionText;
        private GameObject InstructionVisual => _instructionVisual;
        private TMP_Text InstructionText => _instructionText;

        // --- Public APIs ---
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

        // --- FOTLMonoBehaviour Overrides ---
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
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Profile == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialObjectController)}] " + $"requires a {nameof(TutorialObjectProfile)}.", this);
            }
            if (this.InstructionVisual == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialObjectController)}] InstructionVisual reference is missing.", this);
            }
            if (this.InstructionText == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialObjectController)}] InstructionText reference is missing.", this);
            }
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.SetVisible(this.VisibleOnStart);
        }
    }
}
