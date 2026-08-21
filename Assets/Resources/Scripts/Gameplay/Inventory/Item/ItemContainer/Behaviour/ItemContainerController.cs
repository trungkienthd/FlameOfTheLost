using UnityEngine;

namespace FlameOfTheLost
{
    public class ItemContainerController : FOTLWorldObjectProfileBehaviour<ItemContainerProfile>,
        IPlayerInteractable,
        IPlayerInteractionPromptSource
    {
        // --- Data / Configs
        //  3. Prompt Settings
        [Header("Prompt Settings")]
        [SerializeField] private int _interactionPromptPriority = 100;
        public int InteractionPromptPriority => _interactionPromptPriority;

        // --- References ---
        [Header("References")]
        [SerializeField] private InventoryController _inventoryController;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private ItemContainerPanelController _itemContainerPanelController;
        private InventoryController InventoryController => _inventoryController;
        private SpriteRenderer SpriteRenderer => _spriteRenderer;
        private ItemContainerPanelController ItemContainerPanelController => _itemContainerPanelController;

        public bool CanInteract(GameObject interactor)
        {
            if (interactor == null) return false;
            if (this.InventoryController == null) return false;

            InventoryController playerInventoryController = interactor.GetComponentInChildren<InventoryController>();
            return playerInventoryController != null;
        }

        public bool TryInteract(GameObject interactor)
        {
            if (!this.CanInteract(interactor)) return false;
            if (this.ItemContainerPanelController == null) return false;

            InventoryController playerInventoryController = interactor.GetComponentInChildren<InventoryController>();
            if (playerInventoryController == null) return false;

            this.ItemContainerPanelController.Open
            (
                sourceContainer: this,
                sourceInventoryController: this.InventoryController,
                playerInventoryController: playerInventoryController
            );

            return true;
        }

        public bool CanShowInteractionPrompt(GameObject interactor)
        {
            return true;
        }
        public string GetInteractionPromptText(GameObject interactor)
        {
            return $"Open {this.DisplayName}";
        }

        private void InitializeInventory()
        {
            if (this.InventoryController == null) return;

            this.InventoryController.InitializeInventory
            (
                maximumEntryCount: this.Profile.MaximumEntryCount,
                sourceEntries: this.Profile.InitialItems
            );
        }

        private void InitializeVisual()
        {
            if (this.SpriteRenderer == null) return;

            if (this.SpriteRenderer.sprite == null) this.SpriteRenderer.sprite = this.Profile.Icon;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.InventoryController == null) _inventoryController = this.GetComponentInChildren<InventoryController>();
            if (this.SpriteRenderer == null) _spriteRenderer = this.GetComponentInChildren<SpriteRenderer>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.InventoryController == null)
            {
                Debug.LogError($"[{this.name}.{nameof(ItemContainerController)}]" + $"requires an {nameof(FlameOfTheLost.InventoryController)}.", this);
            }
            if (this.SpriteRenderer == null)
            {
                Debug.LogError($"[{this.name}.{nameof(ItemContainerController)}] " + $"requires a {nameof(UnityEngine.SpriteRenderer)}.", this);
            }
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.ItemContainerPanelController == null)
            {
                GameObject canvas = GameObject.Find("Canvas");
                if (canvas != null) _itemContainerPanelController = canvas.GetComponentInChildren<ItemContainerPanelController>(true);
            }
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            if (this.Profile == null) return;

            this.InitializeInventory();
            this.InitializeVisual();
        }
    }
}
