using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class AimingHandler : ScriptableObject
    {
        public abstract bool TryGetAimDirection(Transform ownerTransform, out Vector2 aimDirection);
    }
}
