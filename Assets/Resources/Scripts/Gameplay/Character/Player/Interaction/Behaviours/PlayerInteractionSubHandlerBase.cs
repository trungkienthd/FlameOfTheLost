using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class PlayerInteractionSubHandlerBase : FOTLMonoBehaviour
    {
        // --- References ---
        //  1. Brain Controller
        private PlayerInteractionHandler _interactionHandler;
        protected PlayerInteractionHandler InteractionHandler
        {
            get => _interactionHandler;
            private set => _interactionHandler = value;
        }

        // --- Runtime ---
        public bool IsBound => this.InteractionHandler != null;

        // --- Public APIs ---
        //  1. Brain Binding
        public void Bind(PlayerInteractionHandler interactionHandler)
        {
            if (interactionHandler == null)
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}.{this.GetType().Name}] " +
                    $"Cannot bind a null {nameof(PlayerInteractionHandler)}.",
                    this
                );

                return;
            }

            if (this.InteractionHandler == interactionHandler) return;

            if (this.InteractionHandler != null)
            {
                PlayerInteractionHandler previousHandler = this.InteractionHandler;

                this.OnUnbinding(previousHandler);
                this.InteractionHandler = null;
                this.OnUnbound(previousHandler);
            }

            this.InteractionHandler = interactionHandler;
            this.OnBound(interactionHandler);
        }
        public void Unbind(PlayerInteractionHandler interactionHandler)
        {
            if (interactionHandler == null) return;
            if (this.InteractionHandler != interactionHandler) return;

            this.OnUnbinding(interactionHandler);
            this.InteractionHandler = null;
            this.OnUnbound(interactionHandler);
        }

        // --- Helpers ---
        protected bool EnsureBound()
        {
            if (this.IsBound) return true;

            Debug.LogError
            (
                $"[{this.gameObject.name}.{this.GetType().Name}] " +
                $"Handler is not bound to a {nameof(PlayerInteractionHandler)}.",
                this
            );

            return false;
        }

        // --- Binding Hooks ---
        protected virtual void OnBound(PlayerInteractionHandler interactionHandler)
        {
        }
        protected virtual void OnUnbinding(PlayerInteractionHandler interactionHandler)
        {
        }
        protected virtual void OnUnbound(PlayerInteractionHandler interactionHandler)
        {
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void UnbindInternalReferences()
        {
            if (this.InteractionHandler != null)
            {
                PlayerInteractionHandler interactionHandler = this.InteractionHandler;

                this.OnUnbinding(interactionHandler);
                this.InteractionHandler = null;
                this.OnUnbound(interactionHandler);
            }

            base.UnbindInternalReferences();
        }
    }
}
