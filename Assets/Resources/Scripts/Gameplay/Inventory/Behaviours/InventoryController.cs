using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class InventoryController : MonoBehaviour
    {
        // --- States ---
        //  1. Inventory (Items)
        [Header("Items")]
        [SerializeField] private List<InventoryItemEntry> _items = new();
        public IReadOnlyList<InventoryItemEntry> Items => _items;
        private List<InventoryItemEntry> GetEntriesOfType<TItemProfile>(bool availableOnly = false) where TItemProfile : ItemProfile
        {
            List<InventoryItemEntry> result = new();
            if (_items == null) return result;

            for (int i = 0; i < this.Items.Count; i++)
            {
                InventoryItemEntry entry = this.Items[i];
                if (entry == null || entry.Item == null) continue;
                if (entry.Quantity <= 0) continue;
                if (availableOnly && entry.Equipped) continue;
                if (entry.Item is not TItemProfile) continue;

                result.Add(entry);
            }

            return result;
        }
        private bool TryGetItemFromEntry<TItemProfile>(InventoryItemEntry entry, out TItemProfile item) where TItemProfile : ItemProfile
        {
            item = null;
            if (entry == null) return false;
            if (entry.Item == null) return false;
            item = entry.Item as TItemProfile;
            return item != null;
        }

        public IReadOnlyList<InventoryItemEntry> AllWeaponEntries => this.GetEntriesOfType<WeaponProfile>();
        public IReadOnlyList<InventoryItemEntry> AvailableWeaponEntries => this.GetEntriesOfType<WeaponProfile>(availableOnly: true);

        public IReadOnlyList<InventoryItemEntry> AllConsumableItemEntries => this.GetEntriesOfType<ConsumableItemProfile>();
        public IReadOnlyList<InventoryItemEntry> AvailableConsumableItemEntries => this.GetEntriesOfType<ConsumableItemProfile>(availableOnly: true);

        public IReadOnlyList<InventoryItemEntry> AllInteractableItemEntries => this.GetEntriesOfType<InteractableItemProfile>();
        public IReadOnlyList<InventoryItemEntry> AvailableInteractableItemEntries => this.GetEntriesOfType<InteractableItemProfile>(availableOnly: true);

        public IReadOnlyList<InventoryItemEntry> AllLightSourceEntries => this.GetEntriesOfType<LightSourceProfile>();
        public IReadOnlyList<InventoryItemEntry> AllKeyItemEntries => this.GetEntriesOfType<KeyItemProfile>();

        public bool TryGetWeaponFromEntry(InventoryItemEntry weaponEntry, out WeaponProfile weapon) => this.TryGetItemFromEntry(weaponEntry, out weapon);
        public bool TryGetConsumableItemFromEntry(InventoryItemEntry consumableEntry, out ConsumableItemProfile consumable) => this.TryGetItemFromEntry(consumableEntry, out consumable);
        public bool TryGetInteractableItemFromEntry(InventoryItemEntry interactableEntry, out InteractableItemProfile interactable) => this.TryGetItemFromEntry(interactableEntry, out interactable);

        //  2. (Learned) Special Skills
        [Header("Learned Special Skills")]
        [SerializeField] private List<SkillProfile> _learnedSpecialSkills = new();
        public IReadOnlyList<SkillProfile> LearnedSpecialSkills => _learnedSpecialSkills;

        public List<SkillProfile> GetPossibleActiveSpecialSkillCandidates(InventoryItemEntry weaponEntry, List<SkillProfile> excludedSkills = null)
        {
            List<SkillProfile> result = new();

            if (!this.TryGetWeaponFromEntry(weaponEntry, out WeaponProfile weapon)) return result;

            SkillProfile innateSkill = weapon.InnateSpecialSkill;
            if (innateSkill != null && innateSkill.IsCompatibleWithWeaponType(weapon.Type) && !this.IsExcluded(innateSkill, excludedSkills))
            {
                result.Add(innateSkill);
            }

            if (this.LearnedSpecialSkills == null) return result;

            for (int i = 0; i < this.LearnedSpecialSkills.Count; i++)
            {
                SkillProfile learnedSkill = this.LearnedSpecialSkills[i];
                if (learnedSkill == null) continue;
                if (learnedSkill.DefaultProvisionType != SkillProvisionType.CharacterLearned) continue;
                if (!learnedSkill.IsCompatibleWithWeaponType(weapon.Type)) continue;
                if (this.IsExcluded(learnedSkill, excludedSkills)) continue;
                if (result.Contains(learnedSkill)) continue;

                result.Add(learnedSkill);
            }

            return result;
        }

        //  3. Events
        public event Action<ItemProfile, int> OnItemAdded;
        public event Action<ItemProfile, int> OnItemRemoved;
        public event Action<SkillProfile> OnLearnedSpecialSkillAdded;
        public event Action<SkillProfile> OnLearnedSpecialSkillRemoved;

        // --- Unity methods ---
        private void Awake()
        {   
        }

        // --- Public APIs ---
        public int GetItemCount(ItemProfile item)
        {
            if (item == null) return 0;

            int total = 0;
            for (int i = 0; i < this.Items.Count; i++)
            {
                InventoryItemEntry stack = this.Items[i];
                if (stack == null || stack.Item == null) continue;
                if (!UnityEngine.Object.ReferenceEquals(stack.Item, item)) continue;

                total += stack.Quantity;
            }

            return total;
        }

        public bool HasItem(ItemProfile item, int requiredQuantity = 1)
        {
            if (item == null) return false;
            if (requiredQuantity <= 0) return false;

            int total = this.GetItemCount(item);
            return total >= requiredQuantity;
        }

        public bool TryAddItem(ItemProfile item, int quantity = 1)
        {
            if (item == null) return false;
            if (quantity <= 0) return false;

            int remainingQuantity = quantity;

            // If possible, merge into existing entries first (only for stackable items, until reaching RemainingCapacity (= MaxStackSize - Current Quantity))
            if (item.IsStackable)
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    InventoryItemEntry entry = this.Items[i];
                    if (entry == null || entry.Item == null || !entry.CanMergeWith(item)) continue;

                    int addableQuantity = Math.Min(entry.RemainingCapacity, remainingQuantity);
                    if (addableQuantity <= 0) continue;

                    entry.Quantity += addableQuantity;
                    remainingQuantity -= addableQuantity;

                    if (remainingQuantity <= 0)
                    {
                        this.OnItemAdded?.Invoke(item, quantity);
                        return true;
                    }
                }
            }

            while (remainingQuantity > 0)
            {
                int entryQuantity = item.IsStackable ? Math.Min(item.MaxStackSize, remainingQuantity) : 1;
                InventoryItemEntry newEntry = new InventoryItemEntry
                {
                    Item = item,
                    Quantity = entryQuantity
                };

                _items.Add(newEntry);
                remainingQuantity -= entryQuantity;
            }

            this.OnItemAdded?.Invoke(item, quantity);
            return true;
        }

        public bool TryRemoveItem(ItemProfile item, int quantity = 1)
        {
            if (item == null) return false;
            if (quantity <= 0) return false;
            if (!this.HasItem(item, quantity)) return false;

            int remainingQuantity = quantity;

            for (int i = this.Items.Count - 1; i >= 0; i--)
            {
                InventoryItemEntry stack = this.Items[i];
                if (stack == null || stack.Item == null) continue;
                if (!UnityEngine.Object.ReferenceEquals(stack.Item, item)) continue;

                int removeAmount = Mathf.Min(stack.Quantity, remainingQuantity);
                stack.Quantity -= removeAmount;
                remainingQuantity -= removeAmount;

                if (remainingQuantity <= 0) break;
            }

            this.OnItemRemoved?.Invoke(item, quantity);
            return true;
        }

        public InventoryItemEntry FindFirstEntry(ItemProfile item, int requiredQuantity = 1)
        {
            if (item == null) return null;
            if (requiredQuantity <= 0) return null;

            for (int i = 0; i < this.Items.Count; i++)
            {
                InventoryItemEntry entry = this.Items[i];
                if (entry == null || entry.Item == null) continue;
                if (!UnityEngine.Object.ReferenceEquals(entry.Item, item)) continue;
                if (entry.Quantity < requiredQuantity) continue;

                return entry;
            }

            return null;
        }

        public bool ContainsEntry(InventoryItemEntry entry)
        {
            if (entry == null) return false;
            return _items.Contains(entry);
        }

        public bool TryRemoveFromEntry(InventoryItemEntry entry, int quantity = 1)
        {
            if (entry == null) return false;
            if (quantity <= 0) return false;
            if (!this.ContainsEntry(entry)) return false;
            if (entry.Item == null) return false;
            if (entry.Quantity < quantity) return false;

            ItemProfile removedItem = entry.Item;

            entry.Quantity -= quantity;

            if (entry.IsEmpty)
            {
                _items.Remove(entry);
            }

            this.OnItemRemoved?.Invoke(removedItem, quantity);
            return true;
        }

        public bool TryAddLearnedSpecialSkill(SkillProfile skill)
        {
            if (skill == null) return false;
            if (_learnedSpecialSkills.Contains(skill)) return false;
            if (skill.DefaultProvisionType != SkillProvisionType.CharacterLearned) return false;

            _learnedSpecialSkills.Add(skill);
            this.OnLearnedSpecialSkillAdded?.Invoke(skill);
            return true;
        }

        public bool TryRemoveLearnedSpecialSkill(SkillProfile skill)
        {
            if (skill == null) return false;

            int index = _learnedSpecialSkills.IndexOf(skill);
            if (index < 0) return false;

            _learnedSpecialSkills.RemoveAt(index);
            this.OnLearnedSpecialSkillRemoved?.Invoke(skill);

            return true;
        }

        public bool HasLearnedSpecialSkill(SkillProfile skill)
        {
            if (skill == null) return false;
            return _learnedSpecialSkills.Contains(skill);
        }

        // --- Helpers ---
        private bool IsExcluded(SkillProfile skill, List<SkillProfile> excludedSkills)
        {
            return skill != null && excludedSkills != null && excludedSkills.Contains(skill);
        }
    }
}
