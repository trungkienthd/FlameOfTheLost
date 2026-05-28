using UnityEngine;

namespace FlameOfTheLost
{
    public class FixedDirectionAimingHandler : MonoBehaviour, ICombatAimingProvider
    {
        [SerializeField] private Vector2 _fixedAimDirection = Vector2.right;
        public Vector2 FixedAimDirection
        {
            get => _fixedAimDirection;
            private set => _fixedAimDirection = value;
        }

        public bool TryGetAimDirection(Transform ownerTransform, out Vector2 aimDirection)
        {
            aimDirection = this.FixedAimDirection.sqrMagnitude > Mathf.Epsilon ? this.FixedAimDirection.normalized : Vector2.right;
            return true;
        }
    }
}
