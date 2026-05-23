using UnityEngine;

namespace FlameOfTheLost
{
    public class ItemContainerController : MonoBehaviour,
        IIdentityProvider,
        IPlayerInteractable,
        IPlayerInteractionPromptSource
    {
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _displayName = "Chest";
        [SerializeField, TextArea(5, 5)] private string _description;
        public int ID => _id;
        public string DisplayName => _displayName;
        public string Description => _description;

        [Header("References")]
        [SerializeField] private InventoryController _inventoryController;
        [SerializeField] private ItemContainerPanelController _itemContainerPanelController;
        private InventoryController InventoryController => _inventoryController;
        private ItemContainerPanelController ItemContainerPanelController => _itemContainerPanelController;

        private void Awake()
        {
            this.CacheReferences();
        }

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

        private void CacheReferences()
        {
            if (this.InventoryController == null) _inventoryController = this.GetComponentInChildren<InventoryController>();

            if (this.ItemContainerPanelController == null)
            {
                GameObject canvas = GameObject.Find("Canvas");
                if (canvas != null) _itemContainerPanelController = canvas.GetComponentInChildren<ItemContainerPanelController>(true);
            }
        }
    }
}
