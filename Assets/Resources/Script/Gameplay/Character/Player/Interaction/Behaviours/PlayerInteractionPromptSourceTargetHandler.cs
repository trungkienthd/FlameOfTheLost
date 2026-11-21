using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerInteractionPromptSourceTargetHandler : PlayerInteractionTargetHandlerBase
    {        
        // --- Events ---
        public event Action PromptRefreshRequested;

        // --- Public APIs ---
        //  1. (Best) Prompt Source Getters
        public IPlayerInteractionPromptSource GetBestPromptSource()
        {
            return this.GetBestPromptSource(this.InteractionHandler?.GetInteractor());
        }

        public IPlayerInteractionPromptSource GetBestPromptSource(GameObject interactor)
        {
            this.RemoveInvalidTargets();

            Vector3 origin = this.ResolveOrigin(interactor);

            IPlayerInteractionPromptSource bestPromptSource = null;
            int bestPriority = int.MinValue;
            float bestSqrDistance = float.MaxValue;

            foreach (MonoBehaviour behaviour in this.TargetDetectionCounts.Keys)
            {
                if (behaviour is not IPlayerInteractionPromptSource promptSource) continue;
                if (!promptSource.CanShowInteractionPrompt(interactor)) continue;

                int priority = promptSource.InteractionPromptPriority;
                float sqrDistance = (behaviour.transform.position - origin).sqrMagnitude;

                bool isHigherPriority = priority > bestPriority;
                bool isSamePriorityButCloser =
                    priority == bestPriority &&
                    sqrDistance < bestSqrDistance;

                if (!isHigherPriority && !isSamePriorityButCloser) continue;

                bestPriority = priority;
                bestSqrDistance = sqrDistance;
                bestPromptSource = promptSource;
            }

            return bestPromptSource;
        }

        // --- Helpers ---
        //  1. PlayerInteractionTargetHandlerBase Overrides
        protected override MonoBehaviour ResolveTarget(Collider2D other)
        {
            IPlayerInteractionPromptSource promptSource =
                other.GetComponentInParent<IPlayerInteractionPromptSource>();

            return promptSource as MonoBehaviour;
        }

        protected override bool IsValidTarget(MonoBehaviour target)
        {
            return base.IsValidTarget(target) &&
                   target is IPlayerInteractionPromptSource;
        }

        protected override void OnTargetRegistered(MonoBehaviour target)
        {
            base.OnTargetRegistered(target);

            if (target is IPlayerInteractionPromptRefreshNotifier notifier)
            {
                notifier.PromptRefreshRequested += this.HandlePromptRefreshRequested;
            }
        }

        protected override void OnTargetUnregistered(MonoBehaviour target)
        {
            if (target is IPlayerInteractionPromptRefreshNotifier notifier)
            {
                notifier.PromptRefreshRequested -= this.HandlePromptRefreshRequested;
            }

            base.OnTargetUnregistered(target);
        }

        //  2. Event Handling
        private void HandlePromptRefreshRequested()
        {
            this.PromptRefreshRequested?.Invoke();
        }
    }
}
