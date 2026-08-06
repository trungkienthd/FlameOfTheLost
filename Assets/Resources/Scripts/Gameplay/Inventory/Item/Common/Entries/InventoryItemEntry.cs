using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class InventoryItemEntry
    {
        [SerializeField] private ItemProfile _item;
        [SerializeField] private int _quantity;
        [SerializeField] private bool _equipped;

        public ItemProfile Item
        {
            get => _item;
            set => _item = value;
        }

        public int Quantity
        {
            get => _quantity;
            set => _quantity = value;
        }

        public bool Equipped
        {
            get => _equipped;
            set => _equipped = value;
        }

        public Sprite Icon => this.Item != null ? this.Item.Icon : null;

        public bool IsEmpty => this.Item == null || this.Quantity <= 0;
        public bool CanMergeWith(ItemProfile otherItem)
        {
            if (this.Item == null || otherItem == null) return false;
            if (!UnityEngine.Object.ReferenceEquals(this.Item, otherItem)) return false;
            if (!this.Item.IsStackable) return false;

            return true;
        }

        public int RemainingCapacity
        {
            get
            {
                if (this.Item == null) return 0;
                if (!this.Item.IsStackable) return 0;

                return Mathf.Max(0, this.Item.MaxStackSize - this.Quantity);
            }
        }

        public bool TryAddQuantity(int amount)
        {
            if (amount <= 0) return false;
            this.Quantity += amount;
            return true;
        }
        public bool TryRemoveQuantity(int amount)
        {
            if (amount <= 0) return false;
            if (this.Quantity < amount) return false;

            this.Quantity -= amount;
            return true;
        }

        public static InventoryItemEntry CreateRuntimeCopy(InventoryItemEntry source, bool resetEquipped = true)
        {
            if (source == null) return null;
            if (source.Item == null) return null;
            if (source.Quantity <= 0) return null;

            return new InventoryItemEntry
            {
                Item = source.Item,
                Quantity = source.Quantity,
                Equipped = resetEquipped ? false : source.Equipped
            };
        }
    }
}