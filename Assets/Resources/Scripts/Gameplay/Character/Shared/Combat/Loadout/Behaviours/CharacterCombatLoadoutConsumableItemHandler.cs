using UnityEngine;
using System.Collections.Generic;

namespace FlameOfTheLost
{
    public class CharacterCombatLoadoutConsumableItemHandler : CharacterCombatLoadoutEntryHandlerBase
    {
        // --- States ---
        //  1. Equipped Consumable Items
        protected override int MaxEquippedEntryCapacity => 8;
        protected override int DefaultEquippedEntryCapacity => 2;
        protected override string EntryDisplayName => "Consumable Item";

        //  2. References
        private CharacterCombatResourcesHandler CombatResourcesHandler => this.CharacterRootController.CombatResourcesHandler;
        private CharacterCombatStatusEffectApplicationHandler CombatStatusEffectHandler => this.CharacterRootController.CombatStatusEffectApplicationHandler;

        // --- Public APIs ---
        public bool TryGetItemFromEntry(InventoryItemEntry consumableEntry, out ConsumableItemProfile consumableItem)
        {
            consumableItem = null;

            if (this.InventoryController == null) return false;
            return this.InventoryController.TryGetConsumableItemFromEntry(consumableEntry, out consumableItem);
        }

        public override bool IsEntryValid(InventoryItemEntry consumableEntry)
        {
            if (!this.TryGetItemFromEntry(consumableEntry, out ConsumableItemProfile _)) return false;
            if (consumableEntry.Quantity <= 0) return false;
            if (this.InventoryController == null) return false;
            if (!this.InventoryController.ContainsEntry(consumableEntry)) return false;

            return true;
        }

        public bool TryConsume()
        {
            if (this.CombatLoadoutController == null) return false;

            if (this.CombatLoadoutController.IsAttackLocked)
            {
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutConsumableItemHandler)}] TryConsume blocked: Cannot consume during locked attack.");
                return false;
            }
            if (this.CombatLoadoutController.IsInAttackWindow)
            {
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutConsumableItemHandler)}] TryConsume blocked: Consuming is locked.");
                return false;
            }

            InventoryItemEntry activeConsumableEntry = this.ActiveEquippedEntry;
            if (!this.TryGetItemFromEntry(activeConsumableEntry, out ConsumableItemProfile consumableItem)) return false;
            if (!this.IsEntryValid(activeConsumableEntry)) return false;

            if (this.InventoryController == null)
            {
                Debug.LogError($"[{this.CharacterName}.CharacterCombatLoadoutController] TryConsume failed: InventoryController is missing.");
                return false;
            }

            // a. Pay resource cost first. TryPayImmediateCost only deducts if valid.
            if (this.CombatResourcesHandler == null)
            {
                Debug.LogError($"[{this.CharacterName}.CharacterCombatLoadoutController] TryConsume failed: CombatResourcesHandler is missing.");
                return false;
            }

            if (!this.CombatResourcesHandler.TryPayImmediateCost(consumableItem.UseResourceCost))
            {
                Debug.LogWarning($"[{this.CharacterName}.CharacterCombatLoadoutController] TryConsume failed: Not enough resources to pay the cost.");
                return false;
            }


            // b. Consume exactly 1 quantity only after cost payment succeeds
            if (!this.InventoryController.TryRemoveFromEntry(activeConsumableEntry, 1))
            {
                Debug.LogWarning($"[{this.CharacterName}.CharacterCombatLoadoutController] TryConsume failed: inventory entry could not be reduced.");
                return false;
            }

            // c. Resolve status effects after successful consume
            if (this.CombatStatusEffectHandler == null)
            {
                Debug.LogError($"[{this.CharacterName}.CharacterCombatLoadoutController] TryConsume failed: CombatStatusEffectHandler is missing.");
                return false;
            }

            this.CombatStatusEffectHandler.TryApplyStatusEffectApplicationsToSelf(consumableItem.StatusEffectApplications);


            // d. Refresh equipped slot / active entry state after successful consumption
            this.RefreshConsumedEntryState(activeConsumableEntry);

            return true;
        }

        // --- Helpers ---
        protected override IReadOnlyList<InventoryItemEntry> GetQuickEquipCandidateEntries()
        {
            return this.InventoryController != null ? this.InventoryController.AvailableConsumableItemEntries : null;
        }

        protected override void NotifyActiveEntryChanged(InventoryItemEntry activeEntry)
        {
            if (this.CombatLoadoutController == null) return;

            this.CombatLoadoutController.NotifyActiveEquippedConsumableItemChanged(activeEntry);
        }

        private void RefreshConsumedEntryState(InventoryItemEntry consumedEntry)
        {
            if (consumedEntry == null) return;

            if (this.InventoryController == null)
            {
                this.EnsureActiveEntryIsValid();
                return;
            }

            if (!this.InventoryController.ContainsEntry(consumedEntry) || consumedEntry.IsEmpty)
            {
                int equippedIndex = -1;

                for (int i = 0; i < this.EquippedEntries.Count; i++)
                {
                    if (UnityEngine.Object.ReferenceEquals(this.EquippedEntries[i], consumedEntry))
                    {
                        equippedIndex = i;
                        break;
                    }
                }

                if (equippedIndex >= 0)
                {
                    this.SetEquippedEntry(equippedIndex, null);
                }

                if (UnityEngine.Object.ReferenceEquals(this.ActiveEquippedEntry, consumedEntry))
                {
                    this.EnsureActiveEntryIsValid();
                }

                return;
            }

            this.NotifyActiveEntryChanged(this.ActiveEquippedEntry);
        }
    }
}
