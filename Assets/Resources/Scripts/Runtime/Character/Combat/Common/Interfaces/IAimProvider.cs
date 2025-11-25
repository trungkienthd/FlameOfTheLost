using UnityEngine;

namespace FlameOfTheLost
{
    public interface IAimProvider
    {
        public bool TryGetAimDirection(Transform ownerTransform, out Vector2 aimDirection);
    }
}
