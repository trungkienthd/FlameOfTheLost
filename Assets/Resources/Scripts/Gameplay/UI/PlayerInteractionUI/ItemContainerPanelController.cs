using UnityEngine;

namespace FlameOfTheLost
{
    public class ItemContainerPanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ItemContainerPanelItemEntryListSubPanelController _entryListSubPanelController;
        private ItemContainerPanelItemEntryListSubPanelController EntryListSubPanelController => _entryListSubPanelController;

        public ItemContainerController CurrentContainer { get; private set; }
        public InventoryController SourceInventoryController { get; private set; }
        public InventoryController PlayerInventoryController { get; private set; }

        private void Awake()
        {
            this.CacheReferences();

            this.Close();
        }

        public void Open(ItemContainerController sourceContainer, InventoryController sourceInventoryController, InventoryController playerInventoryController)
        {
            if (sourceContainer == null) return;
            if (sourceInventoryController == null) return;
            if (playerInventoryController == null) return;

            this.CurrentContainer = sourceContainer;
            this.SourceInventoryController = sourceInventoryController;
            this.PlayerInventoryController = playerInventoryController;

            this.gameObject.SetActive(true);

            if (this.EntryListSubPanelController != null)
            {
                this.EntryListSubPanelController.Bind(sourceInventoryController, playerInventoryController);
            }
        }

        public void Close()
        {
            this.CurrentContainer = null;
            this.SourceInventoryController = null;
            this.PlayerInventoryController = null;

            if (this.EntryListSubPanelController != null)
            {
                this.EntryListSubPanelController.Clear();
            }

            this.gameObject.SetActive(false);
        }

        private void CacheReferences()
        {
            if (this.EntryListSubPanelController == null) _entryListSubPanelController = this.GetComponentInChildren<ItemContainerPanelItemEntryListSubPanelController>(true);
        }
    }
}
