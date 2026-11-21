using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Collider2D))]
    public class CharacterAITargetDetectionTriggerController : CharacterBehaviourControllerBase
    {
        // --- References ---
        [Header("References - AI Detection")]
        [SerializeField]
        private CharacterAITargetDetectionHandler _targetDetectionHandler;
        private CharacterAITargetDetectionHandler TargetDetectionHandler => _targetDetectionHandler;

        [Header("References - Collider (Trigger)")]
        [SerializeField] private Collider2D _triggerCollider;
        private Collider2D TriggerCollider => _triggerCollider;

        // --- Configs ---
        [Header("Detection")]
        [SerializeField] private CharacterAITargetDetectionRangeType _rangeType;
        private CharacterAITargetDetectionRangeType RangeType => _rangeType;

        // --- Unity Callbacks ---
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (this.TargetDetectionHandler == null) return;
            if (!this.TryGetTarget(other, out CharacterRootController target)) return;

            this.LogDebug($"Target entered detection trigger. Target: {this.GetObjectName(target)}, Range Type: {this.RangeType}.");

            this.TargetDetectionHandler.RegisterTarget(target, this.RangeType);
        }   
        private void OnTriggerExit2D(Collider2D other)
        {
            if (this.TargetDetectionHandler == null) return;
            if (!this.TryGetTarget(other, out CharacterRootController target)) return;

            this.LogDebug($"Target left detection trigger. Target: {this.GetObjectName(target)}, Range Type: {this.RangeType}.");

            this.TargetDetectionHandler.UnregisterTarget(target, this.RangeType);
        }

        // --- Helpers ---
        private bool TryGetTarget(Collider2D other, out CharacterRootController target)
        {
            target = null;

            if (other == null) return false;

            target = other.GetComponentInParent<CharacterRootController>();
            if (target == null) return false;

            if (target.MainCollider2D == null) return false;
            if (other != target.MainCollider2D) return false;

            return true;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.TriggerCollider == null)
            {
                _triggerCollider = this.GetComponent<Collider2D>();
            }

            if (this.TargetDetectionHandler == null)
            {
                _targetDetectionHandler = this.GetComponentInParent<CharacterAITargetDetectionHandler>(true);
            }

            if (this.TriggerCollider != null)
            {
                this.TriggerCollider.isTrigger = true;
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.TargetDetectionHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterAITargetDetectionTriggerController)}] {nameof(CharacterAITargetDetectionHandler)} is missing.", this);
            }

            if (this.TriggerCollider == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CharacterAITargetDetectionTriggerController)}] Trigger Collider is missing.", this);
            }
        }
    }
}
