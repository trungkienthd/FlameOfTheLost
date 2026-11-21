using TMPro;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class UICanvasHUDCombatLoadoutItemSlotHandlerBase : UICanvasHUDCombatLoadoutSlotHandlerBase
    {
        // --- States ---
        [Header("References - Quantity")]
        [SerializeField] private TMP_Text _quantityText;
        protected TMP_Text QuantityText => _quantityText;

        [Header("Quantity Text")]
        [SerializeField] private float _singleDigitQuantityFontSize = 26f;
        [SerializeField] private float _doubleDigitQuantityFontSize = 20f;
        private float SingleDigitQuantityFontSize => _singleDigitQuantityFontSize;
        private float DoubleDigitQuantityFontSize => _doubleDigitQuantityFontSize;

        // --- Public APIs ---
        public override void RefreshSlot(CharacterCombatLoadoutController combatLoadoutController)
        {
            if (combatLoadoutController == null)
            {
                this.RefreshItemSlot(null, null);
                return;
            }

            Sprite itemIcon = this.GetItemIcon(combatLoadoutController);
            InventoryItemEntry itemEntry = this.GetItemEntry(combatLoadoutController);

            this.RefreshItemSlot(itemIcon, itemEntry);
        }

        // --- Helpers ---
        protected abstract Sprite GetItemIcon(CharacterCombatLoadoutController combatLoadoutController);

        protected abstract InventoryItemEntry GetItemEntry(CharacterCombatLoadoutController combatLoadoutController);

        private void RefreshItemSlot(Sprite itemIcon, InventoryItemEntry itemEntry)
        {
            this.RefreshMainIcon(itemIcon);
            this.RefreshQuantityText(itemIcon, itemEntry);
        }

        private void RefreshQuantityText(Sprite itemIcon, InventoryItemEntry itemEntry)
        {
            if (this.QuantityText == null) return;

            bool hasItemIcon = itemIcon != null;
            bool hasEntry = itemEntry != null;

            if (!hasItemIcon || !hasEntry)
            {
                this.DisableQuantityText();
                return;
            }

            int quantity = itemEntry.Quantity;

            if (quantity <= 0)
            {
                this.DisableQuantityText();
                return;
            }

            if (quantity > 99)
            {
                quantity = 99;
            }

            this.QuantityText.enabled = true;
            this.QuantityText.text = quantity.ToString();
            this.ApplyQuantityTextFontSize(quantity);
        }

        private void ApplyQuantityTextFontSize(int quantity)
        {
            if (this.QuantityText == null) return;

            if (quantity <= 9)
            {
                this.QuantityText.fontSize = this.SingleDigitQuantityFontSize;
            }
            else
            {
                this.QuantityText.fontSize = this.DoubleDigitQuantityFontSize;
            }
        }

        private void DisableQuantityText()
        {
            if (this.QuantityText == null) return;

            this.QuantityText.enabled = false;
            this.QuantityText.text = string.Empty;
        }

        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.QuantityText == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Quantity Text is not set.");
            }
        }
    }
}