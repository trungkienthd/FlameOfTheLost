using UnityEngine;
using UnityEngine.InputSystem;

namespace FlameOfTheLost
{
    public class MouseCombatAimingHandler : CombatAimingHandler
    {
        public override bool TryGetAimDirection(Transform ownerTransform, out Vector2 aimDirection)
        {
            aimDirection = Vector2.zero;

            var camera = Camera.main;
            if (!camera || !ownerTransform) return false;
            if (Mouse.current == null) return false;

            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

            Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(
                new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Mathf.Abs(camera.transform.position.z - ownerTransform.position.z))
            );

            Vector2 delta = (Vector2)(mouseWorldPosition - ownerTransform.position);
            if (delta.sqrMagnitude < Mathf.Epsilon) return false;

            aimDirection = delta.normalized;
            return true;
        }
    }
}
