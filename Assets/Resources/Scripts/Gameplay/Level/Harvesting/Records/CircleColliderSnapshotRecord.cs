using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class CircleColliderSnapshotRecord : ColliderSnapshotRecord
    {
        // --- States ---
        private float Radius { get; }

        // --- Constructors ---
        public CircleColliderSnapshotRecord(CircleCollider2D collider) : base(collider)
        {
            this.Radius = collider.radius;
        }

        // --- Helpers ---
        protected override bool CanApplyTo(Collider2D target)
        {
            return target is CircleCollider2D;
        }
        
        protected override void ApplyShapeTo(Collider2D target)
        {
            CircleCollider2D collider = (CircleCollider2D)target;

            collider.radius = this.Radius;
        }
    }
}
