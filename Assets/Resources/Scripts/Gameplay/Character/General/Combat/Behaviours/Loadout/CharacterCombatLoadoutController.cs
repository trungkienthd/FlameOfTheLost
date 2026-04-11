using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public partial class CharacterCombatLoadoutController : MonoBehaviour, IStatusEffectFlagConsumer
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private CharacterCombatAttackController _combatAttackController;
        [SerializeField] private CharacterCombatResourcesHandler _combatResourcesHandler;
        [SerializeField] private CharacterCombatStatusEffectHandler _combatStatusEffectHandler;
        [SerializeField] private InventoryController _inventoryController;
        private CharacterCombatAttackController CombatAttackController => _combatAttackController;
        private CharacterCombatResourcesHandler CombatResourcesHandler => _combatResourcesHandler;
        private CharacterCombatStatusEffectHandler CombatStatusEffectHandler => _combatStatusEffectHandler;
        private InventoryController InventoryController => _inventoryController;

        //  2. Loadout Capacity
        [Header("Loadout Capacity")]
        [SerializeField, Range(1, 4)] private int _maxEquippedWeapons = 2;
        [SerializeField, Range(1, 8)] private int _maxEquippedConsumableItems = 4;
        public int MaxEquippedWeapons
        {
            get => _maxEquippedWeapons;
            private set => _maxEquippedWeapons = Mathf.Max(1, value);
        }
        public int MaxEquippedConsumables
        {
            get => _maxEquippedConsumableItems;
            private set => _maxEquippedConsumableItems = Mathf.Max(1, value);
        }

        //  6. Status Effects states
        [Header("Status Effects Flags (Runtime)")]
        [SerializeField] private bool _cannotSwapLoadout;
        [SerializeField] private bool _ignoreSwapLock;
        private bool CannotSwapLoadout
        {
            get => _cannotSwapLoadout;
            set => _cannotSwapLoadout = value;
        }
        private bool IgnoreSwapLoadoutLock
        {
            get => _ignoreSwapLock;
            set => _ignoreSwapLock = value;
        }
        public bool IsLoadoutSwapLocked => this.CannotSwapLoadout && !this.IgnoreSwapLoadoutLock;

        //  3. Combat attack state
        public bool IsInAttackWindow => (this.CombatAttackController != null && this.CombatAttackController.IsInAttackWindow);

        // --- Unity Methods ---
        private void Awake()
        {
            if (this.CombatAttackController == null)    _combatAttackController     = this.GetComponent<CharacterCombatAttackController>();
            if (this.CombatResourcesHandler == null)    _combatResourcesHandler     = this.GetComponent<CharacterCombatResourcesHandler>();
            if (this.CombatStatusEffectHandler == null) _combatStatusEffectHandler  = this.GetComponent<CharacterCombatStatusEffectHandler>();
            if (this.InventoryController == null) _inventoryController = this.GetComponent<InventoryController>();

            this.ClampEquippedWeaponsToCapacity();
            this.ClampEquippedConsumableItemsToCapacity();

            this.SyncEquippedStates();

            this.EnsureActiveWeaponIsValid();
            this.EnsureActiveConsumableItemIsValid();
        }

        private void OnValidate()
        {
            if (this.MaxEquippedWeapons < 1) this.MaxEquippedWeapons = 1;
            if (this.MaxEquippedConsumables < 1) this.MaxEquippedConsumables = 1;
        }

        // --- Public APIs ---
        //  0. General Loadout Management
        //      0.1. Set Entry Equipped State
        private void SetEntryEquippedState(InventoryItemEntry entry, bool equipped)
        {
            if (entry == null) return;
            entry.Equipped = equipped;
        }
        //      0.2. Sync Equipped State for all entries
        private void SyncEquippedStates()
        {
            if (this.InventoryController == null) return;

            // a. Clear all
            var items = this.InventoryController.Items;
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] != null)
                        items[i].Equipped = false;
                }
            }

            // b. Mark weapons
            for (int i = 0; i < this.EquippedWeaponEntries.Count; i++)
            {
                if (this.EquippedWeaponEntries[i] != null)
                    this.EquippedWeaponEntries[i].Equipped = true;
            }

            // c. Mark consumables
            for (int i = 0; i < this.EquippedConsumableItemEntries.Count; i++)
            {
                if (this.EquippedConsumableItemEntries[i] != null)
                    this.EquippedConsumableItemEntries[i].Equipped = true;
            }
        }


        //  6. Status Effect Flag consumption
        public void SetStatusEffectFlag(StatusEffectFlagType flagType, bool value)
        {
            switch (flagType)
            {
                case StatusEffectFlagType.CannotSwapLoadout:
                    this.CannotSwapLoadout = value;
                    return;

                case StatusEffectFlagType.IgnoreSwapLoadoutLock:
                    this.IgnoreSwapLoadoutLock = value;
                    return;

                default:
                    return;
            }
        }
    }
}
