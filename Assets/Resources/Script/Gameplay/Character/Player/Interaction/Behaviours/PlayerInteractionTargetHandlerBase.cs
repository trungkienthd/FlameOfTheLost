using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class PlayerInteractionTargetHandlerBase : PlayerInteractionSubHandlerBase
    {
        // --- Runtime ---
        private readonly Dictionary<MonoBehaviour, int> _targetDetectionCounts = new();
        private readonly List<MonoBehaviour> _cleanupBuffer = new();
        protected IReadOnlyDictionary<MonoBehaviour, int> TargetDetectionCounts => _targetDetectionCounts;
        private IReadOnlyList<MonoBehaviour> CleanupBuffer => _cleanupBuffer;
        public int NearbyTargetCount => this.TargetDetectionCounts.Count;

        // --- Public APIs ---
        //  1. Target Registration
        public void RegisterNearbyCollider(Collider2D other)
        {
            if (other == null || !other.isTrigger) return;

            MonoBehaviour target = this.ResolveTarget(other);
            if (target == null) return;

            bool wasRegistered = this.TargetDetectionCounts.ContainsKey(target);

            this.IncrementDetectionCount(target);

            if (!wasRegistered)
            {
                this.OnTargetRegistered(target);
            }
        }
        public void UnregisterNearbyCollider(Collider2D other)
        {
            if (other == null || !other.isTrigger) return;

            MonoBehaviour target = this.ResolveTarget(other);
            if (target == null) return;

            bool wasCompletelyRemoved = this.DecrementDetectionCount(target);
            if (!wasCompletelyRemoved) return;

            this.OnTargetUnregistered(target);
        }

        //  2. Target Validation / Cleanup
        public void RemoveInvalidTargets()
        {
            _cleanupBuffer.Clear();

            foreach (MonoBehaviour target in this.TargetDetectionCounts.Keys)
            {
                if (!this.IsValidTarget(target))
                {
                    _cleanupBuffer.Add(target);
                }
            }

            for (int i = 0; i < this.CleanupBuffer.Count; i++)
            {
                MonoBehaviour target = this.CleanupBuffer[i];

                this.OnTargetUnregistered(target);
                _targetDetectionCounts.Remove(target);
            }

            _cleanupBuffer.Clear();
        }

        public void ClearNearbyTargets()
        {
            foreach (MonoBehaviour target in this.TargetDetectionCounts.Keys)
            {
                this.OnTargetUnregistered(target);
            }

            _targetDetectionCounts.Clear();
            _cleanupBuffer.Clear();
        }

        // --- Helpers ---
        //  1. Target Resolution
        protected abstract MonoBehaviour ResolveTarget(Collider2D other);
        protected virtual bool IsValidTarget(MonoBehaviour target)
        {
            return target != null && target.gameObject.activeInHierarchy;
        }
        protected virtual void OnTargetRegistered(MonoBehaviour target)
        {
        }
        protected virtual void OnTargetUnregistered(MonoBehaviour target)
        {
        }

        //  2. Detection Counting
        private void IncrementDetectionCount(MonoBehaviour target)
        {
            if (_targetDetectionCounts.TryGetValue(target, out int currentCount))
            {
                _targetDetectionCounts[target] = currentCount + 1;
                return;
            }

            _targetDetectionCounts.Add(target, 1);
        }
        private bool DecrementDetectionCount(MonoBehaviour target)
        {
            if (!_targetDetectionCounts.TryGetValue(target, out int currentCount))
            {
                return false;
            }

            if (currentCount > 1)
            {
                _targetDetectionCounts[target] = currentCount - 1;
                return false;
            }

            _targetDetectionCounts.Remove(target);
            return true;
        }

        //  3. Origin
        protected Vector3 ResolveOrigin(GameObject interactor)
        {
            return interactor != null
                ? interactor.transform.position
                : this.transform.position;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            _targetDetectionCounts.Clear();
            _cleanupBuffer.Clear();
        }

        protected override void CleanupStates()
        {
            this.ClearNearbyTargets();

            base.CleanupStates();
        }
    }
}
