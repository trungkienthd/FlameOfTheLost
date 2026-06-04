using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public class PlayerInteractionBasePanelController : MonoBehaviour
    {
        [Header("Shared References")]
        [SerializeField] private PlayerInteractionPanelListManager _panelListManager;
        [SerializeField] private PlayerInput _playerInput;
        protected PlayerInteractionPanelListManager PanelListManager => _panelListManager;
        protected PlayerInput PlayerInput => _playerInput;

        public bool IsOpen => this.gameObject.activeSelf;

        private const string PLAYER_ACTION_MAP = "Player";
        protected const string PLAYER_INTERACTION_PANEL_MAP = "PlayerInteractionPanel";

        public virtual void Open()
        {
            this.gameObject.SetActive(true);

            if (this.PanelListManager != null)
            {
                this.PanelListManager.RegisterOpenedPanel(this);
            }

            if (this.PlayerInput != null)
            {
                this.PlayerInput.SwitchCurrentActionMap("PlayerInteractionPanel");
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
                this.PlayerInput.SwitchCurrentActionMap(PLAYER_ACTION_MAP);
            }
        }

        protected virtual void OnOpened()
        {
        }

        protected virtual void OnClosing()
        {
        }

        protected virtual void CacheReferences()
        {
            if (this.PanelListManager == null) _panelListManager = this.GetComponentInParent<PlayerInteractionPanelListManager>(true);

            if (this.PlayerInput == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null) _playerInput = player.GetComponent<PlayerInput>();
            }
        }
    }
}