using UnityEngine;
using System.Collections.Generic;

namespace FlameOfTheLost
{
    public class CharacterCombatLoadoutWeaponHandler : CharacterCombatLoadoutEntryHandlerBase
    {
        // --- Configs ---
        //  1. Equipped Weapons
        public override int MaxEquippedEntryCapacity => 3;
        protected override int DefaultEquippedEntryCapacity => 1;
        protected override string EntryDisplayName => "Weapon";

        public RuntimeWeapon ActiveEquippedRuntimeWeapon => this.GetOrCreateRuntimeInstance(this.ActiveEquippedEntry);

        // --- Runtime --- 
        //  1. Runtime Weapon Instances
        [Header("Runtime - Weapon Map Config")]
        [SerializeField] private RuntimeWeaponDictionary _runtimeWeaponMap = new();
        private RuntimeWeaponDictionary RuntimeWeaponMap => _runtimeWeaponMap;

        // --- Public APIs ---
        //  1. Runtime Weapon
        public RuntimeWeapon GetOrCreateRuntimeInstance(InventoryItemEntry weaponEntry)
        {
            if (!this.TryGetItemFromEntry(weaponEntry, out WeaponProfile weapon)) return null;

            if (this.RuntimeWeaponMap.TryGetValue(weaponEntry, out var runtimeWeapon) && runtimeWeapon != null) return runtimeWeapon;

            runtimeWeapon = new RuntimeWeapon(weapon, this.CharacterRootController);
            this.RuntimeWeaponMap[weaponEntry] = runtimeWeapon;

            return runtimeWeapon;
        }

        internal void PrepareForSaveDataRestore()
        {
            this.RuntimeWeaponMap.Clear();
        }

        //  2. Item Queries
        public bool TryGetItemFromEntry(InventoryItemEntry weaponEntry, out WeaponProfile weapon)
        {
            weapon = null;

            if (this.InventoryController == null) return false;

            return this.InventoryController.TryGetWeaponFromEntry(weaponEntry, out weapon);
        }
        public override bool IsEntryValid(InventoryItemEntry entry)
        {
            if (!this.TryGetItemFromEntry(entry, out WeaponProfile weapon)) return false;
            if (!this.CanEquipWeaponByProgression(weapon)) return false;
            if (entry.Quantity != 1) return false;
            if (this.InventoryController == null) return false;
            if (!this.InventoryController.ContainsEntry(entry)) return false;

            return true;
        }

        //  3. Special Skill
        public SkillProfile ResolveActiveSpecialSkill()
        {
            if (!this.TryGetItemFromEntry(this.ActiveEquippedEntry, out WeaponProfile weapon)) return null;

            RuntimeWeapon runtimeWeapon = this.GetOrCreateRuntimeInstance(this.ActiveEquippedEntry);
            return weapon.ResolveSpecialSkill(runtimeWeapon != null ? runtimeWeapon.ActiveSpecialSkill : null);
        }

        // --- Helpers ---
        protected override IReadOnlyList<InventoryItemEntry> GetQuickEquipCandidateEntries()
        {
            return this.InventoryController != null ? this.InventoryController.AvailableWeaponEntries : null;
        }

        protected override void NotifyActiveEntryChanged(InventoryItemEntry activeEntry)
        {
            if (this.CombatLoadoutController == null) return;

            this.CombatLoadoutController.NotifyActiveEquippedWeaponChanged(activeEntry);
            this.CombatLoadoutController.NotifyActiveSpecialSkillChanged(this.ResolveActiveSpecialSkill());
        }

        protected override void OnEquippedEntryChanged(int slotIndex, InventoryItemEntry oldEntry, InventoryItemEntry newEntry)
        {
            if (newEntry != null)
            {
                this.GetOrCreateRuntimeInstance(newEntry);
            }

            if (oldEntry != null && !this.IsEquippedEntry(oldEntry))
            {
                this.RuntimeWeaponMap.Remove(oldEntry);
            }

            this.CombatLoadoutController?.NotifyEquippedWeaponEntriesChanged();
        }

        protected override void OnEntryReplaced(int slotIndex, InventoryItemEntry oldEntry, InventoryItemEntry newEntry)
        {
            if (newEntry != null)
            {
                this.GetOrCreateRuntimeInstance(newEntry);
            }
        }

        protected override void OnEntryQuickEquipped(int slotIndex, InventoryItemEntry equippedEntry)
        {
            if (equippedEntry != null)
            {
                this.GetOrCreateRuntimeInstance(equippedEntry);
            }
        }

        private bool CanEquipWeaponByProgression(WeaponProfile weapon)
        {
            if (weapon == null) return false;

            if (this.ProgressionController == null) return true;

            return weapon.CanBeEquippedBy(this.ProgressionController);
        }
    }
}
