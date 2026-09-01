using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterCombatLoadoutEntryHandlerBase : CharacterCombatLoadoutSubBehaviourBase
    {
        // --- Runtime ---
        //  1. Capacity
        [Header("Runtime (Inherited) - Loadout Capacity")]
        [SerializeField, Min(1)] private int _maxEquippedEntries = 1;
        public int MaxEquippedEntries
        {
            get => _maxEquippedEntries;
            private set => _maxEquippedEntries = Mathf.Clamp(value, this.MinEquippedEntryCapacity, this.MaxEquippedEntryCapacity);
        }

        //  2. Equipped Entries
        [Header("Runtime (Inherited) - Equipped Entries")]
        [SerializeField] private List<InventoryItemEntry> _equippedEntries = new();
        [SerializeField] private InventoryItemEntry _activeEquippedEntry;
        public IReadOnlyList<InventoryItemEntry> EquippedEntries => _equippedEntries;

        public InventoryItemEntry ActiveEquippedEntry
        {
            get => _activeEquippedEntry;
            private set => _activeEquippedEntry = value;
        }

        // --- Configs ---
        protected virtual int MinEquippedEntryCapacity => 1;
        public abstract int MaxEquippedEntryCapacity { get; }
        protected abstract int DefaultEquippedEntryCapacity { get; }
        protected abstract string EntryDisplayName { get; }

        // --- Public APIs ---
        public void SetMaxEquippedEntriesFromProgression(int amount)
        {
            int clampedAmount = Mathf.Clamp(amount, this.MinEquippedEntryCapacity, this.MaxEquippedEntryCapacity);

            if (this.MaxEquippedEntries == clampedAmount) return;

            this.MaxEquippedEntries = clampedAmount;

            this.ClampEquippedEntriesToCapacity();
            this.RemoveInvalidEquippedEntries();
            this.SyncEquippedStates();
            this.EnsureActiveEntryIsValid();

            //this.NotifyActiveEntryChanged(this.ActiveEquippedEntry);
        }

        public bool EquippedEntryListIsValid()
        {
            return _equippedEntries != null && this.EquippedEntries.Count == this.MaxEquippedEntries;
        }

        public bool IsEquippedEntry(InventoryItemEntry entry)
        {
            return entry != null && _equippedEntries != null && this.EquippedEntries.Contains(entry);
        }

        public bool TryCycleEquippedEntry(int step)
        {
            if (this.IsLoadoutChangeBlocked(nameof(CharacterCombatLoadoutEntryHandlerBase.TryCycleEquippedEntry))) return false;

            step = (int)Mathf.Sign(step);
            if (step == 0) return false;

            if (!this.HasAnyValidEquippedEntry()) return false;

            int count = this.EquippedEntries.Count;

            if (this.ActiveEquippedEntry == null)
            {
                InventoryItemEntry fallback = step > 0
                    ? this.GetNextValidEntry(-1, 1)
                    : this.GetNextValidEntry(count, -1);

                if (fallback == null) return false;

                this.SetActiveEntry(fallback);
                return true;
            }

            int currentIndex = _equippedEntries.IndexOf(this.ActiveEquippedEntry);
            if (currentIndex < 0)
            {
                InventoryItemEntry fallback = this.GetNextValidEntry(-1, 1);
                if (fallback == null) return false;

                this.SetActiveEntry(fallback);
                return true;
            }

            InventoryItemEntry nextEntry = this.GetNextValidEntry(currentIndex, step);
            if (nextEntry == null) return false;
            if (UnityEngine.Object.ReferenceEquals(nextEntry, this.ActiveEquippedEntry)) return false;

            this.SetActiveEntry(nextEntry);
            return true;
        }

        public bool TryReplaceEquippedEntry(int slotIndex, InventoryItemEntry newEntry, out InventoryItemEntry removedEntry)
        {
            removedEntry = null;

            if (this.IsLoadoutChangeBlocked(nameof(CharacterCombatLoadoutEntryHandlerBase.TryReplaceEquippedEntry))) return false;

            if (!this.IsEntryValid(newEntry)) return false;
            if (slotIndex < 0 || slotIndex >= this.EquippedEntries.Count) return false;

            int existingIndex = _equippedEntries.IndexOf(newEntry);
            if (existingIndex >= 0)
            {
                if (existingIndex == slotIndex)
                {
                    removedEntry = null;
                    return true;
                }

                return false;
            }

            removedEntry = this.EquippedEntries[slotIndex];

            this.SetEquippedEntry(slotIndex, newEntry);
            this.OnEntryReplaced(slotIndex, removedEntry, newEntry);

            if (this.ActiveEquippedEntry == null || UnityEngine.Object.ReferenceEquals(this.ActiveEquippedEntry, removedEntry))
            {
                this.SetActiveEntry(newEntry);
            }
            else
            {
                this.EnsureActiveEntryIsValid();
            }

            return true;
        }

        public bool TryUnequipEntry(InventoryItemEntry entry)
        {
            if (this.IsLoadoutChangeBlocked(nameof(CharacterCombatLoadoutEntryHandlerBase.TryUnequipEntry))) return false;
            if (entry == null) return false;

            int slotIndex = _equippedEntries.IndexOf(entry);
            if (slotIndex < 0) return false;

            this.SetEquippedEntry(slotIndex, null);
            this.OnEntryUnequipped(slotIndex, entry);

            if (UnityEngine.Object.ReferenceEquals(this.ActiveEquippedEntry, entry))
            {
                this.EnsureActiveEntryIsValid();
            }

            return true;
        }

        public virtual bool TryQuickEquip()
        {
            if (this.IsLoadoutChangeBlocked(nameof(CharacterCombatLoadoutEntryHandlerBase.TryQuickEquip))) return false;
            if (this.InventoryController == null) return false;

            this.ClampEquippedEntriesToCapacity();

            InventoryItemEntry firstFoundEntry = null;
            IReadOnlyList<InventoryItemEntry> candidates = this.GetQuickEquipCandidateEntries();

            if (candidates != null)
            {
                for (int i = 0; i < candidates.Count; i++)
                {
                    InventoryItemEntry candidate = candidates[i];
                    if (candidate == null) continue;
                    if (this.IsEquippedEntry(candidate)) continue;
                    if (!this.IsEntryValid(candidate)) continue;

                    firstFoundEntry = candidate;
                    break;
                }
            }

            if (firstFoundEntry == null) return false;

            int emptySlotIndex = this.GetFirstEmptySlotIndex();
            if (emptySlotIndex < 0) return false;

            this.SetEquippedEntry(emptySlotIndex, firstFoundEntry);
            this.OnEntryQuickEquipped(emptySlotIndex, firstFoundEntry);

            this.SetActiveEntry(firstFoundEntry);
            return true;
        }

        public Sprite GetActiveEntryIcon()
        {
            return this.ActiveEquippedEntry != null ? this.ActiveEquippedEntry.Icon : null;
        }

        public void RefreshLoadoutAfterCharacterRefreshment()
        {
            this.ClampEquippedEntriesToCapacity();
            this.RemoveInvalidEquippedEntries();
            this.SyncEquippedStates();
            this.EnsureActiveEntryIsValid();

            //this.NotifyActiveEntryChanged(this.ActiveEquippedEntry);
        }

        // --- Helpers ---
        public abstract bool IsEntryValid(InventoryItemEntry entry);
        protected abstract void NotifyActiveEntryChanged(InventoryItemEntry activeEntry);
        protected virtual IReadOnlyList<InventoryItemEntry> GetQuickEquipCandidateEntries()
        {
            return null;
        }
        protected virtual void OnEntryReplaced(int slotIndex, InventoryItemEntry oldEntry, InventoryItemEntry newEntry)
        {
        }
        protected virtual void OnEntryUnequipped(int slotIndex, InventoryItemEntry removedEntry)
        {
        }
        protected virtual void OnEntryQuickEquipped(int slotIndex, InventoryItemEntry equippedEntry)
        {
        }
        protected virtual void OnEquippedEntryChanged(int slotIndex, InventoryItemEntry oldEntry, InventoryItemEntry newEntry)
        {
        }
        protected virtual void OnActiveEntryChanged(InventoryItemEntry oldEntry, InventoryItemEntry newEntry)
        {
        }

        protected virtual bool IsLoadoutChangeBlocked(string actionName)
        {
            if (this.CombatLoadoutController == null) return false;

            if (this.CombatLoadoutController.IsInAttackWindow)
            {
                Debug.Log($"[{this.CharacterName}.{this.GetType().Name}] {actionName} blocked: {this.EntryDisplayName} loadout change is locked during attack window.");
                return true;
            }

            if (this.CombatLoadoutController.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.CharacterName}.{this.GetType().Name}] {actionName} blocked: CannotSwapLoadout flag is active.");
                return true;
            }

            return false;
        }

        protected void SetActiveEntrySilently(InventoryItemEntry entry)
        {
            this.ActiveEquippedEntry = entry;
        }

        protected void SetEquippedEntry(int slotIndex, InventoryItemEntry newEntry)
        {
            if (slotIndex < 0 || slotIndex >= this.EquippedEntries.Count) return;

            InventoryItemEntry oldEntry = this.EquippedEntries[slotIndex];
            if (ReferenceEquals(oldEntry, newEntry)) return;

            if (oldEntry != null)
            {
                oldEntry.Equipped = false;
            }

            _equippedEntries[slotIndex] = newEntry;

            if (newEntry != null)
            {
                newEntry.Equipped = true;
            }

            this.OnEquippedEntryChanged(slotIndex, oldEntry, newEntry);
        }

        protected void SetActiveEntry(InventoryItemEntry newEntry)
        {
            if (UnityEngine.Object.ReferenceEquals(this.ActiveEquippedEntry, newEntry)) return;

            InventoryItemEntry oldEntry = this.ActiveEquippedEntry;

            if (newEntry == null)
            {
                this.ActiveEquippedEntry = null;
                this.OnActiveEntryChanged(oldEntry, null);
                this.NotifyActiveEntryChanged(null);
                return;
            }

            if (!this.IsEntryValid(newEntry))
            {
                Debug.LogError($"[{this.CharacterName}.{this.GetType().Name}] SetActiveEntry failed: new entry is not a valid {this.EntryDisplayName} entry.");
                return;
            }

            int equippedIndex = _equippedEntries.IndexOf(newEntry);
            if (equippedIndex < 0)
            {
                int emptySlotIndex = this.GetFirstEmptySlotIndex();

                if (emptySlotIndex < 0)
                {
                    Debug.LogWarning($"[{this.CharacterName}.{this.GetType().Name}] SetActiveEntry failed: {this.EntryDisplayName} entry is not equipped and there is no empty slot.");
                    return;
                }

                this.SetEquippedEntry(emptySlotIndex, newEntry);
            }

            this.ActiveEquippedEntry = newEntry;

            this.OnActiveEntryChanged(oldEntry, newEntry);
            this.NotifyActiveEntryChanged(this.ActiveEquippedEntry);
        }

        protected void EnsureActiveEntryIsValid()
        {
            if (!this.EquippedEntryListIsValid())
            {
                this.SetActiveEntry(null);
                return;
            }

            if (this.IsEntryValid(this.ActiveEquippedEntry) && this.EquippedEntries.Contains(this.ActiveEquippedEntry)) return;

            for (int i = 0; i < this.EquippedEntries.Count; i++)
            {
                if (this.IsEntryValid(this.EquippedEntries[i]))
                {
                    this.SetActiveEntry(this.EquippedEntries[i]);
                    return;
                }
            }

            this.SetActiveEntry(null);
        }

        protected void ClampEquippedEntriesToCapacity()
        {
            if (_equippedEntries == null)
            {
                _equippedEntries = new List<InventoryItemEntry>();
            }

            int max = this.MaxEquippedEntries;
            int currentCount = this.EquippedEntries.Count;

            if (currentCount > max)
            {
                for (int i = currentCount - 1; i >= max; i--)
                {
                    InventoryItemEntry removedEntry = _equippedEntries[i];

                    if (removedEntry != null)
                    {
                        removedEntry.Equipped = false;
                    }

                    _equippedEntries.RemoveAt(i);

                    this.OnEquippedEntryChanged(i, removedEntry,null);
                }

                return;
            }

            while (this.EquippedEntries.Count < max)
            {
                _equippedEntries.Add(null);
            }
        }

        protected void SyncEquippedStates()
        {
            if (_equippedEntries == null) return;

            for (int i = 0; i < this.EquippedEntries.Count; i++)
            {
                InventoryItemEntry entry = this.EquippedEntries[i];
                if (entry != null)
                {
                    entry.Equipped = true;
                }
            }
        }

        protected void RemoveInvalidEquippedEntries()
        {
            if (_equippedEntries == null) return;

            for (int i = 0; i < this.EquippedEntries.Count; i++)
            {
                InventoryItemEntry entry = this.EquippedEntries[i];
                if (entry == null) continue;
                if (this.IsEntryValid(entry)) continue;

                this.SetEquippedEntry(i, null);
            }
        }

        protected int GetFirstEmptySlotIndex()
        {
            if (_equippedEntries == null) return -1;

            for (int i = 0; i < this.EquippedEntries.Count; i++)
            {
                if (this.EquippedEntries[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }

        protected bool HasAnyValidEquippedEntry()
        {
            if (!this.EquippedEntryListIsValid()) return false;

            for (int i = 0; i < this.EquippedEntries.Count; i++)
            {
                if (this.IsEntryValid(this.EquippedEntries[i])) return true;
            }

            Debug.LogWarning($"[{this.CharacterName}.{this.GetType().Name}] No valid equipped {this.EntryDisplayName} found.");
            return false;
        }

        protected InventoryItemEntry GetNextValidEntry(int currentIndex, int step)
        {
            if (!this.EquippedEntryListIsValid()) return null;

            int count = this.EquippedEntries.Count;

            step = (int)Mathf.Sign(step);

            if (step == 0) return null;

            int index = currentIndex;
            for (int i = 0; i < count; i++)
            {
                index = (index + step + count) % count;

                InventoryItemEntry candidate = this.EquippedEntries[index];
                if (this.IsEntryValid(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }

        private void ClampMaxEquippedEntries()
        {
            this.MaxEquippedEntries = Mathf.Clamp(this.MaxEquippedEntries, this.MinEquippedEntryCapacity, this.MaxEquippedEntryCapacity);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.MaxEquippedEntries = this.DefaultEquippedEntryCapacity;

            this.ClampMaxEquippedEntries();
            this.ClampEquippedEntriesToCapacity();
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.RefreshLoadoutAfterCharacterRefreshment();
        }
    }
}
