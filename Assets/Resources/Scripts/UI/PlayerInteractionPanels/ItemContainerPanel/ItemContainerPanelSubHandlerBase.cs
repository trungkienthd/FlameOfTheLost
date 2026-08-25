using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class ItemContainerPanelSubHandlerBase : FOTLMonoBehaviour
    {
        // --- References ---
        //  1. Brain Controller
        private ItemContainerPanelController _itemContainerPanelController;
        protected ItemContainerPanelController ItemContainerPanelController
        {
            get => _itemContainerPanelController;
            private set => _itemContainerPanelController = value;
        }

        // --- Runtime ---
        public bool IsBound => this.ItemContainerPanelController != null;

        // --- Public APIs ---
        //  1. Brain Binding
        public void Bind(ItemContainerPanelController itemContainerPanelController)
        {
            if (itemContainerPanelController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] " + $"Cannot bind a null {nameof(ItemContainerPanelController)}.", this);
                return;
            }

            if (this.ItemContainerPanelController == itemContainerPanelController) return;

            if (this.ItemContainerPanelController != null)
            {
                ItemContainerPanelController previousController = this.ItemContainerPanelController;

                this.OnUnbinding(previousController);
                this.ItemContainerPanelController = null;
                this.OnUnbound(previousController);
            }

            this.ItemContainerPanelController = itemContainerPanelController;
            this.OnBound(itemContainerPanelController);
        }
        public void Unbind(ItemContainerPanelController itemContainerPanelController)
        {
            if (itemContainerPanelController == null) return;
            if (this.ItemContainerPanelController != itemContainerPanelController) return;

            this.OnUnbinding(itemContainerPanelController);
            this.ItemContainerPanelController = null;
            this.OnUnbound(itemContainerPanelController);
        }

        // --- Helpers ---
        protected bool EnsureBound()
        {
            if (this.IsBound) return true;

            Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] " + $"Sub-behaviour is not bound to a {nameof(FlameOfTheLost.ItemContainerPanelController)}.", this);

            return false;
        }

        // --- Binding Hooks ---
        protected virtual void OnBound(ItemContainerPanelController itemContainerPanelController)
        {
        }
        protected virtual void OnUnbinding(ItemContainerPanelController itemContainerPanelController)
        {
        }
        protected virtual void OnUnbound(ItemContainerPanelController itemContainerPanelController)
        {
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void UnbindInternalReferences()
        {
            if (this.ItemContainerPanelController != null)
            {
                ItemContainerPanelController itemContainerPanelController = this.ItemContainerPanelController;

                this.OnUnbinding(itemContainerPanelController);
                this.ItemContainerPanelController = null;
                this.OnUnbound(itemContainerPanelController);
            }

            base.UnbindInternalReferences();
        }
    }
}
