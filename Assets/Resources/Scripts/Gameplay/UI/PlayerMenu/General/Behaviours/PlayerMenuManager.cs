using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public class PlayerMenuManager : MonoBehaviour
    {
        [Header("References (Player Input)")]
        [SerializeField] private PlayerInput _playerInput;
        private PlayerInput PlayerInput => _playerInput;

        [Header("References (Panels)")]
        [SerializeField] private PlayerMenuPanelController _settingPanel;
        [SerializeField] private PlayerMenuPanelController _inventoryPanel;
        [SerializeField] private PlayerMenuPanelController _loadoutPanel;
        [SerializeField] private PlayerMenuPanelController _questPanel;
        [SerializeField] private PlayerMenuPanelController _mapPanel;
        private PlayerMenuPanelController SettingPanelController => _settingPanel;
        private PlayerMenuPanelController InventoryPanelController => _inventoryPanel;
        private PlayerMenuPanelController LoadoutPanelController => _loadoutPanel;
        private PlayerMenuPanelController QuestPanelController => _questPanel;
        private PlayerMenuPanelController MapPanelController => _mapPanel;


        private const string PLAYER_ACTION_MAP = "Player";
        private const string UI_ACTION_MAP = "PlayerMenu";

        private PlayerMenuPanelType _currentPanelType = PlayerMenuPanelType.None;
        public PlayerMenuPanelType CurrentPanelType
        {
            get => _currentPanelType;
            private set => _currentPanelType = value;
        }
        public bool IsAnyPanelOpen => this.CurrentPanelType != PlayerMenuPanelType.None;


        private void Awake()
        {
            if (this.PlayerInput == null)
            {
                GameObject player = GameObject.Find("Player");
                _playerInput = player.GetComponent<PlayerInput>();
            }

            if (this.SettingPanelController == null)
            {
                Transform settingPanelTransform = this.transform.Find("SettingPanel");
                if (settingPanelTransform != null)
                {
                    _settingPanel = settingPanelTransform.GetComponent<PlayerMenuPanelController>();
                }
            }
            if (this.InventoryPanelController == null)
            {
                Transform inventoryPanelTransform = this.transform.Find("InventoryPanel");
                if (inventoryPanelTransform != null)
                {
                    _inventoryPanel = inventoryPanelTransform.GetComponent<PlayerMenuPanelController>();
                }
            }
            if (this.LoadoutPanelController == null)
            {
                Transform loadoutPanelTransform = this.transform.Find("LoadoutPanel");
                if (loadoutPanelTransform != null)
                {
                    _loadoutPanel = loadoutPanelTransform.GetComponent<PlayerMenuPanelController>();
                }
            }
            if (this.QuestPanelController == null)
            {
                Transform questPanelTransform = this.transform.Find("QuestPanel");
                if (questPanelTransform != null)
                {
                    _questPanel = questPanelTransform.GetComponent<PlayerMenuPanelController>();
                }
            }
            if (this.MapPanelController == null)
            {
                Transform mapPanelTransform = this.transform.Find("MapPanel");
                if (mapPanelTransform != null)
                {
                    _mapPanel = mapPanelTransform.GetComponent<PlayerMenuPanelController>();
                }
            }
        }
        private void Start()
        {
            this.HideAllPanelsImmediate();
            this.ResumeGameplay();
            this.SwitchToPlayerActionMap();
        }

        // --- Public APIs ---
        public void OpenSettingsPanel()
        {
            this.OpenPanel(PlayerMenuPanelType.Settings);
        }
        public void OpenInventoryPanel()
        {
            this.OpenPanel(PlayerMenuPanelType.Inventory);
        }
        public void OpenLoadoutPanel()
        {
            this.OpenPanel(PlayerMenuPanelType.Loadout);
        }
        public void OpenQuestPanel()
        {
            this.OpenPanel(PlayerMenuPanelType.Quest);
        }
        public void OpenMapPanel()
        {
            this.OpenPanel(PlayerMenuPanelType.Map);
        }

        public void CloseCurrentPanel()
        {
            if (!this.IsAnyPanelOpen) return;

            this.HideAllPanelsImmediate();
            this.CurrentPanelType = PlayerMenuPanelType.None;

            this.ResumeGameplay();
            this.SwitchToPlayerActionMap();
        }

        public void HandleCancel()
        {
            this.CloseCurrentPanel();
        }

        // --- Helpers ---
        private void OpenPanel(PlayerMenuPanelType panelType)
        {
            if (panelType == PlayerMenuPanelType.None)
            {
                this.CloseCurrentPanel();
                return;
            }

            this.HideAllPanelsImmediate();

            switch (panelType)
            {
                case PlayerMenuPanelType.Settings:
                    if (this.SettingPanelController != null)
                    {
                        this.SettingPanelController.ShowPanel();
                    }
                    break;

                case PlayerMenuPanelType.Inventory:
                    if (this.InventoryPanelController != null)
                    {
                        this.InventoryPanelController.ShowPanel();
                    }
                    break;

                case PlayerMenuPanelType.Loadout:
                    if (this.LoadoutPanelController != null)
                    {
                        this.LoadoutPanelController.ShowPanel();
                    }
                    break;

                case PlayerMenuPanelType.Quest:
                    if (this.QuestPanelController != null)
                    {
                        this.QuestPanelController.ShowPanel();
                    }
                    break;

                case PlayerMenuPanelType.Map:
                    if (this.MapPanelController != null)
                    {
                        this.MapPanelController.ShowPanel();
                    }
                    break;
            }

            this.CurrentPanelType = panelType;

            this.PauseGameplay();
            this.SwitchToUIActionMap();
        }

        private void HideAllPanelsImmediate()
        {
            if (this.SettingPanelController != null)
            {
                this.SettingPanelController.HidePanel();
            }
            if (this.InventoryPanelController != null)
            {
                this.InventoryPanelController.HidePanel();
            }
            if (this.LoadoutPanelController != null)
            {
                this.LoadoutPanelController.HidePanel();
            }
            if (this.QuestPanelController != null)
            {
                this.QuestPanelController.HidePanel();
            }
            if (this.MapPanelController != null)
            {
                this.MapPanelController.HidePanel();
            }
        }

        private void PauseGameplay()
        {
            Time.timeScale = 0f;
        }

        private void ResumeGameplay()
        {
            Time.timeScale = 1f;
        }

        private void SwitchToPlayerActionMap()
        {
            if (this.PlayerInput == null) return;
            this.PlayerInput.SwitchCurrentActionMap(PLAYER_ACTION_MAP);
        }

        private void SwitchToUIActionMap()
        {
            if (this.PlayerInput == null) return;
            this.PlayerInput.SwitchCurrentActionMap(UI_ACTION_MAP);
        }
    }
}
