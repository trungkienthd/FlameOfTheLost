using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class TutorialProgressEquipmentHandler : TutorialProgressSubBehaviourBase
    {
        // --- Public APIs ---
        public void RefreshConditions()
        {
            if (!this.EnsureBound()) return;

            this.RefreshPlayerHasMultipleWeaponsCondition();
            this.RefreshPlayerReadyToCycleWeaponsCondition();
        }

        // --- Helpers ---
        //  1. TutorialProgressSubBehaviourBase Overrides
        //      1.1. Validation
        protected override bool ValidateGameplayReferences()
        {
            if (!this.ValidatePlayerReference()) return false;

            bool isValid = true;

            if (this.Player.InventoryController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressEquipmentHandler)}] {nameof(this.Player)} has no {nameof(InventoryController)}.", this);
                isValid = false;
            }
            if (this.Player.CombatLoadoutController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(TutorialProgressEquipmentHandler)}] {nameof(this.Player)} has no {nameof(CharacterCombatLoadoutController)}.", this);
                isValid = false;
            }

            return isValid;
        }

        //      1.2. Others
        protected override void BindGameplayReferencesInternal()
        {
            base.BindGameplayReferencesInternal();

            this.Player.InventoryController.OnItemAdded += this.HandlePlayerItemAdded;
            this.Player.InventoryController.OnItemRemoved += this.HandlePlayerItemRemoved;

            this.Player.CombatLoadoutController.OnEquippedWeaponEntriesChanged += this.HandleEquippedWeaponEntriesChanged;
            this.Player.CombatLoadoutController.OnActiveEquippedWeaponChanged += this.HandleActiveEquippedWeaponChanged;
        }
        protected override void UnbindGameplayReferencesInternal()
        {
            if (this.Player?.InventoryController != null)
            {
                this.Player.InventoryController.OnItemAdded -= this.HandlePlayerItemAdded;
                this.Player.InventoryController.OnItemRemoved -= this.HandlePlayerItemRemoved;
            }

            if (this.Player?.CombatLoadoutController != null)
            {
                this.Player.CombatLoadoutController.OnEquippedWeaponEntriesChanged -= this.HandleEquippedWeaponEntriesChanged;
                this.Player.CombatLoadoutController.OnActiveEquippedWeaponChanged -= this.HandleActiveEquippedWeaponChanged;
            }

            base.UnbindGameplayReferencesInternal();
        }
        protected override void OnGameplayReferencesBound()
        {
            base.OnGameplayReferencesBound();

            this.RefreshConditions();
        }

        //  2. Player Equipment Progress
        private void RefreshPlayerHasMultipleWeaponsCondition()
        {
            if (!this.EnsureBound()) return;
            if (this.Player == null) return;
            if (this.Player.InventoryController == null) return;

            IReadOnlyList<InventoryItemEntry> weaponEntries = this.Player.InventoryController.AllWeaponEntries;

            bool playerHasMultipleWeapons = 
                weaponEntries != null && 
                weaponEntries.Count >= 2;

            this.TutorialProgressController.SetPlayerHasMultipleWeapons(playerHasMultipleWeapons);
        }
        private void RefreshPlayerReadyToCycleWeaponsCondition()
        {
            if (!this.EnsureBound()) return;
            if (this.Player == null) return;
            if (this.Player.CombatLoadoutController == null) return;

            CharacterCombatLoadoutController combatLoadoutController = this.Player.CombatLoadoutController;

            IReadOnlyList<InventoryItemEntry> equippedWeaponEntries = combatLoadoutController.EquippedWeaponEntries;
            InventoryItemEntry activeWeaponEntry = combatLoadoutController.ActiveEquippedWeaponEntry;

            int equippedWeaponCount = 0;
            bool activeWeaponIsEquipped = false;

            if (equippedWeaponEntries != null)
            {
                for (int i = 0; i < equippedWeaponEntries.Count; i++)
                {
                    InventoryItemEntry equippedWeaponEntry = equippedWeaponEntries[i];

                    if (equippedWeaponEntry == null) continue;

                    equippedWeaponCount++;

                    if (UnityEngine.Object.ReferenceEquals(equippedWeaponEntry, activeWeaponEntry))
                    {
                        activeWeaponIsEquipped = true;
                    }
                }
            }

            bool playerReadyToCycleWeapons =
                equippedWeaponCount >= 2    &&
                activeWeaponEntry != null   &&
                activeWeaponIsEquipped;

            this.TutorialProgressController.SetPlayerReadyToCycleWeapons(playerReadyToCycleWeapons);
        }

        //  3. Event Handling
        private void HandlePlayerItemAdded(ItemProfile item, int quantity)
        {
            if (item is not WeaponProfile) return;

            this.RefreshPlayerHasMultipleWeaponsCondition();
        }
        private void HandlePlayerItemRemoved(ItemProfile item, int quantity)
        {
            if (item is not WeaponProfile) return;

            this.RefreshPlayerHasMultipleWeaponsCondition();
        }
        private void HandleEquippedWeaponEntriesChanged()
        {
            this.RefreshPlayerReadyToCycleWeaponsCondition();
        }
        private void HandleActiveEquippedWeaponChanged(InventoryItemEntry activeWeaponEntry)
        {
            this.RefreshPlayerReadyToCycleWeaponsCondition();
        }
    }
}
