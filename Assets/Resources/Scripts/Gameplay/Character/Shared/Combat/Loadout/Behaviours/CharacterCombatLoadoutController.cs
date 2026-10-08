using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(CharacterCombatLoadoutCapacityHandler))]
    [RequireComponent(typeof(CharacterCombatLoadoutWeaponHandler))]
    [RequireComponent(typeof(CharacterCombatLoadoutProjectileHandler))]
    [RequireComponent(typeof(CharacterCombatLoadoutSpecialSkillHandler))]
    [RequireComponent(typeof(CharacterCombatLoadoutConsumableItemHandler))]
    public class CharacterCombatLoadoutController : CharacterBehaviourControllerBase, 
        IStatusEffectFlagConsumer, 
        ICharacterDeathDisableable,
        ISaveDataHandler<PlayerCombatLoadoutSaveData>
    {
        // --- Referneces ---
        //  1. Character
        private CharacterCombatController CombatController => this.CharacterRootController != null
            ? this.CharacterRootController.CombatController
            : null;
        internal InventoryController InventoryController => this.CharacterRootController != null
            ? this.CharacterRootController.InventoryController
            : null;
        internal CharacterProgressionController ProgressionController => this.CharacterRootController != null
            ? this.CharacterRootController.ProgressionController
            : null;
        internal CharacterCombatResourcesHandler CombatResourcesHandler => this.CharacterRootController != null
            ? this.CharacterRootController.CombatResourcesHandler
            : null;
        internal CharacterCombatStatusEffectApplicationHandler CombatStatusEffectApplicationHandler => this.CharacterRootController != null
            ? this.CharacterRootController.CombatStatusEffectApplicationHandler
            : null;

        //  2. Sub-Behaviours
        [Header("References - Sub Behaviours")]
        [SerializeField] private CharacterCombatLoadoutCapacityHandler _capacityHandler;
        [SerializeField] private CharacterCombatLoadoutWeaponHandler _weaponHandler;
        [SerializeField] private CharacterCombatLoadoutProjectileHandler _projectileHandler;
        [SerializeField] private CharacterCombatLoadoutSpecialSkillHandler _specialSkillHandler;
        [SerializeField] private CharacterCombatLoadoutConsumableItemHandler _consumableItemHandler;
        internal CharacterCombatLoadoutCapacityHandler CapacityHandler => _capacityHandler;
        internal CharacterCombatLoadoutWeaponHandler WeaponHandler => _weaponHandler;
        internal CharacterCombatLoadoutProjectileHandler ProjectileHandler => _projectileHandler;
        internal CharacterCombatLoadoutSpecialSkillHandler SpecialSkillHandler => _specialSkillHandler;
        internal CharacterCombatLoadoutConsumableItemHandler ConsumableItemHandler => _consumableItemHandler;

        // --- Runtime ---
        //  1. Status Effects states
        [Header("Runtime - Status Effects Flags")]
        [SerializeField] private bool _cannotSwapLoadout;
        [SerializeField] private bool _ignoreSwapLock;
        private bool CannotSwapLoadout
        {
            get => _cannotSwapLoadout;
            set => _cannotSwapLoadout = value;
        }
        private bool IgnoreSwapLoadoutLock
        {
            get => _ignoreSwapLock;
            set => _ignoreSwapLock = value;
        }
        public bool IsLoadoutSwapLocked => this.CannotSwapLoadout && !this.IgnoreSwapLoadoutLock;

        //  2. Combat attack state
        public bool IsInAttackWindow    => (this.CombatController != null && this.CombatController.IsInAttackWindow);
        public bool IsAttackLocked      => (this.CombatController != null && this.CombatController.IsAttackLocked);

        //  3. Death Disable State (Runtime)
        [Header("Runtime - Death Disable")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        // --- Events ---
        public event Action OnEquippedWeaponEntriesChanged;
        public event Action<InventoryItemEntry> OnActiveEquippedWeaponChanged;
        public event Action<InventoryItemEntry> OnActiveEquippedProjectileChanged;
        public event Action<SkillProfile> OnActiveSpecialSkillChanged;
        public event Action<InventoryItemEntry> OnActiveEquippedConsumableItemChanged;

        public event Action OnCombatLoadoutRestored;

        // --- Public APIs ---
        //  1. Weapon APIs
        public int MaxEquippedWeaponCapacity                            => this.WeaponHandler.MaxEquippedEntryCapacity;
        public int MaxEquippedWeapons                                   => this.WeaponHandler.MaxEquippedEntries;
        public IReadOnlyList<InventoryItemEntry> EquippedWeaponEntries  => this.WeaponHandler.EquippedEntries;
        public InventoryItemEntry ActiveEquippedWeaponEntry             => this.WeaponHandler.ActiveEquippedEntry;
        public RuntimeWeapon ActiveEquippedRuntimeWeapon                => this.WeaponHandler.ActiveEquippedRuntimeWeapon;

        //  2. Projectile APIs
        public int MaxEquippedProjectileCapacity                            => this.ProjectileHandler.MaxEquippedEntryCapacity;
        public int MaxEquippedProjectiles                                   => this.ProjectileHandler.MaxEquippedEntries;
        public IReadOnlyList<InventoryItemEntry> EquippedProjectileEntries  => this.ProjectileHandler.EquippedEntries;
        public InventoryItemEntry ActiveEquippedProjectileEntry             => this.ProjectileHandler.ActiveEquippedEntry;

        //  3. Consumable item APIs
        public int MaxEquippedConsumableCapacity                                => this.ConsumableItemHandler.MaxEquippedEntryCapacity;
        public int MaxEquippedConsumables                                       => this.ConsumableItemHandler.MaxEquippedEntries;
        public IReadOnlyList<InventoryItemEntry> EquippedConsumableItemEntries  => this.ConsumableItemHandler.EquippedEntries;
        public InventoryItemEntry ActiveEquippedConsumableItemEntry             => this.ConsumableItemHandler.ActiveEquippedEntry;

        //  4. Progression-affected Loadout Max Capacity
        public void SetMaxEquippedWeaponsFromProgression(int amount)            => this.CapacityHandler.SetMaxEquippedWeaponsFromProgression(amount);
        public void SetMaxEquippedProjectilesFromProgression(int amount)        => this.CapacityHandler.SetMaxEquippedProjectilesFromProgression(amount);
        public void SetMaxEquippedConsumableItemsFromProgression(int amount)    => this.CapacityHandler.SetMaxEquippedConsumableItemsFromProgression(amount);

        //  5. Weapon Management
        public bool TryCycleWeaponEntry(int step)
        {
            if (this.IsDeathDisabled) return false;

            return this.WeaponHandler.TryCycleEquippedEntry(step);
        }
        public bool TryReplaceEquippedWeaponEntry(int slotIndex, InventoryItemEntry newWeaponEntry, out InventoryItemEntry removedWeaponEntry)
        {
            if (this.IsDeathDisabled)
            {
                removedWeaponEntry = null;
                return false;
            }

            return this.WeaponHandler.TryReplaceEquippedEntry(slotIndex, newWeaponEntry, out removedWeaponEntry);
        }
        public bool TryUnequipWeaponEntry(InventoryItemEntry weaponEntry)
        {
            if (this.IsDeathDisabled) return false;

            return this.WeaponHandler.TryUnequipEntry(weaponEntry);
        }
        public bool TryQuickEquipWeapon()
        {
            if (this.IsDeathDisabled) return false;

            return this.WeaponHandler.TryQuickEquip();
        }

        //  6. Projectile Management
        public bool TryCycleProjectileEntry(int step)
        {
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryCycleEquippedEntry(step);
        }
        public bool TryReplaceEquippedProjectileEntry(int slotIndex, InventoryItemEntry newProjectileEntry, out InventoryItemEntry removedProjectileEntry)
        {
            if (this.IsDeathDisabled)
            {
                removedProjectileEntry = null;
                return false;
            }

            return this.ProjectileHandler.TryReplaceEquippedEntry(slotIndex, newProjectileEntry, out removedProjectileEntry);
        }
        public bool TryUnequipEquippedProjectileEntry(InventoryItemEntry projectileEntry)
        {
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryUnequipEntry(projectileEntry);
        }
        public bool TryQuickEquipProjectile()
        {
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryQuickEquip();
        }
        public bool TryResolveProjectileForRangedSkill(RangedSkillProfile rangedSkill, out ProjectileProfile projectileItem)
        {
            projectileItem = null;
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryResolveProjectileForRangedSkill(rangedSkill, out projectileItem);
        }
        public bool TryConsumeProjectileForRangedSkill(RangedSkillProfile rangedSkill, out ProjectileCostRecord projectileCostReceipt)
        {
            projectileCostReceipt = null;
            if (this.IsDeathDisabled) return false;

            return this.ProjectileHandler.TryConsumeProjectileForRangedSkill(rangedSkill, out projectileCostReceipt);
        }
        public void RefundProjectileCost(ProjectileCostRecord projectileCostRecord)
        {
            this.ProjectileHandler.RefundProjectileCost(projectileCostRecord);
        }

        //  7. Special Skill handling
        public bool TrySetEquippedWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry, SkillProfile specialSkill)
        {
            if (this.IsDeathDisabled) return false;

            return this.SpecialSkillHandler.TrySetEquippedWeaponActiveSpecialSkill(weaponEntry, specialSkill);
        }
        public bool TrySetActiveEquippedWeaponActiveSpecialSkill(SkillProfile specialSkill)
        {
            if (this.IsDeathDisabled) return false;

            return this.SpecialSkillHandler.TrySetActiveEquippedWeaponActiveSpecialSkill(specialSkill);
        }
        public bool TryClearWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            if (this.IsDeathDisabled) return false;

            return this.SpecialSkillHandler.TryClearWeaponActiveSpecialSkill(weaponEntry);
        }
        public SkillProfile GetWeaponActiveSpecialSkill(InventoryItemEntry weaponEntry)
        {
            return this.SpecialSkillHandler.GetWeaponActiveSpecialSkill(weaponEntry);
        }

        //  8. Consumable Item Management
        public bool TryCycleConsumableItemEntry(int step)
        {
            if (this.IsDeathDisabled) return false;

            return this.ConsumableItemHandler.TryCycleEquippedEntry(step);
        }
        public bool TryReplaceEquippedConsumableItemEntry(int slotIndex, InventoryItemEntry newConsumableEntry, out InventoryItemEntry removedConsumableEntry)
        {
            if (this.IsDeathDisabled)
            {
                removedConsumableEntry = null;
                return false;
            }

            return this.ConsumableItemHandler.TryReplaceEquippedEntry(slotIndex, newConsumableEntry, out removedConsumableEntry);
        }
        public bool TryUnequipEquippedConsumableItemEntry(InventoryItemEntry consumableEntry)
        {
            if (this.IsDeathDisabled) return false;

            return this.ConsumableItemHandler.TryUnequipEntry(consumableEntry);
        }
        public bool TryConsumeConsumableItem()
        {
            if (this.IsDeathDisabled) return false;

            return this.ConsumableItemHandler.TryConsume();
        }

        //  9. UI APIs
        public Sprite GetActiveEquippedWeaponIcon()     => this.WeaponHandler.GetActiveEntryIcon();
        public Sprite GetActiveEquippedProjectileIcon() => this.ProjectileHandler.GetActiveEntryIcon();
        public Sprite GetActiveSpecialSkillIcon()       => this.SpecialSkillHandler.GetActiveSpecialSkillIcon();
        public Sprite GetActiveConsumableItemIcon()     => this.ConsumableItemHandler.GetActiveEntryIcon();


        //  10. Event Notifiers
        internal void NotifyEquippedWeaponEntriesChanged()                                                    => this.OnEquippedWeaponEntriesChanged?.Invoke();
        internal void NotifyActiveEquippedWeaponChanged(InventoryItemEntry activeWeaponEntry)                 => this.OnActiveEquippedWeaponChanged?.Invoke(activeWeaponEntry);
        internal void NotifyActiveEquippedProjectileChanged(InventoryItemEntry activeProjectileEntry)         => this.OnActiveEquippedProjectileChanged?.Invoke(activeProjectileEntry);
        internal void NotifyActiveSpecialSkillChanged(SkillProfile activeSpecialSkill)                        => this.OnActiveSpecialSkillChanged?.Invoke(activeSpecialSkill);
        internal void NotifyActiveEquippedConsumableItemChanged(InventoryItemEntry activeConsumableItemEntry) => this.OnActiveEquippedConsumableItemChanged?.Invoke(activeConsumableItemEntry);

        //  11. IStatusEffectFlagConsumer implementation
        public void SetStatusEffectFlag(StatusEffectFlagTarget flagType, bool value)
        {
            switch (flagType)
            {
                case StatusEffectFlagTarget.CannotSwapLoadout:
                    this.CannotSwapLoadout = value;
                    return;

                case StatusEffectFlagTarget.IgnoreSwapLoadoutLock:
                    this.IgnoreSwapLoadoutLock = value;
                    return;

                default:
                    return;
            }
        }

        //  12. Refresh Character: Reset all loadout states and ensure validity
        public void RefreshLoadoutAfterCharacterRefreshment()
        {
            this.IsDeathDisabled = false;

            this.WeaponHandler.RefreshLoadoutAfterCharacterRefreshment();
            this.ProjectileHandler.RefreshLoadoutAfterCharacterRefreshment();
            this.SpecialSkillHandler.RefreshLoadoutAfterCharacterRefreshment();
            this.ConsumableItemHandler.RefreshLoadoutAfterCharacterRefreshment();
        }

        //  13. ICharacterDeathDisableable implementation
        public int DeathDisablePriority => 40;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;
        }

        //  14. ISaveDataHandler implementation
        public PlayerCombatLoadoutSaveData CaptureSaveData()
        {
            PlayerCombatLoadoutSaveData saveData = new();

            // a. Weapons
            this.CaptureEquippedEntryIds(this.EquippedWeaponEntries, saveData.EquippedWeaponEntryIds);

            saveData.ActiveWeaponEntryId = this.GetRuntimeId(this.ActiveEquippedWeaponEntry);

            // b. Weapon Special Skills
            this.CaptureWeaponActiveSpecialSkills(saveData);

            // c. Projectiles
            this.CaptureEquippedEntryIds(this.EquippedProjectileEntries, saveData.EquippedProjectileEntryIds);

            saveData.ActiveProjectileEntryId = this.GetRuntimeId(this.ActiveEquippedProjectileEntry);

            // d. Consumables
            this.CaptureEquippedEntryIds(this.EquippedConsumableItemEntries, saveData.EquippedConsumableEntryIds);

            saveData.ActiveConsumableEntryId = this.GetRuntimeId(this.ActiveEquippedConsumableItemEntry);

            return saveData;
        }
        public void RestoreSaveData(PlayerCombatLoadoutSaveData saveData)
        {
            if (this.InventoryController == null)
            {
                Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] {nameof(InventoryController)} is missing. Combat Loadout cannot be restored.", this);
                return;
            }

            // Inventory has already been restored. Progression has also already been restored, therefore the current loadout capacities are already authoritative.
            this.WeaponHandler.PrepareForSaveDataRestore();

            if (saveData == null)
            {
                this.RestoreEmptyLoadout();
                this.NotifyCombatLoadoutRestored();

                return;
            }

            // a. Weapons
            List<InventoryItemEntry> restoredWeapons = this.ResolveInventoryEntries(saveData.EquippedWeaponEntryIds);
            InventoryItemEntry activeWeapon = this.ResolveInventoryEntry(saveData.ActiveWeaponEntryId);

            this.WeaponHandler.RestoreEquippedEntries(restoredWeapons, activeWeapon);

            this.EnsureRuntimeWeaponsForEquippedEntries();

            // b. Special Skills
            this.RestoreWeaponActiveSpecialSkills(saveData.WeaponActiveSpecialSkills);

            // c. Projectiles
            List<InventoryItemEntry> restoredProjectiles = this.ResolveInventoryEntries(saveData.EquippedProjectileEntryIds);
            InventoryItemEntry activeProjectile = this.ResolveInventoryEntry(saveData.ActiveProjectileEntryId);

            this.ProjectileHandler.RestoreEquippedEntries(restoredProjectiles, activeProjectile);

            // d. Consumables
            List<InventoryItemEntry> restoredConsumables = this.ResolveInventoryEntries(saveData.EquippedConsumableEntryIds);
            InventoryItemEntry activeConsumable = this.ResolveInventoryEntry(saveData.ActiveConsumableEntryId);

            this.ConsumableItemHandler.RestoreEquippedEntries(restoredConsumables, activeConsumable);

            this.NotifyCombatLoadoutRestored();
        }

        // --- Helpers ---
        //  1. Capture
        private void CaptureEquippedEntryIds(IReadOnlyList<InventoryItemEntry> equippedEntries, List<string> destination)
        {
            if (destination == null) return;

            destination.Clear();

            if (equippedEntries == null) return;

            // One save value per slot. Empty slot => string.Empty.
            for (int i = 0; i < equippedEntries.Count; i++)
            {
                destination.Add(this.GetRuntimeId(equippedEntries[i]));
            }
        }

        private void CaptureWeaponActiveSpecialSkills(PlayerCombatLoadoutSaveData saveData)
        {
            if (saveData == null) return;

            if (saveData.WeaponActiveSpecialSkills == null)
            {
                saveData.WeaponActiveSpecialSkills = new List<PlayerCombatLoadoutWeaponSkillSaveEntry>();
            }
            else
            {
                saveData.WeaponActiveSpecialSkills.Clear();
            }

            IReadOnlyList<InventoryItemEntry> weapons = this.EquippedWeaponEntries;
            if (weapons == null) return;

            for (int i = 0; i < weapons.Count; i++)
            {
                InventoryItemEntry weaponEntry = weapons[i];
                if (weaponEntry == null) continue;

                string weaponEntryId = this.GetRuntimeId(weaponEntry);
                if (string.IsNullOrWhiteSpace(weaponEntryId)) continue;

                SkillProfile activeSpecialSkill = this.GetWeaponActiveSpecialSkill(weaponEntry);

                PlayerCombatLoadoutWeaponSkillSaveEntry skillSaveEntry = new()
                {
                    WeaponEntryId = weaponEntryId,
                    ActiveSpecialSkillId = activeSpecialSkill != null
                       ? activeSpecialSkill.Id
                       : PlayerCombatLoadoutWeaponSkillSaveEntry.NO_SPECIAL_SKILL_ID
                };

                saveData.WeaponActiveSpecialSkills.Add(skillSaveEntry);
            }
        }

        private string GetRuntimeId(InventoryItemEntry entry)
        {
            if (entry == null) return string.Empty;

            entry.EnsureRuntimeId();

            return entry.RuntimeId;
        }

        //  2. Restore - Inventory Entries
        private InventoryItemEntry ResolveInventoryEntry(string runtimeId)
        {
            if (string.IsNullOrWhiteSpace(runtimeId)) return null;

            InventoryItemEntry entry = this.InventoryController?.GetEntryByRuntimeId(runtimeId);

            if (entry == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] Unable to resolve Inventory entry RuntimeId '{runtimeId}'.", this);
            }

            return entry;
        }

        private List<InventoryItemEntry> ResolveInventoryEntries(IReadOnlyList<string> runtimeIds)
        {
            List<InventoryItemEntry> result = new();

            if (runtimeIds == null) return result;

            // Preserve every slot index.
            for (int i = 0; i < runtimeIds.Count; i++)
            {
                InventoryItemEntry entry = this.ResolveInventoryEntry(runtimeIds[i]);

                result.Add(entry);
            }

            return result;
        }

        //  3. Restore - Runtime Weapons
        private void EnsureRuntimeWeaponsForEquippedEntries()
        {
            IReadOnlyList<InventoryItemEntry> weapons = this.EquippedWeaponEntries;

            if (weapons == null) return;

            for (int i = 0; i < weapons.Count; i++)
            {
                InventoryItemEntry weaponEntry = weapons[i];
                if (weaponEntry == null) continue;

                this.WeaponHandler.GetOrCreateRuntimeInstance(weaponEntry);
            }
        }

        //  4. Restore - Weapon Special Skills
        private void RestoreWeaponActiveSpecialSkills(IReadOnlyList<PlayerCombatLoadoutWeaponSkillSaveEntry> savedSkillEntries)
        {
            if (savedSkillEntries == null)
            {
                // Older save data: retain RuntimeWeapon constructor defaults.
                return;
            }

            for (int i = 0; i < savedSkillEntries.Count; i++)
            {
                PlayerCombatLoadoutWeaponSkillSaveEntry savedEntry = savedSkillEntries[i];
                if (savedEntry == null) continue;

                InventoryItemEntry weaponEntry = this.ResolveInventoryEntry(savedEntry.WeaponEntryId);
                if (weaponEntry == null) continue;

                // Ignore mappings referring to Inventory weapons which are not currently equipped.
                if (!this.WeaponHandler.IsEquippedEntry(weaponEntry)) continue;

                SkillProfile specialSkill = null;

                if (savedEntry.ActiveSpecialSkillId != PlayerCombatLoadoutWeaponSkillSaveEntry.NO_SPECIAL_SKILL_ID)
                {
                    specialSkill = InventorySkillProfileResolver.GetById(savedEntry.ActiveSpecialSkillId);
                    if (specialSkill == null)
                    {
                        Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] Unable to restore active special skill ID '{savedEntry.ActiveSpecialSkillId}' for weapon entry '{savedEntry.WeaponEntryId}'.", this);
                        continue;
                    }
                }

                bool restored = this.SpecialSkillHandler.RestoreWeaponActiveSpecialSkill(weaponEntry, specialSkill);
                if (!restored)
                {
                    Debug.LogWarning($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] Unable to restore active special skill for weapon entry '{savedEntry.WeaponEntryId}'.", this);
                }
            }
        }

        //  5. Empty Restore
        private void RestoreEmptyLoadout()
        {
            this.WeaponHandler.RestoreEquippedEntries(null, null);
            this.ProjectileHandler.RestoreEquippedEntries(null, null);
            this.ConsumableItemHandler.RestoreEquippedEntries(null, null);
        }

        //  6. Restore Notification
        private void NotifyCombatLoadoutRestored()
        {
            this.NotifyEquippedWeaponEntriesChanged();

            this.NotifyActiveEquippedWeaponChanged(this.ActiveEquippedWeaponEntry);
            this.NotifyActiveEquippedProjectileChanged(this.ActiveEquippedProjectileEntry);
            this.NotifyActiveEquippedConsumableItemChanged(this.ActiveEquippedConsumableItemEntry);
            this.NotifyActiveSpecialSkillChanged(this.WeaponHandler.ResolveActiveSpecialSkill());

            this.OnCombatLoadoutRestored?.Invoke();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.CapacityHandler        == null) _capacityHandler       = this.GetComponent<CharacterCombatLoadoutCapacityHandler>();
            if (this.WeaponHandler          == null) _weaponHandler         = this.GetComponent<CharacterCombatLoadoutWeaponHandler>();
            if (this.ProjectileHandler      == null) _projectileHandler     = this.GetComponent<CharacterCombatLoadoutProjectileHandler>();
            if (this.SpecialSkillHandler    == null) _specialSkillHandler   = this.GetComponent<CharacterCombatLoadoutSpecialSkillHandler>();
            if (this.ConsumableItemHandler  == null) _consumableItemHandler = this.GetComponent<CharacterCombatLoadoutConsumableItemHandler>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.CapacityHandler        == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] {nameof(this.CapacityHandler)} reference is missing.", this);
            if (this.WeaponHandler          == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] {nameof(this.WeaponHandler)} reference is missing.", this);
            if (this.ProjectileHandler      == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] " + $"{nameof(this.ProjectileHandler)} reference is missing.", this);
            if (this.SpecialSkillHandler    == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] {nameof(this.SpecialSkillHandler)} reference is missing.", this);
            if (this.ConsumableItemHandler  == null) Debug.LogError($"[{this.CharacterName}.{nameof(CharacterCombatLoadoutController)}] {nameof(this.ConsumableItemHandler)} reference is missing.", this);
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.CapacityHandler?.Bind(this);

            this.WeaponHandler?.Bind(this);
            this.ProjectileHandler?.Bind(this);
            this.SpecialSkillHandler?.Bind(this);
            this.ConsumableItemHandler?.Bind(this);
        }
        protected override void UnbindInternalReferences()
        {
            this.ConsumableItemHandler?.Unbind(this);
            this.SpecialSkillHandler?.Unbind(this);
            this.ProjectileHandler?.Unbind(this);
            this.WeaponHandler?.Unbind(this);

            this.CapacityHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
