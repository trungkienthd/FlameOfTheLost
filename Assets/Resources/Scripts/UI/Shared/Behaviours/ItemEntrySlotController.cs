using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class ItemEntrySlotController : FOTLMonoBehaviour
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
        [SerializeField] private TMP_Text _subSlotQuantityText;
        private GameObject SubSlotRoot => _subSlotRoot;
        private Image SubSlotIcon => _subSlotIcon;
        private TMP_Text SubSlotQuantityText => _subSlotQuantityText;

        [Header("Quantity Text")]
        [SerializeField] private float _singleDigitQuantityFontSize = 26f;
        [SerializeField] private float _doubleDigitQuantityFontSize = 20f;
        private float SingleDigitQuantityFontSize => _singleDigitQuantityFontSize;
        private float DoubleDigitQuantityFontSize => _doubleDigitQuantityFontSize;

        [Header("Selection")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _selectedColor = Color.red;
        private Color NormalColor => _normalColor;
        private Color SelectedColor => _selectedColor;

        public event Action<ItemEntrySlotController> OnClicked;

        public InventoryItemEntry BoundEntry { get; private set; }

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
                case ItemType.Weapon:
                    this.ConfigureWeaponSubSlot(entry);
                    break;

                case ItemType.Projectile:
                case ItemType.ConsumableItem:
                    this.ConfigureQuantitySubSlot(entry.Quantity);
                    break;

                case ItemType.LightSource:
                case ItemType.KeyItem:
                default:
                    this.ClearSubSlot();
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
            this.ClearSubSlot();
            this.SetSubSlotVisible(false);
        }

        public void SetSelected(bool selected)
        {
            if (this.BackgroundImage == null) return;
            this.BackgroundImage.color = selected ? this.SelectedColor : this.NormalColor;
        }

        private void ClearSubSlot()
        {
            if (this.SubSlotIcon != null)
            {
                this.SubSlotIcon.sprite = null;
                this.SubSlotIcon.enabled = false;
            }

            if (this.SubSlotQuantityText != null)
            {
                this.SubSlotQuantityText.text = string.Empty;
                this.SubSlotQuantityText.enabled = false;
            }
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

        private void HandleClick()
        {
            if (this.BoundEntry == null) return;
            this.OnClicked?.Invoke(this);
        }

        private void ConfigureWeaponSubSlot(InventoryItemEntry entry)
        {
            this.ClearSubSlot();

            if (entry == null || entry.Item is not WeaponProfile weaponProfile)
            {
                this.SetSubSlotVisible(false);
                return;
            }

            Sprite innateSpecialSkillIcon = weaponProfile.InnateSpecialSkill != null
                ? weaponProfile.InnateSpecialSkill.Icon
                : null;

            if (innateSpecialSkillIcon == null)
            {
                this.SetSubSlotVisible(false);
                return;
            }

            this.SetSubSlotVisible(true);

            if (this.SubSlotIcon != null)
            {
                this.SubSlotIcon.sprite = innateSpecialSkillIcon;
                this.SubSlotIcon.enabled = true;
            }
        }

        private void ConfigureQuantitySubSlot(int quantity)
        {
            this.ClearSubSlot();
            this.SetSubSlotVisible(true);

            quantity = Mathf.Clamp(quantity, 0, 99);

            if (this.SubSlotQuantityText == null) return;

            this.SubSlotQuantityText.text = quantity.ToString();
            this.SubSlotQuantityText.fontSize = quantity <= 9
                ? this.SingleDigitQuantityFontSize
                : this.DoubleDigitQuantityFontSize;

            this.SubSlotQuantityText.enabled = true;
        }

        protected override void CacheInternalReferences()
        {
            if (this.BackgroundImage == null) _backgroundImage = this.GetComponent<Image>();

            if (this.Button == null) _button = this.GetComponent<Button>();

            if (this.MainIcon == null)
            {
                Transform mainIconTransform = this.transform.Find("Icon");
                if (mainIconTransform != null)
                {
                    _mainIcon = mainIconTransform.GetComponent<Image>();
                }
            }

            if (this.SubSlotRoot == null)
            {
                Transform subSlotTransform = this.transform.Find("SubSlot");
                if (subSlotTransform != null)
                {
                    _subSlotRoot = subSlotTransform.gameObject;
                }
            }

            if (this.SubSlotIcon == null && this.SubSlotRoot != null)
            {
                Transform iconTransform = this.SubSlotRoot.transform.Find("Icon");
                if (iconTransform != null)
                {
                    _subSlotIcon = iconTransform.GetComponent<Image>();
                }
            }

            if (this.SubSlotQuantityText == null && this.SubSlotRoot != null)
            {
                Transform quantityTextTransform = this.SubSlotRoot.transform.Find("QuantityText");
                if (quantityTextTransform != null)
                {
                    _subSlotQuantityText = quantityTextTransform.GetComponent<TMP_Text>();
                }
            }
        }

        protected override void BindInternalReferences()
        {
            if (this.Button != null)
            {
                this.Button.onClick.AddListener(this.HandleClick);
            }
        }

        protected override void InitializeInternalState()
        {
            this.ResetBindings();
        }

        protected override void UnbindInternalReferences()
        {
            if (this.Button != null)
            {
                this.Button.onClick.RemoveListener(this.HandleClick);
            }
        }
    }
}
