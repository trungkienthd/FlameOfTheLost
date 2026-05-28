using UnityEngine;

namespace FlameOfTheLost
{
    public interface ICombatAimingProvider
    {
        public bool TryGetAimDirection(Transform ownerTransform, out Vector2 aimDirection);
    }
}
