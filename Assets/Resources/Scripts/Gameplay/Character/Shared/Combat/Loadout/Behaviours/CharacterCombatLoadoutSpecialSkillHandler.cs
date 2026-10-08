using UnityEngine;
using System.Collections.Generic;

namespace FlameOfTheLost
{
    public class CharacterCombatLoadoutSpecialSkillHandler : CharacterCombatLoadoutSubBehaviourBase
    {
        // --- References ---
        private CharacterCombatLoadoutWeaponHandler CombatLoadoutWeaponHandler => this.CombatLoadoutController != null
            ? this.CombatLoadoutController.WeaponHandler
            : null;

        // --- Public APIs ---
        //  1. Gameplay Skill Selection
        public bool TrySetEquippedWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry, SkillProfile specialSkill)
        {
            if (this.CombatLoadoutWeaponHandler == null) return false;
            if (!this.CombatLoadoutWeaponHandler.TryGetItemFromEntry(weaponEntry, out WeaponProfile weapon)) return false;
            if (specialSkill == null) return false;

            if (this.CombatLoadoutController.IsInAttackWindow) return false;
            if (this.CombatLoadoutController.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutSpecialSkillHandler)}] {nameof(this.TrySetEquippedWeaponActiveSpecialSkill)} blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            if (!this.CanEquipSkillByProgression(specialSkill))
            {
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutSpecialSkillHandler)}] {nameof(this.TrySetEquippedWeaponActiveSpecialSkill)} blocked: Skill progression requirements not met.");
                return false;
            }

            if (!this.CombatLoadoutWeaponHandler.EquippedEntryListIsValid()) return false;
            if (!this.CombatLoadoutWeaponHandler.IsEquippedEntry(weaponEntry)) return false;

            if (this.InventoryController == null) return false;

            List<SkillProfile> candidates = this.InventoryController.GetPossibleActiveSpecialSkillCandidates(weaponEntry);
            if (candidates == null || !candidates.Contains(specialSkill)) return false;

            RuntimeWeapon runtimeWeapon = this.CombatLoadoutWeaponHandler.GetOrCreateRuntimeInstance(weaponEntry);
            if (runtimeWeapon == null) return false;

            runtimeWeapon.ActiveSpecialSkill = specialSkill;

            // Refresh UI only if this weapon is currently active
            if (UnityEngine.Object.ReferenceEquals(this.CombatLoadoutWeaponHandler.ActiveEquippedEntry, weaponEntry))
            {
                this.CombatLoadoutController.NotifyActiveSpecialSkillChanged(weapon.ResolveSpecialSkill(runtimeWeapon.ActiveSpecialSkill));
            }

            return true;
        }

        public bool TrySetActiveEquippedWeaponActiveSpecialSkill(SkillProfile specialSkill)
        {
            if (this.CombatLoadoutWeaponHandler == null) return false;
            if (this.CombatLoadoutWeaponHandler.ActiveEquippedEntry == null) return false;

            return this.TrySetEquippedWeaponActiveSpecialSkill(this.CombatLoadoutWeaponHandler.ActiveEquippedEntry, specialSkill);
        }

        public bool TryClearWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            if (this.CombatLoadoutWeaponHandler == null) return false;
            if (!this.CombatLoadoutWeaponHandler.TryGetItemFromEntry(weaponEntry, out WeaponProfile weapon)) return false;

            if (this.CombatLoadoutController.IsInAttackWindow) return false;
            if (this.CombatLoadoutController.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutSpecialSkillHandler)}] {nameof(this.TryClearWeaponActiveSpecialSkill)} blocked: CannotSwapLoadout flag is active.", this);
                return false;
            }

            if (!this.CombatLoadoutWeaponHandler.EquippedEntryListIsValid()) return false;
            if (!this.CombatLoadoutWeaponHandler.IsEquippedEntry(weaponEntry)) return false;

            RuntimeWeapon runtimeWeapon = this.CombatLoadoutWeaponHandler.GetOrCreateRuntimeInstance(weaponEntry);

            if (runtimeWeapon == null) return false;

            runtimeWeapon.ActiveSpecialSkill = null;

            if (UnityEngine.Object.ReferenceEquals(this.CombatLoadoutWeaponHandler.ActiveEquippedEntry, weaponEntry))
            {
                this.CombatLoadoutController.NotifyActiveSpecialSkillChanged(weapon.ResolveSpecialSkill(null));
            }

            return true;
        }

        public SkillProfile GetWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            if (this.CombatLoadoutWeaponHandler == null) return null;
            if (!this.CombatLoadoutWeaponHandler.IsEntryValid(weaponEntry)) return null;

            RuntimeWeapon runtimeWeapon = this.CombatLoadoutWeaponHandler.GetOrCreateRuntimeInstance(weaponEntry);
            return runtimeWeapon != null ? runtimeWeapon.ActiveSpecialSkill : null;
        }

        //  2. Save Restoration
        internal bool RestoreWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry, SkillProfile specialSkill)
        {
            if (this.CombatLoadoutWeaponHandler == null) return false;
            if (!this.CombatLoadoutWeaponHandler.IsEquippedEntry(weaponEntry)) return false;
            if (!this.CombatLoadoutWeaponHandler.IsEntryValid(weaponEntry)) return false;

            RuntimeWeapon runtimeWeapon = this.CombatLoadoutWeaponHandler.GetOrCreateRuntimeInstance(weaponEntry);
            if (runtimeWeapon == null) return false;

            // Explicit null is a valid persisted state.
            if (specialSkill == null)
            {
                runtimeWeapon.ActiveSpecialSkill = null;
                return true;
            }

            if (!this.CanEquipSkillByProgression(specialSkill)) return false;

            if (this.InventoryController == null) return false;

            List<SkillProfile> candidates = this.InventoryController.GetPossibleActiveSpecialSkillCandidates(weaponEntry);
            if (candidates == null || !candidates.Contains(specialSkill)) return false;

            runtimeWeapon.ActiveSpecialSkill = specialSkill;

            return true;
        }

        //  3. UI
        public Sprite GetActiveSpecialSkillIcon()
        {
            if (this.CombatLoadoutWeaponHandler == null) return null;

            SkillProfile resolvedSkill = this.CombatLoadoutWeaponHandler.ResolveActiveSpecialSkill();

            return resolvedSkill != null ? resolvedSkill.Icon : null;
        }

        //  4. Character Refresh
        public void RefreshLoadoutAfterCharacterRefreshment()
        {
            // this.CombatLoadoutController.NotifyActiveSpecialSkillChanged(this.CombatLoadoutWeaponHandler.ActiveEquippedRuntimeWeapon?.ActiveSpecialSkill);
        }

        // --- Helpers ---
        private bool CanEquipSkillByProgression(SkillProfile skill)
        {
            if (skill == null) return false;

            if (this.ProgressionController == null) return true;

            return skill.CanBeUsedBy(this.ProgressionController);
        }
    }
}
