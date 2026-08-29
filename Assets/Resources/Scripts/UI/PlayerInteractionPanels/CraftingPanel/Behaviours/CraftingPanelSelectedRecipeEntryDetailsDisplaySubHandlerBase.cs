using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CraftingPanelSelectedRecipeEntryDetailsDisplaySubHandlerBase : FOTLMonoBehaviour
    {
        // --- References ---
        //  1. Brain Controller
        private CraftingPanelSelectedRecipeEntryDetailsDisplayController _detailsDisplayController;
        protected CraftingPanelSelectedRecipeEntryDetailsDisplayController DetailsDisplayController
        {
            get => _detailsDisplayController;
            private set => _detailsDisplayController = value;
        }

        // --- Runtime ---
        public bool IsBound => this.DetailsDisplayController != null;

        // --- Public APIs ---
        //  1. Brain Binding
        public void Bind(CraftingPanelSelectedRecipeEntryDetailsDisplayController detailsDisplayController)
        {
            if (detailsDisplayController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Cannot bind a null {nameof(CraftingPanelSelectedRecipeEntryDetailsDisplayController)}.", this);
                return;
            }

            if (this.DetailsDisplayController == detailsDisplayController) return;

            if (this.DetailsDisplayController != null)
            {
                CraftingPanelSelectedRecipeEntryDetailsDisplayController previousController =
                    this.DetailsDisplayController;

                this.OnUnbinding(previousController);
                this.DetailsDisplayController = null;
                this.OnUnbound(previousController);
            }

            this.DetailsDisplayController = detailsDisplayController;

            this.OnBound(detailsDisplayController);
        }

        public void Unbind(CraftingPanelSelectedRecipeEntryDetailsDisplayController detailsDisplayController)
        {
            if (detailsDisplayController == null) return;
            if (this.DetailsDisplayController != detailsDisplayController) return;

            this.OnUnbinding(detailsDisplayController);

            this.DetailsDisplayController = null;

            this.OnUnbound(detailsDisplayController);
        }

        // --- Helpers ---
        //  1. Ensure the Brain Behaviour is bound
        protected bool EnsureBound()
        {
            if (this.IsBound) return true;

            Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Sub-behaviour is not bound to a {nameof(CraftingPanelSelectedRecipeEntryDetailsDisplayController)}.", this);
            return false;
        }
        //  2. Binding Hooks
        protected virtual void OnBound(CraftingPanelSelectedRecipeEntryDetailsDisplayController detailsDisplayController)
        {
        }
        protected virtual void OnUnbinding(CraftingPanelSelectedRecipeEntryDetailsDisplayController detailsDisplayController)
        {
        }
        protected virtual void OnUnbound(CraftingPanelSelectedRecipeEntryDetailsDisplayController detailsDisplayController)
        {
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void UnbindInternalReferences()
        {
            if (this.DetailsDisplayController != null)
            {
                CraftingPanelSelectedRecipeEntryDetailsDisplayController detailsDisplayController = this.DetailsDisplayController;

                this.OnUnbinding(detailsDisplayController);

                this.DetailsDisplayController = null;

                this.OnUnbound(detailsDisplayController);
            }

            base.UnbindInternalReferences();
        }
    }
}