using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerInteractionTargetHandler : FOTLMonoBehaviour
    {
        // --- References ---
        private PlayerInteractionHandler _interactionHandler; 
        private PlayerInteractionHandler InteractionHandler => _interactionHandler;

        // --- States ---
        private readonly Dictionary<MonoBehaviour, int> _interactableDetectionCounts = new(); 
        private readonly Dictionary<MonoBehaviour, int> _promptSourceDetectionCounts = new(); 
        private readonly List<MonoBehaviour> _cleanupBuffer = new(); 
        private IReadOnlyDictionary<MonoBehaviour, int> InteractableDetectionCounts => _interactableDetectionCounts;
        private IReadOnlyDictionary<MonoBehaviour, int> PromptSourceDetectionCounts => _promptSourceDetectionCounts;
        private IReadOnlyList<MonoBehaviour> CleanupBuffer => _cleanupBuffer;
        public int NearbyInteractableCount => this.InteractableDetectionCounts.Count; 
        public int NearbyPromptSourceCount => this.PromptSourceDetectionCounts.Count;

        // --- Events ---
        public event Action PromptRefreshRequested;

        // --- Public APIs ---
        //  0. Initialization
        public void Initialize(PlayerInteractionHandler interactionHandler)
        {
            _interactionHandler = interactionHandler;
        }

        //  1. Interactable/Prompt Source Registration
        public void RegisterNearbyCollider(Collider2D other)
        {
            if (other == null || !other.isTrigger) return;

            this.RegisterInteractable(other);
            this.RegisterPromptSource(other);
        }
        public void UnregisterNearbyCollider(Collider2D other)
        {
            if (other == null || !other.isTrigger) return;

            this.UnregisterInteractable(other);
            this.UnregisterPromptSource(other);
        }

        //  2. (Nearest) Interactable Getters
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

            foreach (MonoBehaviour behaviour in this.InteractableDetectionCounts.Keys)
            {
                if (!this.IsValidBehaviour(behaviour)) continue;

                if (behaviour is not IPlayerInteractable interactable) continue;
                if (!interactable.CanInteract(interactor)) continue;

                float sqrDistance = (behaviour.transform.position - origin).sqrMagnitude;

                if (sqrDistance >= nearestSqrDistance) continue;

                nearestSqrDistance = sqrDistance;
                nearestInteractable = interactable;
            }

            return nearestInteractable;
        }

        //  3. (Best) Prompt Source Getters
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

            foreach (MonoBehaviour behaviour in this.PromptSourceDetectionCounts.Keys)
            {
                if (!this.IsValidBehaviour(behaviour)) continue;
                if (behaviour is not IPlayerInteractionPromptSource promptSource) continue;
                if (!promptSource.CanShowInteractionPrompt(interactor)) continue;

                int priority = promptSource.InteractionPromptPriority;

                float sqrDistance = (behaviour.transform.position - origin).sqrMagnitude;

                bool isHigherPriority = priority > bestPriority;

                bool isSamePriorityButCloser = (priority == bestPriority) && (sqrDistance < bestSqrDistance);

                if (!isHigherPriority && !isSamePriorityButCloser) continue;

                bestPriority = priority;
                bestSqrDistance = sqrDistance;
                bestPromptSource = promptSource;
            }

            return bestPromptSource;
        }

        //  4. Target Validation
        public void RemoveInvalidTargets()
        {
            this.RemoveInvalidInteractables();
            this.RemoveInvalidPromptSources();
        }

        //  5. Cleanup
        public void ClearNearbyTargets()
        {
            this.UnbindAllPromptRefreshNotifiers();

            _interactableDetectionCounts.Clear();
            _promptSourceDetectionCounts.Clear();
            _cleanupBuffer.Clear();
        }

        // --- Helpers ---
        //  1. Interactable Registration
        private void RegisterInteractable(Collider2D other)
        {
            IPlayerInteractable interactable = other.GetComponentInParent<IPlayerInteractable>();

            if (interactable is not MonoBehaviour behaviour) return;

            this.IncrementDetectionCount(_interactableDetectionCounts, behaviour);
        }
        private void UnregisterInteractable(Collider2D other)
        {
            IPlayerInteractable interactable = other.GetComponentInParent<IPlayerInteractable>();

            if (interactable is not MonoBehaviour behaviour) return;

            this.DecrementDetectionCount(_interactableDetectionCounts, behaviour);
        }

        //  2. Prompt Source Registration
        private void RegisterPromptSource(Collider2D other)
        {
            IPlayerInteractionPromptSource promptSource = other.GetComponentInParent<IPlayerInteractionPromptSource>();

            if (promptSource is not MonoBehaviour behaviour) return;

            bool wasRegistered = this.PromptSourceDetectionCounts.ContainsKey(behaviour);

            this.IncrementDetectionCount(_promptSourceDetectionCounts, behaviour);

            if (wasRegistered) return;

            if (promptSource is IPlayerInteractionPromptRefreshNotifier notifier)
            {
                notifier.PromptRefreshRequested += this.HandlePromptRefreshRequested;
            }
        }
        private void UnregisterPromptSource(Collider2D other)
        {
            IPlayerInteractionPromptSource promptSource = other.GetComponentInParent<IPlayerInteractionPromptSource>();

            if (promptSource is not MonoBehaviour behaviour) return;

            bool wasCompletelyRemoved = this.DecrementDetectionCount(_promptSourceDetectionCounts, behaviour);

            if (!wasCompletelyRemoved) return;

            if (promptSource is IPlayerInteractionPromptRefreshNotifier notifier)
            {
                notifier.PromptRefreshRequested -= this.HandlePromptRefreshRequested;
            }
        }

        //  3. Detection Counting
        private void IncrementDetectionCount(Dictionary<MonoBehaviour, int> detectionCounts, MonoBehaviour behaviour)
        {
            if (behaviour == null) return;

            if (detectionCounts.TryGetValue(behaviour, out int currentCount))
            {
                detectionCounts[behaviour] = currentCount + 1;

                return;
            }

            detectionCounts.Add(behaviour, 1);
        }
        private bool DecrementDetectionCount(Dictionary<MonoBehaviour, int> detectionCounts, MonoBehaviour behaviour)
        {
            if (behaviour == null) return false;

            if (!detectionCounts.TryGetValue(behaviour, out int currentCount))
            {
                return false;
            }

            if (currentCount > 1) 
            {
                detectionCounts[behaviour] = currentCount - 1; 
                return false; 
            }

            detectionCounts.Remove(behaviour);
            return true;
        }

        //  4. Cleanup
        private void RemoveInvalidInteractables()
        {
            _cleanupBuffer.Clear();

            foreach (MonoBehaviour behaviour in this.InteractableDetectionCounts.Keys)
            {
                if (!this.IsValidBehaviour(behaviour) || behaviour is not IPlayerInteractable)
                {
                    _cleanupBuffer.Add(behaviour);
                }
            }

            for (int i = 0; i < _cleanupBuffer.Count; i++)
            {
                _interactableDetectionCounts.Remove(_cleanupBuffer[i]);
            }
        }

        private void RemoveInvalidPromptSources()
        {
            _cleanupBuffer.Clear();

            foreach (MonoBehaviour behaviour in this.PromptSourceDetectionCounts.Keys)
            {
                if (!this.IsValidBehaviour(behaviour) || behaviour is not IPlayerInteractionPromptSource)
                {
                    _cleanupBuffer.Add(behaviour);
                }
            }

            for (int i = 0; i < this.CleanupBuffer.Count; i++)
            {
                MonoBehaviour behaviour = this.CleanupBuffer[i];

                if (behaviour != null && behaviour is IPlayerInteractionPromptRefreshNotifier notifier)
                {
                    notifier.PromptRefreshRequested -= this.HandlePromptRefreshRequested;
                }

                _promptSourceDetectionCounts.Remove(behaviour);
            }

            _cleanupBuffer.Clear();
        }

        private void UnbindAllPromptRefreshNotifiers()
        {
            foreach (MonoBehaviour behaviour in this.PromptSourceDetectionCounts.Keys)
            {
                if (behaviour == null) continue;

                if (behaviour is IPlayerInteractionPromptRefreshNotifier notifier)
                {
                    notifier.PromptRefreshRequested -= this.HandlePromptRefreshRequested;
                }
            }
        }

        private bool IsValidBehaviour(MonoBehaviour behaviour)
        {
            return behaviour != null && behaviour.gameObject.activeInHierarchy;
        }

        private Vector3 ResolveOrigin(GameObject interactor)
        {
            return interactor != null ? interactor.transform.position : this.transform.position;
        }

        //  5. Event Handling
        private void HandlePromptRefreshRequested()
        {
            this.PromptRefreshRequested?.Invoke();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.InteractionHandler == null) _interactionHandler = this.GetComponent<PlayerInteractionHandler>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.InteractionHandler == null) Debug.LogError($"[{nameof(PlayerInteractionTargetHandler)}] No {nameof(PlayerInteractionHandler)} reference was found on the same GameObject as this component. This component will not function correctly without a valid reference.", this);
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _interactableDetectionCounts.Clear();
            _promptSourceDetectionCounts.Clear();
            _cleanupBuffer.Clear();
        }
        protected override void CleanupStates()
        {
            this.ClearNearbyTargets();

            base.CleanupStates();
        }
    }
}