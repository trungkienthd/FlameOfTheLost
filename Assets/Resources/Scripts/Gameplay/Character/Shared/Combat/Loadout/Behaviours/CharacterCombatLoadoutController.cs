using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(CharacterCombatLoadoutWeaponHandler))]
    [RequireComponent(typeof(CharacterCombatLoadoutSpecialSkillHandler))]
    [RequireComponent(typeof(CharacterCombatLoadoutConsumableItemHandler))]
    public class CharacterCombatLoadoutController : CharacterBehaviourControllerBase, IStatusEffectFlagConsumer
    {
        // --- States ---
        //  1. References
        private CharacterCombatController CombatController => this.CharacterRootController.CombatController;

        private CharacterCombatLoadoutWeaponHandler WeaponHandler => this.CharacterRootController.CombatLoadoutWeaponHandler;
        private CharacterCombatLoadoutSpecialSkillHandler SpecialSkillHandler => this.CharacterRootController.CombatLoadoutSpecialSkillHandler;
        private CharacterCombatLoadoutConsumableItemHandler ConsumableItemHandler => this.CharacterRootController.CombatLoadoutConsumableItemHandler;

        //  2. Status Effects states
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
        public bool IsAttackLocked => (this.CombatController != null && this.CombatController.IsAttackLocked);

        //  4. Weapon references
        public int MaxEquippedWeapons => this.WeaponHandler.MaxEquippedEntries;
        public IReadOnlyList<InventoryItemEntry> EquippedWeaponEntries => this.WeaponHandler.EquippedEntries;
        public InventoryItemEntry ActiveEquippedWeaponEntry => this.WeaponHandler.ActiveEquippedEntry;
        public RuntimeWeapon ActiveEquippedRuntimeWeapon => this.WeaponHandler.ActiveEquippedRuntimeWeapon;

        //  5. Consumable item references
        public int MaxEquippedConsumables => this.ConsumableItemHandler.MaxEquippedEntries;
        public IReadOnlyList<InventoryItemEntry> EquippedConsumableItemEntries => this.ConsumableItemHandler.EquippedEntries;
        public InventoryItemEntry ActiveEquippedConsumableItemEntry => this.ConsumableItemHandler.ActiveEquippedEntry;

        // --- Events ---
        public event Action<InventoryItemEntry> OnActiveEquippedWeaponChanged;
        public event Action<SkillProfile> OnActiveSpecialSkillChanged;
        public event Action<InventoryItemEntry> OnActiveEquippedConsumableItemChanged;

        // --- Public APIs ---
        //  1. Progression-affected Loadout Max Capacity
        public void SetMaxEquippedWeaponsFromProgression(int amount)
        {
            this.WeaponHandler.SetMaxEquippedEntriesFromProgression(amount);
        }
        public void SetMaxEquippedConsumablesFromProgression(int amount)
        {
            this.ConsumableItemHandler.SetMaxEquippedEntriesFromProgression(amount);
        }

        //  2. Weapon Management
        //      2.1. Cycle Weapon Entry in the equipped list: Step:
        //          PositiveValue = Next,
        //          NegativeValue = Previous
        public bool TryCycleWeaponEntry(int step)
        {
            return this.WeaponHandler.TryCycleEquippedEntry(step);
        }

        //      2.2. Replace Equipped Weapon at specific slot index, returning removed weapon if any
        public bool TryReplaceEquippedWeaponEntry(int slotIndex, InventoryItemEntry newWeaponEntry, out InventoryItemEntry removedWeaponEntry)
        {
            return this.WeaponHandler.TryReplaceEquippedEntry(slotIndex, newWeaponEntry, out removedWeaponEntry);
        }

        //      2.3. Unequip Weapon
        public bool TryUnequipWeaponEntry(InventoryItemEntry weaponEntry)
        {
            return this.WeaponHandler.TryUnequipEntry(weaponEntry);
        }

        //      2.4. Quick equip weapon
        public bool TryQuickEquipWeapon()
        {
            return this.WeaponHandler.TryQuickEquip();
        }

        //  3. Active Special Skill handling
        //      3.1. Attach / Replace runtime active special skill on an equipped weapon
        public bool TrySetEquippedWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry, SkillProfile specialSkill)
        {
            return this.SpecialSkillHandler.TrySetEquippedWeaponActiveSpecialSkill(weaponEntry, specialSkill);
        }

        //      3.2. Quick set active equipped weapon's active special skill
        public bool TrySetActiveEquippedWeaponActiveSpecialSkill(SkillProfile specialSkill)
        {
            return this.SpecialSkillHandler.TrySetActiveEquippedWeaponActiveSpecialSkill(specialSkill);
        }

        //      3.3. Clear runtime active special skill so resolution falls back to innate special skill
        public bool TryClearWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            return this.SpecialSkillHandler.TryClearWeaponActiveSpecialSkill(weaponEntry);
        }

        //      3.4. Get runtime active special skill directly (not resolved fallback)
        public SkillProfile GetWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            return this.SpecialSkillHandler.GetWeaponActiveSpecialSkill(weaponEntry);
        }

        //  4. Consumable Item Management
        //      4.1. Cycle through equipped consumable items in a specified direction (step: +1 for next, -1 for previous),
        //          with optional wrapping around the list, and set the next valid consumable item as active.
        public bool TryCycleConsumableItemEntry(int step)
        {
            return this.ConsumableItemHandler.TryCycleEquippedEntry(step);
        }

        //      4.2. Try to replace an equipped consumable item entry at a specified slot index with a new entry,
        //          with validation and checks to ensure the new entry is valid and not already equipped in another slot,
        //          and update the active entry reference if necessary.
        public bool TryReplaceEquippedConsumableItemEntry(int slotIndex, InventoryItemEntry newConsumableEntry, out InventoryItemEntry removedConsumableEntry)
        {
            return this.ConsumableItemHandler.TryReplaceEquippedEntry(slotIndex, newConsumableEntry, out removedConsumableEntry);
        }

        //      4.3. Try to unequip an equipped consumable item entry, with checks to ensure the entry is currently equipped,
        //          and update the active entry reference if necessary.
        public bool TryUnequipEquippedConsumableItemEntry(InventoryItemEntry consumableEntry)
        {
            return this.ConsumableItemHandler.TryUnequipEntry(consumableEntry);
        }

        //      4.4. Try to consume the active equipped consumable item, applying its effects and reducing quantity,
        public bool TryConsumeConsumableItem()
        {
            return this.ConsumableItemHandler.TryConsume();
        }

        //  5. UI APIs
        //      5.1. Get Equipped Weapon (Active) Icon
        public Sprite GetActiveEquippedWeaponIcon()
        {
            return this.WeaponHandler.GetActiveEntryIcon();
        }

        //      5.2. Get Current Special Skill (Active) Icon
        public Sprite GetActiveSpecialSkillIcon()
        {
            return this.SpecialSkillHandler.GetActiveSpecialSkillIcon();
        }

        //      5.3. Get Equipped Consumable Item (Active) Icon
        public Sprite GetActiveConsumableItemIcon()
        {
            return this.ConsumableItemHandler.GetActiveEntryIcon();
        }

        //  6. Event Notifiers
        public void NotifyActiveEquippedWeaponChanged(InventoryItemEntry activeWeaponEntry)
        {
            this.OnActiveEquippedWeaponChanged?.Invoke(activeWeaponEntry);
        }
        public void NotifyActiveSpecialSkillChanged(SkillProfile activeSpecialSkill)
        {
            this.OnActiveSpecialSkillChanged?.Invoke(activeSpecialSkill);
        }
        public void NotifyActiveEquippedConsumableItemChanged(InventoryItemEntry activeConsumableItemEntry)
        {
            this.OnActiveEquippedConsumableItemChanged?.Invoke(activeConsumableItemEntry);
        }

        //  7. IStatusEffectFlagConsumer implementation
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

        //  8. Refresh for Character Placement: Reset all loadout states and ensure validity
        public void RefreshLoadoutAfterCharacterPlacement()
        {
            this.WeaponHandler.RefreshLoadoutAfterCharacterPlacement();
            this.SpecialSkillHandler.RefreshLoadoutAfterCharacterPlacement();
            this.ConsumableItemHandler.RefreshLoadoutAfterCharacterPlacement();
        }
    }
}
