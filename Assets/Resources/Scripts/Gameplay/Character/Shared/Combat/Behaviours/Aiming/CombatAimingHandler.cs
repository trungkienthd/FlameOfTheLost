using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CombatAimingHandler : MonoBehaviour
    {
        public abstract bool TryGetAimDirection(Transform ownerTransform, out Vector2 aimDirection);
    }
}
