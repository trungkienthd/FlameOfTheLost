using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerCarryLoadoutController : CharacterBehaviourControllerBase, 
        ICharacterRefreshable,
        ICharacterDeathDisableable
    {
        // --- States ---
        //  1. References
        private InventoryController InventoryController => this.CharacterRootController.InventoryController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => this.CharacterRootController.CombatLoadoutController;

        //  2. Equipped Slot
        [Header("Equipped Carrying Entry (Runtime)")]
        [SerializeField] private InventoryItemEntry _equippedCarriableEntry = null;
        public InventoryItemEntry EquippedCarriableItemEntry
        {
            get => _equippedCarriableEntry;
            private set => _equippedCarriableEntry = value;
        }

        //  3. Runtime Carrying State
        [Header("Carrying State (Runtime)")]
        [SerializeField] private bool _isCarryingCarriableItem;
        public bool IsCarryingCarriableItem
        {
            get => _isCarryingCarriableItem;
            private set => _isCarryingCarriableItem = value;
        }

        //  4. Combat Loadout State
        public bool IsInAttackWindow => this.PlayerCombatLoadoutController.IsInAttackWindow;
        public bool IsLoadoutSwapLocked => this.PlayerCombatLoadoutController.IsLoadoutSwapLocked;

        //  5. Death Disable State (Runtime)
        [Header("Death Disable (Runtime)")]
        [SerializeField] private bool _isDeathDisabled;
        public bool IsDeathDisabled
        {
            get => _isDeathDisabled;
            private set => _isDeathDisabled = value;
        }

        // --- Events ---
        public event Action<InventoryItemEntry> OnEquippedCarriableItemChanged;
        public event Action<InventoryItemEntry> OnCarriableItemEquipped;
        public event Action<InventoryItemEntry> OnCarriableItemUnequipped;
        public event Action<bool> OnCarriableItemCarryingStateChanged;

        // --- Unity Callbacks ---
        protected override void Awake()
        {
            base.Awake();

            this.SyncEquippedState();
        }
        private void Start()
        {
            this.TrySetCarriableItemCarryingState(false);
        }

        // --- Public APIs ---
        //  1. Queries
        //      1.1. Equipped Item Type Queries
        public CharacterCarryItemType GetCharacterCarryItemType()
        {
            if (this.IsDeathDisabled) return CharacterCarryItemType.None;
            if (!this.TryGetCarriableItemFromEntry(this.EquippedCarriableItemEntry, out CarriableItemProfile carriableItem)) return CharacterCarryItemType.None;

            if (this.EquippedCarriableItemEntry == null)
            {
                return CharacterCarryItemType.None;
            }
            else if (carriableItem is LightSourceProfile)
            {
                return CharacterCarryItemType.LightSource;
            }
            else if (carriableItem is KeyItemProfile)
            {
                return CharacterCarryItemType.KeyItem;
            }

            return CharacterCarryItemType.None;
        }

        //      1.2. Light Source Queries
        public LightSourceType GetCurrentLightSourceType()
        {
            if (this.IsDeathDisabled) return LightSourceType.None;
            if (!this.TryGetCarriableItemFromEntry(this.EquippedCarriableItemEntry, out CarriableItemProfile carriableItem)) return LightSourceType.None;
           
            CharacterCarryItemType currentCarryingItemType = this.GetCharacterCarryItemType();
            if (currentCarryingItemType != CharacterCarryItemType.LightSource) return LightSourceType.None;

            return (carriableItem as LightSourceProfile).LightSourceType;
        }

        //  2. Actions
        //      2.1. Replace Equipped Carriable Item
        public bool TryReplaceEquippedCarriableItemEntry(InventoryItemEntry newCarriableEntry, out InventoryItemEntry removedCarriableEntry)
        {
            removedCarriableEntry = null;

            if (this.IsDeathDisabled) return false;

            if (this.IsInAttackWindow)
            {
                //Debug.Log($"[{this.gameObject.name}.{nameof(PlayerCarryLoadoutController)}] {nameof(TryReplaceEquippedCarriableItemEntry)} blocked: Attack window is active.");
                return false;
            }
            if (this.IsLoadoutSwapLocked)
            {
                //Debug.Log($"[{this.gameObject.name}.{nameof(PlayerCarryLoadoutController)}] {nameof(TryReplaceEquippedCarriableItemEntry)} blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            if (!this.IsCarriableItemEntryValid(newCarriableEntry)) return false;
            if (newCarriableEntry.Equipped && !UnityEngine.Object.ReferenceEquals(this.EquippedCarriableItemEntry, newCarriableEntry)) return false;

            InventoryItemEntry previousEntry = this.EquippedCarriableItemEntry;
            this.SetEquippedCarriableItemEntry(newCarriableEntry);

            if (previousEntry != null)
            {
                removedCarriableEntry = previousEntry;
                this.OnCarriableItemUnequipped?.Invoke(previousEntry);
            }

            this.OnCarriableItemEquipped?.Invoke(newCarriableEntry);
            this.OnEquippedCarriableItemChanged?.Invoke(this.EquippedCarriableItemEntry);
            return true;
        }
        //      2.2. Unequip Carriable Item
        public bool TryUnequipCarriableEntry()
        {
            if (this.IsDeathDisabled) return false;

            if (this.IsInAttackWindow)
            {
                //Debug.Log($"[{this.gameObject.name}.{nameof(PlayerCarryLoadoutController)}] {nameof(TryUnequipCarriableEntry)} blocked: Attack window is active.");
                return false;
            }
            if (this.IsLoadoutSwapLocked)
            {
                //Debug.Log($"[{this.gameObject.name}.{nameof(PlayerCarryLoadoutController)}] {nameof(TryUnequipCarriableEntry)} blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            if (!this.IsCarriableItemEntryValid(this.EquippedCarriableItemEntry)) return false;

            InventoryItemEntry previousItem = this.EquippedCarriableItemEntry;
            this.SetEquippedCarriableItemEntry(null);

            this.TrySetCarriableItemCarryingState(false);

            this.OnCarriableItemUnequipped?.Invoke(previousItem);
            this.OnEquippedCarriableItemChanged?.Invoke(this.EquippedCarriableItemEntry);
            return true;
        }

        //      2.3. Set Carrying State
        public bool TrySetCarriableItemCarryingState(bool isCarrying)
        {
            if (this.IsDeathDisabled)
            {
                if (this.IsCarryingCarriableItem)
                {
                    this.SetCarriableItemCarryingStateInternal(false);
                }

                return false;
            }

            if (isCarrying && !this.IsCarriableItemEntryValid(this.EquippedCarriableItemEntry)) return false;

            return this.SetCarriableItemCarryingStateInternal(isCarrying);
        }

        //  3. ICharacterRefreshable implementation
        public int RefreshmentConfigurePriority => 45;
        public void RefreshCharacter(CharacterRefreshContext context)
        {
            this.IsDeathDisabled = false;

            this.SyncEquippedState();

            if (!this.IsCarriableItemEntryValid(this.EquippedCarriableItemEntry))
            {
                this.SetEquippedCarriableItemEntry(null);
            }

            this.SetCarriableItemCarryingStateInternal(false);

            this.OnEquippedCarriableItemChanged?.Invoke(this.EquippedCarriableItemEntry);
        }

        //  4. ICharacterDeathDisableable implementation
        public int DeathDisablePriority => 35;
        public void DisableAfterCharacterDeath(CharacterDeathContext context)
        {
            this.IsDeathDisabled = true;

            // Preserve equipped item, but stop actively carrying it.
            this.SetCarriableItemCarryingStateInternal(false);
        }

        // --- Helpers ---
        //  1. Carriable Item Entry Validation
        private bool IsCarriableItemEntryValid(InventoryItemEntry entry)
        {
            if (!this.TryGetCarriableItemFromEntry(entry, out _)) return false;
            if (entry.Quantity != 1) return false;
            if (this.InventoryController == null) return false;
            if (!this.InventoryController.ContainsEntry(entry)) return false;

            return true;
        }

        //  2. Carriable Item Profile Retrieval
        private bool TryGetCarriableItemFromEntry(InventoryItemEntry carriableEntry, out CarriableItemProfile carriableItem)
        {
            carriableItem = null;

            if (this.InventoryController == null) return false;
            return this.InventoryController.TryGetCarriableItemFromEntry(carriableEntry, out carriableItem);
        }

        //  3. Equipped State of Carriable Item Entry Management
        private void SetEntryEquippedState(InventoryItemEntry entry, bool equipped)
        {
            if (entry == null) return;
            entry.Equipped = equipped;
        }

        private void SyncEquippedState()
        {
            if (this.InventoryController == null) return;
            if (this.EquippedCarriableItemEntry == null) return;

            this.SetEntryEquippedState(this.EquippedCarriableItemEntry, true);
        }

        private void SetEquippedCarriableItemEntry(InventoryItemEntry newCarriableEntry)
        {
            InventoryItemEntry oldEntry = this.EquippedCarriableItemEntry;
            if (UnityEngine.Object.ReferenceEquals(oldEntry, newCarriableEntry)) return;

            if (oldEntry != null)
            {
                this.SetEntryEquippedState(oldEntry, false);
            }

            this.EquippedCarriableItemEntry = newCarriableEntry;

            if (newCarriableEntry != null)
            {
                this.SetEntryEquippedState(newCarriableEntry, true);
            }
        }

        private bool SetCarriableItemCarryingStateInternal(bool isCarrying)
        {
            if (this.IsCarryingCarriableItem == isCarrying) return true;

            this.IsCarryingCarriableItem = isCarrying;

            this.OnCarriableItemCarryingStateChanged?.Invoke(this.IsCarryingCarriableItem);
            return true;
        }
    }
}
