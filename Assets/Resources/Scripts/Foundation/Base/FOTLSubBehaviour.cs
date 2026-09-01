using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class FOTLSubBehaviour<TBrainBehaviour> : FOTLMonoBehaviour where TBrainBehaviour : FOTLMonoBehaviour
    {
        // --- References ---
        //  1. Brain Behaviour
        [Header("References (FOTLSubBehaviour) - Brian Behaviour")]
        [SerializeField] private TBrainBehaviour _brainBehaviour;
        protected TBrainBehaviour BrainBehaviour
        {
            get => _brainBehaviour;
            private set => _brainBehaviour = value;
        }

        // --- Runtime ---
        public bool IsBound => (UnityEngine.Object)this.BrainBehaviour != null;

        // --- Public APIs ---
        //  1. Brain Binding
        public void Bind(TBrainBehaviour brainBehaviour)
        {
            if ((UnityEngine.Object)brainBehaviour == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Cannot bind a null {typeof(TBrainBehaviour).Name}.", this);
                return;
            }

            if (this.BrainBehaviour == brainBehaviour) return;

            if ((UnityEngine.Object)this.BrainBehaviour != null)
            {
                TBrainBehaviour previousBrainBehaviour = this.BrainBehaviour;

                this.OnUnbinding(previousBrainBehaviour);
                this.BrainBehaviour = null;
                this.OnUnbound(previousBrainBehaviour);
            }

            this.BrainBehaviour = brainBehaviour;
            this.OnBound(brainBehaviour);
        }
        public void Unbind(TBrainBehaviour brainBehaviour)
        {
            if ((UnityEngine.Object)brainBehaviour == null) return;
            if (this.BrainBehaviour != brainBehaviour) return;

            this.UnbindCurrentBrain();
        }

        // --- Helpers ---
        //  1. Binding
        protected bool EnsureBound()
        {
            if (this.IsBound) return true;

            Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Sub-behaviour is not bound to a {typeof(TBrainBehaviour).Name}.", this);
            return false;
        }
        private void UnbindCurrentBrain()
        {
            if ((Object)this.BrainBehaviour == null) return;

            TBrainBehaviour previousBrainBehaviour = this.BrainBehaviour;

            this.OnUnbinding(previousBrainBehaviour);
            this.BrainBehaviour = null;
            this.OnUnbound(previousBrainBehaviour);
        }

        //  2. Binding Hooks
        protected virtual void OnBound(TBrainBehaviour brainBehaviour)
        {
        }
        protected virtual void OnUnbinding(TBrainBehaviour brainBehaviour)
        {
        }
        protected virtual void OnUnbound(TBrainBehaviour brainBehaviour)
        {
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void UnbindInternalReferences()
        {
            this.UnbindCurrentBrain();

            base.UnbindInternalReferences();
        }
    }
}
