using UnityEngine;

namespace FlameOfTheLost
{
    public class ItemContainerPanelSelectionHandler : ItemContainerPanelSubHandlerBase
    {
        // --- Runtime ---
        internal InventoryItemEntry CurrentSelectedEntry { get; private set; }

        // --- Internal APIs ---
        internal void Select(InventoryItemEntry entry)
        {
            if (!this.EnsureBound()) return;

            if (entry == null)
            {
                this.Clear();
                return;
            }

            if (!this.ItemContainerPanelController.ContainsEntry(entry))
            {
                this.Clear();
                return;
            }

            this.CurrentSelectedEntry = entry;

            this.ItemContainerPanelController.RefreshSelectionVisuals();
        }

        internal void Clear()
        {
            this.CurrentSelectedEntry = null;

            if (!this.IsBound) return;

            this.ItemContainerPanelController.RefreshSelectionVisuals();
        }

        internal bool ValidateSelection()
        {
            if (!this.EnsureBound()) return false;

            if (this.CurrentSelectedEntry == null)
            {
                this.ItemContainerPanelController.RefreshSelectionVisuals();
                return false;
            }

            if (!this.ItemContainerPanelController.ContainsEntry(this.CurrentSelectedEntry))
            {
                this.Clear();
                return false;
            }

            this.ItemContainerPanelController.RefreshSelectionVisuals();

            return true;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.CurrentSelectedEntry = null;
        }
        protected override void OnUnbinding(ItemContainerPanelController itemContainerPanelController)
        {
            this.CurrentSelectedEntry = null;

            base.OnUnbinding(itemContainerPanelController);
        }
    }
}
