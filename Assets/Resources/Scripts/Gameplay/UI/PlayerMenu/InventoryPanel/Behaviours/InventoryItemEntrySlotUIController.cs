using System;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class InventoryItemEntrySlotUIController : MonoBehaviour
    {
        [Header("References (Main)")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _mainIcon;
        private Button Button => _button;
        private Image BackgroundImage => _backgroundImage;
        private Image MainIcon => _mainIcon;

        [Header("References (Sub Slot)")]
        [SerializeField] private GameObject _subSlotRoot;
        [SerializeField] private Image _subSlotIcon;
        [SerializeField] private Image _subSlotIconBackup;
        [SerializeField] private RectTransform _subSlotIconRect;
        [SerializeField] private RectTransform _subSlotIconBackupRect;
        private GameObject SubSlotRoot => _subSlotRoot;
        private Image SubSlotIcon => _subSlotIcon;
        private Image SubSlotIconBackup => _subSlotIconBackup;
        private RectTransform SubSlotIconRectTransform => _subSlotIconRect;
        private RectTransform SubSlotIconBackupRectTransform => _subSlotIconBackupRect;

        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _selectedColor = Color.red;
        private Color NormalColor => _normalColor;
        private Color SelectedColor => _selectedColor;

        public event Action<InventoryItemEntrySlotUIController> OnClicked;

        public InventoryItemEntry BoundEntry { get; private set; }

        private void Awake()
        {
            if (this.MainIcon == null)
            {
                Transform mainIconTransform = this.transform.Find("MainIcon");
                if (mainIconTransform != null) _mainIcon = mainIconTransform.GetComponent<Image>();
            }

            if (this.Button == null) _button = this.GetComponent<Button>();
            if (this.Button != null) this.Button.onClick.AddListener(this.HandleClick);

            if (this.SubSlotRoot == null)
            {
                Transform subSlotTransform = this.transform.Find("SubSlot");
                if (subSlotTransform != null) _subSlotRoot = subSlotTransform.gameObject;
            }
            if (this.SubSlotIcon == null && this.SubSlotRoot != null)
            {
                Transform iconTransform = this.SubSlotRoot.transform.Find("Icon");
                if (iconTransform != null) _subSlotIcon = iconTransform.GetComponent<Image>();
            }
            if (this.SubSlotIconBackup == null && this.SubSlotRoot != null)
            {
                Transform iconBackupTransform = this.SubSlotRoot.transform.Find("Icon_Backup");
                if (iconBackupTransform != null) _subSlotIconBackup = iconBackupTransform.GetComponent<Image>();
            }

            if (this.SubSlotIconRectTransform == null && this.SubSlotIcon != null) _subSlotIconRect = this.SubSlotIcon.rectTransform;
            if (this.SubSlotIconBackupRectTransform == null && this.SubSlotIconBackup != null) _subSlotIconBackupRect = this.SubSlotIconBackup.rectTransform;
        }

        private void OnDestroy()
        {
            if (this.Button != null) this.Button.onClick.RemoveListener(this.HandleClick);
        }

        private void HandleClick()
        {
            if (this.BoundEntry == null) return;
            this.OnClicked?.Invoke(this);
        }

        public void Bind(InventoryItemEntry entry)
        {
            this.BoundEntry = entry;

            bool hasContent = entry != null && entry.Item != null;
            if (!hasContent)
            {
                this.ResetBindings();
                return;
            }

            this.SetMainIcon(entry.Icon, true);

            if (this.Button != null)
            {
                this.Button.interactable = true;
            }

            switch (entry.Item.ItemType)
            {
                case ItemType.ConsumableItem:
                    this.SetSubSlotVisible(true);
                    this.ConfigureQuantitySubSlot(Mathf.Clamp(entry.Quantity, 0, 99), hasContent);
                    break;

                case ItemType.Weapon:
                case ItemType.LightSource:
                case ItemType.KeyItem:
                default:
                    this.SetSubSlotVisible(false);
                    break;
            }

            this.SetSelected(false);
        }

        public void ResetBindings()
        {
            this.BoundEntry = null;

            if (this.Button != null)
            {
                this.Button.interactable = false;
            }

            this.SetMainIcon(null, false);
            this.SetSelected(false);
            this.SetSubSlotVisible(false);

            if (this.SubSlotIcon != null)
            {
                this.SubSlotIcon.sprite = null;
                this.SubSlotIcon.enabled = false;
            }
            if (this.SubSlotIconBackup != null)
            {
                this.SubSlotIconBackup.sprite = null;
                this.SubSlotIconBackup.enabled = false;
            }
        }

        public void SetSelected(bool selected)
        {
            if (this.BackgroundImage == null) return;
            this.BackgroundImage.color = selected ? this.SelectedColor : this.NormalColor;
        }

        private void SetMainIcon(Sprite sprite, bool enabled)
        {
            if (this.MainIcon != null)
            {
                this.MainIcon.sprite = sprite;
                this.MainIcon.enabled = enabled;
            }
        }

        private void SetSubSlotVisible(bool visible)
        {
            if (this.SubSlotRoot != null)
            {
                this.SubSlotRoot.SetActive(visible);
            }
        }

        private void ConfigureQuantitySubSlot(int quantity, bool hasContent)
        {
            if (!hasContent)
            {
                if (this.SubSlotIcon != null)
                {
                    this.SubSlotIcon.sprite = null;
                    this.SubSlotIcon.enabled = false;
                }

                if (this.SubSlotIconBackup != null)
                {
                    this.SubSlotIconBackup.sprite = null;
                    this.SubSlotIconBackup.enabled = false;
                }

                return;
            }

            quantity = Mathf.Clamp(quantity, 0, 99);
            if (quantity <= 9)
            {
                if (this.SubSlotIconRectTransform != null)
                {
                    this.SubSlotIconRectTransform.sizeDelta = new Vector2(18f, 18f);
                    this.SubSlotIconRectTransform.anchoredPosition = Vector2.zero;
                }

                this.SubSlotIcon.sprite = SharedSpritesUIUtility.GetDigitSprite(quantity);
                this.SubSlotIcon.enabled = true;

                if (this.SubSlotIconBackup != null)
                {
                    this.SubSlotIconBackup.sprite = null;
                    this.SubSlotIconBackup.enabled = false;
                }

                return;
            }

            int firstDigit = quantity / 10;
            int secondDigit = quantity % 10;

            if (this.SubSlotIconRectTransform != null)
            {
                this.SubSlotIconRectTransform.sizeDelta = new Vector2(12f, 12f);
                this.SubSlotIconRectTransform.anchoredPosition = new Vector2(-7.5f, 0f);
            }

            if (this.SubSlotIconBackupRectTransform != null)
            {
                this.SubSlotIconBackupRectTransform.sizeDelta = new Vector2(12f, 12f);
                this.SubSlotIconBackupRectTransform.anchoredPosition = new Vector2(7.5f, 0f);
            }

            this.SubSlotIcon.sprite = SharedSpritesUIUtility.GetDigitSprite(firstDigit);
            this.SubSlotIcon.enabled = true;

            if (this.SubSlotIconBackup != null)
            {
                this.SubSlotIconBackup.sprite = SharedSpritesUIUtility.GetDigitSprite(secondDigit);
                this.SubSlotIconBackup.enabled = true;
            }
        }
    }
}
