using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterConfiguredBehaviourBase<TConfig> : CharacterBehaviourControllerBase where TConfig : CharacterDataAsset
    {
        // --- Data / Configs ---
        [Header("Config")]
        [SerializeField] private TConfig _config;
        public TConfig Config => _config;

        // --- Public APIs ---
        public void CacheConfigIfNull(TConfig config)
        {
            if (this.Config != null) return;

            _config = config;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Config == null)
            {
                Debug.LogError($"[{this.CharacterName}.{this.GetType().Name}] {typeof(TConfig).Name} reference is not set.", this);
            }
        }
    }
}
