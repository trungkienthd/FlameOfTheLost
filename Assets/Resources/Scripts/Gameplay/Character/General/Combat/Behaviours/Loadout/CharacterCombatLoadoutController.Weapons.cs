using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public partial class CharacterCombatLoadoutController : MonoBehaviour, IStatusEffectFlagConsumer
    {
        // --- States ---
        //  4. Equipped weapons
        [Header("Equipped Weapons")]
        [SerializeField] private List<InventoryItemEntry> _equippedWeaponEntries = new();
        [SerializeField] private InventoryItemEntry _activeEquippedWeaponEntry;   // Currently equipped weapon
        public IReadOnlyList<InventoryItemEntry> EquippedWeaponEntries => _equippedWeaponEntries;
        private InventoryItemEntry ActiveEquippedWeaponEntry
        {
            get => _activeEquippedWeaponEntry;
            set => _activeEquippedWeaponEntry = value;
        }
        public RuntimeWeapon ActiveEquippedRuntimeWeapon => this.GetOrCreateRuntimeInstance(this.ActiveEquippedWeaponEntry);

        //      4.1. Per-weapon runtime config (per character)
        // Key: InventoryItemEntry (Item is Weapon)
        [Header("Runtime Weapon Config (Per Character)")]
        [SerializeField] private RuntimeWeaponDictionary _runtimeWeaponMap = new();
        private RuntimeWeaponDictionary RuntimeWeaponMap => _runtimeWeaponMap;

        //  7. Events
        public event Action<InventoryItemEntry> OnActiveEquippedWeaponChange;
        public event Action<SkillProfile> OnActiveSpecialSkillChange;

        // --- Public APIs ---
        //  2. Weapon Management
        //      2.1. Cycle Weapon Entry in the equipped list: Step:
        //          PositiveValue = Next,
        //          NegativeValue = Previous
        public bool TryCycleWeaponEntry(int step)
        {
            if (this.IsInAttackWindow)
            {
                Debug.Log($"[{this.gameObject.name}.CharacterCombatLoadoutController] TryCycleWeaponEntry blocked: Weapon switching is locked.");
                return false;
            }
            if (this.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.gameObject.name}.CharacterCombatLoadoutController] TryCycleWeaponEntry blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            step = (int)Mathf.Sign(step);
            if (step == 0) return false;

            if (!this.HasAnyValidEquippedWeapon()) return false;

            int count = this.EquippedWeaponEntries.Count;

            // If nothing equipped yet: pick first or last depending on direction
            if (this.ActiveEquippedWeaponEntry == null)
            {
                InventoryItemEntry fallback = step > 0 ? this.GetNextValidWeaponEntry(-1, 1) : this.GetNextValidWeaponEntry(count, -1);
                if (fallback == null) return false;

                this.SetActiveWeaponEntry(fallback);
                return true;
            }

            int currentIndex = _equippedWeaponEntries.IndexOf(this.ActiveEquippedWeaponEntry);
            if (currentIndex < 0)
            {
                InventoryItemEntry fallback = this.GetNextValidWeaponEntry(-1, +1);
                if (fallback == null) return false;

                this.SetActiveWeaponEntry(fallback);
                return true;
            }

            InventoryItemEntry nextEntry = this.GetNextValidWeaponEntry(currentIndex, step);
            if (nextEntry == null) return false;
            if (System.Object.ReferenceEquals(nextEntry, this.ActiveEquippedWeaponEntry)) return false;

            // Equip next weapon
            this.SetActiveWeaponEntry(nextEntry);
            return true;
        }

        //      2.2. Replace Equipped Weapon at specific slot index, returning removed weapon if any
        public bool TryReplaceEquippedWeaponEntry(int slotIndex, InventoryItemEntry newWeaponEntry, out InventoryItemEntry removedWeaponEntry)
        {
            removedWeaponEntry = null;

            if (this.IsInAttackWindow)
            {
                Debug.Log($"[{this.gameObject.name}.CharacterCombatLoadoutController] TryReplaceEquippedWeapon blocked: Weapon Replacement is locked.");
                return false;
            }
            if (this.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.gameObject.name}.CharacterCombatLoadoutController] TryReplaceEquippedWeapon blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            if (!this.IsWeaponEntryValid(newWeaponEntry)) return false;
            if (slotIndex < 0 || slotIndex >= this.EquippedWeaponEntries.Count) return false;

            // Check if the new entry is already equipped in another slot
            int existingIndex = _equippedWeaponEntries.IndexOf(newWeaponEntry);
            if (existingIndex >= 0)
            {
                // If it's already equipped in the same slot, do nothing
                if (existingIndex == slotIndex)
                {
                    removedWeaponEntry = null;
                    return true;
                }

                return false;           // Otherwise, we cannot equip the same entry in multiple slots
            }

            // Replace the entry in the specified slot
            removedWeaponEntry = this.EquippedWeaponEntries[slotIndex];
            this.SetEquippedWeaponEntry(slotIndex, newWeaponEntry);
            this.GetOrCreateRuntimeInstance(newWeaponEntry);

            if (this.ActiveEquippedWeaponEntry == null || UnityEngine.Object.ReferenceEquals(this.ActiveEquippedWeaponEntry, removedWeaponEntry))
            {
                this.SetActiveWeaponEntry(newWeaponEntry);
            }
            else
            {
                this.EnsureActiveWeaponIsValid();
            }

            return true;
        }

        //      2.3. Unequip Weapon
        public bool TryUnequipWeaponEntry(InventoryItemEntry weaponEntry)
        {
            if (this.IsInAttackWindow)
            {
                Debug.Log($"[{this.gameObject.name}.CharacterCombatLoadoutController] TryUnequipWeapon blocked: Weapon Unequipping is locked.");
                return false;
            }
            if (this.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.gameObject.name}.CharacterCombatLoadoutController] TryUnequipWeapon blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            if (weaponEntry == null) return false;

            int slotIndex = _equippedWeaponEntries.IndexOf(weaponEntry);
            if (slotIndex < 0) return false;
            this.SetEquippedWeaponEntry(slotIndex, null);

            // If removing active weapon, choose a new active
            if (UnityEngine.Object.ReferenceEquals(this.ActiveEquippedWeaponEntry, weaponEntry))
            {
                this.EnsureActiveWeaponIsValid();
            }

            return true;
        }

        //      2.4. Quick equip weapon
        public bool TryQuickEquipWeapon()
        {
            if (this.IsInAttackWindow)
            {
                Debug.Log($"[{this.gameObject.name}.CharacterCombatLoadoutController] TryQuickEquipWeapon blocked: Weapon Quick Equipping is locked.");
                return false;
            }
            if (this.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.gameObject.name}.CharacterCombatLoadoutController] TryQuickEquipWeapon blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            if (this.InventoryController == null) return false;

            this.ClampEquippedWeaponsToCapacity();

            // a. Find first available weapon entry in inventory that is not already equipped
            InventoryItemEntry firstFoundWeaponEntry = null;
            IReadOnlyList<InventoryItemEntry> inventoryWeapons = this.InventoryController.AvailableWeaponEntries;

            if (inventoryWeapons != null)
            {
                for (int i = 0; i < inventoryWeapons.Count; i++)
                {
                    InventoryItemEntry candidate = inventoryWeapons[i];
                    if (candidate == null) continue;

                    firstFoundWeaponEntry = candidate;
                    break;
                }
            }

            if (firstFoundWeaponEntry == null) return false;

            // b. Find first empty equipped slot
            int emptySlotIndex = -1;
            for (int i = 0; i < this.EquippedWeaponEntries.Count; i++)
            {
                if (this.EquippedWeaponEntries[i] != null) continue;

                emptySlotIndex = i;
                break;
            }

            if (emptySlotIndex < 0) return false;

            // c. Equip into that slot
            this.SetEquippedWeaponEntry(emptySlotIndex, firstFoundWeaponEntry);
            this.GetOrCreateRuntimeInstance(firstFoundWeaponEntry);

            // d. Set active equipped weapon entry
            this.SetActiveWeaponEntry(firstFoundWeaponEntry);

            return true;
        }

        //  3. Active Special Skill handling
        //      3.1. Attach / Replace runtime active special skill on an equipped weapon
        public bool TrySetEquippedWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry, SkillProfile specialSkill)
        {
            if (!this.TryGetWeaponFromEntry(weaponEntry, out WeaponProfile weapon)) return false;
            if (specialSkill == null) return false;

            if (this.IsInAttackWindow) return false;
            if (this.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.gameObject.name}.CharacterCombatLoadoutController] TrySetWeaponActiveSpecialSkill blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            if (_equippedWeaponEntries == null || !_equippedWeaponEntries.Contains(weaponEntry)) return false;

            List<SkillProfile> candidates = this.InventoryController.GetPossibleActiveSpecialSkillCandidates(weaponEntry);
            if (!candidates.Contains(specialSkill)) return false;

            RuntimeWeapon runtimeWeapon = this.GetOrCreateRuntimeInstance(weaponEntry);
            if (runtimeWeapon == null) return false;

            runtimeWeapon.ActiveSpecialSkill = specialSkill;

            // Refresh UI only if this weapon is currently active
            if (UnityEngine.Object.ReferenceEquals(this.ActiveEquippedWeaponEntry, weaponEntry))
            {
                this.OnActiveSpecialSkillChange?.Invoke(weapon.ResolveSpecialSkill(runtimeWeapon.ActiveSpecialSkill));
            }

            return true;
        }

        //      3.2. Quick set active equipped weapon's active special skill
        public bool TrySetActiveEquippedWeaponActiveSpecialSkill(SkillProfile specialSkill)
        {
            if (this.ActiveEquippedWeaponEntry == null) return false;
            return this.TrySetEquippedWeaponActiveSpecialSkill(this.ActiveEquippedWeaponEntry, specialSkill);
        }

        //      3.3. Clear runtime active special skill so resolution falls back to innate special skill
        public bool TryClearWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            if (!this.TryGetWeaponFromEntry(weaponEntry, out WeaponProfile weapon)) return false;

            SkillProfile innateSkill = weapon.InnateSpecialSkill;
            if (innateSkill == null) return false;

            return this.TrySetEquippedWeaponActiveSpecialSkill(weaponEntry, innateSkill);
        }

        //      3.4. Get runtime active special skill directly (not resolved fallback)
        public SkillProfile GetWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            if (!this.IsWeaponEntryValid(weaponEntry)) return null;

            RuntimeWeapon runtimeWeapon = this.GetOrCreateRuntimeInstance(weaponEntry);
            return runtimeWeapon != null ? runtimeWeapon.ActiveSpecialSkill : null;
        }

        //  5. UI APIs
        //      5.1. Get Equipped Weapon (Active) Icon
        public Sprite GetActiveEquippedWeaponIcon()
        {
            if (this.ActiveEquippedWeaponEntry != null) return this.ActiveEquippedWeaponEntry.Icon;

            return null;
        }

        //      5.2. Get Current Special Skill (Active) Icon
        public Sprite GetActiveSpecialSkillIcon()
        {
            if (!this.TryGetWeaponFromEntry(this.ActiveEquippedWeaponEntry, out WeaponProfile weapon)) return null;

            var runtimeWeapon = this.GetOrCreateRuntimeInstance(this.ActiveEquippedWeaponEntry);
            SkillProfile resolvedSkill = weapon.ResolveSpecialSkill(runtimeWeapon?.ActiveSpecialSkill);

            return resolvedSkill != null ? resolvedSkill.Icon : null;
        }

        // --- Helpers ---
        //  1. Weapon Management
        //      1.0. OnStart: Clamp the equipped weapon entry list to the max capacity,
        //          ensuring it has exactly MaxEquippedWeapons slots (filled with null if necessary),
        //          and ensure the active weapon entry is valid
        private void ClampEquippedWeaponsToCapacity()
        {
            if (_equippedWeaponEntries == null) _equippedWeaponEntries = new List<InventoryItemEntry>();

            int max = this.MaxEquippedWeapons;
            int currentCount = this.EquippedWeaponEntries.Count;

            // Too many -> trim
            if (currentCount > max)
            {
                _equippedWeaponEntries.RemoveRange(max, currentCount - max);
                return;
            }

            // Too few -> add empty slots
            while (this.EquippedWeaponEntries.Count < max)
            {
                _equippedWeaponEntries.Add(null);
            }
        }

        //      1.1. Set a equipped weapon entry slot
        private void SetEquippedWeaponEntry(int slotIndex, InventoryItemEntry newWeaponEntry)
        {
            if (slotIndex < 0 || slotIndex >= this.EquippedWeaponEntries.Count) return;

            InventoryItemEntry oldEntry = this.EquippedWeaponEntries[slotIndex];
            if (UnityEngine.Object.ReferenceEquals(oldEntry, newWeaponEntry)) return;

            // a. Unequip old
            if (oldEntry != null)
            {
                this.SetEntryEquippedState(oldEntry, false);
            }

            // b. Equip new
            _equippedWeaponEntries[slotIndex] = newWeaponEntry;

            // c. Set equipped state on new entry
            if (newWeaponEntry != null)
            {
                this.SetEntryEquippedState(newWeaponEntry, true);
            }
        }

        //      1.2. Set the active weapon entry slot
        private void SetActiveWeaponEntry(InventoryItemEntry newWeaponEntry)
        {
            if (UnityEngine.Object.ReferenceEquals(this.ActiveEquippedWeaponEntry, newWeaponEntry)) return;

            if (newWeaponEntry == null)
            {
                this.ActiveEquippedWeaponEntry = null;
                this.OnActiveEquippedWeaponChange?.Invoke(null);
                this.OnActiveSpecialSkillChange?.Invoke(null);
                return;
            }

            if (!this.TryGetWeaponFromEntry(newWeaponEntry, out WeaponProfile newWeapon))
            {
                Debug.LogError($"[{this.gameObject.name}.CharacterCombatLoadoutController] SetActiveWeapon failed: newEntry does not contain a valid WeaponProfile.");
                return;
            }

            int equippedIndex = _equippedWeaponEntries.IndexOf(newWeaponEntry);
            if (equippedIndex < 0)
            {
                int emptySlotIndex = -1;
                for (int i = 0; i < this.EquippedWeaponEntries.Count; i++)
                {
                    if (this.EquippedWeaponEntries[i] == null)
                    {
                        emptySlotIndex = i;
                        break;
                    }
                }

                if (emptySlotIndex < 0)
                {
                    Debug.LogWarning($"[{this.gameObject.name}.CharacterCombatLoadoutController] SetActiveWeapon failed: weapon entry is not equipped and there is no empty slot.");
                    return;
                }

                this.SetEquippedWeaponEntry(emptySlotIndex, newWeaponEntry);
                this.GetOrCreateRuntimeInstance(newWeaponEntry);
            }

            this.ActiveEquippedWeaponEntry = newWeaponEntry;
            this.OnActiveEquippedWeaponChange?.Invoke(this.ActiveEquippedWeaponEntry);

            RuntimeWeapon runtimeWeapon = this.GetOrCreateRuntimeInstance(newWeaponEntry);
            this.OnActiveSpecialSkillChange?.Invoke(newWeapon.ResolveSpecialSkill(runtimeWeapon?.ActiveSpecialSkill));
        }

        //      1.3. Get or create runtime weapon instance for a given equipped weapon entry
        private RuntimeWeapon GetOrCreateRuntimeInstance(InventoryItemEntry weaponEntry)
        {
            if (!this.TryGetWeaponFromEntry(weaponEntry, out WeaponProfile weapon)) return null;

            if (this.RuntimeWeaponMap.TryGetValue(weaponEntry, out var runtimeWeapon) && runtimeWeapon != null) return runtimeWeapon;

            runtimeWeapon = new RuntimeWeapon(weapon, this.gameObject);
            this.RuntimeWeaponMap[weaponEntry] = runtimeWeapon;

            return runtimeWeapon;
        }

        //      1.4. Check if a weapon entry is valid:
        //          - not null, and
        //          - has a valid weapon profile, and
        //          - quantity == 1 (Weapon is non-stackable), and
        //          - exists in the inventory
        private bool IsWeaponEntryValid(InventoryItemEntry entry)
        {
            if (!this.TryGetWeaponFromEntry(entry, out _)) return false;
            if (entry.Quantity != 1) return false;
            if (this.InventoryController == null) return false;
            if (!this.InventoryController.ContainsEntry(entry)) return false;

            return true;
        }
        //          1.4.1. Check if there is at least one valid (specified above) weapon entry in the equipped list  
        private bool HasAnyValidEquippedWeapon()
        {
            if (_equippedWeaponEntries == null || this.EquippedWeaponEntries.Count == 0) return false;

            for (int i = 0; i < this.EquippedWeaponEntries.Count; i++)
            {
                if (this.IsWeaponEntryValid(this.EquippedWeaponEntries[i])) return true;
            }

            Debug.LogWarning($"[{this.gameObject.name}.CharacterCombatLoadoutController] No valid equipped weapon found.");
            return false;
        }
        //          1.4.2. Try to get WeaponProfile from a weapon entry, returning null if invalid
        private bool TryGetWeaponFromEntry(InventoryItemEntry weaponEntry, out WeaponProfile weapon)
        {
            weapon = null;

            if (this.InventoryController == null) return false;
            return this.InventoryController.TryGetWeaponFromEntry(weaponEntry, out weapon);
        }

        //      1.5. Check if the active weapon entry is valid (in the equipped list), if not try to find the next valid one, if none found set to null
        private void EnsureActiveWeaponIsValid()
        {
            if (_equippedWeaponEntries == null || this.EquippedWeaponEntries.Count == 0)
            {
                this.SetActiveWeaponEntry(null);
                return;
            }

            if (this.IsWeaponEntryValid(this.ActiveEquippedWeaponEntry) && _equippedWeaponEntries.Contains(this.ActiveEquippedWeaponEntry)) return;

            for (int i = 0; i < this.EquippedWeaponEntries.Count; i++)
            {
                if (this.IsWeaponEntryValid(this.EquippedWeaponEntries[i]))
                {
                    this.SetActiveWeaponEntry(this.EquippedWeaponEntries[i]);
                    return;
                }
            }

            this.SetActiveWeaponEntry(null);
        }

        //      1.6. Get the next entry for Switching between equipped weapons
        private InventoryItemEntry GetNextValidWeaponEntry(int currentIndex, int step)
        {
            if (_equippedWeaponEntries == null || this.EquippedWeaponEntries.Count == 0) return null;

            int count = this.EquippedWeaponEntries.Count;
            step = (int)Mathf.Sign(step);
            if (step == 0) return null;

            int index = currentIndex;
            for (int i = 0; i < count; i++)
            {
                index = (index + step + count) % count;

                InventoryItemEntry candidate = this.EquippedWeaponEntries[index];
                if (this.IsWeaponEntryValid(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }
    }
}
