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

        public LoadoutPanelSlotType SlotType { get; private set; }

        public InventoryItemEntry BoundWeaponEntry { get; private set; }
        public InventoryItemEntry BoundConsumableEntry { get; private set; }
        public InventoryItemEntry BoundInteractableItem { get; private set; }
        public SkillProfile BoundSpecialSkill { get; private set; }

        //  2. Events
        public event Action<CandidateLoadoutSlotUIController> OnClicked;

        // --- Unity Callbacks ---
        private void Awake()
        {
            if (this.MainIcon == null)
            {
                Transform mainIconTransform = this.transform.Find("MainIcon");
                if (mainIconTransform != null) _icon = mainIconTransform.GetComponent<Image>();
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

            if (this.SubSlotIconRectTransform == null) _subSlotIconRect = this.SubSlotIcon.rectTransform;
            if (this.SubSlotIconBackupRectTransform == null) _subSlotIconBackupRect = this.SubSlotIconBackup.rectTransform;
        }

        private void OnDestroy()
        {
            if (this.Button != null) this.Button.onClick.RemoveListener(this.HandleClick);
        }

        // --- Public APIs ---
        public void BindWeapon(InventoryItemEntry weaponEntry)
        {
            this.ResetBindings();

            this.SlotType = LoadoutPanelSlotType.Weapon;
            this.BoundWeaponEntry = weaponEntry;

            this.SetMainIcon(weaponEntry != null ? weaponEntry.Item.Icon : null, weaponEntry != null);
            this.SetSubSlotVisible(false);
        }

        public void BindConsumableItem(InventoryItemEntry consumableEntry)
        {
            this.ResetBindings();

            this.SlotType = LoadoutPanelSlotType.Consumable;
            this.BoundConsumableEntry = consumableEntry;

            bool hasContent = consumableEntry != null && consumableEntry.Item != null;
            this.SetMainIcon(hasContent ? consumableEntry.Item.Icon : null, hasContent);
            this.SetSubSlotVisible(true);
            this.ConfigureConsumableQuantitySubSlot(hasContent ? Mathf.Clamp(consumableEntry.Quantity, 0, 99) : 0, hasContent);
        }

        public void BindInteractableItem(InventoryItemEntry interactableEntry)
        {
            this.ResetBindings();

            this.SlotType = LoadoutPanelSlotType.InteractableItem;
            this.BoundInteractableItem = interactableEntry;

            this.SetMainIcon(interactableEntry != null ? interactableEntry.Icon : null, interactableEntry != null);
            this.SetSubSlotVisible(false);
        }

        public void BindSpecialSkill(SkillProfile skill)
        {
            this.ResetBindings();

            this.SlotType = LoadoutPanelSlotType.SpecialSkillCandidate;
            this.BoundSpecialSkill = skill;

            this.SetMainIcon(skill != null ? skill.Icon : null, skill != null);
            this.SetSubSlotVisible(false);
        }

        // --- Helpers ---
        private void HandleClick()
        {
            this.OnClicked?.Invoke(this);
        }

        private void ResetBindings()
        {
            this.SlotType = LoadoutPanelSlotType.None;

            this.BoundWeaponEntry = null;
            this.BoundConsumableEntry = null;
            this.BoundInteractableItem = null;
            this.BoundSpecialSkill = null;

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
    }
}
