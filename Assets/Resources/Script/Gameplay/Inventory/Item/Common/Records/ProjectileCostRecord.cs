using UnityEngine;

namespace FlameOfTheLost
{
    public class ProjectileCostRecord
    {
        public ProjectileProfile Projectile { get; }
        public int Quantity { get; }
        public int EquippedSlotIndex { get; }
        public bool WasActiveEntry { get; }

        public ProjectileCostRecord(ProjectileProfile projectile, int quantity, int equippedSlotIndex, bool wasActiveEntry)
        {
            this.Projectile = projectile;
            this.Quantity = quantity;
            this.EquippedSlotIndex = equippedSlotIndex;
            this.WasActiveEntry = wasActiveEntry;
        }

        public bool IsValid => this.Projectile != null && this.Quantity > 0 && this.EquippedSlotIndex >= 0;
    }
}