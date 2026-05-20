using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterComponentController : MonoBehaviour
    {
        [Header("References (Core)")]
        [SerializeField] private CharacterIdentityController _identityController;
        [SerializeField] private MovementHandler _movementHandler;
        [SerializeField] private CharacterTerrainContactHandler _terrainContactHandler;
        public CharacterIdentityController IdentityController => _identityController;
        public MovementHandler MovementHandler => _movementHandler;
        public CharacterTerrainContactHandler TerrainContactHandler => _terrainContactHandler;

        [Header("References (Visual)")]
        [SerializeField] private CharacterVisualController _visualController;
        public CharacterVisualController VisualController => _visualController;

        [Header("References (Combat)")]
        [SerializeField] private CharacterCombatController _combatController;
        [SerializeField] private CharacterCombatLoadoutController _combatLoadoutController;
        [SerializeField] private CharacterCombatAimingHandler _combatAimingHandler;
        [SerializeField] private CharacterCombatResourcesHandler _combatResourcesHandler;
        [SerializeField] private CharacterCombatStatusEffectApplicationHandler _combatStatusEffectApplicationHandler;
        public CharacterCombatController CombatController => _combatController;
        public CharacterCombatLoadoutController CombatLoadoutController => _combatLoadoutController;
        public CharacterCombatAimingHandler CombatAimingHandler => _combatAimingHandler;
        public CharacterCombatResourcesHandler CombatResourcesHandler => _combatResourcesHandler;
        public CharacterCombatStatusEffectApplicationHandler CombatStatusEffectApplicationHandler => _combatStatusEffectApplicationHandler;

        [Header("References (Resources)")]
        [SerializeField] private CharacterResourcesController _resourcesController;
        [SerializeField] private HPController _hpController;
        [SerializeField] private FPController _fpController;
        [SerializeField] private StaminaController _staminaController;
        public CharacterResourcesController ResourcesController => _resourcesController;
        public HPController HPController => _hpController;
        public FPController FPController => _fpController;
        public StaminaController StaminaController => _staminaController;

        [Header("References (Status Effects)")]
        [SerializeField] private StatusEffectController _statusEffectController;
        [SerializeField] private StatusEffectFlagHandler _statusEffectFlagHandler;
        [SerializeField] private StatusEffectStatModifierHandler _statusEffectStatModifierHandler;
        [SerializeField] private StatusEffectStatPeriodicHandler _statusEffectStatPeriodicHandler;
        public StatusEffectController StatusEffectController => _statusEffectController;
        public StatusEffectFlagHandler StatusEffectFlagHandler => _statusEffectFlagHandler;
        public StatusEffectStatModifierHandler StatusEffectStatModifierHandler => _statusEffectStatModifierHandler;
        public StatusEffectStatPeriodicHandler StatusEffectStatPeriodicHandler => _statusEffectStatPeriodicHandler;

        [Header("References (Inventory)")]
        [SerializeField] private InventoryController _inventoryController;
        public InventoryController InventoryController => _inventoryController;

        private void Awake()
        {
            this.CacheReferences();
        }

        protected virtual void CacheReferences()
        {
            if (this.IdentityController == null) _identityController = this.GetComponent<CharacterIdentityController>();

            if (this.MovementHandler == null) _movementHandler = this.GetComponent<MovementHandler>();

            if (this.TerrainContactHandler == null) _terrainContactHandler = this.GetComponent<CharacterTerrainContactHandler>();

            if (this.VisualController == null) _visualController = this.GetComponentInChildren<CharacterVisualController>(true);

            if (this.CombatController                       == null) _combatController                      = this.GetComponentInChildren<CharacterCombatController>(true);
            if (this.CombatLoadoutController                == null) _combatLoadoutController               = this.GetComponentInChildren<CharacterCombatLoadoutController>(true);
            if (this.CombatAimingHandler                    == null) _combatAimingHandler                   = this.GetComponentInChildren<CharacterCombatAimingHandler>(true);
            if (this.CombatResourcesHandler                 == null) _combatResourcesHandler                = this.GetComponentInChildren<CharacterCombatResourcesHandler>(true);
            if (this.CombatStatusEffectApplicationHandler   == null) _combatStatusEffectApplicationHandler  = this.GetComponentInChildren<CharacterCombatStatusEffectApplicationHandler>(true);

            if (this.ResourcesController    == null) _resourcesController   = this.GetComponentInChildren<CharacterResourcesController>(true);
            if (this.HPController           == null) _hpController          = this.GetComponentInChildren<HPController>(true);
            if (this.FPController           == null) _fpController          = this.GetComponentInChildren<FPController>(true);
            if (this.StaminaController      == null) _staminaController     = this.GetComponentInChildren<StaminaController>(true);

            if (this.StatusEffectController             == null) _statusEffectController            = this.GetComponentInChildren<StatusEffectController>(true);
            if (this.StatusEffectFlagHandler            == null) _statusEffectFlagHandler           = this.GetComponentInChildren<StatusEffectFlagHandler>(true);
            if (this.StatusEffectStatModifierHandler    == null) _statusEffectStatModifierHandler   = this.GetComponentInChildren<StatusEffectStatModifierHandler>(true);
            if (this.StatusEffectStatPeriodicHandler    == null) _statusEffectStatPeriodicHandler   = this.GetComponentInChildren<StatusEffectStatPeriodicHandler>(true);

            if (this.InventoryController == null) _inventoryController = this.GetComponentInChildren<InventoryController>(true);
        }
    }
}
