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
                    _attributeTitleText.text = displayEntry.DisplayName;
                }
            }

            bool selected = controller.SelectedAttributeType == this.AttributeType;

            if (_barImage != null)
            {
                _barImage.color = selected
                    ? _selectedColor
                    : _normalColor;
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
        //  1. Event Handlers
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
