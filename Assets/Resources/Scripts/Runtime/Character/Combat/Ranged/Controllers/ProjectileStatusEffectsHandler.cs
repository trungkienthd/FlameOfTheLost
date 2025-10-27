using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Collider2D))]
    public class ProjectileStatusEffectsHandler : MonoBehaviour
    {
        [Header("Optional Filters")]
        [SerializeField] private LayerMask _targets = ~0;
        [SerializeField] private MonoBehaviour _hitFilterBehaviour;

        public LayerMask Targets
        {
            get => _targets;
            private set => _targets = value;
        }
        public MonoBehaviour HitFilterMonoBehaviour
        {
            get => _hitFilterBehaviour;
            private set => _hitFilterBehaviour = value;
        }

        private IReadOnlyList<StatusEffectApplication> _statusEffectApplications;
        private GameObject _source;
        private IHitFilter _hitFilter;
        private readonly HashSet<Transform> _alreadyApplied = new HashSet<Transform>();
        private Dictionary<int, int> _perStatusEffectAppliedCounts;

        public IReadOnlyList<StatusEffectApplication> StatusEffectApplications
        {
            get => _statusEffectApplications;
            private set => _statusEffectApplications = value;
        }
        public GameObject StatusEffectSource
        {
            get => _source;
            private set => _source = value;
        }
        private IHitFilter HitFilter
        {
            get => _hitFilter;
            set => _hitFilter = value;
        }
        public HashSet<Transform> AlreadyAppliedTargets => _alreadyApplied;
        public Dictionary<int, int> PerStatusEffectAppliedCounts
        {
            get => _perStatusEffectAppliedCounts;
            private set => _perStatusEffectAppliedCounts = value;
        }

        public void Initialize(GameObject source, IReadOnlyList<StatusEffectApplication> statusEffectApplications, LayerMask? targets = null, MonoBehaviour hitFilterBehaviour = null)
        {
            this.StatusEffectSource = source;
            this.StatusEffectApplications = statusEffectApplications;
            this.PerStatusEffectAppliedCounts = (this.StatusEffectApplications != null && this.StatusEffectApplications.Count > 0) ? new Dictionary<int, int>(this.StatusEffectApplications.Count) : null;

            if (targets.HasValue) this.Targets = targets.Value;
            if (hitFilterBehaviour) this.HitFilterMonoBehaviour = hitFilterBehaviour;

            if (this.HitFilterMonoBehaviour is IHitFilter hf)
            {
                this.HitFilter = hf;
            }
            else if (this.TryGetComponent(out IHitFilter hf2))
            {
                this.HitFilter = hf2;
            }

            var collider = this.GetComponent<Collider2D>();
            if (collider != null && !collider.isTrigger)
            {
                collider.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D otherCollider)
        {
            if (this.StatusEffectApplications == null || this.StatusEffectApplications.Count == 0 || !this.StatusEffectSource) return;
            if (this.Targets.value != 0 && ((this.Targets.value & (1 << otherCollider.gameObject.layer)) == 0)) return;

            var targetStatusEffectController = otherCollider.GetComponentInParent<StatusEffectController>();
            if (targetStatusEffectController == null) return;

            if (this.HitFilter != null) 
            {
                var damageable = otherCollider.GetComponentInChildren<IDamageable>();
                if (damageable == null || !this.HitFilter.CanHit(damageable)) return;
            }

            if (this.AlreadyAppliedTargets.Contains(targetStatusEffectController.transform)) return;
            this.AlreadyAppliedTargets.Add(targetStatusEffectController.transform);

            foreach (var application in this.StatusEffectApplications)
            {
                if (application == null || application.StatusEffectProfile == null) continue;
                if (application.Timing != StatusEffectApplicationTiming.OnProjectileImpact) continue;

                int statusEffectKey = application.GetHashCode();
                int usedApplicationsCount = 0;
                if (this.PerStatusEffectAppliedCounts != null && this.PerStatusEffectAppliedCounts.TryGetValue(statusEffectKey, out usedApplicationsCount) && application.MaxTargets > 0 && usedApplicationsCount >= application.MaxTargets)
                {
                    continue;
                }

                // Roll chance 
                if (Random.value > Mathf.Clamp01(application.ApplicationChance)) continue;

                // Compute final values
                float potencyScale = Mathf.Max(0f, application.PotencyScale);
                float durationInSecs = (application.DurationOverrideInSeconds > 0f) ? application.DurationOverrideInSeconds : application.StatusEffectProfile.BaseDurationInSeconds;
                int stacks = Mathf.Max(1, application.StacksToApply);

                // Apply <stacks> times; Controller enforces MaxStacks and Stacking policy
                for (int i = 0; i < stacks; i++)
                {
                    targetStatusEffectController.TryApply(application.StatusEffectProfile, this.StatusEffectSource, potencyScale, durationInSecs);
                }

                // Count 
                if (this.PerStatusEffectAppliedCounts != null) this.PerStatusEffectAppliedCounts[statusEffectKey] = usedApplicationsCount + 1;
            }
        }
    }
}
