using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class ItemContainerPanelSelectedItemEntryDisplayHandler : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _displayNameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Button _pickUpButton;
        [SerializeField] private Sprite _defaultSprite;
        private Image Icon => _iconImage;
        private TMP_Text DisplayNameText => _displayNameText;
        private TMP_Text DescriptionText => _descriptionText;
        private Button PickUpButton => _pickUpButton;
        private Sprite DefaultSprite => _defaultSprite;

        public InventoryItemEntry CurrentSelectedEntry { get; private set; }

        public event Action<InventoryItemEntry> OnPickUpClicked;

        private void Awake()
        {
            this.CacheReferences();

            if (this.PickUpButton != null)
            {
                this.PickUpButton.onClick.AddListener(this.HandlePickUpClicked);
            }

            this.Refresh();
        }

        private void OnDestroy()
        {
            if (this.PickUpButton != null)
            {
                this.PickUpButton.onClick.RemoveListener(this.HandlePickUpClicked);
            }
        }

        public void SetSelectedEntry(InventoryItemEntry entry, List<InventoryItemEntry> validEntries)
        {
            if (entry == null || validEntries == null || !validEntries.Contains(entry))
            {
                this.ClearSelectedEntry();
                return;
            }

            this.CurrentSelectedEntry = entry;
            this.Refresh();
        }

        public void ValidateSelection(List<InventoryItemEntry> validEntries)
        {
            if (this.CurrentSelectedEntry == null) return;

            if (validEntries == null || !validEntries.Contains(this.CurrentSelectedEntry))
            {
                this.ClearSelectedEntry();
                return;
            }

            this.Refresh();
        }

        public void ClearSelectedEntry()
        {
            this.CurrentSelectedEntry = null;
            this.Refresh();
        }

        private void HandlePickUpClicked()
        {
            if (this.CurrentSelectedEntry == null) return;
            this.OnPickUpClicked?.Invoke(this.CurrentSelectedEntry);
        }

        private void Refresh()
        {
            bool hasSelection = this.CurrentSelectedEntry != null && this.CurrentSelectedEntry.Item != null;

            if (!hasSelection)
            {
                if (this.Icon != null)
                {
                    this.Icon.sprite = this.DefaultSprite;
                    this.Icon.color = new Color(1f, 0.48f, 0f, 0.4f);
                }

                if (this.DisplayNameText != null) this.DisplayNameText.text = string.Empty;
                if (this.DescriptionText != null) this.DescriptionText.text = string.Empty;
                if (this.PickUpButton != null) this.PickUpButton.interactable = false;

                return;
            }

            if (this.Icon != null)
            {
                this.Icon.sprite = this.CurrentSelectedEntry.Icon != null
                    ? this.CurrentSelectedEntry.Icon
                    : this.DefaultSprite;

                this.Icon.color = Color.white;
            }

            if (this.DisplayNameText != null)
            {
                this.DisplayNameText.text = this.CurrentSelectedEntry.Item.DisplayName ?? string.Empty;
            }

            if (this.DescriptionText != null)
            {
                this.DescriptionText.text = this.CurrentSelectedEntry.Item.Description ?? string.Empty;
            }

            if (this.PickUpButton != null)
            {
                this.PickUpButton.interactable = true;
            }
        }

        private void CacheReferences()
        {
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

            if (this.PickUpButton == null)
            {
                Transform buttonPickUp = this.transform.Find("ButtonPickUp");
                if (buttonPickUp != null) _pickUpButton = buttonPickUp.GetComponent<Button>();
            }
        }
    }
}
