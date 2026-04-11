using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class MeleeHitboxController : HitboxController
    {
        // --- References
        [Header("References")]
        [SerializeField] private PolygonCollider2D _polygonCollider2D;
        private PolygonCollider2D PolygonCollider2D
        {
            get => _polygonCollider2D;
            set => _polygonCollider2D = value;
        }

        private IDamager _damager;
        public IDamager Damager
        {
            get => _damager;
            private set => _damager = value;
        }

        [Header("Shapes")]
        [SerializeField] private MeleeHitboxShape _defaultMeleeHitbox;
        [SerializeField] private MeleeHitboxShape _overrideHitbox;
        [SerializeField] private bool _hasOverrideMeleeHitboxShape = false;
        private MeleeHitboxShape DefaultMeleeHitbox => _defaultMeleeHitbox;
        private MeleeHitboxShape OverrideHitbox
        {
            get => _overrideHitbox;
            set => _overrideHitbox = value;
        }
        private MeleeHitboxShape ActiveMeleeHitbox => this.HasOverrideMeleeHitboxShape ? this.OverrideHitbox : this.DefaultMeleeHitbox;
        private bool HasOverrideMeleeHitboxShape
        {
            get => _hasOverrideMeleeHitboxShape;
            set => _hasOverrideMeleeHitboxShape = value;
        }

        // --- Facing state
        [Header("Facing Sync")]
        [SerializeField] private bool _autoSyncFromFacingProvider = true;
        private IFacingProvider _facingProvider; // usually the AnimationHandler
        private bool AutoSyncFromFacingProvider => _autoSyncFromFacingProvider;
        private IFacingProvider FacingProvider
        {
            get => _facingProvider;
            set => _facingProvider = value;
        }

        // --- Other states
        protected override bool AllowMultipleTargetsPerEnableWindow => true; // Melee hitboxes always allow multiple targets

        // --- Unity methods
        protected override void Awake()
        {
            base.Awake();

            // Setup Collider
            if (this.PolygonCollider2D == null) this.PolygonCollider2D = this.GetComponent<PolygonCollider2D>();

            // Require an IDamager on the same object (DamageDealingHandler implements it)
            this.Damager = this.GetComponent<IDamager>();
            if (this.Damager == null)
            {
                Debug.LogError("[MeleeHitboxController] Missing IDamager (e.g., DamageDealingHandler) on the same GameObject.", this);
            }

            // Setup FacingProvider (normally implemented in the CharacterAnimationHandler)
            if (!this.AutoSyncFromFacingProvider) return;

            // Look upward first
            this.FacingProvider = this.GetComponentInParent<IFacingProvider>();
            if (this.FacingProvider != null) return;

            // Fallback: search from the root (Player) across ALL children (include inactive)
            var root = this.transform.root; // 'Player'
            this.FacingProvider = root.GetComponentInChildren<IFacingProvider>(true);
            if (this.FacingProvider == null)
            {
                Debug.LogWarning("[MeleeHitboxHandler] AutoSyncFromFacingProvider enabled but no IFacingProvider found under root.", this);
            }
        }

        // --- Public APIs
        //      1.1. Enable the hitbox
        public override void EnableHitbox(HitboxDamagePayload payload, AttackSideType attackSideType)
        {
            base.EnableHitbox(payload, attackSideType);

            if (this.PolygonCollider2D) this.PolygonCollider2D.enabled = true;
            if ((this.ActiveRuntimeSkill.Profile is MeleeSkillProfile meleeSkillProfile) && (this.ActiveRuntimeSkill.TryGetMeleeHitboxShape(attackSideType, out var meleeHitboxShape)))
            {
                this.SetOverrideMeleeHitboxShape(meleeHitboxShape);   // Apply the melee hitbox shape
            }
        }

        //      1.2. Disable the hitbox
        public override void DisableHitbox()
        {
            base.DisableHitbox();

            if (this.PolygonCollider2D) this.PolygonCollider2D.enabled = false;
            this.ClearOverrideMeleeHitboxShape();
        }

        //      2.2. Set the Hitbox Shape override
        public void SetOverrideMeleeHitboxShape(MeleeHitboxShape meleeHitbox)
        {
            this.OverrideHitbox = meleeHitbox;
            this.HasOverrideMeleeHitboxShape = true;

            this.SyncPolygonColliderShapeFromFacing();
        }
        //      2.3. Clear the hitbox Shape override
        public void ClearOverrideMeleeHitboxShape()
        {
            this.HasOverrideMeleeHitboxShape = false;
        }

        // --- Helpers
        private void SyncPolygonColliderShapeFromFacing()
        {
            if ((!this.PolygonCollider2D) || (this.FacingProvider == null)) return;

            FacingStatusType facing = this.FacingProvider != null ? this.FacingProvider.FacingStatus : FacingStatusType.Side;
            bool isFacingRight = this.FacingProvider != null ? this.FacingProvider.IsFacingRight : true;

            PolygonColliderShape shape = facing switch
            {
                FacingStatusType.Down => this.ActiveMeleeHitbox.FacingDownShape,
                FacingStatusType.Side => this.ActiveMeleeHitbox.FacingSideShape,
                _ => this.ActiveMeleeHitbox.FacingUpShape,
            };

            // Copy points so we can mutate safely
            var pts = shape.Points != null ? new List<Vector2>(shape.Points) : new List<Vector2>();
            var offset = shape.Offset;

            // NOTE:
            // Left/right (during side-facing) mirroring is handled entirely by FlipGroup transform scaling, since this hitbox is a child of FlipGroup.

            this.PolygonCollider2D.pathCount = 1;
            this.PolygonCollider2D.SetPath(0, pts.ToArray());
            this.PolygonCollider2D.offset = offset;
        }
    }
}
