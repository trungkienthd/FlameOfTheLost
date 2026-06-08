using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public partial class CharacterCombatLoadoutController
    {
        //  5. Equipped Consumable Items
        [Header("Equipped Consumable Items")]
        [SerializeField] private List<InventoryItemEntry> _equippedConsumableItemEntries = new();
        [SerializeField] private InventoryItemEntry _activeEquippedConsumableItemEntry;
        public IReadOnlyList<InventoryItemEntry> EquippedConsumableItemEntries => _equippedConsumableItemEntries;
        public InventoryItemEntry ActiveEquippedConsumableItemEntry
        {
            get => _activeEquippedConsumableItemEntry;
            private set => _activeEquippedConsumableItemEntry = value;
        }

        //  7. Events
        public event Action<InventoryItemEntry> OnActiveEquippedConsumableItemChanged;

        // --- Public APIs ---
        //  4. Consumable Item Management
        //      4.1. Cycle through equipped consumable items in a specified direction (step: +1 for next, -1 for previous),
        //          with optional wrapping around the list, and set the next valid consumable item as active.
        public bool TryCycleConsumableItemEntry(int step)
        {
            if (this.IsInAttackWindow)
            {
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] TryCycleConsumableItemEntry blocked: switching is locked.");
                return false;
            }
            if (this.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] TryCycleConsumableItemEntry blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            step = (int)Mathf.Sign(step);
            if (step == 0) return false;

            if (!this.HasAnyValidEquippedConsumableItem()) return false;

            // If nothing equipped yet: pick first or last depending on direction
            if (this.ActiveEquippedConsumableItemEntry == null)
            {
                InventoryItemEntry fallback = step > 0 ? this.GetNextValidConsumableItemEntry(-1, +1) : this.GetNextValidConsumableItemEntry(this.EquippedConsumableItemEntries.Count, -1);
                if (fallback == null) return false;

                this.SetActiveConsumableItemEntry(fallback);
                return true;
            }

            int currentIndex = _equippedConsumableItemEntries.IndexOf(this.ActiveEquippedConsumableItemEntry);
            if (currentIndex < 0)
            {
                InventoryItemEntry fallback = this.GetNextValidConsumableItemEntry(-1, +1);
                if (fallback == null) return false;

                this.SetActiveConsumableItemEntry(fallback);
                return true;
            }

            InventoryItemEntry nextEntry = this.GetNextValidConsumableItemEntry(currentIndex, step);
            if (nextEntry == null) return false;
            if (UnityEngine.Object.ReferenceEquals(nextEntry, this.ActiveEquippedConsumableItemEntry)) return false;

            this.SetActiveConsumableItemEntry(nextEntry);
            return true;
        }

        //      4.2. Try to replace an equipped consumable item entry at a specified slot index with a new entry,
        //          with validation and checks to ensure the new entry is valid and not already equipped in another slot,
        //          and update the active entry reference if necessary.
        public bool TryReplaceEquippedConsumableItemEntry(int slotIndex, InventoryItemEntry newConsumableEntry, out InventoryItemEntry removedConsumableEntry)
        {
            removedConsumableEntry = null;

            if (this.IsInAttackWindow)
            {
                Debug.Log($"[{this.CharacterName}.CharacterCombatLoadoutController] TryReplaceEquippedConsumableItemEntry blocked: Consumable Replacement is locked.");
                return false;
            }
            if (this.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.CharacterName}.CharacterCombatLoadoutController] TryReplaceEquippedConsumableItemEntry blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            if (!this.IsConsumableItemEntryValid(newConsumableEntry)) return false;
            if (slotIndex < 0 || slotIndex >= this.EquippedConsumableItemEntries.Count) return false;

            // Check if the new entry is already equipped in another slot
            int existingIndex = _equippedConsumableItemEntries.IndexOf(newConsumableEntry);
            if (existingIndex >= 0)
            {
                // If it's already equipped in the same slot, do nothing
                if (existingIndex == slotIndex)
                {
                    removedConsumableEntry = null;
                    return true;
                }

                return false;           // Otherwise, we cannot equip the same entry in multiple slots
            }

            // Replace the entry in the specified slot
            removedConsumableEntry = this.EquippedConsumableItemEntries[slotIndex];
            this.SetEquippedConsumableItemEntry(slotIndex, newConsumableEntry);

            // The new replaced entry might be the active one, if so we need to update the active entry reference to point to the new entry
            if (this.ActiveEquippedConsumableItemEntry == null || UnityEngine.Object.ReferenceEquals(this.ActiveEquippedConsumableItemEntry, removedConsumableEntry))
            {
                this.SetActiveConsumableItemEntry(newConsumableEntry);
            }
            else
            {
                this.EnsureActiveConsumableItemIsValid();
            }

            return true;
        }

        //      4.3. Try to unequip an equipped consumable item entry, with checks to ensure the entry is currently equipped,
        //          and update the active entry reference if necessary.
        public bool TryUnequipEquippedConsumableItemEntry(InventoryItemEntry consumableEntry)
        {
            if (this.IsInAttackWindow)
            {
                Debug.Log($"[{this.CharacterName}.CharacterCombatLoadoutController] TryReplaceEquippedConsumableItemEntry blocked: Consumable Unequipping is locked.");
                return false;
            }
            if (this.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.CharacterName}.CharacterCombatLoadoutController] TryUnequipConsumableItem blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            if (consumableEntry == null) return false;

            int slotIndex = _equippedConsumableItemEntries.IndexOf(consumableEntry);
            if (slotIndex < 0) return false;
            this.SetEquippedConsumableItemEntry(slotIndex, null);

            if (UnityEngine.Object.ReferenceEquals(this.ActiveEquippedConsumableItemEntry, consumableEntry))
            {
                this.EnsureActiveConsumableItemIsValid();
            }

            return true;
        }

        //      4.4. Try to consume the active equipped consumable item, applying its effects and reducing quantity,
        public bool TryConsumeConsumableItem()
        {
            if (this.CombatController.IsAttackLocked)
            {
                Debug.Log($"[{this.CharacterName}.CharacterCombatLoadoutController] TryConsumeConsumableItem blocked: Cannot consume during locked attack.");
                return false;
            }
            if (this.IsInAttackWindow)
            {
                Debug.Log($"[{this.CharacterName}.CharacterCombatLoadoutController] TryConsumeConsumableItem blocked: Consuming is locked.");
                return false;
            }

            InventoryItemEntry activeConsumableEntry = this.ActiveEquippedConsumableItemEntry;
            if (!this.TryGetConsumableItemFromEntry(activeConsumableEntry, out ConsumableItemProfile consumableItem)) return false;
            if (!this.IsConsumableItemEntryValid(activeConsumableEntry)) return false;

            if (this.InventoryController == null)
            {
                Debug.LogError($"[{this.CharacterName}.CharacterCombatLoadoutController] TryConsumeConsumableItem failed: InventoryController is missing.");
                return false;
            }

            // a. Pay resource cost first. TryPayImmediateCost only deducts if valid.
            if (this.CombatResourcesHandler == null)
            {
                Debug.LogError($"[{this.CharacterName}.CharacterCombatLoadoutController] TryConsumeConsumableItem failed: CombatResourcesHandler is missing.");
                return false;
            }
            else
            {
                if (!this.CombatResourcesHandler.TryPayImmediateCost(consumableItem.UseResourceCost))
                {
                    Debug.LogWarning($"[{this.CharacterName}.CharacterCombatLoadoutController] TryConsumeConsumableItem failed: Not enough resources to pay the cost.");
                    return false;
                }
            }

            // b. Consume exactly 1 quantity only after cost payment succeeds
            if (!this.InventoryController.TryRemoveFromEntry(activeConsumableEntry, 1))
            {
                Debug.LogWarning($"[{this.CharacterName}.CharacterCombatLoadoutController] TryConsumeConsumableItem failed: inventory entry could not be reduced.");
                return false;
            }

            // c. Resolve status effects after successful consume
            if (this.CombatStatusEffectHandler == null)
            {
                Debug.LogError($"[{this.CharacterName}.CharacterCombatLoadoutController] UseConsumableItem failed: CombatStatusEffectHandler is missing.");
                return false;
            }
            else
            {
                this.CombatStatusEffectHandler.TryApplyStatusEffectApplicationsToSelf(consumableItem.StatusEffectApplications);
            }

            // d. Refresh equipped slot / active entry state after successful consumption
            if (!this.InventoryController.ContainsEntry(activeConsumableEntry) || activeConsumableEntry.IsEmpty)
            {
                int equippedIndex = _equippedConsumableItemEntries.IndexOf(activeConsumableEntry);
                if (equippedIndex >= 0)
                {
                    this.SetEquippedConsumableItemEntry(equippedIndex, null);
                }

                if (UnityEngine.Object.ReferenceEquals(this.ActiveEquippedConsumableItemEntry, activeConsumableEntry))
                {
                    this.EnsureActiveConsumableItemIsValid();
                }
            }
            else
            {
                this.OnActiveEquippedConsumableItemChanged?.Invoke(this.ActiveEquippedConsumableItemEntry);
            }

            return true;
        }

        //  5. UI APIs
        //      5.3. Get Equipped Consumable Item (Active) Icon
        public Sprite GetActiveConsumableItemIcon()
        {
            if (this.ActiveEquippedConsumableItemEntry != null) return this.ActiveEquippedConsumableItemEntry.Icon;

            return null;
        }

        // --- Helpers --- 
        //  2. Consumable Item Management
        //      2.0. OnStart: Clamp the equipped consumable item entry list to the max capacity,
        //          ensuring it has exactly MaxEquippedConsumables slots (filled with null if necessary),
        //          and ensure the active consumable item entry is valid
        private void ClampEquippedConsumableItemsToCapacity()
        {
            if (_equippedConsumableItemEntries == null) _equippedConsumableItemEntries = new List<InventoryItemEntry>();

            int max = this.MaxEquippedConsumables;
            int currentCount = this.EquippedConsumableItemEntries.Count;

            if (currentCount > max)
            {
                _equippedConsumableItemEntries.RemoveRange(max, currentCount - max);
                return;
            }

            while (this.EquippedConsumableItemEntries.Count < max)
            {
                _equippedConsumableItemEntries.Add(null);
            }
        }

        //      2.1. Set a equipped weapon entry slot
        private void SetEquippedConsumableItemEntry(int slotIndex, InventoryItemEntry newConsumableEntry)
        {
            if (slotIndex < 0 || slotIndex >= this.EquippedConsumableItemEntries.Count) return;

            InventoryItemEntry oldEntry = this.EquippedConsumableItemEntries[slotIndex];
            if (UnityEngine.Object.ReferenceEquals(oldEntry, newConsumableEntry)) return;

            // a. Unequip old
            if (oldEntry != null)
            {
                this.SetEntryEquippedState(oldEntry, false);
            }

            // b. Equip new
            _equippedConsumableItemEntries[slotIndex] = newConsumableEntry;

            // c. Set equipped state on new entry
            if (newConsumableEntry != null)
            {
                this.SetEntryEquippedState(newConsumableEntry, true);
            }
        }

        //      2.2. Set the active consumable item entry slot
        private void SetActiveConsumableItemEntry(InventoryItemEntry newEntry)
        {
            if (UnityEngine.Object.ReferenceEquals(this.ActiveEquippedConsumableItemEntry, newEntry)) return;

            if (newEntry == null)
            {
                this.ActiveEquippedConsumableItemEntry = null;
                this.OnActiveEquippedConsumableItemChanged?.Invoke(null);
                return;
            }

            if (!this.IsConsumableItemEntryValid(newEntry))
            {
                Debug.LogError($"[{this.CharacterName}.CharacterCombatLoadoutController] SetActiveConsumableItemEntry failed: newEntry is not a valid consumable entry.");
                return;
            }


            int equippedIndex = _equippedConsumableItemEntries.IndexOf(newEntry);
            if (equippedIndex < 0)
            {
                int emptySlotIndex = -1;
                for (int i = 0; i < this.EquippedConsumableItemEntries.Count; i++)
                {
                    if (this.EquippedConsumableItemEntries[i] == null)
                    {
                        emptySlotIndex = i;
                        break;
                    }
                }

                if (emptySlotIndex < 0)
                {
                    Debug.LogWarning($"[{this.CharacterName}.CharacterCombatLoadoutController] SetActiveConsumableItemEntry failed: entry is not equipped and there is no empty slot.");
                    return;
                }

                this.SetEquippedConsumableItemEntry(emptySlotIndex, newEntry);
            }

            this.ActiveEquippedConsumableItemEntry = newEntry;
            this.OnActiveEquippedConsumableItemChanged?.Invoke(this.ActiveEquippedConsumableItemEntry);
        }

        //      2.3. Check if a consumable item entry is valid:
        //          - not null, and
        //          - has a valid consumable item profile, and
        //          - quantity > 0, and
        //          - exists in the inventory
        private bool IsConsumableItemEntryValid(InventoryItemEntry consumableEntry)
        {
            if (!this.TryGetConsumableItemFromEntry(consumableEntry, out ConsumableItemProfile _)) return false;
            if (consumableEntry.Quantity <= 0) return false;
            if (this.InventoryController == null) return false;
            if (!this.InventoryController.ContainsEntry(consumableEntry)) return false;

            return true;
        }
        //          2.3.1. Check if there is at least one valid (specified above) consumable item entry in the equipped list  
        private bool HasAnyValidEquippedConsumableItem()
        {
            if (_equippedConsumableItemEntries == null || this.EquippedConsumableItemEntries.Count == 0) return false;

            for (int i = 0; i < this.EquippedConsumableItemEntries.Count; i++)
            {
                if (this.IsConsumableItemEntryValid(this.EquippedConsumableItemEntries[i])) return true;
            }

            Debug.LogWarning($"[{this.CharacterName}.CharacterCombatLoadoutController] No valid equipped consumable item found.");
            return false;
        }
        //          2.3.2. Try to get the consumable item profile from a consumable item entry, with validation
        private bool TryGetConsumableItemFromEntry(InventoryItemEntry consumableEntry, out ConsumableItemProfile consumableItem)
        {
            consumableItem = null;

            if (this.InventoryController == null) return false;
            return this.InventoryController.TryGetConsumableItemFromEntry(consumableEntry, out consumableItem);
        }

        //      2.4. Check if the active consumable item entry is valid (in the equipped list), if not try to find the next valid one, if none found set to null
        private void EnsureActiveConsumableItemIsValid()
        {
            if (_equippedConsumableItemEntries == null || this.EquippedConsumableItemEntries.Count == 0)
            {
                this.SetActiveConsumableItemEntry(null);
                return;
            }

            if (this.IsConsumableItemEntryValid(this.ActiveEquippedConsumableItemEntry) && _equippedConsumableItemEntries.Contains(this.ActiveEquippedConsumableItemEntry)) return;

            for (int i = 0; i < this.EquippedConsumableItemEntries.Count; i++)
            {
                if (this.IsConsumableItemEntryValid(this.EquippedConsumableItemEntries[i]))
                {
                    this.SetActiveConsumableItemEntry(this.EquippedConsumableItemEntries[i]);
                    return;
                }
            }

            this.SetActiveConsumableItemEntry(null);
        }

        //      2.5. Get the next entry for Switching between equipped consumables
        private InventoryItemEntry GetNextValidConsumableItemEntry(int currentIndex, int step)
        {
            if (_equippedConsumableItemEntries == null || this.EquippedConsumableItemEntries.Count == 0) return null;

            int count = this.EquippedConsumableItemEntries.Count;
            step = (int)Mathf.Sign(step);
            if (step == 0) return null;

            int index = currentIndex;
            for (int i = 0; i < count; i++)
            {
                index = (index + step + count) % count;

                InventoryItemEntry candidate = this.EquippedConsumableItemEntries[index];
                if (this.IsConsumableItemEntryValid(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }
    }
}