using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CheckpointUpgradeAttributeBarController : CheckpointUpgradeBoardSubBehaviourBase
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private Button _selectButton;
        [SerializeField] private Image _barImage;

        [SerializeField] private Image _attributeIcon;
        [SerializeField] private TMP_Text _attributeTitleText;

        [SerializeField] private Button _stageUpgradeButton;
        [SerializeField] private TMP_Text _upgradeCostText;

        // --- Config ---

        //  1. Attribute
        [Header("Attribute")]
        [SerializeField] private CharacterProgressionAttributeType _attributeType;
        public CharacterProgressionAttributeType AttributeType => _attributeType;

        //  2. Visual
        [Header("Selection Colors")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _selectedColor = Color.yellow;
        private Color NormalColor => _normalColor;
        private Color SelectedColor => _selectedColor;

        [Header("Attribute Value Colors")]
        [SerializeField] private Color _realValueColor = new Color32(70, 0, 0, 255);
        [SerializeField] private Color _stagedValueColor = Color.green;
        private Color RealValueColor => _realValueColor;
        private Color StagedValueColor => _stagedValueColor;

        // --- Public APIs ---
        public void RefreshDisplay()
        {
            if (!this.EnsureBound()) return;

            CheckpointUpgradePlayerSubPanelController controller = this.UpgradePlayerSubPanelController;

            CharacterProgressionAttributeDisplayEntry displayEntry = controller.GetAttributeDisplayEntry(this.AttributeType);

            if (displayEntry != null)
            {
                if (_attributeIcon != null)
                {
                    _attributeIcon.sprite = displayEntry.Icon;
                }

                if (_attributeTitleText != null)
                {
                    this.RefreshAttributeTitle(controller, displayEntry.DisplayName);
                }
            }

            bool selected = controller.SelectedAttributeType == this.AttributeType;

            if (_barImage != null)
            {
                _barImage.color = selected
                    ? this.SelectedColor
                    : this.NormalColor;
            }

            int nextCost = controller.GetNextUpgradeCost(this.AttributeType);

            if (_upgradeCostText != null)
            {
                _upgradeCostText.text = nextCost >= 0
                    ? nextCost.ToString()
                    : "-";
            }

            if (_stageUpgradeButton != null)
            {
                _stageUpgradeButton.interactable = controller.CanStageUpgrade(this.AttributeType);
            }
        }

        // --- Helpers ---
        //  0. Event Handlers
        private void HandleSelectClicked()
        {
            if (!this.EnsureBound()) return;

            this.UpgradePlayerSubPanelController.SelectAttribute(this.AttributeType);
        }
        private void HandleStageUpgradeClicked()
        {
            if (!this.EnsureBound()) return;

            this.UpgradePlayerSubPanelController.StageUpgrade(this.AttributeType);
        }

        //  1. Display
        private void RefreshAttributeTitle(CheckpointUpgradePlayerSubPanelController controller, string displayName)
        {
            if (_attributeTitleText == null) return;
            if (controller == null) return;

            int currentValue = controller.GetCurrentAttributeValue(this.AttributeType);
            int stagedCount = controller.GetStagedUpgradeCount(this.AttributeType);

            bool hasStagedUpgrade = stagedCount > 0;

            int displayedValue = hasStagedUpgrade
                ? controller.GetProjectedAttributeValue(this.AttributeType)
                : currentValue;

            Color valueColor = hasStagedUpgrade
                ? _stagedValueColor
                : _realValueColor;

            string colorHex = ColorUtility.ToHtmlStringRGBA(valueColor);

            _attributeTitleText.text = $"{displayName} (<color=#{colorHex}>{displayedValue}</color>)";
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            _selectButton?.onClick.AddListener(this.HandleSelectClicked);
            _stageUpgradeButton?.onClick.AddListener(this.HandleStageUpgradeClicked);
        }
        protected override void UnbindInternalReferences()
        {
            _selectButton?.onClick.RemoveListener(this.HandleSelectClicked);
            _stageUpgradeButton?.onClick.RemoveListener(this.HandleStageUpgradeClicked);

            base.UnbindInternalReferences();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (_selectButton == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Attribute selection Button reference is missing.", this);
            }

            if (_stageUpgradeButton == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Stage Upgrade Button reference is missing.", this);
            }

            if (_upgradeCostText == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Upgrade Cost TMP reference is missing.", this);
            }
        }
    }
}
