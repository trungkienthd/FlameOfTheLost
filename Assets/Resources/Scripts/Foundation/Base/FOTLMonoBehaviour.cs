using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class FOTLMonoBehaviour : MonoBehaviour
    {
        // --- Debugging ---
        [Header("Debugging")]
        [SerializeField] private bool _enableDebugLogs = false;
        private bool EnableDebugLogs => _enableDebugLogs;

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
            this.CleanupStates();
            this.UnbindInternalReferences();
            this.UnbindExternalReferences();
        }

        // --- Helpers ---
        //  1. Lifecycle
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

        protected virtual void UnbindExternalReferences()
        {
        }

        protected virtual void CleanupStates()
        {
        }

        //  2. Debugging
        protected void LogDebug(string message)
        {
            if (!this.EnableDebugLogs) return;

            Debug.Log($"[{this.gameObject.name}.{this.GetType().Name}] {message}", this);
        }

        protected string GetObjectName(Object target)
        {
            return target != null
                ? target.name
                : "null";
        }
    }
}
