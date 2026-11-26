using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class NPCDialoguePanelLayoutHandler
        : NPCDialoguePanelSubHandlerBase
    {
        // --- References ---
        //  1. Layout
        [Header("References - Layout")]

        [SerializeField] private RectTransform _dialogueWindow;
        [SerializeField] private RectTransform _speakerNameDisplay;
        [SerializeField] private RectTransform _choiceList;
        [SerializeField] private Image _dividerImage;
        private RectTransform DialogueWindow => _dialogueWindow;
        private RectTransform SpeakerNameDisplay => _speakerNameDisplay;
        public RectTransform ChoiceList => _choiceList;
        private Image DividerImage => _dividerImage;

        // --- Runtime ---
        public bool HasRequiredReferences =>
            this.DialogueWindow != null &&
            this.SpeakerNameDisplay != null &&
            this.ChoiceList != null &&
            this.DividerImage != null;

        // --- Public APIs ---
        //  1. Choice Presentation
        public void SetChoicesVisible(bool isVisible)
        {
            if (this.DividerImage != null)
            {
                this.DividerImage.enabled = isVisible;

                this.DividerImage.gameObject.SetActive(isVisible);
            }

            if (this.ChoiceList != null)
            {
                this.ChoiceList.gameObject.SetActive(isVisible);
            }
        }

        //  2. Layout
        public void RefreshLayout()
        {
            if (!this.gameObject.activeInHierarchy) return;

            Canvas.ForceUpdateCanvases();

            if (this.SpeakerNameDisplay != null && this.SpeakerNameDisplay.gameObject.activeInHierarchy)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(this.SpeakerNameDisplay);
            }

            if (this.DialogueWindow != null && this.DialogueWindow.gameObject.activeInHierarchy)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(this.DialogueWindow);
            }
        }

        public void ResetLayout()
        {
            this.SetChoicesVisible(false);

            this.RefreshLayout();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.DialogueWindow == null)
            {
                Debug.LogError($"[{this.GetType().Name}] Dialogue Window reference is missing.", this);
            }

            if (this.SpeakerNameDisplay == null)
            {
                Debug.LogError($"[{this.GetType().Name}] Speaker Name Display reference is missing.", this);
            }

            if (this.ChoiceList == null)
            {
                Debug.LogError($"[{this.GetType().Name}] Choice List reference is missing.", this);
            }

            if (this.DividerImage == null)
            {
                Debug.LogError($"[{this.GetType().Name}] Divider Image reference is missing.", this);
            }
        }
    }
}