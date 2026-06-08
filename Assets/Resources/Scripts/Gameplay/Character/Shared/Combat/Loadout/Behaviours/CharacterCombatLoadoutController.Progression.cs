using UnityEngine;

namespace FlameOfTheLost
{
    public partial class CharacterCombatLoadoutController
    {
        // --- Public APIs ---
        public void SetMaxEquippedWeaponsFromProgression(int amount)
        {
            int clampedAmount = Mathf.Clamp(amount, 1, 4);

            if (this.MaxEquippedWeapons == clampedAmount) return;

            this.MaxEquippedWeapons = clampedAmount;

            this.ClampEquippedWeaponsToCapacity();
            this.SyncEquippedStates();
            this.EnsureActiveWeaponIsValid();

            this.OnActiveEquippedWeaponChanged?.Invoke(this.ActiveEquippedWeaponEntry);
            this.OnActiveSpecialSkillChanged?.Invoke(this.ActiveEquippedRuntimeWeapon.ActiveSpecialSkill);
        }

        public void SetMaxEquippedConsumablesFromProgression(int amount)
        {
            int clampedAmount = Mathf.Clamp(amount, 1, 8);

            if (this.MaxEquippedConsumables == clampedAmount) return;

            this.MaxEquippedConsumables = clampedAmount;

            this.ClampEquippedConsumableItemsToCapacity();
            this.SyncEquippedStates();
            this.EnsureActiveConsumableItemIsValid();

            this.OnActiveEquippedConsumableItemChanged?.Invoke(this.ActiveEquippedConsumableItemEntry);
        }

        // --- Helpers ---
        private bool CanEquipWeaponByProgression(WeaponProfile weapon)
        {
            if (weapon == null) return false;

            CharacterProgressionController progressionController = this.ProgressionController;
            if (progressionController == null) return true;

            return progressionController.CanSatisfyRequirements(weapon.Requirements);
        }
        private bool CanEquipSkillByProgression(SkillProfile skill)
        {
            if (skill == null) return false;

            CharacterProgressionController progressionController = this.ProgressionController;
            if (progressionController == null) return true;

            return progressionController.CanSatisfyRequirements(skill.Requirements);
        }
    }
}