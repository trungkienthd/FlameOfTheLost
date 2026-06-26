using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class MeleeHitboxController : HitboxController
    {
        // --- States ---
        //  1. References
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

        //  2. Default hitbox shape
        [Header("Shapes")]
        [SerializeField] private MeleeHitboxShapeEntry _defaultMeleeHitbox;
        [SerializeField] private MeleeHitboxShapeEntry _overrideHitbox;
        [SerializeField] private bool _hasOverrideMeleeHitboxShape = false;
        private MeleeHitboxShapeEntry DefaultMeleeHitbox => _defaultMeleeHitbox;
        private MeleeHitboxShapeEntry OverrideHitbox
        {
            get => _overrideHitbox;
            set => _overrideHitbox = value;
        }
        private MeleeHitboxShapeEntry ActiveMeleeHitbox => this.HasOverrideMeleeHitboxShape ? this.OverrideHitbox : this.DefaultMeleeHitbox;
        private bool HasOverrideMeleeHitboxShape
        {
            get => _hasOverrideMeleeHitboxShape;
            set => _hasOverrideMeleeHitboxShape = value;
        }

        //  3. Facing state
        [Header("Facing Sync")]
        [SerializeField] private bool _autoSyncFromFacingProvider = true;
        private IFacingProvider _facingProvider; // usually the AnimationHandler
        private bool AutoSyncFromFacingProvider => _autoSyncFromFacingProvider;
        private IFacingProvider FacingProvider
        {
            get => _facingProvider;
            set => _facingProvider = value;
        }

        //  4. Other states
        protected override bool AllowMultipleTargetsPerEnableWindow => true; // Melee hitboxes always allow multiple targets

        // --- Public APIs ---
        //      1.1. Enable the hitbox
        public override void EnableHitbox(HitboxActivationContext payload, AttackSide attackSideType)
        {
            base.EnableHitbox(payload, attackSideType);

            if (this.PolygonCollider2D) this.PolygonCollider2D.enabled = true;
            if ((this.ActiveRuntimeSkill != null) && (this.ActiveRuntimeSkill.Profile is MeleeSkillProfile) && (this.ActiveRuntimeSkill.TryGetMeleeHitboxShape(attackSideType, out var meleeHitboxShape)))
            {
                this.SetOverrideMeleeHitboxShape(meleeHitboxShape);   // Apply the melee hitbox shape
            }
            else
            {
                this.ClearOverrideMeleeHitboxShape();
                this.SyncPolygonColliderShapeFromFacing();
            }

            if (this.PolygonCollider2D != null)
            {
                this.PolygonCollider2D.enabled = true;
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
        public void SetOverrideMeleeHitboxShape(MeleeHitboxShapeEntry meleeHitbox)
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

        // --- Helpers ---
        //  1. Sync the PolygonCollider2D shape from the current facing direction
        private void SyncPolygonColliderShapeFromFacing()
        {
            if ((this.PolygonCollider2D == null) || (this.FacingProvider == null)) return;

            CharacterFacingStatus facing = this.FacingProvider != null ? this.FacingProvider.FacingStatus : CharacterFacingStatus.Side;

            PolygonColliderShapeEntry shape = facing switch
            {
                CharacterFacingStatus.Down => this.ActiveMeleeHitbox.FacingDownShape,
                CharacterFacingStatus.Side => this.ActiveMeleeHitbox.FacingSideShape,
                _ => this.ActiveMeleeHitbox.FacingUpShape,
            };

            // Copy points so we can mutate safely
            List<Vector2> pts = shape.Points != null ? new List<Vector2>(shape.Points) : new List<Vector2>();
            if (pts.Count < 3) return;

            // NOTE:
            // Left/right (during side-facing) mirroring is handled entirely by FlipGroup transform scaling, since this hitbox is a child of FlipGroup.

            this.PolygonCollider2D.pathCount = 1;
            this.PolygonCollider2D.SetPath(0, pts.ToArray());
            this.PolygonCollider2D.offset = shape.Offset;
        }

        //  2. FOTL Base Behaviour Overrides
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PolygonCollider2D == null) this.PolygonCollider2D = this.GetComponent<PolygonCollider2D>();

            if (this.Damager == null) this.Damager = this.GetComponent<IDamager>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.PolygonCollider2D == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(MeleeHitboxController)}] {nameof(PolygonCollider2D)} reference is missing.", this);
            }

            if (this.Damager == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(MeleeHitboxController)}] Missing {nameof(IDamager)} on the same GameObject.", this);
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.HasOverrideMeleeHitboxShape = false;

            if (this.PolygonCollider2D != null)
            {
                this.PolygonCollider2D.enabled = false;
                this.PolygonCollider2D.isTrigger = true;
            }
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (!this.AutoSyncFromFacingProvider) return;

            this.FacingProvider = this.GetComponentInParent<IFacingProvider>();
            if (this.FacingProvider == null)
            {
                Transform root = this.transform.root;
                this.FacingProvider = root.GetComponentInChildren<IFacingProvider>(true);
            }
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.AutoSyncFromFacingProvider && this.FacingProvider == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(MeleeHitboxController)}] AutoSyncFromFacingProvider is enabled, but no {nameof(IFacingProvider)} was found under root.", this);
            }
        }
    }
}
