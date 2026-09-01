using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterCombatLoadoutCapacityHandler : CharacterCombatLoadoutSubBehaviourBase
    {
        // --- Public APIs ---
        public void SetMaxEquippedWeaponsFromProgression(int amount)
        {
            if (!this.EnsureBound()) return;

            this.CombatLoadoutController.WeaponHandler.SetMaxEquippedEntriesFromProgression(amount);

            this.CombatLoadoutController.NotifyEquippedWeaponEntriesChanged();
        }

        public void SetMaxEquippedProjectilesFromProgression(int amount)
        {
            if (!this.EnsureBound()) return;

            this.CombatLoadoutController.ProjectileHandler.SetMaxEquippedEntriesFromProgression(amount);
        }

        public void SetMaxEquippedConsumableItemsFromProgression(int amount)
        {
            if (!this.EnsureBound()) return;

            this.CombatLoadoutController.ConsumableItemHandler.SetMaxEquippedEntriesFromProgression(amount);
        }
    }
}