using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterRootController : FOTLMonoBehaviour
    {
        [Header("References (Identity)")]
        [SerializeField] private CharacterIdentityController _identityController;
        public CharacterIdentityController IdentityController => _identityController;

        [Header("References (Refreshment)")]
        [SerializeField] private CharacterRefreshmentHandler _refreshmentHandler;
        public CharacterRefreshmentHandler RefreshmentHandler => _refreshmentHandler;

        [Header("References (Input)")]
        [SerializeField] private CharacterInputHandler _inputHandler;
        public CharacterInputHandler InputHandler => _inputHandler;

        [Header("References (Movement)")]
        [SerializeField] private MovementHandler _movementHandler;
        public MovementHandler MovementHandler => _movementHandler;

        [Header("References (Terrain Contact)")]
        [SerializeField] private CharacterTerrainContactHandler _terrainContactHandler;
        public CharacterTerrainContactHandler TerrainContactHandler => _terrainContactHandler;

        [Header("References (Visual)")]
        [SerializeField] private CharacterVisualController _visualController;
        [SerializeField] private CharacterVisualFacingHandler _visualFacingHandler;
        [SerializeField] private CharacterVisualLocomotionAnimationHandler _visualLocomotionAnimationHandler;
        [SerializeField] private CharacterVisualCombatAnimationHandler _visualCombatAnimationHandler;
        public CharacterVisualController VisualController => _visualController;
        public CharacterVisualFacingHandler VisualFacingHandler => _visualFacingHandler;
        public CharacterVisualLocomotionAnimationHandler VisualLocomotionAnimationHandler => _visualLocomotionAnimationHandler;
        public CharacterVisualCombatAnimationHandler VisualCombatAnimationHandler => _visualCombatAnimationHandler;

        [Header("References (Visual - VFX)")]
        [SerializeField] private Transform _statusEffectVFXHolder;
        [SerializeField] private CharacterRewardVFXHandler _rewardVFXHandler;
        public Transform StatusEffectVFXHolder => _statusEffectVFXHolder;
        public CharacterRewardVFXHandler RewardVFXHandler => _rewardVFXHandler;

        [Header("References (Combat)")]
        [SerializeField] private CharacterCombatController _combatController;
        [SerializeField] private CharacterCombatLoadoutController _combatLoadoutController;
        [SerializeField] private CharacterCombatLoadoutWeaponHandler _combatLoadoutWeaponHandler;
        [SerializeField] private CharacterCombatLoadoutSpecialSkillHandler _combatLoadoutSpecialSkillHandler;
        [SerializeField] private CharacterCombatLoadoutConsumableItemHandler _combatLoadoutConsumableItemHandler;
        [SerializeField] private CharacterCombatAimingHandler _combatAimingHandler;
        [SerializeField] private CharacterCombatResourcesHandler _combatResourcesHandler;
        [SerializeField] private CharacterCombatStatusEffectApplicationHandler _combatStatusEffectApplicationHandler;
        public CharacterCombatController CombatController => _combatController;
        public CharacterCombatLoadoutController CombatLoadoutController => _combatLoadoutController;
        public CharacterCombatLoadoutWeaponHandler CombatLoadoutWeaponHandler => _combatLoadoutWeaponHandler;
        public CharacterCombatLoadoutSpecialSkillHandler CombatLoadoutSpecialSkillHandler => _combatLoadoutSpecialSkillHandler;
        public CharacterCombatLoadoutConsumableItemHandler CombatLoadoutConsumableItemHandler => _combatLoadoutConsumableItemHandler;
        public CharacterCombatAimingHandler CombatAimingHandler => _combatAimingHandler;
        public CharacterCombatResourcesHandler CombatResourcesHandler => _combatResourcesHandler;
        public CharacterCombatStatusEffectApplicationHandler CombatStatusEffectApplicationHandler => _combatStatusEffectApplicationHandler;

        [Header("References (Resources)")]
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

        [Header("References (Status Effects)")]
        [SerializeField] private StatusEffectController _statusEffectController;
        [SerializeField] private StatusEffectFlagHandler _statusEffectFlagHandler;
        [SerializeField] private StatusEffectStatModifierHandler _statusEffectStatModifierHandler;
        [SerializeField] private StatusEffectStatPeriodicHandler _statusEffectStatPeriodicHandler;
        public StatusEffectController StatusEffectController => _statusEffectController;
        public StatusEffectFlagHandler StatusEffectFlagHandler => _statusEffectFlagHandler;
        public StatusEffectStatModifierHandler StatusEffectStatModifierHandler => _statusEffectStatModifierHandler;
        public StatusEffectStatPeriodicHandler StatusEffectStatPeriodicHandler => _statusEffectStatPeriodicHandler;

        [Header("References (Progression)")]
        [SerializeField] private CharacterProgressionController _progressionController;
        [SerializeField] private CharacterProgressionUpgradeHandler _progressionUpgradeHandler;
        public CharacterProgressionController ProgressionController => _progressionController;
        public CharacterProgressionUpgradeHandler ProgressionUpgradeHandler => _progressionUpgradeHandler;

        [Header("References (Reward)")]

        [SerializeField] private CharacterRewardWalletController _rewardWalletController;
        public CharacterRewardWalletController RewardWalletController => _rewardWalletController;

        [Header("References (Inventory)")]
        [SerializeField] private InventoryController _inventoryController;
        public InventoryController InventoryController => _inventoryController;

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.IdentityController == null) _identityController = this.GetComponentInChildren<CharacterIdentityController>(true);

            if (this.RefreshmentHandler == null) _refreshmentHandler = this.GetComponentInChildren<CharacterRefreshmentHandler>(true);

            if (this.InputHandler == null) _inputHandler = this.GetComponent<CharacterInputHandler>();

            if (this.MovementHandler == null) _movementHandler = this.GetComponent<MovementHandler>();

            if (this.TerrainContactHandler == null) _terrainContactHandler = this.GetComponent<CharacterTerrainContactHandler>();

            if (this.VisualController                   == null) _visualController                  = this.GetComponentInChildren<CharacterVisualController>(true);
            if (this.VisualFacingHandler                == null) _visualFacingHandler               = this.GetComponentInChildren<CharacterVisualFacingHandler>(true);
            if (this.VisualLocomotionAnimationHandler   == null) _visualLocomotionAnimationHandler  = this.GetComponentInChildren<CharacterVisualLocomotionAnimationHandler>(true);
            if (this.VisualCombatAnimationHandler       == null) _visualCombatAnimationHandler      = this.GetComponentInChildren<CharacterVisualCombatAnimationHandler>(true);

            if (this.StatusEffectVFXHolder == null)
            { 
                Transform visual = this.VisualController?.transform;
                if (visual != null)
                {
                    _statusEffectVFXHolder = visual.Find("StatusEffectVFXHolder");
                }
            }
            if (this.RewardVFXHandler       == null) _rewardVFXHandler      = this.GetComponentInChildren<CharacterRewardVFXHandler>(true);

            if (this.CombatController                       == null) _combatController                      = this.GetComponentInChildren<CharacterCombatController>(true);
            if (this.CombatLoadoutController                == null) _combatLoadoutController               = this.GetComponentInChildren<CharacterCombatLoadoutController>(true);
            if (this.CombatLoadoutWeaponHandler             == null) _combatLoadoutWeaponHandler            = this.GetComponentInChildren<CharacterCombatLoadoutWeaponHandler>(true);
            if (this.CombatLoadoutSpecialSkillHandler       == null) _combatLoadoutSpecialSkillHandler      = this.GetComponentInChildren<CharacterCombatLoadoutSpecialSkillHandler>(true);
            if (this.CombatLoadoutConsumableItemHandler     == null) _combatLoadoutConsumableItemHandler    = this.GetComponentInChildren<CharacterCombatLoadoutConsumableItemHandler>(true);
            if (this.CombatAimingHandler                    == null) _combatAimingHandler                   = this.GetComponentInChildren<CharacterCombatAimingHandler>(true);
            if (this.CombatResourcesHandler                 == null) _combatResourcesHandler                = this.GetComponentInChildren<CharacterCombatResourcesHandler>(true);
            if (this.CombatStatusEffectApplicationHandler   == null) _combatStatusEffectApplicationHandler  = this.GetComponentInChildren<CharacterCombatStatusEffectApplicationHandler>(true);

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
        }
    }
}
