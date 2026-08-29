using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CraftingPanelResultDisplayHandler : CraftingPanelSelectedRecipeEntryDetailsDisplaySubHandlerBase
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private Image _resultIconImage;
        [SerializeField] private TMP_Text _resultNameAndQuantityText;
        private Image ResultIcon => _resultIconImage;
        private TMP_Text ResultNameAndQuantityText => _resultNameAndQuantityText;

        // --- Public APIs ---
        public void RefreshDisplay(CraftingRecipeProfile recipe, int craftCount)
        {
            if (recipe == null || recipe.ResultItem == null || craftCount <= 0)
            {
                this.ClearDisplay();
                return;
            }

            int totalResultQuantity = this.DetailsDisplayController.GetScaledQuantity(recipe.ResultQuantity, craftCount);

            if (this.ResultIcon != null)
            {
                this.ResultIcon.sprite = recipe.ResultItem.Icon;
                this.ResultIcon.enabled = recipe.ResultItem.Icon != null;
            }

            if (this.ResultNameAndQuantityText != null)
            {
                this.ResultNameAndQuantityText.text = $"{recipe.ResultItem.DisplayName} ({totalResultQuantity}x)";
            }
        }

        public void ClearDisplay()
        {
            if (this.ResultIcon != null)
            {
                this.ResultIcon.sprite = null;
                this.ResultIcon.enabled = false;
            }

            if (this.ResultNameAndQuantityText != null)
            {
                this.ResultNameAndQuantityText.text = string.Empty;
            }
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.ResultIcon == null)
            {
                Transform iconTransform = this.transform.Find("ResultSlot/Icon");
                if (iconTransform != null) _resultIconImage = iconTransform.GetComponent<Image>();
            }

            if (this.ResultNameAndQuantityText == null)
            {
                Transform textTransform = this.transform.Find("ResultNameAndQuantity");
                if (textTransform != null) _resultNameAndQuantityText = textTransform.GetComponent<TMP_Text>();
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ClearDisplay();
        }
    }
}
