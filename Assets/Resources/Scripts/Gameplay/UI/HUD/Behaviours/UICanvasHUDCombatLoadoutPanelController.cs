using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class UICanvasHUDCombatLoadoutPanelController : MonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private CharacterCombatLoadoutController _playerCombatLoadoutController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;

        //  2. Display
        //      2.1. Weapon
        [Header("Weapon")]
        [SerializeField] private Image _weaponIcon;
        private Image ActiveEquippedWeaponIcon  => _weaponIcon;

        //      2.2. Active Skill
        [Header("Special Skills")]
        [SerializeField] private Image _activeSpecialSkillIcon;
        private Image ActiveSpecialSkillIcon => _activeSpecialSkillIcon;

        //      2.3. Consumable Item
        [Header("Consumable Item")]
        [SerializeField] private Image _consumableItemIcon;
        [SerializeField] private Image _consumableQuantityIcon;
        [SerializeField] private Image _consumableQuantityIconBackup;
        private Image ConsumableItemIcon => _consumableItemIcon;
        private Image ConsumableQuantityIcon => _consumableQuantityIcon;
        private Image ConsumableQuantityIconBackup => _consumableQuantityIconBackup;

        //  3. Refresh
        [Header("Refresh")]
        [Tooltip("If you don't have events yet, a very light polling is fine.")]
        [SerializeField] private float _refreshInterval = 0.1f;
        private float RefreshInterval => _refreshInterval;

        private float _time;
        private float TimeData
        {
            get => _time;
            set => _time = value;
        }

        // --- Unity methods ---
        private void Awake()
        {
            if (this.PlayerCombatLoadoutController == null)
            {
                GameObject player = GameObject.Find("Player");
                _playerCombatLoadoutController = player.GetComponentInChildren<CharacterCombatLoadoutController>();
                if (this.PlayerCombatLoadoutController == null) Debug.LogWarning($"[{this.gameObject.name}.HUDCombatLoadoutPanelController] Player's CharacterCombatLoadoutController is not found.");
            }
        }
        private void Start()
        {
            this.RefreshAll();
        }
        private void Update()
        {
            this.TimeData += Time.unscaledDeltaTime;
            if (this.TimeData >= this.RefreshInterval)
            {
                this.TimeData = 0f;
                this.RefreshAll();
            }
        }

        // --- Helpers ---
        private void RefreshAll()
        {
            if (!this.PlayerCombatLoadoutController) return;

            this.RefreshWeaponIcon();
            this.RefreshActiveSpecialSkillIcon();
            this.RefreshConsumableItemIcon();
            this.RefreshConsumableQuantity();
        }

        private void RefreshWeaponIcon()
        {
            if (!this.ActiveEquippedWeaponIcon) return;

            Sprite weaponIcon = this.PlayerCombatLoadoutController.GetActiveEquippedWeaponIcon();
            bool hasWeaponIcon = weaponIcon != null;

            this.ActiveEquippedWeaponIcon.enabled = hasWeaponIcon;
            this.ActiveEquippedWeaponIcon.sprite = weaponIcon;
        }

        private void RefreshActiveSpecialSkillIcon()
        {
            if (!this.ActiveSpecialSkillIcon) return;

            Sprite skillIcon = this.PlayerCombatLoadoutController.GetActiveSpecialSkillIcon();
            bool hasSkillIcon = skillIcon != null;

            this.ActiveSpecialSkillIcon.enabled = hasSkillIcon;
            this.ActiveSpecialSkillIcon.sprite = skillIcon;
        }

        private void RefreshConsumableItemIcon()
        {
            if (!this.ConsumableItemIcon) return;

            Sprite consumableIcon = this.PlayerCombatLoadoutController.GetActiveConsumableItemIcon();
            bool hasConsumableIcon = consumableIcon != null;

            this.ConsumableItemIcon.enabled = hasConsumableIcon;
            this.ConsumableItemIcon.sprite = consumableIcon;
        }

        private void RefreshConsumableQuantity()
        {
            if (!this.ConsumableQuantityIcon || !this.ConsumableQuantityIconBackup) return;

            InventoryItemEntry consumableEntry = this.PlayerCombatLoadoutController.ActiveEquippedConsumableItemEntry;
            if (consumableEntry == null) return;

            int quantity = consumableEntry.Quantity;

            if (quantity < 0) quantity = 0;
            if (quantity > 99) quantity = 99;

            bool hasConsumable = this.PlayerCombatLoadoutController.GetActiveConsumableItemIcon() != null;

            if (!hasConsumable)
            {
                this.DisableConsumableQuantityDigits();
                return;
            }

            if (quantity <= 9)
            {
                this.SetupSingleDigitQuantity(quantity);
            }
            else
            {
                this.SetupDoubleDigitQuantity(quantity);
            }
        }

        private void SetupSingleDigitQuantity(int quantity)
        {
            this.ConsumableQuantityIcon.enabled = true;
            this.ConsumableQuantityIconBackup.enabled = false;

            this.ConsumableQuantityIcon.sprite = SharedSpritesUIUtility.GetDigitSprite(quantity);
            this.ConsumableQuantityIconBackup.sprite = null;

            RectTransform iconRect = this.ConsumableQuantityIcon.rectTransform;
            iconRect.sizeDelta = new Vector2(20f, 20f);
            iconRect.anchoredPosition = Vector2.zero;
        }

        private void SetupDoubleDigitQuantity(int quantity)
        {
            int tensDigit = quantity / 10;
            int onesDigit = quantity % 10;

            this.ConsumableQuantityIcon.enabled = true;
            this.ConsumableQuantityIconBackup.enabled = true;

            this.ConsumableQuantityIcon.sprite = SharedSpritesUIUtility.GetDigitSprite(tensDigit);
            this.ConsumableQuantityIconBackup.sprite = SharedSpritesUIUtility.GetDigitSprite(onesDigit);

            RectTransform iconRect = this.ConsumableQuantityIcon.rectTransform;
            iconRect.sizeDelta = new Vector2(12f, 18f);
            iconRect.anchoredPosition = new Vector2(-8f, 0f);

            RectTransform backupRect = this.ConsumableQuantityIconBackup.rectTransform;
            backupRect.sizeDelta = new Vector2(12f, 18f);
            backupRect.anchoredPosition = new Vector2(8f, 0f);
        }

        private void DisableConsumableQuantityDigits()
        {
            this.ConsumableQuantityIcon.enabled = false;
            this.ConsumableQuantityIconBackup.enabled = false;

            this.ConsumableQuantityIcon.sprite = null;
            this.ConsumableQuantityIconBackup.sprite = null;
        }
    }
}
