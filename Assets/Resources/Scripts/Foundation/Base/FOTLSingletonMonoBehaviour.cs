using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class FOTLSingletonMonoBehaviour<T> : FOTLMonoBehaviour where T : FOTLSingletonMonoBehaviour<T>
    {
        // --- Singleton ---
        public static T Instance { get; private set; }

        // --- Configs ---
        protected virtual bool PersistAcrossScenes => false;

        // --- Helpers ---
        //  1. Singleton Initialization
        protected override bool InitializeBehaviour()
        {
            if (!base.InitializeBehaviour()) return false;

            T currentInstance = this as T;

            if (currentInstance == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] Invalid Singleton type configuration: {typeof(T).Name}.", this);
                return false;
            }

            // Reject duplicate instances.
            if (FOTLSingletonMonoBehaviour<T>.Instance != null && Instance != currentInstance)
            {
                UnityEngine.Object.Destroy(this.gameObject);
                return false;
            }

            // Register the authoritative instance.
            FOTLSingletonMonoBehaviour<T>.Instance = currentInstance;

            // Preserve this GameObject and its children when requested.
            if (this.PersistAcrossScenes)
            {
                UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
            }

            return true;
        }

        //  2. Singleton Cleanup
        protected override void CleanupStates()
        {
            T currentInstance = this as T;

            if (FOTLSingletonMonoBehaviour<T>.Instance == currentInstance)
            {
                FOTLSingletonMonoBehaviour<T>.Instance = null;
            }

            base.CleanupStates();
        }
    }
}