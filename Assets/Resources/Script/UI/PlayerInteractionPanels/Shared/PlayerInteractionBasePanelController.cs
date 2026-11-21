using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public class PlayerInteractionBasePanelController : FOTLMonoBehaviour
    {
        // --- References ---
        [Header("References (Shared)")]

        //  1. Internal
        [SerializeField] private PlayerInteractionPanelListManager _panelListManager;
        protected PlayerInteractionPanelListManager PanelListManager => _panelListManager;

        //  2. External
        [SerializeField] private PlayerInput _playerInput;
        protected PlayerInput PlayerInput => _playerInput;

        // --- Runtime ---
        public bool IsOpen => this.gameObject.activeSelf;

        // --- Constants ---
        private const string PLAYER_ACTION_MAP = "Player";
        protected const string PLAYER_INTERACTION_PANEL_MAP = "PlayerInteractionPanel";

        // --- Public APIs ---
        public virtual void Open()
        {
            this.gameObject.SetActive(true);

            if (this.PanelListManager != null)
            {
                this.PanelListManager.RegisterOpenedPanel(this);
            }

            if (this.PlayerInput != null)
            {
                this.PlayerInput.SwitchCurrentActionMap(PlayerInteractionBasePanelController.PLAYER_INTERACTION_PANEL_MAP);
            }

            this.OnOpened();
        }
        public virtual void Close()
        {
            this.OnClosing();

            if (this.PanelListManager != null)
            {
                this.PanelListManager.RegisterClosedPanel(this);
            }

            this.gameObject.SetActive(false);

            if (this.PlayerInput != null)
            {
                this.PlayerInput.SwitchCurrentActionMap(PlayerInteractionBasePanelController.PLAYER_ACTION_MAP);
            }
        }

        // --- Helpers ---
        //  1. Hooks
        protected virtual void OnOpened()
        {
        }
        protected virtual void OnClosing()
        {
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PanelListManager == null) _panelListManager = this.GetComponentInParent<PlayerInteractionPanelListManager>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.PanelListManager == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(PlayerInteractionPanelListManager)} reference is missing.", this);
            }
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.PlayerInput == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null) _playerInput = player.GetComponent<PlayerInput>();
            }
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.PlayerInput == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] " + $"{nameof(PlayerInput)} reference is missing.", this);
            }
        }
    }
}