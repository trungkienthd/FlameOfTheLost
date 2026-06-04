using UnityEngine;

namespace FlameOfTheLost
{
    public class TargetCombatAimingProvider : CharacterBehaviourBaseController, ICombatAimingProvider
    {
        // --- States ---
        //  1. References
        private CharacterAITargetDetectionHandler TargetDetectionHandler
        {
            get => this.CharacterRootController is CharacterAIRootController characterAIRootController ? characterAIRootController.TargetDetectionHandler : null;
        }

        //  2. Runtime Target
        private Transform CurrentTargetTransform => this.TargetDetectionHandler != null
            ? this.TargetDetectionHandler.CurrentTargetTransform
            : null;

        public bool TryGetAimDirection(Transform ownerTransform, out Vector2 aimDirection)
        {
            aimDirection = Vector2.zero;
            if (!ownerTransform || !this.CurrentTargetTransform) return false;

            Vector2 delta = (this.CurrentTargetTransform.position - ownerTransform.position);
            if (delta.sqrMagnitude < Mathf.Epsilon) return false;

            aimDirection = delta.normalized;
            return true;
        }
    }
}
