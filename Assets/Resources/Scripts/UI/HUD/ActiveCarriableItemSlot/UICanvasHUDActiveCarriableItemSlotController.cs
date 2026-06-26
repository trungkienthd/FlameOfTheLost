using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class UICanvasHUDActiveCarriableItemSlotController : FOTLMonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private PlayerCarryLoadoutController _playerCarryLoadoutController;
        private PlayerCarryLoadoutController PlayerCarryLoadoutController => _playerCarryLoadoutController;

        [SerializeField] private Image _slotBackground;
        [SerializeField] private Image _icon;
        private Image SlotBackground => _slotBackground;
        private Image Icon => _icon;

        //  2. Refresh
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

        [Header("Slot Colors")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _carriedColor = Color.green;
        private Color NormalColor => _normalColor;
        private Color CarriedColor => _carriedColor;

        // --- Unity Callbacks ---
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
            if (this.PlayerCarryLoadoutController == null) return;
            this.RefreshIcon();
            this.RefreshSlotColor();
        }

        private void RefreshIcon()
        {
            if (this.Icon == null) return;

            InventoryItemEntry equippedEntry = this.PlayerCarryLoadoutController.EquippedCarriableItemEntry;
            CarriableItemProfile carriableItem = equippedEntry != null
                ? equippedEntry.Item as CarriableItemProfile
                : null;

            if (carriableItem == null || carriableItem.Icon == null)
            {
                this.Icon.enabled = false;
                this.Icon.sprite = null;
                return;
            }

            this.Icon.enabled = true;
            this.Icon.sprite = carriableItem.Icon;
        }

        private void RefreshSlotColor()
        {
            if (this.SlotBackground == null) return;

            bool isCarrying = this.PlayerCarryLoadoutController.IsCarryingCarriableItem;
            this.SlotBackground.color = isCarrying ? this.CarriedColor : this.NormalColor;
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PlayerCarryLoadoutController == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null) _playerCarryLoadoutController = player.GetComponent<PlayerCarryLoadoutController>();
            }

            if (this.SlotBackground == null) _slotBackground = this.GetComponent<Image>();
            if (this.Icon == null)
            {
                Transform iconTransform = this.transform.Find("Icon");
                if (iconTransform != null) _icon = iconTransform.GetComponent<Image>();
            }
        }

        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.PlayerCarryLoadoutController == null) Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDActiveCarriableItemSlotController)}] PlayerCarryLoadoutController reference is missing.", this);
            if (this.SlotBackground == null) Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDActiveCarriableItemSlotController)}] SlotBackground Image reference is missing.", this);
            if (this.Icon == null) Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDActiveCarriableItemSlotController)}] Icon Image reference is missing.", this);
        }
    }
}
