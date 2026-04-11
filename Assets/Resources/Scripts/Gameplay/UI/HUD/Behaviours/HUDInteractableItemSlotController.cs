using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class HUDInteractableItemSlotController : MonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private PlayerInteractionLoadoutController _playerInteractionLoadoutController;
        private PlayerInteractionLoadoutController PlayerInteractionLoadoutController => _playerInteractionLoadoutController;

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
        private void Awake()
        {
            if (this.PlayerInteractionLoadoutController == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null) _playerInteractionLoadoutController = player.GetComponent<PlayerInteractionLoadoutController>();
                if (this.PlayerInteractionLoadoutController == null) Debug.LogWarning($"[{this.gameObject.name}.TopLeftInteractableItemSlotUIController] Player's PlayerInteractionLoadoutController is not found.");
            }

            if (this.SlotBackground == null) _slotBackground = this.GetComponent<Image>();
            if (this.Icon == null) _icon = this.transform.GetChild(0).GetComponent<Image>();
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
            if (this.PlayerInteractionLoadoutController == null) return;
            this.RefreshIcon();
            this.RefreshSlotColor();
        }

        private void RefreshIcon()
        {
            if (this.Icon == null) return;

            InventoryItemEntry equippedEntry = this.PlayerInteractionLoadoutController.EquippedInteractableItemEntry;
            InteractableItemProfile interactableItem = equippedEntry != null
                ? equippedEntry.Item as InteractableItemProfile
                : null;

            if (interactableItem == null || interactableItem.Icon == null)
            {
                this.Icon.enabled = false;
                this.Icon.sprite = null;
                return;
            }

            this.Icon.enabled = true;
            this.Icon.sprite = interactableItem.Icon;
        }

        private void RefreshSlotColor()
        {
            if (this.SlotBackground == null) return;

            bool isCarrying = this.PlayerInteractionLoadoutController.IsCarryingInteractableItem;
            this.SlotBackground.color = isCarrying ? this.CarriedColor : this.NormalColor;
        }
    }
}
