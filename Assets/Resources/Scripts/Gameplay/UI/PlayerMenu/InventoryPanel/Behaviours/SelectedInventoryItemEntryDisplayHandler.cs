using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class SelectedInventoryItemEntryDisplayHandler : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _displayNameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Sprite _defaultSprite;
        private Image Icon => _iconImage;
        private TMP_Text DisplayNameText => _displayNameText;
        private TMP_Text DescriptionText => _descriptionText;
        private Sprite DefaultSprite => _defaultSprite;

        public InventoryItemEntry CurrentSelectedEntry { get; private set; }

        private void Awake()
        {
            if (this.Icon == null)
            {
                Transform icon = this.transform.Find("Icon");
                if (icon != null) _iconImage = icon.GetComponent<Image>();
            }
        }

        public void SetSelectedEntry(InventoryItemEntry entry, IReadOnlyList<InventoryItemEntry> filteredEntries)
        {
            if (entry == null || filteredEntries == null || !filteredEntries.Contains(entry))
            {
                this.ClearSelectedEntry();
                return;
            }

            this.CurrentSelectedEntry = entry;
            this.Refresh();
        }

        public void ValidateSelection(IReadOnlyList<InventoryItemEntry> filteredEntries)
        {
            if (this.CurrentSelectedEntry == null) return;

            if (filteredEntries == null || !filteredEntries.Contains(this.CurrentSelectedEntry))
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

        private void Refresh()
        {
            if (this.CurrentSelectedEntry == null || this.CurrentSelectedEntry.Item == null)
            {
                if (this.Icon != null)
                {
                    this.Icon.sprite = this.DefaultSprite;
                    this.Icon.color = new Color(1, 0.48f, 0, 0.4f);
                }
                if (this.DisplayNameText != null) this.DisplayNameText.text = string.Empty;
                if (this.DescriptionText != null) this.DescriptionText.text = string.Empty;
                return;
            }

            if (this.Icon != null)
            {
                this.Icon.sprite = this.CurrentSelectedEntry.Icon != null
                    ? this.CurrentSelectedEntry.Icon
                    : this.DefaultSprite;
                this.Icon.color = new Color(1, 1, 1, 1);
            }

            if (this.DisplayNameText != null)
            {
                this.DisplayNameText.text = this.CurrentSelectedEntry.Item.DisplayName ?? string.Empty;
            }

            if (this.DescriptionText != null)
            {
                this.DescriptionText.text = this.CurrentSelectedEntry.Item.Description ?? string.Empty;
            }
        }
    }
}
