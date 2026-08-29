using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public sealed class CraftingPanelCraftQuantityHandler : CraftingPanelSelectedRecipeEntryDetailsDisplaySubHandlerBase
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private Button _buttonDecrease;
        [SerializeField] private Button _buttonIncrease;
        [SerializeField] private TMP_Text _quantityText;
        private Button ButtonDecrease => _buttonDecrease;
        private Button ButtonIncrease => _buttonIncrease;
        private TMP_Text QuantityText => _quantityText;

        // --- Public APIs ---
        public void RefreshDisplay(int craftCount)
        {
            if (craftCount <= 0)
            {
                this.ClearDisplay();
                return;
            }

            if (this.QuantityText   != null) this.QuantityText.text = craftCount.ToString();
            if (this.ButtonDecrease != null) this.ButtonDecrease.interactable = craftCount > 1;
            if (this.ButtonIncrease != null) this.ButtonIncrease.interactable = craftCount < int.MaxValue;
        }
        public void ClearDisplay()
        {
            if (this.QuantityText   != null) this.QuantityText.text = "1";
            if (this.ButtonDecrease != null) this.ButtonDecrease.interactable = false;
            if (this.ButtonIncrease != null) this.ButtonIncrease.interactable = false;
        }

        // --- Helpers ---
        private void HandleDecreaseClicked()
        {
            if (!this.EnsureBound()) return;

            this.DetailsDisplayController.DecreaseCraftCount();
        }
        public void HandleIncreaseClicked()
        {
            if (!this.EnsureBound()) return;

            this.DetailsDisplayController.IncreaseCraftCount();
        }
        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.ButtonDecrease == null)
            {
                Transform buttonTransform = this.transform.Find("ButtonDecrease");
                if (buttonTransform != null) _buttonDecrease = buttonTransform.GetComponent<Button>();
            }

            if (this.ButtonIncrease == null)
            {
                Transform buttonTransform = this.transform.Find("ButtonIncrease");
                if (buttonTransform != null) _buttonIncrease = buttonTransform.GetComponent<Button>();
            }

            if (this.QuantityText == null)
            {
                Transform quantityTransform = this.transform.Find("QuantityText");
                if (quantityTransform != null) _quantityText = quantityTransform.GetComponent<TMP_Text>();
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.ButtonDecrease != null) this.ButtonDecrease.onClick.AddListener(this.HandleDecreaseClicked);
            if (this.ButtonIncrease != null) this.ButtonIncrease.onClick.AddListener(this.HandleIncreaseClicked);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ClearDisplay();
        }
        protected override void UnbindInternalReferences()
        {
            if (this.ButtonDecrease != null) this.ButtonDecrease.onClick.RemoveListener(this.HandleDecreaseClicked);
            if (this.ButtonIncrease != null) this.ButtonIncrease.onClick.RemoveListener(this.HandleIncreaseClicked);

            base.UnbindInternalReferences();
        }
    }
}