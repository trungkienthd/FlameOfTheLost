using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(CharacterCombatLoadoutWeaponHandler))]
    [RequireComponent(typeof(CharacterCombatLoadoutProjectileHandler))]
    [RequireComponent(typeof(CharacterCombatLoadoutSpecialSkillHandler))]
    [RequireComponent(typeof(CharacterCombatLoadoutConsumableItemHandler))]
    public class CharacterCombatLoadoutController : CharacterBehaviourControllerBase, IStatusEffectFlagConsumer, ICharacterDeathDisableable
    {
        // --- Referneces ---
        private CharacterCombatController CombatController => this.CharacterRootController.CombatController;

        private CharacterCombatLoadoutWeaponHandler WeaponHandler => this.CharacterRootController.CombatLoadoutWeaponHandler;
        private CharacterCombatLoadoutProjectileHandler ProjectileHandler => this.CharacterRootController.CombatLoadoutProjectileHandler;
        private CharacterCombatLoadoutSpecialSkillHandler SpecialSkillHandler => this.CharacterRootController.CombatLoadoutSpecialSkillHandler;
        private CharacterCombatLoadoutConsumableItemHandler ConsumableItemHandler => this.CharacterRootController.CombatLoadoutConsumableItemHandler;

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
        public bool IsInAttackWindow => (this.CombatController != null && this.CombatController.IsInAttackWindow);
        public bool IsAttackLocked => (this.CombatController != null && this.CombatController.IsAttackLocked);

        //  3. Weapon APIs
        public int MaxEquippedWeaponCapacity => this.WeaponHandler.MaxEquippedEntryCapacity;
        public int MaxEquippedWeapons => this.WeaponHandler.MaxEquippedEntries;
        public IReadOnlyList<InventoryItemEntry> EquippedWeaponEntries => this.WeaponHandler.EquippedEntries;
        public InventoryItemEntry ActiveEquippedWeaponEntry => this.WeaponHandler.ActiveEquippedEntry;
        public RuntimeWeapon ActiveEquippedRuntimeWeapon => this.WeaponHandler.ActiveEquippedRuntimeWeapon;

        //  4. Projectile APIs
        public int MaxEquippedProjectileCapacity => this.ProjectileHandler.MaxEquippedEntryCapacity;
        public int MaxEquippedProjectiles => this.ProjectileHandler.MaxEquippedEntries;
        public IReadOnlyList<InventoryItemEntry> EquippedProjectileEntries => this.ProjectileHandler.EquippedEntries;
        public InventoryItemEntry ActiveEquippedProjectileEntry => this.ProjectileHandler.ActiveEquippedEntry;

        //  5. Consumable item APIs
        public int MaxEquippedConsumableCapacity => this.ConsumableItemHandler.MaxEquippedEntryCapacity;
        public int MaxEquippedConsumables => this.ConsumableItemHandler.MaxEquippedEntries;
        public IReadOnlyList<InventoryItemEntry> EquippedConsumableItemEntries => this.ConsumableItemHandler.EquippedEntries;
        public InventoryItemEntry ActiveEquippedConsumableItemEntry => this.ConsumableItemHandler.ActiveEquippedEntry;

        //  6. Death Disable State (Runtime)
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
        //  1. Progression-affected Loadout Max Capacity
        public void SetMaxEquippedWeaponsFromProgression(int amount)
        {
            this.WeaponHandler.SetMaxEquippedEntriesFromProgression(amount);

            this.NotifyEquippedWeaponEntriesChanged();
        }
        public void SetMaxEquippedProjectilesFromProgression(int amount)
        {
            this.ProjectileHandler.SetMaxEquippedEntriesFromProgression(amount);
        }
        public void SetMaxEquippedConsumableItemsFromProgression(int amount)
        {
            this.ConsumableItemHandler.SetMaxEquippedEntriesFromProgression(amount);
        }

        //  2. Weapon Management
        //      2.1. Cycle Weapon Entry in the equipped list: Step:
        //          PositiveValue = Next,
        //          NegativeValue = Previous
        public bool TryCycleWeaponEntry(int step)
        {
            if (this.IsDeathDisabled) return false;

            return this.WeaponHandler.TryCycleEquippedEntry(step);
        }

        //      2.2. Replace Equipped Weapon at specific slot index, returning removed weapon if any
        public bool TryReplaceEquippedWeaponEntry(int slotIndex, InventoryItemEntry newWeaponEntry, out InventoryItemEntry removedWeaponEntry)
        {
            if (this.IsDeathDisabled)
            {
                removedWeaponEntry = null;
                return false;
            }

            return this.WeaponHandler.TryReplaceEquippedEntry(slotIndex, newWeaponEntry, out removedWeaponEntry);
        }

        //      2.3. Unequip Weapon
        public bool TryUnequipWeaponEntry(InventoryItemEntry weaponEntry)
        {
            if (this.IsDeathDisabled) return false;

            return this.WeaponHandler.TryUnequipEntry(weaponEntry);
        }

        //      2.4. Quick equip weapon
        public bool TryQuickEquipWeapon()
        {
            if (this.IsDeathDisabled) return false;

            return this.WeaponHandler.TryQuickEquip();
        }

        //  3. Projectile Management
        //      3.1. Cycle Projectile Entry in the equipped list. Step:
        //          PositiveValue = Next,
        //          NegativeValue = Previous
        public bool TryCycleProjectileEntry(int step)
        {
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryCycleEquippedEntry(step);
        }

        //      3.2. Replace Equipped Projectile at specific slot index, returning removed projectile if any
        public bool TryReplaceEquippedProjectileEntry(int slotIndex, InventoryItemEntry newProjectileEntry, out InventoryItemEntry removedProjectileEntry)
        {
            if (this.IsDeathDisabled)
            {
                removedProjectileEntry = null;
                return false;
            }

            return this.ProjectileHandler.TryReplaceEquippedEntry(slotIndex, newProjectileEntry, out removedProjectileEntry);
        }

        //      3.3. Unequip Projectile
        public bool TryUnequipEquippedProjectileEntry(InventoryItemEntry projectileEntry)
        {
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryUnequipEntry(projectileEntry);
        }

        //     3.4. Quick equip projectile
        public bool TryQuickEquipProjectile()
        {
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryQuickEquip();
        }

        //      3.5. Resolve Projectile from Entry
        public bool TryResolveProjectileForRangedSkill(RangedSkillProfile rangedSkill, out ProjectileProfile projectileItem)
        {
            projectileItem = null;
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryResolveProjectileForRangedSkill(rangedSkill, out projectileItem);
        }

        //      3.6. Consume Projectile from (Equipped) Entry
        public bool TryConsumeProjectileForRangedSkill(RangedSkillProfile rangedSkill, out ProjectileCostRecord projectileCostReceipt)
        {
            projectileCostReceipt = null;
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryConsumeProjectileForRangedSkill(rangedSkill, out projectileCostReceipt);
        }

        //      3.7. Refund Projectile to (Equipped) Entry
        public void RefundProjectileCost(ProjectileCostRecord projectileCostRecord)
        {
            this.ProjectileHandler.RefundProjectileCost(projectileCostRecord);
        }

        //  4. Active Special Skill handling
        //      4.1. Attach / Replace runtime active special skill on an equipped weapon
        public bool TrySetEquippedWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry, SkillProfile specialSkill)
        {
            if (this.IsDeathDisabled) return false;

            return this.SpecialSkillHandler.TrySetEquippedWeaponActiveSpecialSkill(weaponEntry, specialSkill);
        }

        //      4.2. Quick set active equipped weapon's active special skill
        public bool TrySetActiveEquippedWeaponActiveSpecialSkill(SkillProfile specialSkill)
        {
            if (this.IsDeathDisabled) return false;

            return this.SpecialSkillHandler.TrySetActiveEquippedWeaponActiveSpecialSkill(specialSkill);
        }

        //      4.3. Clear runtime active special skill so resolution falls back to innate special skill
        public bool TryClearWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            if (this.IsDeathDisabled) return false;

            return this.SpecialSkillHandler.TryClearWeaponActiveSpecialSkill(weaponEntry);
        }

        //      4.4. Get runtime active special skill directly (not resolved fallback)
        public SkillProfile GetWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            return this.SpecialSkillHandler.GetWeaponActiveSpecialSkill(weaponEntry);
        }

        //  5. Consumable Item Management
        //      5.1. Cycle through equipped consumable items in a specified direction (step: +1 for next, -1 for previous),
        //          with optional wrapping around the list, and set the next valid consumable item as active.
        public bool TryCycleConsumableItemEntry(int step)
        {
            if (this.IsDeathDisabled) return false;

            return this.ConsumableItemHandler.TryCycleEquippedEntry(step);
        }

        //      5.2. Try to replace an equipped consumable item entry at a specified slot index with a new entry,
        //          with validation and checks to ensure the new entry is valid and not already equipped in another slot,
        //          and update the active entry reference if necessary.
        public bool TryReplaceEquippedConsumableItemEntry(int slotIndex, InventoryItemEntry newConsumableEntry, out InventoryItemEntry removedConsumableEntry)
        {
            if (this.IsDeathDisabled)
            {
                removedConsumableEntry = null;
                return false;
            }

            return this.ConsumableItemHandler.TryReplaceEquippedEntry(slotIndex, newConsumableEntry, out removedConsumableEntry);
        }

        //      5.3. Try to unequip an equipped consumable item entry, with checks to ensure the entry is currently equipped,
        //          and update the active entry reference if necessary.
        public bool TryUnequipEquippedConsumableItemEntry(InventoryItemEntry consumableEntry)
        {
            if (this.IsDeathDisabled) return false;

            return this.ConsumableItemHandler.TryUnequipEntry(consumableEntry);
        }

        //      5.4. Try to consume the active equipped consumable item, applying its effects and reducing quantity,
        public bool TryConsumeConsumableItem()
        {
            if (this.IsDeathDisabled) return false;

            return this.ConsumableItemHandler.TryConsume();
        }

        //  6. UI APIs
        //      6.1. Get Equipped Weapon (Active) Icon
        public Sprite GetActiveEquippedWeaponIcon()
        {
            return this.WeaponHandler.GetActiveEntryIcon();
        }

        //      6.2. Get Equipped Projectile (Active) Icon
        public Sprite GetActiveEquippedProjectileIcon()
        {
            return this.ProjectileHandler.GetActiveEntryIcon();
        }

        //      6.3. Get Current Special Skill (Active) Icon
        public Sprite GetActiveSpecialSkillIcon()
        {
            return this.SpecialSkillHandler.GetActiveSpecialSkillIcon();
        }

        //      6.4. Get Equipped Consumable Item (Active) Icon
        public Sprite GetActiveConsumableItemIcon()
        {
            return this.ConsumableItemHandler.GetActiveEntryIcon();
        }

        //  7. Event Notifiers
        public void NotifyEquippedWeaponEntriesChanged()
        {
            this.OnEquippedWeaponEntriesChanged?.Invoke();
        }
        public void NotifyActiveEquippedWeaponChanged(InventoryItemEntry activeWeaponEntry)
        {
            this.OnActiveEquippedWeaponChanged?.Invoke(activeWeaponEntry);
        }
        public void NotifyActiveEquippedProjectileChanged(InventoryItemEntry activeProjectileEntry)
        {
            this.OnActiveEquippedProjectileChanged?.Invoke(activeProjectileEntry);
        }
        public void NotifyActiveSpecialSkillChanged(SkillProfile activeSpecialSkill)
        {
            this.OnActiveSpecialSkillChanged?.Invoke(activeSpecialSkill);
        }
        public void NotifyActiveEquippedConsumableItemChanged(InventoryItemEntry activeConsumableItemEntry)
        {
            this.OnActiveEquippedConsumableItemChanged?.Invoke(activeConsumableItemEntry);
        }

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
    }
}
