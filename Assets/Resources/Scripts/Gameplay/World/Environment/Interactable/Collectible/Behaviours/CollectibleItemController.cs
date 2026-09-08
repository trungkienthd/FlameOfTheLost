using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class CollectibleItemController : FOTLWorldObjectProfileBehaviour<CollectibleItemProfile>,
        IPlayerInteractable,
        IPlayerInteractionPromptSource,
        IPlayerInteractionPromptRefreshNotifier
    {
        // --- Data / Configs ---
        //  1. Item
        public ItemProfile Item => this.Profile != null
            ? this.Profile.Item
            : null;

        public int Quantity => this.Profile != null
            ? this.Profile.Quantity
            : 1;

        //  2. Interaction
        public int InteractionPromptPriority => this.Profile != null
            ? this.Profile.InteractionPromptPriority
            : 100;

        //  3. Collection
        private bool DestroyAfterCollection => this.Profile != null && this.Profile.DestroyAfterCollection;

        // --- Runtime ---
        [Header("Runtime")]
        [SerializeField] private bool _isCollected;
        public bool IsCollected
        {
            get => _isCollected;
            private set => _isCollected = value;
        }

        // --- Events ---
        public event Action PromptRefreshRequested;
        public event Action<CollectibleItemController, GameObject> Collected;

        // --- Public APIs ---
        //  1. IPlayerInteractable
        public bool CanInteract(GameObject interactor)
        {
            if (this.IsCollected) return false;
            if (this.Item == null) return false;

            return this.TryGetInteractorInventory(interactor, out InventoryController inventoryController) && inventoryController.CanAddItem(this.Item, this.Quantity);
        }

        public bool TryInteract(GameObject interactor)
        {
            if (this.IsCollected) return false;
            if (this.Item == null) return false;

            if (!this.TryGetInteractorInventory(interactor, out InventoryController inventoryController))
            {
                return false;
            }

            if (!inventoryController.TryAddItem(this.Item, this.Quantity))
            {
                this.PromptRefreshRequested?.Invoke();
                return false;
            }

            this.CompleteCollection(interactor);
            return true;
        }

        //  2. IPlayerInteractionPromptSource
        public bool CanShowInteractionPrompt(GameObject interactor)
        {
            if (this.IsCollected) return false;
            if (this.Item == null) return false;

            return this.TryGetInteractorInventory(interactor, out InventoryController _);
        }

        public string GetInteractionPromptText(GameObject interactor)
        {
            if (!this.TryGetInteractorInventory(interactor, out InventoryController inventoryController))
            {
                return "Collect";
            }

            if (!inventoryController.CanAddItem(this.Item, this.Quantity))
            {
                return "Inventory Full";
            }

            string itemName = !string.IsNullOrWhiteSpace(this.Item.DisplayName)
                ? this.Item.DisplayName
                : "Item";

            if (this.Quantity <= 1)
            {
                return $"Collect {itemName}";
            }

            return $"Collect {itemName} x{this.Quantity}";
        }

        // --- Helpers ---
        //  1. Collection
        private void CompleteCollection(GameObject interactor)
        {
            this.IsCollected = true;

            this.Collected?.Invoke(this, interactor);
            this.PromptRefreshRequested?.Invoke();

            if (this.DestroyAfterCollection)
            {
                UnityEngine.Object.Destroy(this.gameObject);
                return;
            }

            this.gameObject.SetActive(false);
        }

        //  2. Interactor Inventory
        private bool TryGetInteractorInventory(GameObject interactor, out InventoryController inventoryController)
        {
            inventoryController = null;

            if (interactor == null) return false;

            inventoryController = interactor.GetComponentInChildren<InventoryController>();

            return inventoryController != null;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Item == null) Debug.LogError($"[{this.gameObject.name}.{nameof(CollectibleItemController)}] Item is not assigned.", this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.IsCollected = false;
        }
    }
}