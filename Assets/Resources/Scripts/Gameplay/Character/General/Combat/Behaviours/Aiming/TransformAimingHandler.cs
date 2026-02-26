using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "TransformAimingHandler", menuName = "Data/FlameOfTheLost/Combat/Aiming/TransformAimingHandler")]
    public class TransformAimingHandler : AimingHandler
    {
        [SerializeField] private Transform _targetTransform;
        public Transform TargetTransform
        {
            get => _targetTransform;
            private set => _targetTransform = value;
        }

        public override bool TryGetAimDirection(Transform ownerTransform, out Vector2 aimDirection)
        {
            aimDirection = Vector2.zero;
            if (!ownerTransform || !this.TargetTransform) return false;

            Vector2 delta = (this.TargetTransform.position - ownerTransform.position);
            if (delta.sqrMagnitude < Mathf.Epsilon) return false;

            aimDirection = delta.normalized;
            return true;
        }
    }
}
