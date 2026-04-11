using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(InventoryController))]
    public class PlayerInteractionLoadoutController : MonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private InventoryController _inventoryController;
        [SerializeField] private CharacterCombatLoadoutController _playerCombatLoadoutController;
        private InventoryController InventoryController => _inventoryController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;

        //  2. Equipped Slot
        [Header("Interaction Loadout")]
        [SerializeField] private InventoryItemEntry _equippedInteractableEntry = null;
        public InventoryItemEntry EquippedInteractableItemEntry
        {
            get => _equippedInteractableEntry;
            private set => _equippedInteractableEntry = value;
        }

        //  3. Runtime Interaction State
        [Header("Runtime State")]
        [SerializeField] private bool _isCarryingInteractableItem;
        public bool IsCarryingInteractableItem
        {
            get => _isCarryingInteractableItem;
            private set => _isCarryingInteractableItem = value;
        }

        //  4. Loadout State
        public bool IsInAttackWindow => this.PlayerCombatLoadoutController.IsInAttackWindow;
        public bool IsLoadoutSwapLocked => this.PlayerCombatLoadoutController.IsLoadoutSwapLocked;

        //  5. Events
        public event Action<InventoryItemEntry> OnEquippedInteractableItemChanged;
        public event Action<InventoryItemEntry> OnInteractableItemEquipped;
        public event Action<InventoryItemEntry> OnInteractableItemUnequipped;
        public event Action<bool> OnInteractableItemCarryingStateChanged;

        // --- Unity methods ---
        private void Awake()
        {
            if (this.InventoryController == null) _inventoryController = this.GetComponent<InventoryController>();
            if (this.PlayerCombatLoadoutController == null) _playerCombatLoadoutController = this.GetComponent<CharacterCombatLoadoutController>();

            this.SyncEquippedState();
        }
        private void Start()
        {
            this.TrySetInteractableItemCarryingState(false);
        }

        // --- Public APIs ---
        //  1. Queries
        //      1.1. Equipped Item Type Queries
        public CharacterCarryItemType GetCharacterCarryItemType()
        {
            if (!this.TryGetInteractableItemFromEntry(this.EquippedInteractableItemEntry, out InteractableItemProfile interactableItem)) return CharacterCarryItemType.None;

            if (this.EquippedInteractableItemEntry == null)
            {
                return CharacterCarryItemType.None;
            }
            else if (interactableItem is LightSourceProfile)
            {
                return CharacterCarryItemType.LightSource;
            }
            else if (interactableItem is KeyItemProfile)
            {
                return CharacterCarryItemType.KeyItem;
            }

            return CharacterCarryItemType.None;
        }

        //      1.2. Light Source Queries
        public LightSourceType GetCurrentLightSourceType()
        {
            if (!this.TryGetInteractableItemFromEntry(this.EquippedInteractableItemEntry, out InteractableItemProfile interactableItem)) return LightSourceType.None;
            CharacterCarryItemType currentCarryingItemType = this.GetCharacterCarryItemType();
            if (currentCarryingItemType != CharacterCarryItemType.LightSource)
            {
                return LightSourceType.None;
            }

            return (interactableItem as LightSourceProfile).LightSourceType;
        }

        //  2. Actions
        //      2.1. Replace Equipped Interactable Item
        public bool TryReplaceEquippedInteractabletemEntry(InventoryItemEntry newInteractableEntry, out InventoryItemEntry removedInteractableEntry)
        {
            removedInteractableEntry = null;

            if (this.IsInAttackWindow)
            {
                Debug.Log($"[{this.gameObject.name}.PlayerInteractionLoadoutController] TryReplaceEquippedInteractabletemEntry blocked: Interactable Item Replacement is locked.");
                return false;
            }
            if (this.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.gameObject.name}.PlayerInteractionLoadoutController] TryReplaceEquippedInteractabletemEntry blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            if (!this.IsInteractableEntryValid(newInteractableEntry)) return false;
            if (newInteractableEntry.Equipped && !UnityEngine.Object.ReferenceEquals(this.EquippedInteractableItemEntry, newInteractableEntry)) return false;

            InventoryItemEntry previousEntry = this.EquippedInteractableItemEntry;
            this.SetEquippedInteractableItemEntry(newInteractableEntry);

            if (previousEntry != null)
            {
                removedInteractableEntry = previousEntry;
                this.OnInteractableItemUnequipped?.Invoke(previousEntry);
            }

            this.OnInteractableItemEquipped?.Invoke(newInteractableEntry);
            this.OnEquippedInteractableItemChanged?.Invoke(this.EquippedInteractableItemEntry);
            return true;
        }
        //      2.2. Unequip Interactable Item
        public bool TryUnequipInteractableEntry()
        {
            if (this.IsInAttackWindow)
            {
                Debug.Log($"[{this.gameObject.name}.PlayerInteractionLoadoutController] TryUnequipInteractableEntry blocked: Interactable Item Unequipping is locked.");
                return false;
            }
            if (this.IsLoadoutSwapLocked)
            {
                Debug.Log($"[{this.gameObject.name}.PlayerInteractionLoadoutController] TryUnequipInteractableEntry blocked: CannotSwapLoadout flag is active.");
                return false;
            }

            if (!this.IsInteractableEntryValid(this.EquippedInteractableItemEntry)) return false;

            InventoryItemEntry previousItem = this.EquippedInteractableItemEntry;
            this.SetEquippedInteractableItemEntry(null);

            this.TrySetInteractableItemCarryingState(false);

            this.OnInteractableItemUnequipped?.Invoke(previousItem);
            this.OnEquippedInteractableItemChanged?.Invoke(this.EquippedInteractableItemEntry);
            return true;
        }

        //      2.3. Set Carrying State
        public bool TrySetInteractableItemCarryingState(bool isCarrying)
        {
            if (isCarrying && !this.IsInteractableEntryValid(this.EquippedInteractableItemEntry)) return false;

            this.IsCarryingInteractableItem = isCarrying;

            this.OnInteractableItemCarryingStateChanged?.Invoke(this.IsCarryingInteractableItem);
            return true;
        }

        // --- Helpers ---
        //  1. Interactable Item Entry Validation
        private bool IsInteractableEntryValid(InventoryItemEntry entry)
        {
            if (!this.TryGetInteractableItemFromEntry(entry, out _)) return false;
            if (entry.Quantity != 1) return false;
            if (this.InventoryController == null) return false;
            if (!this.InventoryController.ContainsEntry(entry)) return false;

            return true;
        }
        //  2. Interactable Item Profile Retrieval
        private bool TryGetInteractableItemFromEntry(InventoryItemEntry interactableEntry, out InteractableItemProfile interactableItem)
        {
            interactableItem = null;

            if (this.InventoryController == null) return false;
            return this.InventoryController.TryGetInteractableItemFromEntry(interactableEntry, out interactableItem);
        }

        //  3. Equipped State of Interactable Item Entry Management
        private void SetEntryEquippedState(InventoryItemEntry entry, bool equipped)
        {
            if (entry == null) return;
            entry.Equipped = equipped;
        }

        private void SyncEquippedState()
        {
            if (this.InventoryController == null) return;
            if (this.EquippedInteractableItemEntry == null) return;

            this.SetEntryEquippedState(this.EquippedInteractableItemEntry, true);
        }

        private void SetEquippedInteractableItemEntry(InventoryItemEntry newInteractableEntry)
        {
            InventoryItemEntry oldEntry = this.EquippedInteractableItemEntry;
            if (UnityEngine.Object.ReferenceEquals(oldEntry, newInteractableEntry)) return;

            if (oldEntry != null)
            {
                this.SetEntryEquippedState(oldEntry, false);
            }

            this.EquippedInteractableItemEntry = newInteractableEntry;

            if (newInteractableEntry != null)
            {
                this.SetEntryEquippedState(newInteractableEntry, true);
            }
        }
    }
}
