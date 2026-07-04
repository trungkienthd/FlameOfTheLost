using UnityEngine;

namespace FlameOfTheLost
{
    public class UICanvasHUDCombatLoadoutSlotGroupController : FOTLMonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private CharacterCombatLoadoutController _playerCombatLoadoutController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;

        //  2. References - Slot Handlers
        [Header("References - Slot Handlers")]
        [SerializeField] private UICanvasHUDCombatLoadoutWeaponSlotHandler _weaponSlotHandler;
        [SerializeField] private UICanvasHUDCombatLoadoutProjectileSlotHandler _projectileSlotHandler;
        [SerializeField] private UICanvasHUDCombatLoadoutConsumableItemSlotHandler _consumableItemSlotHandler;
        private UICanvasHUDCombatLoadoutWeaponSlotHandler WeaponSlotHandler => _weaponSlotHandler;
        private UICanvasHUDCombatLoadoutProjectileSlotHandler ProjectileSlotHandler => _projectileSlotHandler;
        private UICanvasHUDCombatLoadoutConsumableItemSlotHandler ConsumableItemSlotHandler => _consumableItemSlotHandler;

        //  3. Refresh
        [Header("Refresh")]
        [SerializeField] private float _refreshInterval = 0.1f;
        private float RefreshInterval => _refreshInterval;

        private float _time;
        private float TimeData
        {
            get => _time;
            set => _time = value;
        }

        private bool _isGameplayInitialized;
        private bool IsGameplayInitialized
        {
            get => _isGameplayInitialized;
            set => _isGameplayInitialized = value;
        }

        // --- Unity Callbacks ---
        private void Update()
        {
            if (!this.IsGameplayInitialized) return;

            this.TimeData += Time.unscaledDeltaTime;
            if (this.TimeData < this.RefreshInterval) return;

            this.TimeData = 0f;
            this.RefreshAll();
        }

        // --- Helpers ---
        private void RefreshAll()
        {
            if (!this.PlayerCombatLoadoutController) return;

            if (this.WeaponSlotHandler != null)
            {
                this.WeaponSlotHandler.RefreshSlot(this.PlayerCombatLoadoutController);
            }

            if (this.ProjectileSlotHandler != null)
            {
                this.ProjectileSlotHandler.RefreshSlot(this.PlayerCombatLoadoutController);
            }

            if (this.ConsumableItemSlotHandler != null)
            {
                this.ConsumableItemSlotHandler.RefreshSlot(this.PlayerCombatLoadoutController);
            }
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.WeaponSlotHandler          == null) _weaponSlotHandler         = this.GetComponentInChildren<UICanvasHUDCombatLoadoutWeaponSlotHandler>(true);
            if (this.ProjectileSlotHandler      == null) _projectileSlotHandler     = this.GetComponentInChildren<UICanvasHUDCombatLoadoutProjectileSlotHandler>(true);
            if (this.ConsumableItemSlotHandler  == null) _consumableItemSlotHandler = this.GetComponentInChildren<UICanvasHUDCombatLoadoutConsumableItemSlotHandler>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.WeaponSlotHandler          == null) Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDCombatLoadoutSlotGroupController)}] Weapon Slot Handler is not set.");
            if (this.ProjectileSlotHandler      == null) Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDCombatLoadoutSlotGroupController)}] Projectile Slot Handler is not set.");
            if (this.ConsumableItemSlotHandler  == null) Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDCombatLoadoutSlotGroupController)}] Consumable Item Slot Handler is not set.");
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.TimeData = 0f;
            this.IsGameplayInitialized = false;
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.PlayerCombatLoadoutController != null) return;

            GameObject player = GameObject.Find("Player");
            if (player == null) return;

            _playerCombatLoadoutController = player.GetComponentInChildren<CharacterCombatLoadoutController>(true);
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.PlayerCombatLoadoutController == null) Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDCombatLoadoutSlotGroupController)}] Player's {nameof(CharacterCombatLoadoutController)} reference is not set on " + this.name);
        }
        protected override void InitializeGameplay()
        {
            base.InitializeGameplay();

            this.IsGameplayInitialized = true;
            this.RefreshAll();
        }
    }
}
