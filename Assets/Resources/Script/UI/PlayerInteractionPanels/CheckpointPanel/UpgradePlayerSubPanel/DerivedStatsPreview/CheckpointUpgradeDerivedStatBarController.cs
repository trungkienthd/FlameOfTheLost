using FlameOfTheLost.FlameOfTheLost;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CheckpointUpgradeDerivedStatBarController : CheckpointUpgradeDerivedStatsPreviewSubBehaviourBase
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _valueText;
        private Image Icon => _icon;
        private TMP_Text TitleText => _titleText;
        private TMP_Text ValueText => _valueText;

        // --- Config ---
        //  1. Derived Stat
        [Header("Derived Stat")]
        [SerializeField]
        private CharacterProgressionDerivedStatType _derivedStatType;
        public CharacterProgressionDerivedStatType DerivedStatType => _derivedStatType;

        //  2. Value Colors
        [Header("Value Colors")]
        [SerializeField] private Color _realValueColor = Color.green;
        [SerializeField] private Color _stagedValueColor = Color.red;
        private Color RealValueColor => _realValueColor;
        private Color StagedValueColor => _stagedValueColor;

        // --- Public APIs ---
        //  1. Refresh
        public void RefreshDisplay()
        {
            if (!this.EnsureBound()) return;

            CheckpointUpgradePlayerSubPanelController controller = this.UpgradePlayerSubPanelController;
            if (controller == null) return;

            CharacterProgressionDerivedStatEntry entry = controller.GetDerivedStatEntry(this.DerivedStatType);
            if (entry == null)
            {
                this.RefreshMissingEntryDisplay();
                return;
            }

            this.RefreshValueDisplay(controller, entry);
        }

        // --- Helpers ---
        //  1. Value Display
        private void RefreshValueDisplay(CheckpointUpgradePlayerSubPanelController controller, CharacterProgressionDerivedStatEntry entry)
        {
            if (_valueText == null) return;

            int baseValue = entry.BaseValue;

            int currentValue = controller.GetCurrentDerivedStatValue(this.DerivedStatType);
            int projectedValue = controller.GetProjectedDerivedStatValue(this.DerivedStatType);

            int currentBonus = controller.GetCurrentDerivedStatAttributeBonus(this.DerivedStatType);
            int projectedBonus = controller.GetProjectedDerivedStatAttributeBonus(this.DerivedStatType);

            bool hasStagedChange = currentValue != projectedValue;

            int displayedBonus = hasStagedChange
                ? projectedBonus
                : currentBonus;

            Color bonusColor = hasStagedChange
                ? this.StagedValueColor
                : this.RealValueColor;

            string bonusColorHex = ColorUtility.ToHtmlStringRGBA(bonusColor);

            _valueText.text = $"{baseValue} (<color=#{bonusColorHex}>+{displayedBonus}</color>)";
        }

        private void RefreshMissingEntryDisplay()
        {
            if (this.ValueText == null) return;

            this.ValueText.text = "-";
            this.ValueText.color = this.RealValueColor;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Icon == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Icon Image reference is missing.", this);
            }
            if (this.TitleText == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Title TMP reference is missing.", this);
            }
            if (this.ValueText == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Value TMP reference is missing.", this);
            }
        }
    }
}