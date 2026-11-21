using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class ItemContainerPanelSelectedItemEntryDisplayHandler : ItemContainerPanelSubHandlerBase
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _displayNameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Button _pickUpButton;
        private Image Icon => _iconImage;
        private TMP_Text DisplayNameText => _displayNameText;
        private TMP_Text DescriptionText => _descriptionText;
        private Button PickUpButton => _pickUpButton;

        // --- Configuration ---
        [Header("Display")]
        [SerializeField] private Sprite _defaultSprite;
        private Sprite DefaultSprite => _defaultSprite;

        // --- Public APIs ---
        public void RefreshDisplay(InventoryItemEntry selectedEntry)
        {
            bool hasSelection = selectedEntry != null && selectedEntry.Item != null;

            if (!hasSelection)
            {
                this.ClearDisplay();
                return;
            }

            if (this.Icon != null)
            {
                this.Icon.sprite = selectedEntry.Icon != null
                    ? selectedEntry.Icon
                    : this.DefaultSprite;

                this.Icon.color = Color.white;
            }

            if (this.DisplayNameText != null)
            {
                this.DisplayNameText.text = selectedEntry.Item.DisplayName ?? string.Empty;
            }

            if (this.DescriptionText != null)
            {
                this.DescriptionText.text = selectedEntry.Item.Description ?? string.Empty;
            }

            if (this.PickUpButton != null)
            {
                this.PickUpButton.interactable = true;
            }
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
            if (this.PickUpButton != null) this.PickUpButton.interactable = false;
        }

        // --- Helpers ---
        private void HandlePickUpClicked()
        {
            if (!this.EnsureBound()) return;

            this.ItemContainerPanelController.TryPickUpSelectedEntry();
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

            if (this.DisplayNameText == null)
            {
                Transform displayNameTransform = this.transform.Find("DisplayName");
                if (displayNameTransform != null) _displayNameText =  displayNameTransform.GetComponent<TMP_Text>();
            }

            if (this.DescriptionText == null)
            {
                Transform descriptionTransform = this.transform.Find("Description");
                if (descriptionTransform != null) _descriptionText = descriptionTransform.GetComponent<TMP_Text>();
            }

            if (this.PickUpButton == null)
            {
                Transform pickUpButtonTransform = this.transform.Find("ButtonPickUp");
                if (pickUpButtonTransform != null) _pickUpButton = pickUpButtonTransform.GetComponent<Button>();
            }
        }

        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.PickUpButton != null)
            {
                this.PickUpButton.onClick.AddListener(this.HandlePickUpClicked);
            }
        }

        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.ClearDisplay();
        }

        protected override void UnbindInternalReferences()
        {
            if (this.PickUpButton != null)
            {
                this.PickUpButton.onClick.RemoveListener(this.HandlePickUpClicked);
            }

            base.UnbindInternalReferences();
        }
    }
}
