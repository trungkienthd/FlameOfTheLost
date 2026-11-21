using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CheckpointMainSubPanelController : CheckpointPanelSubBehaviourBase
    {
        // --- References ---
        [Header("References")]

        //  1. Interaction
        [SerializeField] private CanvasGroup _canvasGroup;
        private CanvasGroup CanvasGroup => _canvasGroup;

        //  2. Display
        [SerializeField] private TMP_Text _mapNameText;
        private TMP_Text MapNameText => _mapNameText;

        //  3. Option Buttons
        [SerializeField] private Button _upgradePlayerButton;
        [SerializeField] private Button _leaveGameButton;
        private Button UpgradePlayerButton => _upgradePlayerButton;
        private Button LeaveGameButton => _leaveGameButton;


        // --- Public APIs ---
        //  1. Visibility
        public void Show()
        {
            this.gameObject.SetActive(true);

            this.SetInteractionEnabled(true);
            this.RefreshMapName();
        }
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        //  2. Interaction
        public void SetInteractionEnabled(bool enabled)
        {
            if (this.CanvasGroup == null) return;

            this.CanvasGroup.interactable = enabled;
            this.CanvasGroup.blocksRaycasts = enabled;
        }

        // --- Helpers ---
        //  1. Display
        private void RefreshMapName()
        {
            if (this.MapNameText == null) return;

            Scene activeScene = SceneManager.GetActiveScene();

            this.MapNameText.text = activeScene.name;
        }

        //  2. Button Events
        private void HandleUpgradePlayerButtonClicked()
        {
            if (!this.EnsureBound()) return;

            this.CheckpointPanelController.ShowUpgradePlayerSubPanel();
        }
        private void HandleLeaveGameButtonClicked()
        {
            if (!this.EnsureBound()) return;

            this.CheckpointPanelController.ShowLeaveSubPanel();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.CanvasGroup == null) _canvasGroup = this.GetComponent<CanvasGroup>();

            if (this.MapNameText == null)
            {
                Transform mapNameDisplay = this.transform.Find("MapNameDisplay");
                if (mapNameDisplay != null) _mapNameText = mapNameDisplay.GetComponentInChildren<TMP_Text>(true);
            }

            if (this.UpgradePlayerButton == null)
            {
                Transform button = this.transform.Find("OptionButtonList/ButtonUpgradePlayer");
                if (button != null) _upgradePlayerButton = button.GetComponent<Button>();
            }
            if (this.LeaveGameButton == null)
            {
                Transform button = this.transform.Find("OptionButtonList/ButtonLeaveGame");
                if (button != null) _leaveGameButton = button.GetComponent<Button>();
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.CanvasGroup == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(CanvasGroup)} reference is missing.", this);
            }

            if (this.MapNameText == null)
            {
                Debug.LogError(
                    $"[{this.gameObject.name}.{this.GetType().Name}] " +
                    $"{nameof(TMP_Text)} reference for Map Name is missing.",
                    this);
            }

            if (this.UpgradePlayerButton == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Upgrade Player {nameof(Button)} reference is missing.", this);
            }
            if (this.LeaveGameButton == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Leave Game {nameof(Button)} reference is missing.", this);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.UpgradePlayerButton    != null) this.UpgradePlayerButton.onClick.AddListener(this.HandleUpgradePlayerButtonClicked);
            if (this.LeaveGameButton        != null) this.LeaveGameButton.onClick.AddListener(this.HandleLeaveGameButtonClicked);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.SetInteractionEnabled(true);
        }
        protected override void UnbindInternalReferences()
        {
            if (this.UpgradePlayerButton    != null) this.UpgradePlayerButton.onClick.RemoveListener(this.HandleUpgradePlayerButtonClicked);
            if (this.LeaveGameButton        != null) this.LeaveGameButton.onClick.RemoveListener(this.HandleLeaveGameButtonClicked);

            base.UnbindInternalReferences();
        }
    }
}