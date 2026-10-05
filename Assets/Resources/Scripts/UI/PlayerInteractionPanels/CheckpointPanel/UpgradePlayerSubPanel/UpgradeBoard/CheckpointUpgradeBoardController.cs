using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CheckpointUpgradeBoardController : CheckpointUpgradePlayerSubPanelSubBehaviourBase
    {
        // --- References ---
        //  1. Displays
        [Header("References - Displays")]
        [SerializeField] private CheckpointUpgradeLostEmberDisplayController _lostEmberDisplay;
        [SerializeField] private CheckpointUpgradeAttributeBarController[] _attributeBars;
        [SerializeField] private CheckpointUpgradeAttributeDetailDisplayController _attributeDetailDisplay;
        public CheckpointUpgradeLostEmberDisplayController LostEmberDisplay => _lostEmberDisplay;
        public CheckpointUpgradeAttributeBarController[] AttributeBars => _attributeBars;
        public CheckpointUpgradeAttributeDetailDisplayController AttributeDetailDisplay => _attributeDetailDisplay;

        //  2. Actions
        [Header("References - Actions")]
        [SerializeField] private Button _dismissButton;
        [SerializeField] private Button _confirmButton;
        private Button DismissButton => _dismissButton;
        private Button ConfirmButton => _confirmButton;

        // --- Public APIs ---
        //  1. Refresh
        public void RefreshDisplay()
        {
            if (!this.EnsureBound()) return;

            this.LostEmberDisplay?.RefreshDisplay();

            if (this.AttributeBars != null)
            {
                foreach (CheckpointUpgradeAttributeBarController attributeBar in this.AttributeBars)
                {
                    attributeBar?.RefreshDisplay();
                }
            }

            this.AttributeDetailDisplay?.RefreshDisplay();

            if (this.ConfirmButton != null) this.ConfirmButton.interactable = this.UpgradePlayerSubPanelController.HasStagedUpgrades;
            if (this.DismissButton != null) this.DismissButton.interactable = this.UpgradePlayerSubPanelController.HasStagedUpgrades;
        }

        // --- Helpers ---
        //  1. Event Handlers
        private void HandleDismissClicked()
        {
            if (!this.EnsureBound()) return;

            this.UpgradePlayerSubPanelController.DismissStagedUpgrades();
        }
        private void HandleConfirmClicked()
        {
            if (!this.EnsureBound()) return;

            this.UpgradePlayerSubPanelController.ConfirmStagedUpgrades();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.LostEmberDisplay == null) _lostEmberDisplay = this.GetComponentInChildren<CheckpointUpgradeLostEmberDisplayController>(true);

            if (this.AttributeBars == null || this.AttributeBars.Length == 0) _attributeBars = this.GetComponentsInChildren<CheckpointUpgradeAttributeBarController>(true);

            if (this.AttributeDetailDisplay == null) _attributeDetailDisplay = this.GetComponentInChildren<CheckpointUpgradeAttributeDetailDisplayController>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.LostEmberDisplay == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(CheckpointUpgradeLostEmberDisplayController)} reference is missing.", this);
            }

            if (this.AttributeDetailDisplay == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(CheckpointUpgradeAttributeDetailDisplayController)} reference is missing.", this);
            }

            if (this.AttributeBars == null || this.AttributeBars.Length == 0)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] No {nameof(CheckpointUpgradeAttributeBarController)} references are assigned.", this);
            }

            if (this.DismissButton == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Dismiss Button reference is missing.", this);
            }

            if (this.ConfirmButton == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Confirm Button reference is missing.", this);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.LostEmberDisplay?.Bind(this);
            this.AttributeDetailDisplay?.Bind(this);

            if (this.AttributeBars != null)
            {
                foreach (CheckpointUpgradeAttributeBarController attributeBar in this.AttributeBars)
                {
                    attributeBar?.Bind(this);
                }
            }

            _dismissButton?.onClick.AddListener(this.HandleDismissClicked);
            _confirmButton?.onClick.AddListener(this.HandleConfirmClicked);
        }
        protected override void UnbindInternalReferences()
        {
            _dismissButton?.onClick.RemoveListener(this.HandleDismissClicked);
            _confirmButton?.onClick.RemoveListener(this.HandleConfirmClicked);

            this.LostEmberDisplay?.Unbind(this);
            this.AttributeDetailDisplay?.Unbind(this);

            if (this.AttributeBars != null)
            {
                foreach (CheckpointUpgradeAttributeBarController attributeBar in this.AttributeBars)
                {
                    attributeBar?.Unbind(this);
                }
            }

            base.UnbindInternalReferences();
        }
    }
}
