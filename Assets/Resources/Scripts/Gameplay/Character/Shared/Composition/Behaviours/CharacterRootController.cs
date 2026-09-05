using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterRootController : FOTLMonoBehaviour
    {
        // --- Data / Configs ---
        public abstract CharacterProfile Profile { get; }

        public int Id               => this.Profile != null ? this.Profile.Id           : 0;
        public string Key           => this.Profile != null ? this.Profile.Key          : string.Empty;
        public string DisplayName   => this.Profile != null ? this.Profile.DisplayName  : string.Empty;
        public Sprite Icon          => this.Profile != null ? this.Profile.Icon         : null;

        public CharacterType CharacterType          => this.Profile != null ? this.Profile.CharacterType    : CharacterType.Default;
        public CharacterFaction Faction             => this.Profile != null ? this.Profile.Faction          : default;
        public CharacterTraversalType TraversalType => this.Profile != null ? this.Profile.TraversalType    : default;

        public GameObject CharacterRoot => this.gameObject;

        [Header("References - Body")]
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Collider2D _mainCollider2D;
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public Collider2D MainCollider2D => _mainCollider2D;

        [Header("References - Refreshment")]
        [SerializeField] private CharacterRefreshHandler _refreshHandler;
        public CharacterRefreshHandler RefreshHandler => _refreshHandler;

        [Header("References - Input")]
        [SerializeField] private CharacterInputHandler _inputHandler;
        public CharacterInputHandler InputHandler => _inputHandler;

        [Header("References - Movement")]
        [SerializeField] private MovementHandler _movementHandler;
        public MovementHandler MovementHandler => _movementHandler;

        [Header("References - Dodge")]
        [SerializeField] private CharacterDodgeController _dodgeController;
        public CharacterDodgeController DodgeController => _dodgeController;

        [Header("References - Terrain Contact")]
        [SerializeField] private CharacterTerrainContactHandler _terrainContactHandler;
        public CharacterTerrainContactHandler TerrainContactHandler => _terrainContactHandler;

        [Header("References - Visual")]
        [SerializeField] private CharacterVisualController _visualController;
        public CharacterVisualController VisualController => _visualController;

        [Header("References - Visual/VFX")]
        [SerializeField] private Transform _statusEffectVFXHolder;
        [SerializeField] private CharacterRewardVFXHandler _rewardVFXHandler;
        public Transform StatusEffectVFXHolder => _statusEffectVFXHolder;
        public CharacterRewardVFXHandler RewardVFXHandler => _rewardVFXHandler;

        [Header("References - Combat")]
        [SerializeField] private CharacterCombatController _combatController;
        [SerializeField] private CharacterCombatLoadoutController _combatLoadoutController;
        [SerializeField] private CharacterCombatAimingHandler _combatAimingHandler;
        [SerializeField] private CharacterCombatResourcesHandler _combatResourcesHandler;
        [SerializeField] private CharacterCombatStatusEffectApplicationHandler _combatStatusEffectApplicationHandler;
        [SerializeField] private DamageReceivingHandler _damageReceivingHandler;
        [SerializeField] private KnockbackReceivingHandler _knockbackReceivingHandler;
        public CharacterCombatController CombatController => _combatController;
        public CharacterCombatLoadoutController CombatLoadoutController => _combatLoadoutController;
        public CharacterCombatAimingHandler CombatAimingHandler => _combatAimingHandler;
        public CharacterCombatResourcesHandler CombatResourcesHandler => _combatResourcesHandler;
        public CharacterCombatStatusEffectApplicationHandler CombatStatusEffectApplicationHandler => _combatStatusEffectApplicationHandler;
        public DamageReceivingHandler DamageReceivingHandler => _damageReceivingHandler;
        public KnockbackReceivingHandler KnockbackReceivingHandler => _knockbackReceivingHandler;

        [Header("References - Resources")]
        [SerializeField] private CharacterResourcesController _resourcesController;
        [SerializeField] private HPController _hpController; 
        [SerializeField] private FPController _fpController;
        [SerializeField] private StaminaController _staminaController;
        [SerializeField] private CharacterDeathHandler _deathHandler;
        public CharacterResourcesController ResourcesController => _resourcesController;
        public HPController HPController => _hpController;
        public FPController FPController => _fpController;
        public StaminaController StaminaController => _staminaController;
        public CharacterDeathHandler DeathHandler => _deathHandler;

        [Header("References - Status Effects")]
        [SerializeField] private StatusEffectController _statusEffectController;
        [SerializeField] private StatusEffectFlagHandler _statusEffectFlagHandler;
        [SerializeField] private StatusEffectStatModifierHandler _statusEffectStatModifierHandler;
        [SerializeField] private StatusEffectStatPeriodicHandler _statusEffectStatPeriodicHandler;
        public StatusEffectController StatusEffectController => _statusEffectController;
        public StatusEffectFlagHandler StatusEffectFlagHandler => _statusEffectFlagHandler;
        public StatusEffectStatModifierHandler StatusEffectStatModifierHandler => _statusEffectStatModifierHandler;
        public StatusEffectStatPeriodicHandler StatusEffectStatPeriodicHandler => _statusEffectStatPeriodicHandler;

        [Header("References - Progression")]
        [SerializeField] private CharacterProgressionController _progressionController;
        [SerializeField] private CharacterProgressionUpgradeHandler _progressionUpgradeHandler;
        public CharacterProgressionController ProgressionController => _progressionController;
        public CharacterProgressionUpgradeHandler ProgressionUpgradeHandler => _progressionUpgradeHandler;

        [Header("References - Reward")]

        [SerializeField] private CharacterRewardWalletController _rewardWalletController;
        public CharacterRewardWalletController RewardWalletController => _rewardWalletController;

        [Header("References - Inventory")]
        [SerializeField] private InventoryController _inventoryController;
        public InventoryController InventoryController => _inventoryController;

        // --- Helpers ---
        protected virtual void CacheCharacterConfigs()
        {
            if (this.Profile == null) return;

            this.MovementHandler?.CacheConfigIfNull(this.Profile.MovementConfig);
            this.VisualController?.CacheConfigIfNull(this.Profile.VisualConfig);
            this.CombatController?.CacheConfigIfNull(this.Profile.CombatConfig);
            this.StatusEffectController?.CacheConfigIfNull(this.Profile.StatusEffectGuardConfig);
            this.ProgressionController?.CacheConfigIfNull(this.Profile.ProgressionConfig);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.Rigidbody2D    == null) _rigidbody2D       = this.GetComponent<Rigidbody2D>();
            if (this.MainCollider2D == null) _mainCollider2D    = this.GetComponent<Collider2D>();

            if (this.RefreshHandler == null) _refreshHandler = this.GetComponentInChildren<CharacterRefreshHandler>(true);

            if (this.InputHandler == null) _inputHandler = this.GetComponent<CharacterInputHandler>();

            if (this.MovementHandler == null) _movementHandler = this.GetComponent<MovementHandler>();

            if (this.DodgeController == null) _dodgeController = this.GetComponent<CharacterDodgeController>();

            if (this.TerrainContactHandler == null) _terrainContactHandler = this.GetComponent<CharacterTerrainContactHandler>();

            if (this.VisualController == null) _visualController = this.GetComponentInChildren<CharacterVisualController>(true);

            if (this.StatusEffectVFXHolder == null)
            {
                Transform visual = this.VisualController?.transform;
                if (visual != null) _statusEffectVFXHolder = visual.Find("StatusEffectVFXHolder");
            }
            if (this.RewardVFXHandler == null) _rewardVFXHandler = this.GetComponentInChildren<CharacterRewardVFXHandler>(true);

            if (this.CombatController                       == null) _combatController                      = this.GetComponentInChildren<CharacterCombatController>(true);
            if (this.CombatLoadoutController                == null) _combatLoadoutController               = this.GetComponentInChildren<CharacterCombatLoadoutController>(true);
            if (this.CombatAimingHandler                    == null) _combatAimingHandler                   = this.GetComponentInChildren<CharacterCombatAimingHandler>(true);
            if (this.CombatResourcesHandler                 == null) _combatResourcesHandler                = this.GetComponentInChildren<CharacterCombatResourcesHandler>(true);
            if (this.CombatStatusEffectApplicationHandler   == null) _combatStatusEffectApplicationHandler  = this.GetComponentInChildren<CharacterCombatStatusEffectApplicationHandler>(true);

            if (this.DamageReceivingHandler == null) _damageReceivingHandler = this.GetComponentInChildren<DamageReceivingHandler>(true);
            if (this.KnockbackReceivingHandler == null) _knockbackReceivingHandler = this.GetComponentInChildren<KnockbackReceivingHandler>(true);

            if (this.ResourcesController    == null) _resourcesController   = this.GetComponentInChildren<CharacterResourcesController>(true);
            if (this.HPController           == null) _hpController          = this.GetComponentInChildren<HPController>(true);
            if (this.FPController           == null) _fpController          = this.GetComponentInChildren<FPController>(true);
            if (this.StaminaController      == null) _staminaController     = this.GetComponentInChildren<StaminaController>(true);
            if (this.DeathHandler           == null) _deathHandler          = this.GetComponentInChildren<CharacterDeathHandler>(true);

            if (this.StatusEffectController             == null) _statusEffectController            = this.GetComponentInChildren<StatusEffectController>(true);
            if (this.StatusEffectFlagHandler            == null) _statusEffectFlagHandler           = this.GetComponentInChildren<StatusEffectFlagHandler>(true);
            if (this.StatusEffectStatModifierHandler    == null) _statusEffectStatModifierHandler   = this.GetComponentInChildren<StatusEffectStatModifierHandler>(true);
            if (this.StatusEffectStatPeriodicHandler    == null) _statusEffectStatPeriodicHandler   = this.GetComponentInChildren<StatusEffectStatPeriodicHandler>(true);

            if (this.ProgressionController      == null) _progressionController     = this.GetComponentInChildren<CharacterProgressionController>(true);
            if (this.ProgressionUpgradeHandler  == null) _progressionUpgradeHandler = this.GetComponentInChildren<CharacterProgressionUpgradeHandler>(true);

            if (this.RewardWalletController == null) _rewardWalletController = this.GetComponentInChildren<CharacterRewardWalletController>(true);

            if (this.InventoryController == null) _inventoryController = this.GetComponentInChildren<InventoryController>(true);

            this.CacheCharacterConfigs();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Profile == null) Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] No Character Profile assigned.", this);
        }
    }
}
