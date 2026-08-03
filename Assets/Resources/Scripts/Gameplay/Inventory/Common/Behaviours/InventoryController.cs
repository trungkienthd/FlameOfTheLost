using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class InventoryController : FOTLMonoBehaviour
    {
        // --- States ---
        //  1. Inventory (Items)
        [Header("Items")]
        [SerializeField] private List<InventoryItemEntry> _items = new();
        public IReadOnlyList<InventoryItemEntry> Items => _items;

        [SerializeField, Min(1)] private int _maximumEntryCount = 20; 
        public int MaximumEntryCount => Mathf.Max(1, _maximumEntryCount);
        public int CurrentEntryCount => _items != null ? _items.Count : 0;
        public int RemainingEntryCount => Mathf.Max(0, this.MaximumEntryCount - this.CurrentEntryCount);

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

        public IReadOnlyList<InventoryItemEntry> AllProjectileEntries => this.GetEntriesOfType<ProjectileProfile>();
        public IReadOnlyList<InventoryItemEntry> AvailableProjectileEntries => this.GetEntriesOfType<ProjectileProfile>(availableOnly: true);

        public IReadOnlyList<InventoryItemEntry> AllCarriableItemEntries => this.GetEntriesOfType<CarriableItemProfile>();
        public IReadOnlyList<InventoryItemEntry> AvailableCarriableItemEntries => this.GetEntriesOfType<CarriableItemProfile>(availableOnly: true);

        public IReadOnlyList<InventoryItemEntry> AllLightSourceEntries => this.GetEntriesOfType<LightSourceProfile>();
        public IReadOnlyList<InventoryItemEntry> AvailableLightSourceEntries => this.GetEntriesOfType<LightSourceProfile>(availableOnly: true);

        public IReadOnlyList<InventoryItemEntry> AllKeyItemEntries => this.GetEntriesOfType<KeyItemProfile>();
        public IReadOnlyList<InventoryItemEntry> AvailableKeyItemEntries => this.GetEntriesOfType<KeyItemProfile>(availableOnly: true);

        public IReadOnlyList<InventoryItemEntry> AllMaterialEntries => this.GetEntriesOfType<MaterialProfile>();
        public IReadOnlyList<InventoryItemEntry> AvailableMaterialEntries => this.GetEntriesOfType<MaterialProfile>(availableOnly: true);

        public bool TryGetWeaponFromEntry(InventoryItemEntry weaponEntry, out WeaponProfile weapon) => this.TryGetItemFromEntry(weaponEntry, out weapon);
        public bool TryGetConsumableItemFromEntry(InventoryItemEntry consumableEntry, out ConsumableItemProfile consumable) => this.TryGetItemFromEntry(consumableEntry, out consumable);
        public bool TryGetProjectileFromEntry(InventoryItemEntry projectileEntry, out ProjectileProfile projectile) => this.TryGetItemFromEntry(projectileEntry, out projectile);
        public bool TryGetCarriableItemFromEntry(InventoryItemEntry interactableEntry, out CarriableItemProfile interactable) => this.TryGetItemFromEntry(interactableEntry, out interactable);

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

        // --- Events ---
        public event Action<ItemProfile, int> OnItemAdded;
        public event Action<ItemProfile, int> OnItemRemoved;
        public event Action<SkillProfile> OnLearnedSpecialSkillAdded;
        public event Action<SkillProfile> OnLearnedSpecialSkillRemoved;

        // --- Public APIs ---
        public void InitializeInventoryEntries(IReadOnlyList<InventoryItemEntry> sourceEntries)
        {
            _items.Clear();

            if (sourceEntries == null)
            {
                Debug.Log($"[{this.gameObject.name}.{nameof(InventoryController)}] Initialized inventory entries. Source entries are null.", this);
                return;
            }

            for (int i = 0; i < sourceEntries.Count; i++)
            {
                InventoryItemEntry sourceEntry = sourceEntries[i];

                if (sourceEntry == null)
                {
                    Debug.LogWarning($"[{this.gameObject.name}.{nameof(InventoryController)}] Initial inventory entry [{i}] is null.", this);
                    continue;
                }
                if (sourceEntry.Item == null)
                {
                    Debug.LogWarning($"[{this.gameObject.name}.{nameof(InventoryController)}] Initial inventory entry [{i}] has no item assigned.", this);
                    continue;
                }
                if (sourceEntry.Quantity <= 0)
                {
                    Debug.LogWarning($"[{this.gameObject.name}.{nameof(InventoryController)}] Initial inventory entry [{i}] has invalid quantity: {sourceEntry.Quantity}. Item: {sourceEntry.Item.name}", this);
                    continue;
                }

                bool added = this.TryAddItemEntryInternal
                (
                    sourceEntry.Item,
                    sourceEntry.Quantity,
                    sourceEntry.Equipped,
                    preserveEntryEquippedState: false
                );

                if (!added)
                {
                    Debug.LogWarning($"[{this.gameObject.name}.{nameof(InventoryController)}] Failed to initialize inventory entry [{i}]. Item: {sourceEntry.Item.name}, Quantity: {sourceEntry.Quantity}", this);
                    continue;
                }
            }
        }

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

        public bool CanAddItem(ItemProfile item, int quantity = 1)
        {
            if (item == null) return false;
            if (quantity <= 0) return false;

            int remainingQuantity = quantity;

            if (item.IsStackable)
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    InventoryItemEntry entry = this.Items[i];
                    if (entry == null || entry.Item == null) continue;
                    if (!entry.CanMergeWith(item)) continue;

                    remainingQuantity -= Mathf.Min
                    (
                        entry.RemainingCapacity,
                        remainingQuantity
                    );

                    if (remainingQuantity <= 0)
                    {
                        return true;
                    }
                }
            }

            int requiredNewEntryCount = this.CalculateRequiredNewEntryCount
            (
                item,
                remainingQuantity
            );

            return requiredNewEntryCount <= this.RemainingEntryCount;
        }

        public bool TryAddItemAndGetFirstAffectedEntry(ItemProfile item, int quantity, out InventoryItemEntry addedEntry)
        {
            addedEntry = null;

            if (!this.CanAddItem(item, quantity)) return false;

            int remainingQuantity = quantity;

            if (item.IsStackable)
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    InventoryItemEntry entry = this.Items[i];
                    if (entry == null || entry.Item == null) continue;
                    if (!entry.CanMergeWith(item)) continue;

                    int addableQuantity = Mathf.Min(entry.RemainingCapacity, remainingQuantity);
                    if (addableQuantity <= 0) continue;

                    entry.Quantity += addableQuantity;
                    remainingQuantity -= addableQuantity;

                    addedEntry ??= entry;

                    if (remainingQuantity <= 0) break;
                }
            }

            while (remainingQuantity > 0)
            {
                int entryQuantity = item.IsStackable 
                    ? Math.Min(item.MaxStackSize, remainingQuantity) 
                    : 1;

                InventoryItemEntry newEntry = new InventoryItemEntry
                {
                    Item = item,
                    Quantity = entryQuantity,
                    Equipped = false
                };

                _items.Add(newEntry);

                addedEntry ??= newEntry;
                remainingQuantity -= entryQuantity;
            }

            this.OnItemAdded?.Invoke(item, quantity);
            return true;
        }

        public bool TryAddItem(ItemProfile item, int quantity = 1)
        {
            return this.TryAddItemAndGetFirstAffectedEntry(item, quantity, out InventoryItemEntry _);
        }

        public bool TryRemoveItem(ItemProfile item, int quantity = 1)
        {
            if (item == null) return false;
            if (quantity <= 0) return false;
            if (!this.HasItem(item, quantity)) return false;

            int remainingQuantity = quantity;

            for (int i = this.Items.Count - 1; i >= 0; i--)
            {
                InventoryItemEntry entry = this.Items[i];
                if (entry == null || entry.Item == null) continue;
                if (!UnityEngine.Object.ReferenceEquals(entry.Item, item)) continue;

                int removeAmount = Mathf.Min(entry.Quantity, remainingQuantity);
                entry.Quantity -= removeAmount;
                remainingQuantity -= removeAmount;

                if (entry.IsEmpty)
                {
                    _items.RemoveAt(i);
                }

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

        private int CalculateRequiredNewEntryCount(ItemProfile item, int quantity)
        {
            if (item == null) return int.MaxValue;
            if (quantity <= 0) return 0;

            if (!item.IsStackable)
            {
                return quantity;
            }

            return Mathf.CeilToInt(quantity / (float)item.MaxStackSize);
        }

        private bool TryAddItemEntryInternal(ItemProfile item, int quantity, bool equipped = false, bool preserveEntryEquippedState = false)
        {
            if (!this.CanAddItem(item, quantity)) return false;

            int remainingQuantity = quantity;

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
                    Quantity = entryQuantity,
                    Equipped = preserveEntryEquippedState ? equipped : false
                };

                _items.Add(newEntry);
                remainingQuantity -= entryQuantity;
            }

            return true;
        }
    }
}
