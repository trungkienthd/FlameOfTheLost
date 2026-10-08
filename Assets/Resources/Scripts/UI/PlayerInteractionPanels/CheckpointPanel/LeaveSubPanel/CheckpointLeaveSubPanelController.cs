using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CheckpointLeaveSubPanelController : CheckpointPanelSubBehaviourBase
    {
        // --- References ---
        [Header("References")]
        //  1. Option Buttons
        [SerializeField] private Button _saveAndReturnToTitleButton;
        [SerializeField] private Button _saveAndQuitButton;
        [SerializeField] private Button _returnToJourneyButton;
        private Button SaveAndReturnToTitleButton => _saveAndReturnToTitleButton;
        private Button SaveAndQuitButton => _saveAndQuitButton;
        private Button ReturnToJourneyButton => _returnToJourneyButton;

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

        // --- Helpers ---
        //  1. Button Events
        private void HandleSaveAndReturnToTitleButtonClicked()
        {
            if (!this.EnsureBound()) return;

            // TODO:
            //  1. Save game.
            //  2. Return to Title / Game Menu.
        }
        private void HandleSaveAndQuitButtonClicked()
        {
            if (!this.EnsureBound()) return;

            // TODO:
            //  1. Save game.
            //  2. Quit application.
        }
        private void HandleReturnToJourneyButtonClicked()
        {
            if (!this.EnsureBound()) return;

            this.CheckpointPanelController.Close();
        }
        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.SaveAndReturnToTitleButton == null)
            {
                Transform button = this.transform.Find("OptionButtonList/ButtonSaveAndReturnToTitle");
                if (button != null) _saveAndReturnToTitleButton = button.GetComponent<Button>();
            }
            if (this.SaveAndQuitButton == null)
            {
                Transform button = this.transform.Find("OptionButtonList/ButtonSaveAndQuit");
                if (button != null) _saveAndQuitButton = button.GetComponent<Button>();
            }
            if (this.ReturnToJourneyButton == null)
            {
                Transform button = this.transform.Find("OptionButtonList/ButtonReturnToJourney");
                if (button != null) _returnToJourneyButton = button.GetComponent<Button>();
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.SaveAndReturnToTitleButton == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Save And Return To Title {nameof(Button)} reference is missing.", this);
            }
            if (this.SaveAndQuitButton == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Save And Quit {nameof(Button)} reference is missing.", this);
            }
            if (this.ReturnToJourneyButton == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Return To Journey {nameof(Button)} reference is missing.", this);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.SaveAndReturnToTitleButton != null) this.SaveAndReturnToTitleButton.onClick.AddListener(this.HandleSaveAndReturnToTitleButtonClicked);
            if (this.SaveAndQuitButton          != null) this.SaveAndQuitButton.onClick.AddListener(this.HandleSaveAndQuitButtonClicked);
            if (this.ReturnToJourneyButton      != null) this.ReturnToJourneyButton.onClick.AddListener(this.HandleReturnToJourneyButtonClicked);
        }
        protected override void UnbindInternalReferences()
        {
            if (this.SaveAndReturnToTitleButton != null) this.SaveAndReturnToTitleButton.onClick.RemoveListener(this.HandleSaveAndReturnToTitleButtonClicked);
            if (this.SaveAndQuitButton          != null) this.SaveAndQuitButton.onClick.RemoveListener(this.HandleSaveAndQuitButtonClicked);
            if (this.ReturnToJourneyButton      != null) this.ReturnToJourneyButton.onClick.RemoveListener(this.HandleReturnToJourneyButtonClicked);

            base.UnbindInternalReferences();
        }
    }
}