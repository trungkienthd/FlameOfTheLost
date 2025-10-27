using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class MeleeHitboxController : MonoBehaviour
    {
        [Header("Filter")]
        [SerializeField] private LayerMask _targets;
        public LayerMask Targets => _targets;

        [Header("Owner / Runtime")]
        [SerializeField] private PolygonCollider2D _polygonCollider2D;
        [SerializeField] private Transform _ownerTransform;
        [SerializeField] private bool _isActive = true;
        public PolygonCollider2D PolygonCollider2D
        {
            get => _polygonCollider2D;
            private set => _polygonCollider2D = value;
        }
        private Transform OwnerTransform
        {
            get => _ownerTransform;
            set => _ownerTransform = value;
        }
        private bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        [Header("Damager")]
        private IDamager _damager;
        public IDamager Damager
        {
            get => _damager;
            private set => _damager = value;
        }
        private IHitFilter HitFilter => this.Damager as IHitFilter;
        private IHitPostProcessor HitPostProcessor => this.Damager as IHitPostProcessor;

        [Header("Shapes")]
        [SerializeField] private MeleeHitboxProfile _defaultMeleeHitboxProfile;
        [SerializeField] private MeleeHitboxProfile _overrideHitboxProfile;
        [SerializeField] private bool _hasOverrideMeleeHitboxProfile = true;
        [SerializeField] private bool _autoSyncFromFacingProvider = true;
        private IFacingProvider _facingProvider; // usually the AnimationHandler
        private MeleeHitboxProfile DefaultMeleeHitboxProfile => _defaultMeleeHitboxProfile;
        private MeleeHitboxProfile OverrideHitboxProfile
        {
            get => _overrideHitboxProfile;
            set => _overrideHitboxProfile = value;
        }
        private MeleeHitboxProfile ActiveMeleeHitboxProfile => this.HasOverrideMeleeHitboxProfile ? this.OverrideHitboxProfile : this.DefaultMeleeHitboxProfile;
        private bool HasOverrideMeleeHitboxProfile
        { 
            get => _hasOverrideMeleeHitboxProfile;
            set => _hasOverrideMeleeHitboxProfile = value;
        }
        private bool AutoSyncFromFacingProvider => _autoSyncFromFacingProvider;
        private IFacingProvider FacingProvider
        {
            get => _facingProvider;
            set => _facingProvider = value;
        }

        [Header("Status Effects (runtime)")]
        [SerializeField] private List<StatusEffectApplication> _activeStatusEffects;
        private Dictionary<int, int> _perStatusEffectAppliedCounts;
        private GameObject _statusEffectSource;

        public List<StatusEffectApplication> ActiveStatusEffectApplications
        {
            get => _activeStatusEffects;
            private set => _activeStatusEffects = value;
        }
        public Dictionary<int, int> PerStatusEffectAppliedCounts
        {
            get => _perStatusEffectAppliedCounts;
            private set => _perStatusEffectAppliedCounts = value;
        }
        public GameObject StatusEffectSource
        {
            get => _statusEffectSource;
            private set => _statusEffectSource = value;
        }

        private void Awake()
        {
            // Setup Collider
            this.PolygonCollider2D = this.GetComponent<PolygonCollider2D>();
            this.PolygonCollider2D.isTrigger = true;

            // Default owner to parent (common setup for weapon/hitbox objects)
            this.OwnerTransform = this.transform.parent != null ? this.transform.parent : this.transform;

            // Require an IDamager on the same object (DamageDealingHandler implements it)
            this.Damager = this.GetComponent<IDamager>();
            if (this.Damager == null)
            {
                Debug.LogError("[MeleeHitboxController] Missing IDamager (e.g., DamageDealingHandler) on the same GameObject.", this);
            }

            this.EnableHitbox(false);
        }

        private void Start()
        {
            if (!this.AutoSyncFromFacingProvider) return;

            // 1) Look upward first
            this.FacingProvider = this.GetComponentInParent<IFacingProvider>();
            if (this.FacingProvider != null) return;

            // 2) Fallback: search from the root (Player) across ALL children (include inactive)
            var root = this.transform.root; // 'Player'
            this.FacingProvider = root.GetComponentInChildren<IFacingProvider>();

            if (this.FacingProvider == null)
            {
                Debug.LogWarning("[MeleeHitboxHandler] AutoSyncFromFacingProvider enabled but no IFacingProvider found under root.", this);
            }
        }

        public void EnableHitbox(bool enable)
        {
            this.IsActive = enable;
            if (this.PolygonCollider2D) this.PolygonCollider2D.enabled = enable;
        }

        public void SetOverrideProfile(MeleeHitboxProfile meleeHitboxProfile)
        {
            this.OverrideHitboxProfile = meleeHitboxProfile;
            this.HasOverrideMeleeHitboxProfile = true;
        }
        public void ClearOverrideProfile()
        {
            this.HasOverrideMeleeHitboxProfile = false;
        }

        public void SyncPolygonColliderShapeFromFacing()
        {
            if ((!this.PolygonCollider2D) || (this.FacingProvider == null)) return;

            FacingStatus facing = this.FacingProvider != null ? this.FacingProvider.FacingStatus : FacingStatus.Side;
            bool isFacingRight = this.FacingProvider != null ? this.FacingProvider.IsFacingRight : true;

            PolygonColliderShape shape = facing switch
            {
                FacingStatus.Down => this.ActiveMeleeHitboxProfile.FacingDownShape,
                FacingStatus.Side => this.ActiveMeleeHitboxProfile.FacingSideShape,
                _ => this.ActiveMeleeHitboxProfile.FacingUpShape,
            };

            // Copy points so we can mutate safely
            var pts = shape.Points != null ? new List<Vector2>(shape.Points) : new List<Vector2>();
            var offset = shape.Offset;

            // Detect transform-based mirroring (FlipGroup scale.x = -1)
            bool mirroredByTransform = transform.lossyScale.x < 0f;

            if ((facing == FacingStatus.Side) && (this.ActiveMeleeHitboxProfile.MirrorSideOffsetX) && (!isFacingRight) &&(!mirroredByTransform))
            {
                // Mirror the silhouette
                for (int i = 0; i < pts.Count; i++)
                {
                    var p = pts[i];     // copy the struct
                    p.x = -p.x;         // mutate the copy
                    pts[i] = p;         // assign back
                }

                // Mirror the forward push
                offset.x = -offset.x;
            }

            this.PolygonCollider2D.pathCount = 1;
            this.PolygonCollider2D.SetPath(0, pts.ToArray());
            this.PolygonCollider2D.offset = offset;
        }

        public void BeginStatusEffectWindow(IReadOnlyList<StatusEffectApplication> statusEffectApplications, GameObject source)
        {
            this.StatusEffectSource = source;
            this.ActiveStatusEffectApplications = (statusEffectApplications != null && statusEffectApplications.Count > 0) ? new List<StatusEffectApplication>(statusEffectApplications) : null;
            this.PerStatusEffectAppliedCounts = (this.ActiveStatusEffectApplications != null) ? new Dictionary<int, int>(this.ActiveStatusEffectApplications.Count) : null;
        }

        public void EndStatusEffectWindow()
        {
            this.ActiveStatusEffectApplications = null;
            this.PerStatusEffectAppliedCounts = null;
            this.StatusEffectSource = null;
        }

        private void OnTriggerEnter2D(Collider2D otherCollider)
        {
            if (!this.IsActive) return;

            // Filter layer
            if ((this.Targets.value & (1 << otherCollider.gameObject.layer)) == 0) return;

            // Seek IDamageable
            var damageable = otherCollider.GetComponentInChildren<IDamageable>();
            if (damageable == null) return;

            // Faction rule check
            if (this.HitFilter != null && !this.HitFilter.CanHit(damageable)) return;

            // Compute knockback direction from damager origin (or owner if needed)
            Transform origin = (this.Damager.OriginTransform != null) ? this.Damager.OriginTransform : this.OwnerTransform;
            Vector2 direction = (otherCollider.transform.position - origin.position).normalized;

            // Apply Damage
            damageable.TakeDamage(this.Damager.DamageAmount, direction, this.Damager.KnockbackForce);

            // Apply status effects (if any)
            if (this.ActiveStatusEffectApplications != null && this.ActiveStatusEffectApplications.Count > 0)
            {
                var targetStatusEffectController = otherCollider.GetComponentInParent<StatusEffectController>();
                if (targetStatusEffectController != null && this.StatusEffectSource != null)
                {
                    foreach (var application in this.ActiveStatusEffectApplications)
                    {
                        if (application == null || application.StatusEffectProfile == null) continue;
                        
                        // Only apply effects meant for melee-hit confirmation
                        if (application.Timing != StatusEffectApplicationTiming.OnMeleeHitConfirm) continue;

                        // Respect MaxTargets per effect (per window)
                        int statusEffectKey = application.GetHashCode();
                        int usedApplicationsCount = 0;
                        if (this.PerStatusEffectAppliedCounts != null) this.PerStatusEffectAppliedCounts.TryGetValue(statusEffectKey, out usedApplicationsCount);
                        if (application.MaxTargets > 0 && usedApplicationsCount >= application.MaxTargets) continue;

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

            // Post-hit rule
            this.HitPostProcessor?.OnSuccessfulHit();
        }
    }
}
