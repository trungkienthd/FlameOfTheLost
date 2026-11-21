using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public sealed class InventoryPanelSelectedItemEntryDisplayHandler : InventoryPanelSubBehaviourBase
    {
        // --- References ---
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _displayNameText;
        [SerializeField] private TMP_Text _descriptionText;
        private Image Icon => _iconImage;
        private TMP_Text DisplayNameText => _displayNameText;
        private TMP_Text DescriptionText => _descriptionText;

        // --- Configs ---
        [SerializeField] private Sprite _defaultSprite;
        private Sprite DefaultSprite => _defaultSprite;

        // --- Public APIs ---
        public void DisplayEntry(InventoryItemEntry entry)
        {
            if (entry == null || entry.Item == null)
            {
                this.ClearDisplay();
                return;
            }

            if (this.Icon != null)
            {
                this.Icon.sprite = entry.Icon != null ? entry.Icon : this.DefaultSprite;
                this.Icon.color = Color.white;
            }

            if (this.DisplayNameText != null) this.DisplayNameText.text = entry.Item.DisplayName ?? string.Empty;
            if (this.DescriptionText != null) this.DescriptionText.text = entry.Item.Description ?? string.Empty;
        }
        public void ClearDisplay()
        {
            if (this.Icon != null)
            {
                this.Icon.sprite = this.DefaultSprite;
                this.Icon.color = new Color(1f, 0.48f, 0f, 0.4f);
            }

            if (this.DisplayNameText != null) this.DisplayNameText.text = string.Empty;
            if (this.DescriptionText != null) this.DescriptionText.text = string.Empty;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.Icon == null)
            {
                Transform icon = this.transform.Find("Icon");
                if (icon != null) _iconImage = icon.GetComponent<Image>();
            }
            if (this.DisplayNameText == null)
            {
                Transform displayName = this.transform.Find("DisplayName");
                if (displayName != null) _displayNameText = displayName.GetComponent<TMP_Text>();
            }
            if (this.DescriptionText == null)
            {
                Transform description = this.transform.Find("Description");
                if (description != null) _descriptionText = description.GetComponent<TMP_Text>();
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Icon == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelSelectedItemEntryDisplayHandler)}] {nameof(this.Icon)} is missing.", this);
            }
            if (this.DisplayNameText == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelSelectedItemEntryDisplayHandler)}] {nameof(this.DisplayNameText)} is missing.", this);
            }
            if (this.DescriptionText == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(InventoryPanelSelectedItemEntryDisplayHandler)}] {nameof(this.DescriptionText)} is missing.", this);
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ClearDisplay();
        }
    }
}
