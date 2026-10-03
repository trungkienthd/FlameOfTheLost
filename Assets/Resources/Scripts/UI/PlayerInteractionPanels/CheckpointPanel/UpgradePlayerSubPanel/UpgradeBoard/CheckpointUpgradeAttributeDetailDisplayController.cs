using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CheckpointUpgradeAttributeDetailDisplayController : CheckpointUpgradeBoardSubBehaviourBase
    {
        // --- References ---
        //  1. General
        [Header("References - General")]
        [SerializeField] private Image _attributeIcon;
        [SerializeField] private TMP_Text _attributeNameText;
        [SerializeField] private TMP_Text _attributeDescriptionText;

        //  2. Next Value
        [Header("References - Next Value")]
        [SerializeField] private TMP_Text _currentValueText;
        [SerializeField] private TMP_Text _nextValueText;

        //  3. Cost Scaling
        [Header("References - Cost Scaling")]
        [SerializeField] private TMP_Text _costScalingExplanationText;

        // --- Configs ---
        //  1. Visual
        [Header("Colors")]
        [SerializeField] private Color _defaultNextValueColor = new Color32(70, 0, 0, 255);
        [SerializeField] private Color _stagedNextValueColor = Color.green;

        // --- Public APIs ---
        public void RefreshDisplay()
        {
            if (!this.EnsureBound()) return;

            CheckpointUpgradePlayerSubPanelController controller = this.UpgradePlayerSubPanelController;

            CharacterProgressionAttributeType attributeType = controller.SelectedAttributeType;

            CharacterProgressionAttributeDisplayEntry displayEntry = controller.GetAttributeDisplayEntry(attributeType);

            Debug.Log($"Hello: {displayEntry.DisplayName} - {displayEntry.Description}");

            this.RefreshGeneralSection(displayEntry);
            this.RefreshNextValueSection(controller, attributeType);
            this.RefreshCostScalingSection(controller, attributeType, displayEntry);
        }

        // --- Helpers ---
        //  1. General
        private void RefreshGeneralSection(CharacterProgressionAttributeDisplayEntry displayEntry)
        {
            if (displayEntry == null) return;

            if (_attributeIcon != null)
            {
                _attributeIcon.sprite = displayEntry.Icon;
            }

            if (_attributeNameText != null)
            {
                _attributeNameText.text = displayEntry.DisplayName;
            }

            if (_attributeDescriptionText != null)
            {
                _attributeDescriptionText.text = displayEntry.Description;
            }
        }

        //  2. Next Value
        private void RefreshNextValueSection(CheckpointUpgradePlayerSubPanelController controller, CharacterProgressionAttributeType attributeType)
        {
            int currentValue = controller.GetCurrentAttributeValue(attributeType);

            int projectedValue = controller.GetProjectedAttributeValue(attributeType);

            int stagedCount = controller.GetStagedUpgradeCount(attributeType);

            if (_currentValueText != null)
            {
                _currentValueText.text = currentValue.ToString();
            }

            if (_nextValueText != null)
            {
                _nextValueText.text = projectedValue.ToString();

                _nextValueText.color = stagedCount > 0
                    ? _stagedNextValueColor
                    : _defaultNextValueColor;
            }
        }

        //  3. Cost Scaling
        private void RefreshCostScalingSection(CheckpointUpgradePlayerSubPanelController controller, CharacterProgressionAttributeType attributeType, CharacterProgressionAttributeDisplayEntry displayEntry)
        {
            if (_costScalingExplanationText == null) return;

            CharacterProgressionUpgradeConfig upgradeConfig = controller.ProgressionUpgradeHandler?.UpgradeConfig;

            CharacterProgressionAttributeUpgradeCostEntry costEntry = upgradeConfig?.GetUpgradeCostEntry(attributeType);

            if (costEntry != null)
            {
                _costScalingExplanationText.text = $"Base cost: {costEntry.BaseCost} Lost Embers. Cost increases by {costEntry.CostIncreasePerCurrentValue} for each current Attribute level.";
                return;
            }

            // Fallback authored explanation.
            _costScalingExplanationText.text = displayEntry != null
                ? displayEntry.CostScalingExplanation
                : string.Empty;
        }
    }
}