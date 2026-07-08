using System;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public abstract class EquippedLoadoutSlotController : FOTLMonoBehaviour
    {
        // --- States ---
        [Header("References (Main Slot)")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _slotImage;
        [SerializeField] private Image _icon;
        protected Button Button => _button;
        protected Image SlotImage => _slotImage;
        protected Image MainIcon => _icon;

        [Header("References (Sub Slot)")]
        [SerializeField] private GameObject _subSlotRoot;
        [SerializeField] private Button _subSlotButton;
        protected Button SubSlotButton => _subSlotButton;
        protected GameObject SubSlotRoot => _subSlotRoot;

        [Header("Slot Colors")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _activeColor = Color.green;
        [SerializeField] private Color _selectedColor = Color.red;
        protected Color NormalColor => _normalColor;
        protected Color ActiveColor => _activeColor;
        protected Color SelectedColor => _selectedColor;

        public LoadoutPanelSlotType SlotType { get; protected set; } = LoadoutPanelSlotType.None;
        public int SlotIndex { get; protected set; } = -1;

        public InventoryItemEntry BoundWeaponEntry { get; protected set; }
        public InventoryItemEntry BoundProjectileEntry { get; protected set; }
        public InventoryItemEntry BoundConsumableEntry { get; protected set; }
        public InventoryItemEntry BoundCarriableItemEntry { get; protected set; }
        public SkillProfile BoundWeaponSpecialSkill { get; protected set; }

        public bool IsSelected { get; protected set; }
        public bool IsActive { get; protected set; }

        // --- Events ---
        public event Action<EquippedLoadoutSlotController> OnClicked;
        public event Action<EquippedLoadoutSlotController> OnSubSlotClicked;

        // --- Public APIs ---
        public virtual void BindCarriableItem(InventoryItemEntry carriableEntry, bool isSelected, bool isMainButtonInteractable)
        {
            this.ResetBindings();

            this.SlotType = LoadoutPanelSlotType.CarriableItem;
            this.SlotIndex = 0;
            this.BoundCarriableItemEntry = carriableEntry;

            this.IsActive = false;
            this.IsSelected = isSelected;

            bool hasContent = carriableEntry != null && carriableEntry.Item != null;
            this.SetMainIcon
            (
                sprite: hasContent ? carriableEntry.Icon : null, 
                hasContent: hasContent
            );

            this.SetSubSlotVisible(false);

            this.SetMainButtonInteractable(isMainButtonInteractable);

            this.RefreshSlotColor();
        }

        public bool HasContent()
        {
            return this.SlotType switch
            {
                LoadoutPanelSlotType.Weapon =>
                    this.BoundWeaponEntry != null &&
                    this.BoundWeaponEntry.Item != null,

                LoadoutPanelSlotType.Projectile =>
                    this.BoundProjectileEntry != null &&
                    this.BoundProjectileEntry.Item != null &&
                    this.BoundProjectileEntry.Quantity > 0,

                LoadoutPanelSlotType.Consumable =>
                    this.BoundConsumableEntry != null &&
                    this.BoundConsumableEntry.Item != null &&
                    this.BoundConsumableEntry.Quantity > 0,

                LoadoutPanelSlotType.CarriableItem =>
                    this.BoundCarriableItemEntry != null &&
                    this.BoundCarriableItemEntry.Item != null,

                _ => false
            };
        }

        // --- Helpers ---
        protected virtual void ResetBindings()
        {
            this.SlotType = LoadoutPanelSlotType.None;
            this.SlotIndex = -1;

            this.BoundWeaponEntry = null;
            this.BoundProjectileEntry = null;
            this.BoundConsumableEntry = null;
            this.BoundCarriableItemEntry = null;
            this.BoundWeaponSpecialSkill = null;

            this.IsSelected = false;
            this.IsActive = false;

            this.SetMainIcon(null, false);
            this.SetSubSlotVisible(false);
            this.ClearSubSlotContent();
            this.RefreshSlotColor();
        }

        protected virtual void ClearSubSlotContent()
        {
        }

        protected void SetMainIcon(Sprite sprite, bool hasContent)
        {
            if (this.MainIcon == null) return;

            this.MainIcon.sprite = sprite;
            this.MainIcon.enabled = hasContent;
        }

        protected void SetSubSlotVisible(bool visible)
        {
            if (this.SubSlotRoot != null) this.SubSlotRoot.SetActive(visible);
        }

        protected void SetMainButtonInteractable(bool value)
        {
            if (this.Button == null) return;

            this.Button.interactable = value;
        }

        protected void SetSubSlotButtonInteractable(bool value)
        {
            if (this.SubSlotButton == null) return;

            this.SubSlotButton.interactable = value;

            // Important:
            // SubSlot can be non-interactable, but it should not visually fade.
            this.KeepDisabledButtonVisualAsNormal(this.SubSlotButton);
        }

        protected void RefreshSlotColor()
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

        protected void KeepDisabledButtonVisualAsNormal(Button button)
        {
            if (button == null) return;

            ColorBlock colors = button.colors;
            colors.disabledColor = colors.normalColor;
            button.colors = colors;
        }

        protected virtual bool CanSubmitSubSlotClick()
        {
            return false;
        }

        private void HandleClick()
        {
            this.OnClicked?.Invoke(this);
        }

        private void HandleSubSlotClick()
        {
            if (!this.CanSubmitSubSlotClick()) return;

            this.OnSubSlotClicked?.Invoke(this);
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.Button == null)
            {
                _button = this.GetComponent<Button>();
            }

            if (this.SlotImage == null)
            {
                _slotImage = this.GetComponent<Image>();
            }

            if (this.MainIcon == null)
            {
                Transform iconTransform = this.transform.Find("Icon");
                if (iconTransform != null)
                {
                    _icon = iconTransform.GetComponent<Image>();
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

            if (this.SubSlotButton == null && this.SubSlotRoot != null)
            {
                _subSlotButton = this.SubSlotRoot.GetComponent<Button>();
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            if (this.Button != null)
            {
                this.Button.onClick.RemoveListener(this.HandleClick);
                this.Button.onClick.AddListener(this.HandleClick);
            }

            if (this.SubSlotButton != null)
            {
                this.SubSlotButton.onClick.RemoveListener(this.HandleSubSlotClick);
                this.SubSlotButton.onClick.AddListener(this.HandleSubSlotClick);
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.KeepDisabledButtonVisualAsNormal(this.SubSlotButton);
            this.ResetBindings();
        }
        protected override void UnbindInternalReferences()
        {
            base.UnbindInternalReferences();

            if (this.Button != null)
            {
                this.Button.onClick.RemoveListener(this.HandleClick);
            }

            if (this.SubSlotButton != null)
            {
                this.SubSlotButton.onClick.RemoveListener(this.HandleSubSlotClick);
            }
        }
    }
}