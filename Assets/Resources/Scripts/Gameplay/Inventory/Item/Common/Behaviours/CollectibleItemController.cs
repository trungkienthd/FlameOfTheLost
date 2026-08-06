using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class CollectibleItemController : FOTLMonoBehaviour,
        IPlayerInteractable,
        IPlayerInteractionPromptSource,
        IPlayerInteractionPromptRefreshNotifier
    {
        // --- States ---
        //  1. Item
        [Header("Item")]
        [SerializeField] private ItemProfile _item;
        [SerializeField, Min(1)] private int _quantity = 1;
        public ItemProfile Item => _item;
        public int Quantity
        {
            get => _quantity;
            private set => _quantity = Mathf.Max(1, value);
        }

        //  2. References
        [Header("References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private SpriteRenderer SpriteRenderer => _spriteRenderer;

        //  3. Prompt Settings
        [Header("Prompt Settings")]
        [SerializeField] private int _interactionPromptPriority = 100;
        public int InteractionPromptPriority => _interactionPromptPriority;

        //  4. Collection Settings
        [Header("Collection Settings")]
        [SerializeField] private bool _destroyAfterCollection = true;
        private bool DestroyAfterCollection => _destroyAfterCollection;

        //  5. Runtime State
        [Header("Runtime State")]
        [SerializeField] private bool _isCollected;
        public bool IsCollected
        {
            get => _isCollected;
            private set => _isCollected = value;
        }

        // --- Events ---
        public event Action PromptRefreshRequested;
        public event Action<CollectibleItemController, GameObject> Collected;

        // --- Unity Callbacks ---
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.SpriteRenderer != null && this.Item != null)
            {
                if (this.SpriteRenderer.sprite == null)
                {
                    this.SpriteRenderer.sprite = this.Item.Icon;
                }
            }
        }
#endif

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

        //  3. Visual
        private void ApplyItemVisual()
        {
            if (this.SpriteRenderer == null) return;
            if (this.Item == null) return;

            this.SpriteRenderer.sprite = this.Item.Icon;
        }

        //  4. FOTLMonoBehaviour overrides
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.SpriteRenderer == null) _spriteRenderer = this.GetComponentInChildren<SpriteRenderer>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Item == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CollectibleItemController)}] " + $"Item is not assigned.", this);
            }

            if (this.SpriteRenderer == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CollectibleItemController)}] " + $"Sprite Renderer reference is not assigned.", this);
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.IsCollected = false;
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.ApplyItemVisual();
        }
    }
}