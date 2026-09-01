using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(CharacterCombatLoadoutCapacityHandler))]
    [RequireComponent(typeof(CharacterCombatLoadoutWeaponHandler))]
    [RequireComponent(typeof(CharacterCombatLoadoutProjectileHandler))]
    [RequireComponent(typeof(CharacterCombatLoadoutSpecialSkillHandler))]
    [RequireComponent(typeof(CharacterCombatLoadoutConsumableItemHandler))]
    public class CharacterCombatLoadoutController : CharacterBehaviourControllerBase, 
        IStatusEffectFlagConsumer, 
        ICharacterDeathDisableable
    {
        // --- Referneces ---
        //  1. Character
        private CharacterCombatController CombatController => this.CharacterRootController != null
            ? this.CharacterRootController.CombatController
            : null;
        internal InventoryController InventoryController => this.CharacterRootController != null
            ? this.CharacterRootController.InventoryController
            : null;
        internal CharacterProgressionController ProgressionController => this.CharacterRootController != null
            ? this.CharacterRootController.ProgressionController
            : null;
        internal CharacterCombatResourcesHandler CombatResourcesHandler => this.CharacterRootController != null
            ? this.CharacterRootController.CombatResourcesHandler
            : null;
        internal CharacterCombatStatusEffectApplicationHandler CombatStatusEffectApplicationHandler => this.CharacterRootController != null
            ? this.CharacterRootController.CombatStatusEffectApplicationHandler
            : null;

        //  2. Sub-Behaviours
        [Header("References - Sub Behaviours")]
        [SerializeField] private CharacterCombatLoadoutCapacityHandler _capacityHandler;
        [SerializeField] private CharacterCombatLoadoutWeaponHandler _weaponHandler;
        [SerializeField] private CharacterCombatLoadoutProjectileHandler _projectileHandler;
        [SerializeField] private CharacterCombatLoadoutSpecialSkillHandler _specialSkillHandler;
        [SerializeField] private CharacterCombatLoadoutConsumableItemHandler _consumableItemHandler;
        internal CharacterCombatLoadoutCapacityHandler CapacityHandler => _capacityHandler;
        internal CharacterCombatLoadoutWeaponHandler WeaponHandler => _weaponHandler;
        internal CharacterCombatLoadoutProjectileHandler ProjectileHandler => _projectileHandler;
        internal CharacterCombatLoadoutSpecialSkillHandler SpecialSkillHandler => _specialSkillHandler;
        internal CharacterCombatLoadoutConsumableItemHandler ConsumableItemHandler => _consumableItemHandler;

        // --- Runtime ---
        //  1. Status Effects states
        [Header("Runtime - Status Effects Flags")]
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

        //  2. Combat attack state
        public bool IsInAttackWindow    => (this.CombatController != null && this.CombatController.IsInAttackWindow);
        public bool IsAttackLocked      => (this.CombatController != null && this.CombatController.IsAttackLocked);

        //  3. Death Disable State (Runtime)
        [Header("Runtime - Death Disable")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        // --- Events ---
        public event Action OnEquippedWeaponEntriesChanged;
        public event Action<InventoryItemEntry> OnActiveEquippedWeaponChanged;
        public event Action<InventoryItemEntry> OnActiveEquippedProjectileChanged;
        public event Action<SkillProfile> OnActiveSpecialSkillChanged;
        public event Action<InventoryItemEntry> OnActiveEquippedConsumableItemChanged;

        // --- Public APIs ---
        //  0. Properties
        //      0.1. Weapon APIs
        public int MaxEquippedWeaponCapacity                            => this.WeaponHandler.MaxEquippedEntryCapacity;
        public int MaxEquippedWeapons                                   => this.WeaponHandler.MaxEquippedEntries;
        public IReadOnlyList<InventoryItemEntry> EquippedWeaponEntries  => this.WeaponHandler.EquippedEntries;
        public InventoryItemEntry ActiveEquippedWeaponEntry             => this.WeaponHandler.ActiveEquippedEntry;
        public RuntimeWeapon ActiveEquippedRuntimeWeapon                => this.WeaponHandler.ActiveEquippedRuntimeWeapon;

        //      0.2. Projectile APIs
        public int MaxEquippedProjectileCapacity                            => this.ProjectileHandler.MaxEquippedEntryCapacity;
        public int MaxEquippedProjectiles                                   => this.ProjectileHandler.MaxEquippedEntries;
        public IReadOnlyList<InventoryItemEntry> EquippedProjectileEntries  => this.ProjectileHandler.EquippedEntries;
        public InventoryItemEntry ActiveEquippedProjectileEntry             => this.ProjectileHandler.ActiveEquippedEntry;

        //      0.3. Consumable item APIs
        public int MaxEquippedConsumableCapacity                                => this.ConsumableItemHandler.MaxEquippedEntryCapacity;
        public int MaxEquippedConsumables                                       => this.ConsumableItemHandler.MaxEquippedEntries;
        public IReadOnlyList<InventoryItemEntry> EquippedConsumableItemEntries  => this.ConsumableItemHandler.EquippedEntries;
        public InventoryItemEntry ActiveEquippedConsumableItemEntry             => this.ConsumableItemHandler.ActiveEquippedEntry;

        //  1. Progression-affected Loadout Max Capacity
        public void SetMaxEquippedWeaponsFromProgression(int amount)            => this.CapacityHandler.SetMaxEquippedWeaponsFromProgression(amount);
        public void SetMaxEquippedProjectilesFromProgression(int amount)        => this.CapacityHandler.SetMaxEquippedProjectilesFromProgression(amount);
        public void SetMaxEquippedConsumableItemsFromProgression(int amount)    => this.CapacityHandler.SetMaxEquippedConsumableItemsFromProgression(amount);

        //  2. Weapon Management
        public bool TryCycleWeaponEntry(int step)
        {
            if (this.IsDeathDisabled) return false;

            return this.WeaponHandler.TryCycleEquippedEntry(step);
        }
        public bool TryReplaceEquippedWeaponEntry(int slotIndex, InventoryItemEntry newWeaponEntry, out InventoryItemEntry removedWeaponEntry)
        {
            if (this.IsDeathDisabled)
            {
                removedWeaponEntry = null;
                return false;
            }

            return this.WeaponHandler.TryReplaceEquippedEntry(slotIndex, newWeaponEntry, out removedWeaponEntry);
        }
        public bool TryUnequipWeaponEntry(InventoryItemEntry weaponEntry)
        {
            if (this.IsDeathDisabled) return false;

            return this.WeaponHandler.TryUnequipEntry(weaponEntry);
        }
        public bool TryQuickEquipWeapon()
        {
            if (this.IsDeathDisabled) return false;

            return this.WeaponHandler.TryQuickEquip();
        }

        //  3. Projectile Management
        public bool TryCycleProjectileEntry(int step)
        {
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryCycleEquippedEntry(step);
        }
        public bool TryReplaceEquippedProjectileEntry(int slotIndex, InventoryItemEntry newProjectileEntry, out InventoryItemEntry removedProjectileEntry)
        {
            if (this.IsDeathDisabled)
            {
                removedProjectileEntry = null;
                return false;
            }

            return this.ProjectileHandler.TryReplaceEquippedEntry(slotIndex, newProjectileEntry, out removedProjectileEntry);
        }
        public bool TryUnequipEquippedProjectileEntry(InventoryItemEntry projectileEntry)
        {
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryUnequipEntry(projectileEntry);
        }
        public bool TryQuickEquipProjectile()
        {
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryQuickEquip();
        }
        public bool TryResolveProjectileForRangedSkill(RangedSkillProfile rangedSkill, out ProjectileProfile projectileItem)
        {
            projectileItem = null;
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryResolveProjectileForRangedSkill(rangedSkill, out projectileItem);
        }
        public bool TryConsumeProjectileForRangedSkill(RangedSkillProfile rangedSkill, out ProjectileCostRecord projectileCostReceipt)
        {
            projectileCostReceipt = null;
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryConsumeProjectileForRangedSkill(rangedSkill, out projectileCostReceipt);
        }
        public void RefundProjectileCost(ProjectileCostRecord projectileCostRecord)
        {
            this.ProjectileHandler.RefundProjectileCost(projectileCostRecord);
        }

        //  4. Active Special Skill handling
        public bool TrySetEquippedWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry, SkillProfile specialSkill)
        {
            if (this.IsDeathDisabled) return false;

            return this.SpecialSkillHandler.TrySetEquippedWeaponActiveSpecialSkill(weaponEntry, specialSkill);
        }
        public bool TrySetActiveEquippedWeaponActiveSpecialSkill(SkillProfile specialSkill)
        {
            if (this.IsDeathDisabled) return false;

            return this.SpecialSkillHandler.TrySetActiveEquippedWeaponActiveSpecialSkill(specialSkill);
        }
        public bool TryClearWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            if (this.IsDeathDisabled) return false;

            return this.SpecialSkillHandler.TryClearWeaponActiveSpecialSkill(weaponEntry);
        }
        public SkillProfile GetWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            return this.SpecialSkillHandler.GetWeaponActiveSpecialSkill(weaponEntry);
        }

        //  5. Consumable Item Management
        public bool TryCycleConsumableItemEntry(int step)
        {
            if (this.IsDeathDisabled) return false;

            return this.ConsumableItemHandler.TryCycleEquippedEntry(step);
        }
        public bool TryReplaceEquippedConsumableItemEntry(int slotIndex, InventoryItemEntry newConsumableEntry, out InventoryItemEntry removedConsumableEntry)
        {
            if (this.IsDeathDisabled)
            {
                removedConsumableEntry = null;
                return false;
            }

            return this.ConsumableItemHandler.TryReplaceEquippedEntry(slotIndex, newConsumableEntry, out removedConsumableEntry);
        }
        public bool TryUnequipEquippedConsumableItemEntry(InventoryItemEntry consumableEntry)
        {
            if (this.IsDeathDisabled) return false;

            return this.ConsumableItemHandler.TryUnequipEntry(consumableEntry);
        }
        public bool TryConsumeConsumableItem()
        {
            if (this.IsDeathDisabled) return false;

            return this.ConsumableItemHandler.TryConsume();
        }

        //  6. UI APIs
        public Sprite GetActiveEquippedWeaponIcon()     => this.WeaponHandler.GetActiveEntryIcon();
        public Sprite GetActiveEquippedProjectileIcon() => this.ProjectileHandler.GetActiveEntryIcon();
        public Sprite GetActiveSpecialSkillIcon()       => this.SpecialSkillHandler.GetActiveSpecialSkillIcon();
        public Sprite GetActiveConsumableItemIcon()     => this.ConsumableItemHandler.GetActiveEntryIcon();


        //  7. Event Notifiers
        internal void NotifyEquippedWeaponEntriesChanged()                                                    => this.OnEquippedWeaponEntriesChanged?.Invoke();
        internal void NotifyActiveEquippedWeaponChanged(InventoryItemEntry activeWeaponEntry)                 => this.OnActiveEquippedWeaponChanged?.Invoke(activeWeaponEntry);
        internal void NotifyActiveEquippedProjectileChanged(InventoryItemEntry activeProjectileEntry)         => this.OnActiveEquippedProjectileChanged?.Invoke(activeProjectileEntry);
        internal void NotifyActiveSpecialSkillChanged(SkillProfile activeSpecialSkill)                        => this.OnActiveSpecialSkillChanged?.Invoke(activeSpecialSkill);
        internal void NotifyActiveEquippedConsumableItemChanged(InventoryItemEntry activeConsumableItemEntry) => this.OnActiveEquippedConsumableItemChanged?.Invoke(activeConsumableItemEntry);

        //  8. IStatusEffectFlagConsumer implementation
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

        //  9. Refresh Character: Reset all loadout states and ensure validity
        public void RefreshLoadoutAfterCharacterRefreshment()
        {
            this.IsDeathDisabled = false;

            this.WeaponHandler.RefreshLoadoutAfterCharacterRefreshment();
            this.ProjectileHandler.RefreshLoadoutAfterCharacterRefreshment();
            this.SpecialSkillHandler.RefreshLoadoutAfterCharacterRefreshment();
            this.ConsumableItemHandler.RefreshLoadoutAfterCharacterRefreshment();
        }

        //  10. ICharacterDeathDisableable implementation
        public int DeathDisablePriority => 40;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.CapacityHandler        == null) _capacityHandler       = this.GetComponent<CharacterCombatLoadoutCapacityHandler>();
            if (this.WeaponHandler          == null) _weaponHandler         = this.GetComponent<CharacterCombatLoadoutWeaponHandler>();
            if (this.ProjectileHandler      == null) _projectileHandler     = this.GetComponent<CharacterCombatLoadoutProjectileHandler>();
            if (this.SpecialSkillHandler    == null) _specialSkillHandler   = this.GetComponent<CharacterCombatLoadoutSpecialSkillHandler>();
            if (this.ConsumableItemHandler  == null) _consumableItemHandler = this.GetComponent<CharacterCombatLoadoutConsumableItemHandler>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.CapacityHandler        == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] {nameof(this.CapacityHandler)} reference is missing.", this);
            if (this.WeaponHandler          == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] {nameof(this.WeaponHandler)} reference is missing.", this);
            if (this.ProjectileHandler      == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] " + $"{nameof(this.ProjectileHandler)} reference is missing.", this);
            if (this.SpecialSkillHandler    == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] {nameof(this.SpecialSkillHandler)} reference is missing.", this);
            if (this.ConsumableItemHandler  == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] {nameof(this.ConsumableItemHandler)} reference is missing.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.CapacityHandler?.Bind(this);

            this.WeaponHandler?.Bind(this);
            this.ProjectileHandler?.Bind(this);
            this.SpecialSkillHandler?.Bind(this);
            this.ConsumableItemHandler?.Bind(this);
        }
        protected override void UnbindInternalReferences()
        {
            this.ConsumableItemHandler?.Unbind(this);
            this.SpecialSkillHandler?.Unbind(this);
            this.ProjectileHandler?.Unbind(this);
            this.WeaponHandler?.Unbind(this);

            this.CapacityHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
