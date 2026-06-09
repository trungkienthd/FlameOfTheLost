using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Collider2D))]
    public class CharacterAITargetDetectionTriggerController : CharacterBehaviourControllerBase
    {
        private CharacterAITargetDetectionHandler TargetDetectionHandler
        {
            get
            {
                if (this.CharacterRootController is EnemyRootController enemyRoot)
                {
                    return enemyRoot.TargetDetectionHandler;
                }
                return null;
            }
        }

        [Header("Detection")]
        [SerializeField] private CharacterAITargetDetectionRangeType _rangeType;
        private CharacterAITargetDetectionRangeType RangeType => _rangeType;

        [Header("Collider (Trigger)")]
        [SerializeField] private Collider2D _triggerCollider;
        private Collider2D TriggerCollider => _triggerCollider;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (this.TargetDetectionHandler == null) return;

            CharacterRootController target = other.GetComponentInParent<CharacterRootController>();
            if (target == null) return;

            this.TargetDetectionHandler.RegisterTarget(target, this.RangeType);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (this.TargetDetectionHandler == null) return;

            CharacterRootController target = other.GetComponentInParent<CharacterRootController>();
            if (target == null) return;

            this.TargetDetectionHandler.UnregisterTarget(target, this.RangeType);
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.TriggerCollider == null) _triggerCollider = this.GetComponent<Collider2D>();
            this.TriggerCollider.isTrigger = true;
        }
    }
}
