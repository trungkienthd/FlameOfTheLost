using UnityEngine;
using System.Collections.Generic;

namespace FlameOfTheLost
{
    public class CharacterCombatLoadoutSpecialSkillHandler : CharacterBehaviourControllerBase
    {
        // --- States ---
        //  1. References
        private CharacterCombatLoadoutController CombatLoadoutController => this.CharacterRootController.CombatLoadoutController;
        private CharacterCombatLoadoutWeaponHandler CombatLoadoutWeaponHandler => this.CharacterRootController.CombatLoadoutWeaponHandler;
        private CharacterProgressionController ProgressionController => this.CharacterRootController.ProgressionController;
        private InventoryController InventoryController => this.CharacterRootController.InventoryController;

        // --- Public APIs ---
        public bool TrySetEquippedWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry, SkillProfile specialSkill)
        {
            if (!this.CombatLoadoutWeaponHandler.TryGetItemFromEntry(weaponEntry, out WeaponProfile weapon)) return false;
            if (specialSkill == null) return false;

            if (this.CombatLoadoutController.IsInAttackWindow) return false;
            if (this.CombatLoadoutController.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutSpecialSkillHandler)}] TrySetEquippedWeaponActiveSpecialSkill blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            if (!this.CanEquipSkillByProgression(specialSkill))
            {
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutSpecialSkillHandler)}] TrySetEquippedWeaponActiveSpecialSkill blocked: Skill progression requirements not met.");
                return false;
            }

            if (!this.CombatLoadoutWeaponHandler.EquippedEntryListIsValid()) return false;
            if (!this.CombatLoadoutWeaponHandler.IsEquippedEntry(weaponEntry)) return false;

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
            if (this.CombatLoadoutWeaponHandler.ActiveEquippedEntry == null) return false;
            return this.TrySetEquippedWeaponActiveSpecialSkill(this.CombatLoadoutWeaponHandler.ActiveEquippedEntry, specialSkill);
        }

        public bool TryClearWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            if (!this.CombatLoadoutWeaponHandler.TryGetItemFromEntry(weaponEntry, out WeaponProfile weapon)) return false;

            SkillProfile innateSkill = weapon.InnateSpecialSkill;
            if (innateSkill == null) return false;

            return this.TrySetEquippedWeaponActiveSpecialSkill(weaponEntry, innateSkill);
        }

        public SkillProfile GetWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            if (!this.CombatLoadoutWeaponHandler.IsEntryValid(weaponEntry)) return null;

            RuntimeWeapon runtimeWeapon = this.CombatLoadoutWeaponHandler.GetOrCreateRuntimeInstance(weaponEntry);
            return runtimeWeapon != null ? runtimeWeapon.ActiveSpecialSkill : null;
        }

        public Sprite GetActiveSpecialSkillIcon()
        {
            SkillProfile resolvedSkill = this.CombatLoadoutWeaponHandler.ResolveActiveSpecialSkill();

            return resolvedSkill != null ? resolvedSkill.Icon : null;
        }

        public void RefreshLoadoutAfterCharacterPlacement()
        {
            // this.CombatLoadoutController.NotifyActiveSpecialSkillChanged(this.CombatLoadoutWeaponHandler.ActiveEquippedRuntimeWeapon?.ActiveSpecialSkill);
        }

        // --- Helpers ---
        private bool CanEquipSkillByProgression(SkillProfile skill)
        {
            if (skill == null) return false;

            if (this.ProgressionController == null) return true;

            return this.ProgressionController.CanSatisfyRequirements(skill.Requirements);
        }
    }
}
