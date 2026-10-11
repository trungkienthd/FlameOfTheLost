using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class MainMenuController : FOTLMonoBehaviour
    {
        // --- References ---
        //  1. Sub Panels
        [Header("References - Sub Panels")]
        [SerializeField] private MainMenuMainPanelController _mainPanelController;
        [SerializeField] private MainMenuNewJourneyConfirmationController _newJourneyConfirmationController;
        [SerializeField] private MainMenuSettingsPanelController _settingsPanelController;
        [SerializeField] private MainMenuCreditsPanelController _creditsPanelController;
        [SerializeField] private MainMenuTransitionController _transitionController;
        private MainMenuMainPanelController MainPanelController => _mainPanelController;
        private MainMenuNewJourneyConfirmationController NewJourneyConfirmationController => _newJourneyConfirmationController;
        private MainMenuSettingsPanelController SettingsPanelController => _settingsPanelController;
        private MainMenuCreditsPanelController CreditsPanelController => _creditsPanelController;
        private MainMenuTransitionController TransitionController => _transitionController;

        //  2. Background Click Handlers
        [Header("References - Background Click Handlers")]
        [SerializeField]
        private MainMenuBackgroundClickClearAreaHandler[] _backgroundClickClearAreaHandlers;

        private MainMenuBackgroundClickClearAreaHandler[] BackgroundClickClearAreaHandlers => _backgroundClickClearAreaHandlers;

        // --- Configs ---
        [Header("Configs - Scene")]
        [SerializeField] private string _gameplaySceneName = "GreenvaleBorderPath";
        private string GameplaySceneName => _gameplaySceneName;

        // --- Runtime ---
        private GameSaveFileRepository _repository;
        private bool _isTransitioning;
        private MainMenuPanelType _activePanel;
        private Button _lastMainMenuSelection;
        public bool IsTransitioning => _isTransitioning;

        public bool HasValidSaveGame
        {
            get
            {
                if (_repository == null) return false;
                if (!_repository.TryLoad(out GameSaveData saveData)) return false;
                if (saveData == null) return false;

                return saveData.Version <= GameSaveData.CurrentVersion && saveData.Player != null;
            }
        }

        // --- Unity Callbacks ---
        private void Update()
        {
            if (this.IsTransitioning) return;

            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                this.HandleCancel();
            }

            if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
            {
                this.HandleCancel();
            }
        }

        // --- Public APIs ---
        //  1. Panel Navigation
        public void ShowMainPanel()
        {
            if (this.IsTransitioning) return;

            _activePanel = MainMenuPanelType.Main;

            this.NewJourneyConfirmationController?.Hide();
            this.SettingsPanelController?.Hide();
            this.CreditsPanelController?.Hide();

            this.MainPanelController?.Show();

            Button targetButton = _lastMainMenuSelection;
            if (targetButton == null || !targetButton.IsActive() || !targetButton.IsInteractable())
            {
                targetButton = this.MainPanelController?.DefaultButton;
            }

            this.SelectButton(targetButton);
        }
        public void ShowNewJourneyConfirmation()
        {
            if (this.IsTransitioning) return;

            _lastMainMenuSelection = this.MainPanelController?.NewJourneyButton;

            _activePanel = MainMenuPanelType.NewJourneyConfirmation;

            this.MainPanelController?.Hide();
            this.SettingsPanelController?.Hide();
            this.CreditsPanelController?.Hide();

            this.NewJourneyConfirmationController?.Show();
        }
        public void ShowSettingsPanel()
        {
            if (this.IsTransitioning) return;

            _lastMainMenuSelection = this.MainPanelController?.SettingsButton;

            _activePanel = MainMenuPanelType.Settings;

            this.MainPanelController?.Hide();
            this.NewJourneyConfirmationController?.Hide();
            this.CreditsPanelController?.Hide();

            this.SettingsPanelController?.Show();
        }
        public void ShowCreditsPanel()
        {
            if (this.IsTransitioning) return;

            _lastMainMenuSelection = this.MainPanelController?.CreditsButton;

            _activePanel = MainMenuPanelType.Credits;

            this.MainPanelController?.Hide();
            this.NewJourneyConfirmationController?.Hide();
            this.SettingsPanelController?.Hide();

            this.CreditsPanelController?.Show();
        }

        public void HandleCancel()
        {
            if (this.IsTransitioning) return;

            if (_activePanel == MainMenuPanelType.Main) return;

            this.ShowMainPanel();
        }

        //  2. Journey

        public void RequestContinueJourney()
        {
            if (this.IsTransitioning) return;

            if (!this.HasValidSaveGame)
            {
                Debug.LogWarning($"[{nameof(MainMenuController)}] Cannot continue: no valid save game exists.", this);

                this.MainPanelController?.RefreshContinueButton();
                return;
            }

            _lastMainMenuSelection = this.MainPanelController?.ContinueJourneyButton;

            this.BeginJourney(MainMenuJourneyType.Continue);
        }

        public void RequestNewJourney()
        {
            if (this.IsTransitioning) return;

            if (this.HasValidSaveGame)
            {
                this.ShowNewJourneyConfirmation();
                return;
            }

            // An existing but invalid save still requires confirmation.
            if (_repository != null && _repository.HasSave())
            {
                this.ShowNewJourneyConfirmation();
                return;
            }

            this.BeginJourney(MainMenuJourneyType.New);
        }

        public void ConfirmNewJourney()
        {
            if (this.IsTransitioning) return;

            this.BeginJourney(MainMenuJourneyType.New);
        }

        public void ExitGame()
        {
            if (this.IsTransitioning) return;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // --- Helpers ---
        //  1. Journey

        private void BeginJourney(MainMenuJourneyType journeyType)
        {
            if (!Application.CanStreamedLevelBeLoaded(this.GameplaySceneName))
            {
                Debug.LogError($"[{nameof(MainMenuController)}] Gameplay scene '{this.GameplaySceneName}' is not available in the Build Profiles Scene List.", this);
                return;
            }

            if (this.TransitionController == null)
            {
                Debug.LogError($"[{nameof(MainMenuController)}] Transition controller is missing.", this);
                return;
            }

            _isTransitioning = true;

            MainMenuSessionRequest.SetRequest(journeyType);

            this.StartCoroutine(this.LoadGameplayRoutine());
        }

        private IEnumerator LoadGameplayRoutine()
        {
            this.MainPanelController?.SetInteractionEnabled(false);

            this.NewJourneyConfirmationController?.SetInteractionEnabled(false);

            // Fade to black.
            yield return this.TransitionController.FadeOutRoutine();

            // Display the loading indicator.
            this.TransitionController.ShowLoading();

            // Allow the loading UI to render before scene loading.
            yield return null;

            AsyncOperation operation = null;
            try
            {
                operation = SceneManager.LoadSceneAsync(this.GameplaySceneName);
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception, this);
            }

            if (operation == null)
            {
                Debug.LogError($"[{nameof(MainMenuController)}] Failed to start scene loading.", this);

                MainMenuSessionRequest.ClearRequest();

                _isTransitioning = false;

                this.TransitionController.Hide();

                this.ShowMainPanel();

                yield break;
            }

            while (!operation.isDone) yield return null;
        }

        //  2. UI Selection
        public void SelectButton(Button button)
        {
            if (button == null) return;

            if (!button.IsActive() || !button.IsInteractable()) return;

            if (EventSystem.current == null) return;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.MainPanelController == null || this.NewJourneyConfirmationController == null || this.SettingsPanelController == null || this.CreditsPanelController == null || this.TransitionController == null)
            {
                Debug.LogWarning($"[{nameof(MainMenuController)}] Assign all sub-panel references in the Inspector.", this);
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.MainPanelController                == null) Debug.LogError("Main Panel Controller missing.", this);
            if (this.NewJourneyConfirmationController   == null) Debug.LogError("New Journey Confirmation Controller missing.", this);
            if (this.SettingsPanelController            == null) Debug.LogError("Settings Panel Controller missing.", this);
            if (this.CreditsPanelController             == null) Debug.LogError("Credits Panel Controller missing.", this);
            if (this.TransitionController               == null) Debug.LogError("Transition Controller missing.", this);

            if (this.BackgroundClickClearAreaHandlers == null || this.BackgroundClickClearAreaHandlers.Length == 0)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(MainMenuBackgroundClickClearAreaHandler)} references are missing.", this);
            }
            else
            {
                for (int i = 0; i < this.BackgroundClickClearAreaHandlers.Length; i++)
                {
                    if (this.BackgroundClickClearAreaHandlers[i] != null) continue;

                    Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Background Click Handler at index {i} is null.", this);
                }
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.MainPanelController?.Bind(this);
            this.NewJourneyConfirmationController?.Bind(this);
            this.SettingsPanelController?.Bind(this);
            this.CreditsPanelController?.Bind(this);
            this.TransitionController?.Bind(this);

            if (this.BackgroundClickClearAreaHandlers != null)
            {
                foreach (MainMenuBackgroundClickClearAreaHandler handler in this.BackgroundClickClearAreaHandlers)
                {
                    handler?.Bind(this);
                }
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _repository = new GameSaveFileRepository();

            _isTransitioning = false;

            _activePanel = MainMenuPanelType.Main;

            this.NewJourneyConfirmationController?.Hide();
            this.SettingsPanelController?.Hide();
            this.CreditsPanelController?.Hide();
            this.TransitionController?.Hide();

            this.MainPanelController?.Show();
        }
        protected override void InitializeGameplay()
        {
            base.InitializeGameplay();

            this.MainPanelController?.RefreshContinueButton();

            this.SelectButton(this.MainPanelController?.DefaultButton);
        }
        protected override void UnbindInternalReferences()
        {
            if (this.BackgroundClickClearAreaHandlers != null)
            {
                foreach (MainMenuBackgroundClickClearAreaHandler handler in this.BackgroundClickClearAreaHandlers)
                {
                    handler?.Unbind(this);
                }
            }

            this.MainPanelController?.Unbind(this);
            this.NewJourneyConfirmationController?.Unbind(this);
            this.SettingsPanelController?.Unbind(this);
            this.CreditsPanelController?.Unbind(this);
            this.TransitionController?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}