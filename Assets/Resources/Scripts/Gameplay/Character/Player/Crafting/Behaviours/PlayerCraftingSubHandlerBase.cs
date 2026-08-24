using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerCraftingSubHandlerBase : FOTLMonoBehaviour
    {
        // --- References ---
        //  1. Brain Controller
        private PlayerCraftingController _craftingController;
        protected PlayerCraftingController CraftingController
        {
            get => _craftingController;
            private set => _craftingController = value;
        }

        // --- Runtime ---
        public bool IsBound => this.CraftingController != null;

        // --- Public APIs ---
        public void Bind(PlayerCraftingController craftingController)
        {
            if (craftingController == null)
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}.{this.GetType().Name}] " +
                    $"Cannot bind a null {nameof(PlayerCraftingController)}.",
                    this
                );

                return;
            }

            if (this.CraftingController == craftingController) return;

            if (this.CraftingController != null)
            {
                PlayerCraftingController previousController = this.CraftingController;

                this.OnUnbinding(previousController);
                this.CraftingController = null;
                this.OnUnbound(previousController);
            }

            this.CraftingController = craftingController;
            this.OnBound(craftingController);
        }

        public void Unbind(PlayerCraftingController craftingController)
        {
            if (craftingController == null) return;
            if (this.CraftingController != craftingController) return;

            this.OnUnbinding(craftingController);
            this.CraftingController = null;
            this.OnUnbound(craftingController);
        }

        // --- Helpers ---
        protected bool EnsureBound()
        {
            if (this.IsBound) return true;

            Debug.LogError
            (
                $"[{this.gameObject.name}.{this.GetType().Name}] " +
                $"Handler is not bound to a {nameof(PlayerCraftingController)}.",
                this
            );

            return false;
        }

        protected virtual void OnBound(PlayerCraftingController craftingController)
        {
        }
        protected virtual void OnUnbinding(PlayerCraftingController craftingController)
        {
        }
        protected virtual void OnUnbound(PlayerCraftingController craftingController)
        {
        }

        protected override void UnbindInternalReferences()
        {
            if (this.CraftingController != null)
            {
                PlayerCraftingController craftingController = this.CraftingController;

                this.OnUnbinding(craftingController);
                this.CraftingController = null;
                this.OnUnbound(craftingController);
            }

            base.UnbindInternalReferences();
        }
    }
}
