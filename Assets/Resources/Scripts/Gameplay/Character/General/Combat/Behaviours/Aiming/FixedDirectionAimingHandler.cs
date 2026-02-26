using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "FixedDirectionAimingHandler", menuName = "Data/FlameOfTheLost/Combat/Aiming/FixedDirectionAimingHandler")]
    public class FixedDirectionAimingHandler : AimingHandler
    {
        [SerializeField] private Vector2 _fixedAimDirection = Vector2.right;
        public Vector2 FixedAimDirection
        {
            get => _fixedAimDirection;
            private set => _fixedAimDirection = value;
        }

        public override bool TryGetAimDirection(Transform ownerTransform, out Vector2 aimDirection)
        {
            aimDirection = this.FixedAimDirection.sqrMagnitude > Mathf.Epsilon ? this.FixedAimDirection.normalized : Vector2.right;
            return true;
        }
    }
}
