using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CraftingPanelSubHandlerBase : FOTLMonoBehaviour
    {
        // --- References ---
        //  1. Brain Controller
        private CraftingPanelController _craftingPanelController;
        protected CraftingPanelController CraftingPanelController
        {
            get => _craftingPanelController;
            private set => _craftingPanelController = value;
        }

        // --- Runtime ---
        public bool IsBound => this.CraftingPanelController != null;

        // --- Public APIs ---
        //  1. Brain Binding
        public void Bind(CraftingPanelController craftingPanelController)
        {
            if (craftingPanelController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] " + $"Cannot bind a null {nameof(CraftingPanelController)}.", this);
                return;
            }

            if (this.CraftingPanelController == craftingPanelController) return;

            if (this.CraftingPanelController != null)
            {
                CraftingPanelController previousController = this.CraftingPanelController;

                this.OnUnbinding(previousController);
                this.CraftingPanelController = null;
                this.OnUnbound(previousController);
            }

            this.CraftingPanelController = craftingPanelController;
            this.OnBound(craftingPanelController);
        }
        public void Unbind(CraftingPanelController craftingPanelController)
        {
            if (craftingPanelController == null) return;
            if (this.CraftingPanelController != craftingPanelController) return;

            this.OnUnbinding(craftingPanelController);
            this.CraftingPanelController = null;
            this.OnUnbound(craftingPanelController);
        }

        // --- Helpers ---
        //  1. Ensure Brain Controller is bound
        protected bool EnsureBound()
        {
            if (this.IsBound) return true;

            Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] " + $"Sub-behaviour is not bound to a {nameof(FlameOfTheLost.CraftingPanelController)}.", this);
            return false;
        }
        //  2. Binding Hooks (Virtual)
        protected virtual void OnBound(CraftingPanelController craftingPanelController)
        {
        }
        protected virtual void OnUnbinding(CraftingPanelController craftingPanelController)
        {
        }
        protected virtual void OnUnbound(CraftingPanelController craftingPanelController)
        {
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void UnbindInternalReferences()
        {
            if (this.CraftingPanelController != null)
            {
                CraftingPanelController craftingPanelController = this.CraftingPanelController;

                this.OnUnbinding(craftingPanelController);
                this.CraftingPanelController = null;
                this.OnUnbound(craftingPanelController);
            }

            base.UnbindInternalReferences();
        }
    }
}
