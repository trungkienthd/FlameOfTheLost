using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class FOTLMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            this.CacheInternalReferences();
            this.ValidateInternalReferences();
            this.BindInternalReferences();
            this.InitializeInternalState();
        }

        protected virtual void Start()
        {
            this.CacheExternalReferences();
            this.ValidateExternalReferences();
            this.BindExternalReferences();
            this.InitializePregameplayState();
            this.InitializeGameplay();
        }

        private void OnDestroy()
        {
            this.UnbindInternalReferences();
        }

        protected virtual void CacheInternalReferences()
        {
        }

        protected virtual void ValidateInternalReferences()
        {
        }

        protected virtual void BindInternalReferences()
        {
        }

        protected virtual void InitializeInternalState()
        {
        }

        protected virtual void CacheExternalReferences()
        {
        }

        protected virtual void ValidateExternalReferences()
        {
        }

        protected virtual void BindExternalReferences()
        {
        }

        protected virtual void InitializePregameplayState()
        {
        }

        protected virtual void InitializeGameplay()
        {
        }

        protected virtual void UnbindInternalReferences()
        {
        }
    }
}
