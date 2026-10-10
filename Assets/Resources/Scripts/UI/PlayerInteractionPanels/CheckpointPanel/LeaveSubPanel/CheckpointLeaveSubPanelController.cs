using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CheckpointLeaveSubPanelController : CheckpointPanelSubBehaviourBase
    {
        // --- References ---
        [Header("References")]
        //  1. Option Buttons
        [SerializeField] private Button _returnToMainMenuButton;
        [SerializeField] private Button _exitGameButton;
        [SerializeField] private Button _returnToJourneyButton;
        private Button ReturnToMainMenuButton   => _returnToMainMenuButton;
        private Button ExitGameButton           => _exitGameButton;
        private Button ReturnToJourneyButton    => _returnToJourneyButton;

        //  2. Save System
        [Header("References - Game Save")]
        [SerializeField] private GameSaveController _gameSaveController;
        private GameSaveController GameSaveController => _gameSaveController;

        // --- Configs ---
        [Header("Configs - Scene")]
        [SerializeField] private string _mainMenuSceneName = "MainMenu";
        private string MainMenuSceneName => _mainMenuSceneName;

        // --- Runtime ---
        private bool _isLeaving;
        private bool IsLeaving
        {
            get => _isLeaving;
            set => _isLeaving = value;
        }

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
        private void HandleReturnToTitleButtonClicked()
        {
            if (!this.EnsureBound()) return;
            if (this.IsLeaving) return;

            // a. Validate the Main Menu scene.
            if (!this.ValidateMainMenuScene()) return;

            // b. Save the current game before leaving.
            if (!this.TrySaveBeforeLeaving()) return;

            // c. Prevent repeated button interactions.
            this.IsLeaving = true;
            this.SetOptionButtonsInteractable(false);

            // d. Load the Main Menu.
            SceneManager.LoadScene(this.MainMenuSceneName);
        }
        private void HandleExitGameButtonClicked()
        {
            if (!this.EnsureBound()) return;
            if (this.IsLeaving) return;

            // a. Save the current game before leaving.
            if (!this.TrySaveBeforeLeaving()) return;

            // b. Prevent repeated button interactions.
            this.IsLeaving = true;
            this.SetOptionButtonsInteractable(false);

            // c. Quit the application.
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        private void HandleReturnToJourneyButtonClicked()
        {
            if (!this.EnsureBound()) return;

            this.CheckpointPanelController.Close();
        }

        //  2. Save Before Leaving
        private bool TrySaveBeforeLeaving()
        {
            if (this.GameSaveController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CheckpointLeaveSubPanelController)}] Cannot leave: {nameof(GameSaveController)} reference is missing.", this);
                return false;
            }

            bool success = this.GameSaveController.SaveGame();
            if (!success)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CheckpointLeaveSubPanelController)}] Failed to save game before leaving. The leave operation was cancelled.", this);
                return false;
            }

            return true;
        }

        //  3. Scene Validation
        private bool ValidateMainMenuScene()
        {
            if (string.IsNullOrWhiteSpace(this.MainMenuSceneName))
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CheckpointLeaveSubPanelController)}] Main Menu scene name is empty.", this);
                return false;
            }

            if (!Application.CanStreamedLevelBeLoaded(this.MainMenuSceneName))
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CheckpointLeaveSubPanelController)}] Main Menu scene '{this.MainMenuSceneName}' cannot be loaded. Ensure it is included and enabled in the Build Profiles Scene List.", this);
                return false;
            }

            return true;
        }

        //  4. Button Interaction
        private void SetOptionButtonsInteractable(bool interactable)
        {
            if (this.ReturnToMainMenuButton != null) this.ReturnToMainMenuButton.interactable   = interactable;
            if (this.ExitGameButton         != null) this.ExitGameButton.interactable           = interactable;
            if (this.ReturnToJourneyButton  != null) this.ReturnToJourneyButton.interactable    = interactable;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.ReturnToMainMenuButton == null)
            {
                Transform button = this.transform.Find("OptionButtonList/Button_ReturnToMainMenu");
                if (button != null) _returnToMainMenuButton = button.GetComponent<Button>();
            }
            if (this.ExitGameButton == null)
            {
                Transform button = this.transform.Find("OptionButtonList/Button_ExitGame");
                if (button != null) _exitGameButton = button.GetComponent<Button>();
            }
            if (this.ReturnToJourneyButton == null)
            {
                Transform button = this.transform.Find("OptionButtonList/Button_ReturnToJourney");
                if (button != null) _returnToJourneyButton = button.GetComponent<Button>();
            }

            if (this.GameSaveController == null) _gameSaveController = UnityEngine.Object.FindFirstObjectByType<GameSaveController>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.ReturnToMainMenuButton == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Return To Main Menu {nameof(Button)} reference is missing.", this);
            }
            if (this.ExitGameButton == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Exit Game {nameof(Button)} reference is missing.", this);
            }
            if (this.ReturnToJourneyButton == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Return To Journey {nameof(Button)} reference is missing.", this);
            }

            //  2. Game Save
            if (this.GameSaveController == null)
            {
                Debug.LogError
                ($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(GameSaveController)} reference is missing.", this);
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.IsLeaving = false;

            this.SetOptionButtonsInteractable(true);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.ReturnToMainMenuButton != null) this.ReturnToMainMenuButton.onClick.AddListener(this.HandleReturnToTitleButtonClicked);
            if (this.ExitGameButton         != null) this.ExitGameButton.onClick.AddListener(this.HandleExitGameButtonClicked);
            if (this.ReturnToJourneyButton  != null) this.ReturnToJourneyButton.onClick.AddListener(this.HandleReturnToJourneyButtonClicked);
        }
        protected override void UnbindInternalReferences()
        {
            if (this.ReturnToMainMenuButton != null) this.ReturnToMainMenuButton.onClick.RemoveListener(this.HandleReturnToTitleButtonClicked);
            if (this.ExitGameButton         != null) this.ExitGameButton.onClick.RemoveListener(this.HandleExitGameButtonClicked);
            if (this.ReturnToJourneyButton  != null) this.ReturnToJourneyButton.onClick.RemoveListener(this.HandleReturnToJourneyButtonClicked);

            base.UnbindInternalReferences();
        }
    }
}