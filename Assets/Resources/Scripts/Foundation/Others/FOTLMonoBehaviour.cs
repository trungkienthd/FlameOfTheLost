using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class FOTLMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            this.CacheReferences();
            this.ValidateReferences();
        }

        protected virtual void CacheReferences()
        {
        }

        protected virtual void ValidateReferences()
        {
        }
    }
}
