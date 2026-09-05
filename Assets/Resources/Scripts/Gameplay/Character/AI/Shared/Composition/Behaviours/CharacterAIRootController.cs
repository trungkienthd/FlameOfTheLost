using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterAIRootController : CharacterRootController
    {
        // --- Data / Configs ---
        public abstract CharacterAIProfile CharacterAIProfile { get; }

        // --- References ---
        [Header("References (Character AI-only)")]
        [SerializeField] private CharacterAITargetDetectionHandler _targetDetectionHandler;
        public CharacterAITargetDetectionHandler TargetDetectionHandler => _targetDetectionHandler;

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.TargetDetectionHandler == null)
            {
                _targetDetectionHandler = this.GetComponentInChildren<CharacterAITargetDetectionHandler>(true);
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.TargetDetectionHandler == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] No {nameof(CharacterAITargetDetectionHandler)} assigned.", this);
            }
        }
    }
}
