using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public sealed class CraftingPanelLostEmbersCostDisplayHandler : CraftingPanelSelectedRecipeEntryDetailsDisplaySubHandlerBase
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _quantityText;
        private Image Icon => _iconImage;
        private TMP_Text QuantityText => _quantityText;

        // --- Public APIs ---
        public void RefreshDisplay(CraftingRecipeProfile recipe, int craftCount)
        {
            if (recipe == null || craftCount <= 0)
            {
                this.ClearDisplay();
                return;
            }

            int totalCost = this.DetailsDisplayController.GetScaledQuantity(recipe.LostEmbersCost, craftCount);

            if (this.QuantityText != null)
            {
                this.QuantityText.text = totalCost.ToString();
            }
        }
        public void ClearDisplay()
        {
            if (this.QuantityText != null)
            {
                this.QuantityText.text = string.Empty;
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.Icon == null)
            {
                Transform iconTransform = this.transform.Find("Icon");
                if (iconTransform != null) _iconImage = iconTransform.GetComponent<Image>();
            }

            if (this.QuantityText == null)
            {
                Transform textTransform = this.transform.Find("QuantityText");
                if (textTransform != null) _quantityText = textTransform.GetComponent<TMP_Text>();
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ClearDisplay();
        }
    }
}
