using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public class PlayerMenuManager : FOTLMonoBehaviour, IPlayerInteractionPromptBlocker
    {
        // --- References ---
        //  1. Player
        [Header("References - Player Input")]
        [SerializeField] private PlayerInput _playerInput;
        private PlayerInput PlayerInput => _playerInput;

        //  2. Panels
        [Header("References - Panels")]
        [SerializeField] private PlayerMenuPanelControllerBase _settingPanel;
        [SerializeField] private PlayerMenuPanelControllerBase _inventoryPanel;
        [SerializeField] private PlayerMenuPanelControllerBase _loadoutPanel;
        [SerializeField] private PlayerMenuPanelControllerBase _questPanel;
        [SerializeField] private PlayerMenuPanelControllerBase _mapPanel;
        private PlayerMenuPanelControllerBase SettingPanelController => _settingPanel;
        private PlayerMenuPanelControllerBase InventoryPanelController => _inventoryPanel;
        private PlayerMenuPanelControllerBase LoadoutPanelController => _loadoutPanel;
        private PlayerMenuPanelControllerBase QuestPanelController => _questPanel;
        private PlayerMenuPanelControllerBase MapPanelController => _mapPanel;

        // --- Constants ---
        private const string PLAYER_ACTION_MAP = "Player";
        private const string UI_ACTION_MAP = "PlayerMenu";

        // --- Runtime ---
        //  1. Current Panel
        private PlayerMenuPanelType _currentPanelType = PlayerMenuPanelType.None;
        public PlayerMenuPanelType CurrentPanelType
        {
            get => _currentPanelType;
            private set => _currentPanelType = value;
        }
        public bool IsAnyPanelOpen => this.CurrentPanelType != PlayerMenuPanelType.None;

        //  2. Interaction Prompt Blocking
        public bool IsBlockingPlayerInteractionPrompt => this.IsAnyPanelOpen;

        // --- Events ---
        public event Action<IPlayerInteractionPromptBlocker> PromptBlockStarted;
        public event Action<IPlayerInteractionPromptBlocker> PromptBlockEnded;

        // --- Public APIs ---
        //  1. Opening Panels
        public void OpenSettingsPanel() => this.OpenPanel(PlayerMenuPanelType.Settings);
        public void OpenInventoryPanel() => this.OpenPanel(PlayerMenuPanelType.Inventory);
        public void OpenLoadoutPanel() => this.OpenPanel(PlayerMenuPanelType.Loadout);
        public void OpenQuestPanel() => this.OpenPanel(PlayerMenuPanelType.Quest);
        public void OpenMapPanel() => this.OpenPanel(PlayerMenuPanelType.Map);

        //  2. Closing
        public void CloseCurrentPanel()
        {
            if (!this.IsAnyPanelOpen) return;

            bool wasBlocking = this.IsBlockingPlayerInteractionPrompt;

            this.HideAllPanelsImmediate();
            this.CurrentPanelType = PlayerMenuPanelType.None;

            if (wasBlocking && !this.IsBlockingPlayerInteractionPrompt)
            {
                this.PromptBlockEnded?.Invoke(this);
            }

            this.ResumeGameplay();
            this.SwitchToPlayerActionMap();
        }
        public void HandleCancel()
        {
            this.CloseCurrentPanel();
        }

        // --- Helpers ---
        //  1. Panel Opening
        private void OpenPanel(PlayerMenuPanelType panelType)
        {
            if (panelType == PlayerMenuPanelType.None)
            {
                this.CloseCurrentPanel();
                return;
            }

            PlayerMenuPanelControllerBase targetPanel = this.GetPanelController(panelType);

            if (targetPanel == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerMenuManager)}] No panel controller is configured for {panelType}.", this);
                return;
            }

            bool wasBlocking = this.IsBlockingPlayerInteractionPrompt;

            this.HideAllPanelsImmediate();

            targetPanel.ShowPanel();

            this.CurrentPanelType = panelType;

            if (!wasBlocking && this.IsBlockingPlayerInteractionPrompt)
            {
                this.PromptBlockStarted?.Invoke(this);
            }

            this.PauseGameplay();
            this.SwitchToUIActionMap();
        }
        private PlayerMenuPanelControllerBase GetPanelController(PlayerMenuPanelType panelType)
        {
            return panelType switch
            {
                PlayerMenuPanelType.Settings => this.SettingPanelController,
                PlayerMenuPanelType.Inventory => this.InventoryPanelController,
                PlayerMenuPanelType.Loadout => this.LoadoutPanelController,
                PlayerMenuPanelType.Quest => this.QuestPanelController,
                PlayerMenuPanelType.Map => this.MapPanelController,
                _ => null
            };
        }

        //  2. Panel Hiding
        private void HideAllPanelsImmediate()
        {
            this.HidePanelIfShown(this.SettingPanelController);
            this.HidePanelIfShown(this.InventoryPanelController);
            this.HidePanelIfShown(this.LoadoutPanelController);
            this.HidePanelIfShown(this.QuestPanelController);
            this.HidePanelIfShown(this.MapPanelController);
        }
        private void HidePanelIfShown(PlayerMenuPanelControllerBase panelController)
        {
            if (panelController == null) return;
            if (!panelController.IsPanelShown) return;

            panelController.HidePanel();
        }

        //  3. Gameplay State
        private void PauseGameplay()
        {
            Time.timeScale = 0f;
        }
        private void ResumeGameplay()
        {
            Time.timeScale = 1f;
        }

        //  4. Input Action Maps
        private void SwitchToPlayerActionMap()
        {
            if (this.PlayerInput == null) return;
            this.PlayerInput.SwitchCurrentActionMap(PlayerMenuManager.PLAYER_ACTION_MAP);
        }
        private void SwitchToUIActionMap()
        {
            if (this.PlayerInput == null) return;
            this.PlayerInput.SwitchCurrentActionMap(PlayerMenuManager.UI_ACTION_MAP);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.SettingPanelController == null)
            {
                Transform settingPanelTransform = this.transform.Find("SettingPanel");
                if (settingPanelTransform != null) _settingPanel = settingPanelTransform.GetComponent<PlayerMenuPanelControllerBase>();
            }
            if (this.InventoryPanelController == null)
            {
                Transform inventoryPanelTransform = this.transform.Find("InventoryPanel");
                if (inventoryPanelTransform != null) _inventoryPanel = inventoryPanelTransform.GetComponent<PlayerMenuPanelControllerBase>();
            }
            if (this.LoadoutPanelController == null)
            {
                Transform loadoutPanelTransform = this.transform.Find("LoadoutPanel");
                if (loadoutPanelTransform != null) _loadoutPanel = loadoutPanelTransform.GetComponent<PlayerMenuPanelControllerBase>();
            }
            if (this.QuestPanelController == null)
            {
                Transform questPanelTransform = this.transform.Find("QuestPanel");
                if (questPanelTransform != null) _questPanel = questPanelTransform.GetComponent<PlayerMenuPanelControllerBase>();
            }
            if (this.MapPanelController == null)
            {
                Transform mapPanelTransform = this.transform.Find("MapPanel");
                if (mapPanelTransform != null) _mapPanel = mapPanelTransform.GetComponent<PlayerMenuPanelControllerBase>();
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.CurrentPanelType = PlayerMenuPanelType.None;
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.PlayerInput != null) return;

            GameObject player = GameObject.Find("Player");
            if (player != null) _playerInput = player.GetComponent<PlayerInput>();
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.PlayerInput == null) Debug.LogError($"{nameof(PlayerMenuManager)} requires a " + $"{nameof(UnityEngine.InputSystem.PlayerInput)}.", this);
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.HideAllPanelsImmediate();

            this.CurrentPanelType = PlayerMenuPanelType.None;

            this.ResumeGameplay();
            this.SwitchToPlayerActionMap();
        }
        protected override void CleanupStates()
        {
            if (Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
            }

            base.CleanupStates();
        }
    }
}
