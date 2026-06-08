using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public partial class CharacterCombatLoadoutController : CharacterBehaviourBaseController, IStatusEffectFlagConsumer
    {
        // --- States ---
        //  1. References
        private CharacterCombatController CombatController => this.CharacterRootController.CombatController;
        private CharacterCombatResourcesHandler CombatResourcesHandler => this.CharacterRootController.CombatResourcesHandler;
        private CharacterCombatStatusEffectApplicationHandler CombatStatusEffectHandler => this.CharacterRootController.CombatStatusEffectApplicationHandler;
        private CharacterProgressionController ProgressionController => this.CharacterRootController.ProgressionController;
        private InventoryController InventoryController => this.CharacterRootController.InventoryController;

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
        public bool IsInAttackWindow => (this.CombatController != null && this.CombatController.IsInAttackWindow);

        // --- Unity Methods ---
        protected override void Awake()
        {
            base.Awake();

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
        //  6. Status Effect Flag consumption
        public void SetStatusEffectFlag(StatusEffectFlagTarget flagType, bool value)
        {
            switch (flagType)
            {
                case StatusEffectFlagTarget.CannotSwapLoadout:
                    this.CannotSwapLoadout = value;
                    return;

                case StatusEffectFlagTarget.IgnoreSwapLoadoutLock:
                    this.IgnoreSwapLoadoutLock = value;
                    return;

                default:
                    return;
            }
        }

        //  7. Refresh for Respawn: Reset all loadout states and ensure validity
        public void RefreshLoadoutAfterRespawn()
        {
            this.ClampEquippedWeaponsToCapacity();
            this.ClampEquippedConsumableItemsToCapacity();

            this.SyncEquippedStates();

            this.EnsureActiveWeaponIsValid();
            this.EnsureActiveConsumableItemIsValid();

            this.OnActiveEquippedWeaponChanged?.Invoke(this.ActiveEquippedWeaponEntry);
            this.OnActiveSpecialSkillChanged?.Invoke(this.ActiveEquippedRuntimeWeapon?.ActiveSpecialSkill);
            this.OnActiveEquippedConsumableItemChanged?.Invoke(this.ActiveEquippedConsumableItemEntry);
        }

        // --- Helpers ---
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
                    {
                        items[i].Equipped = false;
                    }
                }
            }

            // b. Mark weapons
            for (int i = 0; i < this.EquippedWeaponEntries.Count; i++)
            {
                if (this.EquippedWeaponEntries[i] != null)
                {
                    this.EquippedWeaponEntries[i].Equipped = true;
                }
            }

            // c. Mark consumables
            for (int i = 0; i < this.EquippedConsumableItemEntries.Count; i++)
            {
                if (this.EquippedConsumableItemEntries[i] != null)
                {
                    this.EquippedConsumableItemEntries[i].Equipped = true;
                }
            }
        }
    }
}
