using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterCombatLoadoutProjectileHandler : CharacterCombatLoadoutEntryHandlerBase
    {
        // --- States ---
        //  1. Equipped Projectile
        protected override int MaxEquippedEntryCapacity => 1;
        protected override int DefaultEquippedEntryCapacity => 1;
        protected override string EntryDisplayName => "Projectile";

        // --- Public APIs ---
        private int GetEquippedEntryIndex(InventoryItemEntry entry)
        {
            if (entry == null) return -1;

            for (int i = 0; i < this.EquippedEntries.Count; i++)
            {
                if (UnityEngine.Object.ReferenceEquals(this.EquippedEntries[i], entry))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool TryGetItemFromEntry(InventoryItemEntry projectileEntry, out ProjectileProfile projectile)
        {
            projectile = null;

            if (this.InventoryController == null) return false;
            return this.InventoryController.TryGetProjectileFromEntry(projectileEntry, out projectile);
        }

        public override bool IsEntryValid(InventoryItemEntry projectileEntry)
        {
            if (!this.TryGetItemFromEntry(projectileEntry, out ProjectileProfile _)) return false;
            if (projectileEntry.Quantity <= 0) return false;
            if (this.InventoryController == null) return false;
            if (!this.InventoryController.ContainsEntry(projectileEntry)) return false;

            return true;
        }

        public bool TryResolveProjectileForRangedSkill(RangedSkillProfile rangedSkill, out ProjectileProfile projectile)
        {
            projectile = null;

            if (rangedSkill == null) return false;

            if (!rangedSkill.CostProjectile)
            {
                projectile = rangedSkill.DefaultProjectile;

                if (projectile == null)
                {
                    Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutProjectileHandler)}] {nameof(this.TryResolveProjectileForRangedSkill)} failed: ranged skill '{rangedSkill.name}' does not cost projectile but has no default projectile.", this);
                    return false;
                }

                if (projectile.ProjectilePrefab == null)
                {
                    Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutProjectileHandler)}] {nameof(this.TryResolveProjectileForRangedSkill)} failed: default projectile '{projectile.name}' has no projectile prefab.", this);
                    return false;
                }

                return true;
            }

            InventoryItemEntry activeProjectileEntry = this.ActiveEquippedEntry;

            if (!this.IsEntryValid(activeProjectileEntry))
            {
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutProjectileHandler)}] {nameof(this.TryResolveProjectileForRangedSkill)} failed: active projectile entry is null or invalid.");
                return false;
            }

            if (!this.TryGetItemFromEntry(activeProjectileEntry, out projectile))
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutProjectileHandler)}] {nameof(this.TryResolveProjectileForRangedSkill)} failed: active projectile entry is not a projectile item.");
                return false;
            }

            if (projectile.ProjectileType != rangedSkill.RequiredProjectileType)
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutProjectileHandler)}] {nameof(this.TryResolveProjectileForRangedSkill)} failed: active projectile type '{projectile.ProjectileType}' does not match required type '{rangedSkill.RequiredProjectileType}'.");
                return false;
            }

            if (projectile.ProjectilePrefab == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutProjectileHandler)}] {nameof(this.TryResolveProjectileForRangedSkill)} failed: projectile '{projectile.name}' has no projectile prefab.", this);
                return false;   
            }

            return true;
        }

        public bool TryConsumeProjectileForRangedSkill(RangedSkillProfile rangedSkill, out ProjectileCostRecord projectileCostRecord)
        {
            projectileCostRecord = null;

            if (rangedSkill == null) return false;

            if (!rangedSkill.CostProjectile)
            {
                return true;
            }

            int projectileCount = rangedSkill.ProjectileCount;
            if (projectileCount <= 0)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutProjectileHandler)}] {nameof(this.TryConsumeProjectileForRangedSkill)} failed: ranged skill '{rangedSkill.name}' has invalid ProjectileCount: {projectileCount}.", this);
                return false;
            }

            InventoryItemEntry activeProjectileEntry = this.ActiveEquippedEntry;

            if (!this.TryResolveProjectileForRangedSkill(rangedSkill, out ProjectileProfile consumedProjectile))
            {
                return false;
            }

            if (this.InventoryController == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutProjectileHandler)}] {nameof(this.TryConsumeProjectileForRangedSkill)} failed: InventoryController is missing.", this);
                return false;
            }

            if (activeProjectileEntry == null || activeProjectileEntry.Quantity < projectileCount)
            {
                Debug.Log($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutProjectileHandler)}] {nameof(this.TryConsumeProjectileForRangedSkill)} failed: active projectile entry quantity is not enough. Required: {projectileCount}, Current: {activeProjectileEntry?.Quantity ?? 0}.");
                return false;
            }

            int equippedSlotIndex = this.GetEquippedEntryIndex(activeProjectileEntry);
            bool wasActiveEntry = UnityEngine.Object.ReferenceEquals(this.ActiveEquippedEntry, activeProjectileEntry);

            if (equippedSlotIndex < 0)
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutProjectileHandler)}] {nameof(this.TryConsumeProjectileForRangedSkill)} failed: active projectile entry is not in equipped projectile slots.", this);
                return false;
            }

            if (!this.InventoryController.TryRemoveFromEntry(activeProjectileEntry, projectileCount))
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutProjectileHandler)}] {nameof(this.TryConsumeProjectileForRangedSkill)} failed: active projectile entry could not be reduced.", this);
                return false;
            }


            projectileCostRecord = new ProjectileCostRecord
            (
                projectile: consumedProjectile,
                quantity: projectileCount,
                equippedSlotIndex: equippedSlotIndex,
                wasActiveEntry: wasActiveEntry
            );

            this.RefreshConsumedEntryState(activeProjectileEntry);
            return true;
        }

        public void RefundProjectileCost(ProjectileCostRecord projectileCostRecord)
        {
            if (projectileCostRecord == null || !projectileCostRecord.IsValid) return;
            if (this.InventoryController == null) return;

            InventoryItemEntry equippedEntryBeforeRefund = null;

            if (projectileCostRecord.EquippedSlotIndex >= 0 && projectileCostRecord.EquippedSlotIndex < this.EquippedEntries.Count)
            {
                equippedEntryBeforeRefund = this.EquippedEntries[projectileCostRecord.EquippedSlotIndex];
            }

            if (equippedEntryBeforeRefund != null && this.InventoryController.ContainsEntry(equippedEntryBeforeRefund) && equippedEntryBeforeRefund.CanMergeWith(projectileCostRecord.Projectile) && equippedEntryBeforeRefund.RemainingCapacity >= projectileCostRecord.Quantity)
            {
                equippedEntryBeforeRefund.TryAddQuantity(projectileCostRecord.Quantity);

                if (projectileCostRecord.WasActiveEntry)
                {
                    this.SetActiveEntry(equippedEntryBeforeRefund);
                }
                else
                {
                    this.NotifyActiveEntryChanged(this.ActiveEquippedEntry);
                }

                return;
            }

            if (!this.InventoryController.TryAddItemAndGetFirstAffectedEntry(projectileCostRecord.Projectile, projectileCostRecord.Quantity, out InventoryItemEntry refundedEntry))
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutProjectileHandler)}] {nameof(this.RefundProjectileCost)} failed: could not refund projectile '{projectileCostRecord.Projectile.name}' x{projectileCostRecord.Quantity}.", this);
                this.RefreshLoadoutAfterCharacterRefreshment();
                return;
            }

            if (refundedEntry != null && projectileCostRecord.EquippedSlotIndex >= 0 && projectileCostRecord.EquippedSlotIndex < this.EquippedEntries.Count)
            {
                this.SetEquippedEntry(projectileCostRecord.EquippedSlotIndex, refundedEntry);
            }

            if (projectileCostRecord.WasActiveEntry && refundedEntry != null)
            {
                this.SetActiveEntry(refundedEntry);
            }

            this.RefreshLoadoutAfterCharacterRefreshment();
        }

        // --- Helpers ---
        protected override IReadOnlyList<InventoryItemEntry> GetQuickEquipCandidateEntries()
        {
            return this.InventoryController != null ? this.InventoryController.AvailableProjectileEntries : null;
        }

        protected override void NotifyActiveEntryChanged(InventoryItemEntry activeEntry)
        {
            if (this.CombatLoadoutController == null) return;

            this.CombatLoadoutController.NotifyActiveEquippedProjectileChanged(activeEntry);
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
