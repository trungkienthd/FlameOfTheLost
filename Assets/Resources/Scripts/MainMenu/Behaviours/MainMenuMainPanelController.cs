using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class MainMenuMainPanelController : MainMenuSubBehaviourBase
    {
        // --- References ---
        [Header("References - Buttons")]
        [SerializeField] private Button _continueJourneyButton;
        [SerializeField] private Button _newJourneyButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _creditsButton;
        [SerializeField] private Button _exitButton;
        public Button ContinueJourneyButton => _continueJourneyButton;
        public Button NewJourneyButton => _newJourneyButton;
        public Button SettingsButton => _settingsButton;
        public Button CreditsButton => _creditsButton;
        public Button ExitButton => _exitButton;

        public Button DefaultButton
        {
            get
            {
                if (this.ContinueJourneyButton != null && this.ContinueJourneyButton.gameObject.activeInHierarchy && this.ContinueJourneyButton.IsInteractable())
                {
                    return this.ContinueJourneyButton;
                }

                return this.NewJourneyButton;
            }
        }

        // --- Public APIs ---
        //  1. Visibility
        public void Show()
        {
            this.gameObject.SetActive(true);

            this.SetInteractionEnabled(true);

            this.RefreshContinueButton();
        }
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        //  2. Interaction

        public void SetInteractionEnabled(bool interactable)
        {
            if (this.ContinueJourneyButton  != null) this.ContinueJourneyButton.interactable    = interactable;
            if (this.NewJourneyButton       != null) this.NewJourneyButton.interactable         = interactable;
            if (this.SettingsButton         != null) this.SettingsButton.interactable           = interactable;
            if (this.CreditsButton          != null) this.CreditsButton.interactable            = interactable;
            if (this.ExitButton             != null) this.ExitButton.interactable               = interactable;

            if (interactable)
            {
                this.RefreshContinueButton();
            }
        }
        public void RefreshContinueButton()
        {
            if (!this.EnsureBound()) return;

            if (this.ContinueJourneyButton == null) return;

            bool hasValidSaveGame = this.MainMenuController.HasValidSaveGame;

            this.ContinueJourneyButton.gameObject.SetActive(hasValidSaveGame);
        }

        // --- Helpers ---
        //  1. Button Events
        private void HandleContinueJourneyClicked()
        {
            if (!this.EnsureBound()) return;

            this.MainMenuController.RequestContinueJourney();
        }
        private void HandleNewJourneyClicked()
        {
            if (!this.EnsureBound()) return;

            this.MainMenuController.RequestNewJourney();
        }
        private void HandleSettingsClicked()
        {
            if (!this.EnsureBound()) return;

            this.MainMenuController.ShowSettingsPanel();
        }
        private void HandleCreditsClicked()
        {
            if (!this.EnsureBound()) return;

            this.MainMenuController.ShowCreditsPanel();
        }
        private void HandleExitClicked()
        {
            if (!this.EnsureBound()) return;

            this.MainMenuController.ExitGame();
        }

        // --- FOTLMonoBehaviour Overrides ---

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            Transform container = this.transform.Find("ButtonContainer");
            if (container == null) return;

            if (_continueJourneyButton == null)
            {
                Transform target = container.Find("Button_ContinueJourney");
                if (target != null) _continueJourneyButton = target.GetComponent<Button>();
            }
            if (_newJourneyButton == null)
            {
                Transform target = container.Find("Button_NewJourney");
                if (target != null) _newJourneyButton = target.GetComponent<Button>();
            }
            if (_settingsButton == null)
            {
                Transform target = container.Find("Button_Settings");
                if (target != null) _settingsButton = target.GetComponent<Button>();
            }
            if (_creditsButton == null)
            {
                Transform target = container.Find("Button_Credits");
                if (target != null) _creditsButton = target.GetComponent<Button>();
            }
            if (_exitButton == null)
            {
                Transform target = container.Find("Button_Exit");
                if (target != null) _exitButton = target.GetComponent<Button>();
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            _continueJourneyButton?.onClick.AddListener(this.HandleContinueJourneyClicked);
            _newJourneyButton?.onClick.AddListener(this.HandleNewJourneyClicked);
            _settingsButton?.onClick.AddListener(this.HandleSettingsClicked);
            _creditsButton?.onClick.AddListener(this.HandleCreditsClicked);
            _exitButton?.onClick.AddListener(this.HandleExitClicked);
        }
        protected override void UnbindInternalReferences()
        {
            _continueJourneyButton?.onClick.RemoveListener(this.HandleContinueJourneyClicked);
            _newJourneyButton?.onClick.RemoveListener(this.HandleNewJourneyClicked);
            _settingsButton?.onClick.RemoveListener(this.HandleSettingsClicked);
            _creditsButton?.onClick.RemoveListener(this.HandleCreditsClicked);
            _exitButton?.onClick.RemoveListener(this.HandleExitClicked);

            base.UnbindInternalReferences();
        }
    }
}
