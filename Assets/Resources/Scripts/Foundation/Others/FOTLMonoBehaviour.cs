using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class FOTLMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            this.CacheInternalReferences();
            this.ValidateInternalReferences();
            this.InitializeInternalState();
        }

        protected virtual void Start()
        {
            this.CacheExternalReferences();
            this.ValidateExternalReferences();
            this.BindExternalReferences();
        }    

        protected virtual void CacheInternalReferences()
        {
        }

        protected virtual void ValidateInternalReferences()
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
    }
}
