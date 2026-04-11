using System;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class EquippedLoadoutSlotUIController : MonoBehaviour
    {
        [Header("References (Main Slot)")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _slotImage;
        [SerializeField] private Image _icon;
        private Button Button => _button;
        private Image SlotImage => _slotImage;
        private Image MainIcon => _icon;

        [Header("References (Sub Slot)")]
        [SerializeField] private GameObject _subSlotRoot;
        [SerializeField] private Button _subSlotButton;
        [SerializeField] private Image _subSlotIcon;
        [SerializeField] private Image _subSlotIconBackup;
        [SerializeField] private RectTransform _subSlotIconRect;
        [SerializeField] private RectTransform _subSlotIconBackupRect;
        private Button SubSlotButton => _subSlotButton;
        private GameObject SubSlotRoot => _subSlotRoot;
        private Image SubSlotIcon => _subSlotIcon;
        private Image SubSlotIconBackup => _subSlotIconBackup;
        private RectTransform SubSlotIconRectTransform => _subSlotIconRect;
        private RectTransform SubSlotIconBackupRectTransform => _subSlotIconBackupRect;

        [Header("Slot Colors")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _activeColor = Color.green;
        [SerializeField] private Color _selectedColor = Color.red;
        private Color NormalColor => _normalColor;
        private Color ActiveColor => _activeColor;
        private Color SelectedColor => _selectedColor;

        public LoadoutPanelSlotType SlotType { get; private set; }
        public int SlotIndex { get; private set; } = -1;

        public InventoryItemEntry BoundWeaponEntry { get; private set; }
        public InventoryItemEntry BoundConsumableEntry { get; private set; }
        public InventoryItemEntry BoundInteractableItemEntry { get; private set; }
        public SkillProfile BoundWeaponSpecialSkill { get; private set; }

        public bool IsSelected { get; private set; }
        public bool IsActive { get; private set; }

        public event Action<EquippedLoadoutSlotUIController> OnClicked;
        public event Action<EquippedLoadoutSlotUIController> OnSubSlotClicked;

        private void Awake()
        {
            if (this.MainIcon == null)
            {
                Transform iconTransform = this.transform.Find("Icon");
                if (iconTransform != null) _icon = iconTransform.GetComponent<Image>();
            }

            if (this.Button == null) _button = this.GetComponent<Button>();
            if (this.Button != null) this.Button.onClick.AddListener(this.HandleClick);

            if (this.SubSlotRoot == null)
            {
                Transform subSlotTransform = this.transform.Find("SubSlot");
                if (subSlotTransform != null) _subSlotRoot = subSlotTransform.gameObject;
            }
            if (this.SubSlotButton == null && this.SubSlotRoot != null) _subSlotButton = this.SubSlotRoot.GetComponent<Button>();
            if (this.SubSlotButton != null) this.SubSlotButton.onClick.AddListener(this.HandleSpecialSkillClick);
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

            if (this.SubSlotIconRectTransform == null) _subSlotIconRect = this.SubSlotIcon.rectTransform;
            if (this.SubSlotIconBackupRectTransform == null) _subSlotIconBackupRect = this.SubSlotIconBackup.rectTransform;
        }

        private void OnDestroy()
        {
            if (this.Button != null) this.Button.onClick.RemoveListener(this.HandleClick);
            if (this.SubSlotButton != null) this.SubSlotButton.onClick.RemoveListener(this.HandleSpecialSkillClick);
        }

        public void BindWeapon(InventoryItemEntry weaponEntry, SkillProfile activeSpecialSkill, int slotIndex, bool isActive, bool isSelected, bool isMainButtonInteractable, bool isSubSlotButtonInteractable)
        {
            this.ResetBindings();

            this.SlotType = LoadoutPanelSlotType.Weapon;
            this.SlotIndex = slotIndex;
            this.BoundWeaponEntry = weaponEntry;
            this.BoundWeaponSpecialSkill = activeSpecialSkill;

            this.IsActive = isActive;
            this.IsSelected = isSelected;

            bool hasContent = weaponEntry != null && weaponEntry.Item != null;
            this.SetMainIcon(hasContent ? weaponEntry.Icon : null, hasContent);

            this.SetSubSlotVisible(true);
            this.ConfigureWeaponSubSlot(activeSpecialSkill != null ? activeSpecialSkill.Icon : null, activeSpecialSkill != null);

            this.SetMainButtonInteractable(isMainButtonInteractable);
            this.SetSubSlotButtonInteractable(weaponEntry != null && isSubSlotButtonInteractable);

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

            bool hasContent = consumableEntry != null && consumableEntry.Item != null && consumableEntry.Quantity > 0;
            this.SetMainIcon(hasContent ? consumableEntry.Icon : null, hasContent);

            this.SetSubSlotVisible(true);
            this.ConfigureConsumableQuantitySubSlot(hasContent ? Mathf.Clamp(consumableEntry.Quantity, 0, 99) : 0, hasContent);

            this.SetMainButtonInteractable(isMainButtonInteractable);
            this.SetSubSlotButtonInteractable(false);

            this.RefreshSlotColor();
        }

        public void BindInteractableItem(InventoryItemEntry interactableEntry, bool isSelected, bool isMainButtonInteractable)
        {
            this.ResetBindings();

            this.SlotType = LoadoutPanelSlotType.InteractableItem;
            this.SlotIndex = 0;
            this.BoundInteractableItemEntry = interactableEntry;

            this.IsActive = false;
            this.IsSelected = isSelected;

            bool hasContent = interactableEntry != null && interactableEntry.Item != null;
            this.SetMainIcon(hasContent ? interactableEntry.Icon : null, hasContent);

            this.SetSubSlotVisible(false);

            this.SetMainButtonInteractable(isMainButtonInteractable);

            this.RefreshSlotColor();
        }

        public bool HasContent()
        {
            return this.BoundWeaponEntry != null ||
                   this.BoundConsumableEntry != null ||
                   (this.BoundInteractableItemEntry != null && this.BoundInteractableItemEntry.Item != null);
        }

        private void HandleClick()
        {
            this.OnClicked?.Invoke(this);
        }

        private void HandleSpecialSkillClick()
        {
            if (this.SlotType != LoadoutPanelSlotType.Weapon) return;
            if (this.BoundWeaponEntry == null) return;

            this.OnSubSlotClicked?.Invoke(this);
        }

        private void ResetBindings()
        {
            this.SlotType = LoadoutPanelSlotType.None;
            this.SlotIndex = -1;

            this.BoundWeaponEntry = null;
            this.BoundConsumableEntry = null;
            this.BoundInteractableItemEntry = null;
            this.BoundWeaponSpecialSkill = null;

            this.IsSelected = false;
            this.IsActive = false;

            this.SetMainIcon(null, false);
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

        private void SetMainIcon(Sprite sprite, bool hasContent)
        {
            if (this.MainIcon == null) return;

            this.MainIcon.sprite = sprite;
            this.MainIcon.enabled = hasContent;
        }

        private void SetSubSlotVisible(bool visible)
        {
            if (this.SubSlotRoot != null) this.SubSlotRoot.SetActive(visible);
        }

        private void ConfigureWeaponSubSlot(Sprite sprite, bool hasContent)
        {
            if (this.SubSlotIcon == null) return;

            if (this.SubSlotIconRectTransform != null)
            {
                SubSlotIconRectTransform.sizeDelta = new Vector2(30f, 30f);
                SubSlotIconRectTransform.anchoredPosition = Vector2.zero;
            }

            this.SubSlotIcon.sprite = sprite;
            this.SubSlotIcon.enabled = hasContent;

            if (this.SubSlotIconBackup != null)
            {
                this.SubSlotIconBackup.sprite = null;
                this.SubSlotIconBackup.enabled = false;
            }
        }

        private void ConfigureConsumableQuantitySubSlot(int quantity, bool hasContent)
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

            // quantity in [0, 9]
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

            // quantity in [10, 99]
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
                this.SubSlotIconBackupRectTransform.anchoredPosition = new Vector2(+7.5f, 0f);
            }

            this.SubSlotIcon.sprite = SharedSpritesUIUtility.GetDigitSprite(firstDigit);
            this.SubSlotIcon.enabled = true;

            if (this.SubSlotIconBackup != null)
            {
                this.SubSlotIconBackup.sprite = SharedSpritesUIUtility.GetDigitSprite(secondDigit);
                this.SubSlotIconBackup.enabled = true;
            }
        }

        private void SetMainButtonInteractable(bool value)
        {
            if (this.Button != null) this.Button.interactable = value;
        }

        private void SetSubSlotButtonInteractable(bool value)
        {
            if (this.SubSlotButton != null) this.SubSlotButton.interactable = value;
        }

        private void RefreshSlotColor()
        {
            if (this.SlotImage == null) return;

            if (this.IsSelected)
            {
                this.SlotImage.color = this.SelectedColor;
            }
            else if (this.IsActive)
            {
                this.SlotImage.color = this.ActiveColor;
            }
            else
            {
                this.SlotImage.color = this.NormalColor;
            }
        }
    }
}