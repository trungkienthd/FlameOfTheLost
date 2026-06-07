using UnityEngine;

namespace FlameOfTheLost
{
    public class ItemContainerPanelController : PlayerInteractionBasePanelController
    {
        [Header("References")]
        [SerializeField] private ItemContainerPanelItemEntryListSubPanelController _entryListSubPanelController;
        private ItemContainerPanelItemEntryListSubPanelController EntryListSubPanelController => _entryListSubPanelController;

        public ItemContainerController CurrentContainerCtrl { get; private set; }
        public InventoryController SourceInventoryController { get; private set; }
        public InventoryController PlayerInventoryController { get; private set; }

        private void Awake()
        {
            this.CacheReferences();

            // this.gameObject.SetActive(false);
        }

        public void Open(ItemContainerController sourceContainer, InventoryController sourceInventoryController, InventoryController playerInventoryController)
        {
            base.Open();

            if (sourceContainer == null) return;
            if (sourceInventoryController == null) return;
            if (playerInventoryController == null) return;

            this.CurrentContainerCtrl = sourceContainer;
            this.SourceInventoryController = sourceInventoryController;
            this.PlayerInventoryController = playerInventoryController;

            if (this.EntryListSubPanelController != null)
            {
                this.EntryListSubPanelController.Bind(sourceInventoryController, playerInventoryController);
            }
        }

        public override void Close()
        {
            this.CurrentContainerCtrl = null;
            this.SourceInventoryController = null;
            this.PlayerInventoryController = null;

            if (this.EntryListSubPanelController != null)
            {
                this.EntryListSubPanelController.Clear();
            }

            base.Close();
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.EntryListSubPanelController == null) _entryListSubPanelController = this.GetComponentInChildren<ItemContainerPanelItemEntryListSubPanelController>(true);
        }
    }
}
