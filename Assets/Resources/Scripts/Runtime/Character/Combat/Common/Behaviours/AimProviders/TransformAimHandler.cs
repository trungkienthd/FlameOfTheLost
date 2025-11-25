using UnityEngine;

namespace FlameOfTheLost
{
    [DisallowMultipleComponent]
    public class TransformAimHandler : MonoBehaviour, IAimProvider
    {
        [SerializeField] private Transform _targetTransform;
        public Transform TargetTransform
        {
            get => _targetTransform;
            private set => _targetTransform = value;
        }

        public bool TryGetAimDirection(Transform ownerTransform, out Vector2 aimDirection)
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
