using UnityEngine;

namespace FlameOfTheLost
{
    [DisallowMultipleComponent]
    public class MouseAimHandler : MonoBehaviour, IAimProvider
    {
        public bool TryGetAimDirection(Transform ownerTransform, out Vector2 aimDirection)
        {
            aimDirection = Vector2.zero;
            var camera = Camera.main;
            if (!camera || !ownerTransform) return false;

            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 delta = (mouseWorldPosition - ownerTransform.position);
            if (delta.sqrMagnitude < Mathf.Epsilon) return false;

            aimDirection = delta.normalized;
            return true;
        }
    }
}
