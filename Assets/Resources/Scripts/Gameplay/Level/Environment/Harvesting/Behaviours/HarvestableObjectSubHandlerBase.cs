using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class HarvestableObjectSubHandlerBase : FOTLMonoBehaviour
    {
        // --- References ---
        //  1. Brain Controller
        private HarvestableObjectController _harvestableObjectController;
        protected HarvestableObjectController HarvestableObjectController
        {
            get => _harvestableObjectController;
            private set => _harvestableObjectController = value;
        }

        // --- Data / Configs ---
        protected HarvestableObjectProfile Profile => this.HarvestableObjectController?.Profile;

        // --- Runtime States ---
        public bool IsBound => this.HarvestableObjectController != null;

        // --- Public APIs ---
        //  1. Brain Binding
        public void Bind(HarvestableObjectController controller)
        {
            if (controller == null)
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}." +
                    $"{this.GetType().Name}] " +
                    $"Cannot bind a null " +
                    $"{nameof(HarvestableObjectController)}.",
                    this
                );

                return;
            }

            if (this.HarvestableObjectController == controller) return;

            if (this.HarvestableObjectController != null)
            {
                Debug.LogWarning
                (
                    $"[{this.gameObject.name}." +
                    $"{this.GetType().Name}] " +
                    $"Replacing the currently bound " +
                    $"{nameof(HarvestableObjectController)}.",
                    this
                );

                HarvestableObjectController previousController = this.HarvestableObjectController;

                this.OnUnbinding(previousController);

                this.HarvestableObjectController = null;

                this.OnUnbound(previousController);
            }

            this.HarvestableObjectController = controller;

            this.OnBound(controller);
        }

        public void Unbind(HarvestableObjectController controller)
        {
            if (controller == null) return;

            if (this.HarvestableObjectController != controller)
            {
                return;
            }

            this.OnUnbinding(controller);

            this.HarvestableObjectController = null;

            this.OnUnbound(controller);
        }

        // --- Helpers ---
        //  1. Binding
        protected bool EnsureBound()
        {
            if (this.IsBound) return true;

            Debug.LogError
            (
                $"[{this.gameObject.name}." +
                $"{this.GetType().Name}] " +
                $"Handler is not bound to a " +
                $"{nameof(HarvestableObjectController)}.",
                this
            );

            return false;
        }

        // --- Binding Hooks ---
        protected virtual void OnBound(HarvestableObjectController controller)
        {
        }

        protected virtual void OnUnbinding(HarvestableObjectController controller)
        {
        }

        protected virtual void OnUnbound(HarvestableObjectController controller)
        {
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void UnbindInternalReferences()
        {
            if (this.HarvestableObjectController != null)
            {
                HarvestableObjectController controller = this.HarvestableObjectController;

                this.OnUnbinding(controller);

                this.HarvestableObjectController = null;

                this.OnUnbound(controller);
            }

            base.UnbindInternalReferences();
        }
    }
}