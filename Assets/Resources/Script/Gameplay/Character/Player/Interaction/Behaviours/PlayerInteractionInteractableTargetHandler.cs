using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerInteractionInteractableTargetHandler : PlayerInteractionTargetHandlerBase
    {
        // --- Public APIs ---
        //  1. (Nearest) Interactable Getters
        public IPlayerInteractable GetNearestInteractable()
        {
            return this.GetNearestInteractable(this.InteractionHandler?.GetInteractor());
        }
        public IPlayerInteractable GetNearestInteractable(GameObject interactor)
        {
            this.RemoveInvalidTargets();

            Vector3 origin = this.ResolveOrigin(interactor);

            IPlayerInteractable nearestInteractable = null;
            float nearestSqrDistance = float.MaxValue;

            foreach (MonoBehaviour behaviour in this.TargetDetectionCounts.Keys)
            {
                if (behaviour == null) continue;
                if (!behaviour.isActiveAndEnabled) continue;

                if (behaviour is not IPlayerInteractable interactable) continue;
                if (!interactable.CanInteract(interactor)) continue;

                float sqrDistance = (behaviour.transform.position - origin).sqrMagnitude;
                if (sqrDistance >= nearestSqrDistance) continue;

                nearestSqrDistance = sqrDistance;
                nearestInteractable = interactable;
            }

            return nearestInteractable;
        }

        //  2. Nearby Interactable Validation
        public bool IsInteractableNearby(IPlayerInteractable interactable)
        {
            if (interactable is not MonoBehaviour behaviour) return false;

            if (behaviour == null) return false;
            if (!behaviour.isActiveAndEnabled) return false;

            return this.ContainsNearbyTarget(behaviour);
        }

        // --- Helpers ---
        //  1. PlayerInteractionTargetHandlerBase Overrides
        protected override MonoBehaviour ResolveTarget(Collider2D other)
        {
            IPlayerInteractable interactable = other.GetComponentInParent<IPlayerInteractable>();

            return interactable as MonoBehaviour;
        }
        protected override bool IsValidTarget(MonoBehaviour target)
        {
            return base.IsValidTarget(target) && target is IPlayerInteractable;
        }
    }
}