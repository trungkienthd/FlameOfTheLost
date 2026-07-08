using UnityEngine;
using TMPro;

namespace FlameOfTheLost
{
    public class EquippedLoadoutQuantityTextSlotController : EquippedLoadoutSlotController
    {
        [Header("References - Quantity Slot")]
        [SerializeField] private TMP_Text _quantityText;
        private TMP_Text QuantityText => _quantityText;

        [Header("Quantity Text Configuration")]
        [SerializeField] private int _singleDigitFontSize = 26;
        [SerializeField] private int _doubleDigitFontSize = 20;
        private int SingleDigitFontSize => _singleDigitFontSize;
        private int DoubleDigitFontSize => _doubleDigitFontSize;

        // --- Public APIs ---
        public void BindProjectileItem(InventoryItemEntry projectileEntry, int slotIndex, bool isSelected, bool isMainButtonInteractable)
        {
            this.ResetBindings();

            this.SlotType = LoadoutPanelSlotType.Projectile;
            this.SlotIndex = slotIndex;
            this.BoundProjectileEntry = projectileEntry;

            this.IsActive = false;
            this.IsSelected = isSelected;

            bool hasProjectile =
                projectileEntry != null &&
                projectileEntry.Item != null &&
                projectileEntry.Quantity > 0;

            this.SetMainIcon
            (
                sprite: hasProjectile ? projectileEntry.Icon : null,
                hasContent: hasProjectile
            );

            this.SetSubSlotVisible(true);

            this.ConfigureQuantityText
            (
                quantity: hasProjectile ? projectileEntry.Quantity : 0,
                hasContent: hasProjectile
            );

            this.SetMainButtonInteractable(isMainButtonInteractable);

            // Quantity SubSlot is display-only.
            this.SetSubSlotButtonInteractable(false);

            this.RefreshSlotColor();
        }

        public void BindConsumableItem(InventoryItemEntry consumableEntry, int slotIndex, bool isActive, bool isSelected, bool isMainButtonInteractable)
        {
            this.ResetBindings();

            this.SlotType = LoadoutPanelSlotType.Consumable;
            this.SlotIndex = slotIndex;
            this.BoundConsumableEntry = consumableEntry;

            this.IsActive = isActive;
            this.IsSelected = isSelected;

            bool hasConsumable =
                consumableEntry != null &&
                consumableEntry.Item != null &&
                consumableEntry.Quantity > 0;

            this.SetMainIcon
            (
                sprite: hasConsumable ? consumableEntry.Icon : null,
                hasContent: hasConsumable
            );

            this.SetSubSlotVisible(true);

            this.ConfigureQuantityText
            (
                quantity: hasConsumable ? consumableEntry.Quantity : 0,
                hasContent: hasConsumable
            );

            this.SetMainButtonInteractable(isMainButtonInteractable);

            // Quantity SubSlot is display-only.
            this.SetSubSlotButtonInteractable(false);

            this.RefreshSlotColor();
        }

        // --- Helpers ---
        protected override void ClearSubSlotContent()
        {
            base.ClearSubSlotContent();

            if (this.QuantityText == null) return;

            this.QuantityText.text = string.Empty;
            this.QuantityText.enabled = false;
            this.QuantityText.gameObject.SetActive(false);
        }

        private void ConfigureQuantityText(int quantity, bool hasContent)
        {
            if (this.QuantityText == null) return;

            if (!hasContent)
            {
                this.QuantityText.text = string.Empty;
                this.QuantityText.enabled = false;
                this.QuantityText.gameObject.SetActive(false);
                return;
            }

            quantity = Mathf.Clamp(quantity, 0, 99);

            this.QuantityText.text = quantity.ToString();
            this.QuantityText.fontSize = quantity <= 9
                ? this.SingleDigitFontSize
                : this.DoubleDigitFontSize;

            this.QuantityText.enabled = true;
            this.QuantityText.gameObject.SetActive(true);
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.QuantityText == null && this.SubSlotRoot != null)
            {
                Transform quantityTextTransform = this.SubSlotRoot.transform.Find("QuantityText");
                if (quantityTextTransform != null)
                {
                    _quantityText = quantityTextTransform.GetComponent<TMP_Text>();
                }
            }
        }
    }
}
