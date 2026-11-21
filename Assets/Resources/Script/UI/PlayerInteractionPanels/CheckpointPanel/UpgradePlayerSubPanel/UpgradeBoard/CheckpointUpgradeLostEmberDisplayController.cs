using TMPro;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CheckpointUpgradeLostEmberDisplayController : CheckpointUpgradeBoardSubBehaviourBase
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private TMP_Text _quantityText;
        private TMP_Text QuantityText => _quantityText;

        // --- Config ---
        [Header("Colors")]
        [SerializeField] private Color _defaultValueColor = Color.white;
        [SerializeField] private Color _stagedValueColor = Color.yellow;

        // --- Public APIs ---
        public void RefreshDisplay()
        {
            if (!this.EnsureBound()) return;
            if (this.QuantityText == null) return;

            CheckpointUpgradePlayerSubPanelController controller = this.UpgradePlayerSubPanelController;

            int displayedAmount = controller.HasStagedUpgrades
                ? controller.GetProjectedLostEmbers()
                : controller.GetCurrentLostEmbers();

            this.QuantityText.text = displayedAmount.ToString();

            this.QuantityText.color = controller.HasStagedUpgrades
                ? _stagedValueColor
                : _defaultValueColor;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.QuantityText == null) _quantityText = this.GetComponentInChildren<TMP_Text>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.QuantityText == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(TMP_Text)} reference is missing.", this);
            }
        }
    }
}
