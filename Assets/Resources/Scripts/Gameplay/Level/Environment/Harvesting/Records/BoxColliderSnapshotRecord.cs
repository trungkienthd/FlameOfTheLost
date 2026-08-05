using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class BoxColliderSnapshotRecord : ColliderSnapshotRecord
    {
        // --- States ---
        private Vector2 Size { get; }
        private float EdgeRadius { get; }
        private bool AutoTiling { get; }

        // --- Constructors ---
        public BoxColliderSnapshotRecord(BoxCollider2D collider) : base(collider)
        {
            this.Size = collider.size;
            this.EdgeRadius = collider.edgeRadius;
            this.AutoTiling = collider.autoTiling;
        }

        // --- Helpers ---
        protected override bool CanApplyTo(Collider2D target)
        {
            return target is BoxCollider2D;
        }

        protected override void ApplyShapeTo(Collider2D target)
        {
            BoxCollider2D collider = (BoxCollider2D)target;

            collider.size = this.Size;
            collider.edgeRadius = this.EdgeRadius;
            collider.autoTiling = this.AutoTiling;
        }
    }
}
