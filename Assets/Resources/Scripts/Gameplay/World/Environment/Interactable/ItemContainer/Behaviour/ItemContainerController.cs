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
        [SerializeField] private ItemContainerPanelController _itemContainerPanelController;
        private InventoryController InventoryController => _inventoryController;
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
            if (this.Profile == null) return;

            this.InventoryController.InitializeInventory
            (
                maximumEntryCount: this.Profile.MaximumEntryCount,
                sourceEntries: this.Profile.InitialItems
            );
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.InventoryController == null) _inventoryController = this.GetComponentInChildren<InventoryController>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.InventoryController == null)
            {
                Debug.LogError($"[{this.name}.{nameof(ItemContainerController)}]" + $"requires an {nameof(InventoryController)}.", this);
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

            this.InitializeInventory();
        }
    }
}
