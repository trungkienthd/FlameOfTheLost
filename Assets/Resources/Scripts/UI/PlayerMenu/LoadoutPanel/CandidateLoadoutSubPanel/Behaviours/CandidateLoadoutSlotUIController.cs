using System;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class CandidateLoadoutSlotUIController : MonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References (Main Slot)")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;

        [Header("References (Sub Slot)")]
        [SerializeField] private GameObject _subSlotRoot;
        [SerializeField] private Image _subSlotIcon;
        [SerializeField] private Image _subSlotIconBackup;
        [SerializeField] private RectTransform _subSlotIconRect;
        [SerializeField] private RectTransform _subSlotIconBackupRect;
        private Button Button => _button;
        private Image MainIcon => _icon;
        private GameObject SubSlotRoot => _subSlotRoot;
        private Image SubSlotIcon => _subSlotIcon;
        private Image SubSlotIconBackup => _subSlotIconBackup;
        private RectTransform SubSlotIconRectTransform => _subSlotIconRect;
        private RectTransform SubSlotIconBackupRectTransform => _subSlotIconBackupRect;

        //  2. Runtime States
        //      2.1. Slot Type 
        public LoadoutPanelSlotType SlotType { get; private set; }

        //      2.2. Bound Content
        public InventoryItemEntry BoundWeaponEntry { get; private set; }
        public InventoryItemEntry BoundProjectileEntry { get; private set; }
        public InventoryItemEntry BoundConsumableEntry { get; private set; }
        public InventoryItemEntry BoundCarriableItem { get; private set; }
        public SkillProfile BoundSpecialSkill { get; private set; }

        //  3. Events
        public event Action<CandidateLoadoutSlotUIController> OnClicked;

        // --- Unity Callbacks ---
        private void Awake()
        {
            this.CacheReferences();

            if (this.Button != null)
            {
                this.Button.onClick.AddListener(this.HandleClick);
            }

            this.Clear();
        }

        private void OnDestroy()
        {
            if (this.Button != null)
            {
                this.Button.onClick.RemoveListener(this.HandleClick);
            }
        }

        // --- Public APIs ---
        public void BindWeapon(InventoryItemEntry weaponEntry)
        {
            this.Clear();

            this.SlotType = LoadoutPanelSlotType.Weapon;
            this.BoundWeaponEntry = weaponEntry;

            bool hasContent = weaponEntry != null && weaponEntry.Item != null;
            this.SetMainIcon(hasContent ? weaponEntry.Item.Icon : null, hasContent);
            this.SetSubSlotVisible(false);
        }

        public void BindProjectile(InventoryItemEntry projectileEntry)
        {
            this.Clear();

            this.SlotType = LoadoutPanelSlotType.Projectile;
            this.BoundProjectileEntry = projectileEntry;

            bool hasContent =
                projectileEntry != null &&
                projectileEntry.Item != null &&
                projectileEntry.Quantity > 0;

            this.SetMainIcon(hasContent ? projectileEntry.Item.Icon : null, hasContent);

            this.SetSubSlotVisible(true);
            this.ConfigureConsumableQuantitySubSlot
            (
                quantity: hasContent ? Mathf.Clamp(projectileEntry.Quantity, 0, 99) : 0,
                hasContent: hasContent
            );
        }

        public void BindConsumableItem(InventoryItemEntry consumableEntry)
        {
            this.Clear();

            this.SlotType = LoadoutPanelSlotType.Consumable;
            this.BoundConsumableEntry = consumableEntry;

            bool hasContent = consumableEntry != null && consumableEntry.Item != null;
            this.SetMainIcon(hasContent ? consumableEntry.Item.Icon : null, hasContent);
            this.SetSubSlotVisible(true);
            this.ConfigureConsumableQuantitySubSlot(hasContent ? Mathf.Clamp(consumableEntry.Quantity, 0, 99) : 0, hasContent);
        }

        public void BindCarriableItem(InventoryItemEntry carriableEntry)
        {
            this.Clear();

            this.SlotType = LoadoutPanelSlotType.CarriableItem;
            this.BoundCarriableItem = carriableEntry;

            bool hasContent = carriableEntry != null;
            this.SetMainIcon(hasContent ? carriableEntry.Icon : null, hasContent);
            this.SetSubSlotVisible(false);
        }

        public void BindSpecialSkill(SkillProfile skill)
        {
            this.Clear();

            this.SlotType = LoadoutPanelSlotType.SpecialSkillCandidate;
            this.BoundSpecialSkill = skill;

            bool hasContent = skill != null;
            this.SetMainIcon(hasContent ? skill.Icon : null, hasContent);
            this.SetSubSlotVisible(false);
        }

        public void Clear()
        {
            this.SlotType = LoadoutPanelSlotType.None;

            this.BoundWeaponEntry = null;
            this.BoundProjectileEntry = null;
            this.BoundConsumableEntry = null;
            this.BoundCarriableItem = null;
            this.BoundSpecialSkill = null;

            this.SetMainIcon(null, false);
            this.SetSubSlotVisible(false);
            this.ClearSubSlotIcons();
        }

        // --- Helpers ---
        private void HandleClick()
        {
            this.OnClicked?.Invoke(this);
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
        private void ClearSubSlotIcons()
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

        private void CacheReferences()
        {
            if (this.Button == null)
            {
                _button = this.GetComponent<Button>();
            }

            if (this.MainIcon == null)
            {
                Transform found = this.transform.Find("MainIcon");
                if (found != null) _icon = found.GetComponent<Image>();
            }

            if (this.SubSlotRoot == null)
            {
                Transform found = this.transform.Find("SubSlot");
                if (found != null) _subSlotRoot = found.gameObject;
            }

            if (this.SubSlotIcon == null && this.SubSlotRoot != null)
            {
                Transform found = this.SubSlotRoot.transform.Find("Icon");
                if (found != null) _subSlotIcon = found.GetComponent<Image>();
            }

            if (this.SubSlotIconBackup == null && this.SubSlotRoot != null)
            {
                Transform found = this.SubSlotRoot.transform.Find("Icon_Backup");
                if (found != null) _subSlotIconBackup = found.GetComponent<Image>();
            }

            if (this.SubSlotIconRectTransform == null && this.SubSlotIcon != null)
            {
                _subSlotIconRect = this.SubSlotIcon.rectTransform;
            }

            if (this.SubSlotIconBackupRectTransform == null && this.SubSlotIconBackup != null)
            {
                _subSlotIconBackupRect = this.SubSlotIconBackup.rectTransform;
            }
        }
    }
}
