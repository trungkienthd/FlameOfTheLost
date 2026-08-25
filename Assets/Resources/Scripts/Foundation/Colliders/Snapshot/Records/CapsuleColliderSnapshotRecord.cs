using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class CapsuleColliderSnapshotRecord : ColliderSnapshotRecord
    {
        // --- States ---
        private Vector2 Size { get; }
        private CapsuleDirection2D Direction { get; }

        // --- Constructors ---
        public CapsuleColliderSnapshotRecord(CapsuleCollider2D collider) : base(collider)
        {
            this.Size = collider.size;
            this.Direction = collider.direction;
        }

        // --- Helpers ---
        protected override bool CanApplyTo(Collider2D target)
        {
            return target is CapsuleCollider2D;
        }
        protected override void ApplyShapeTo(Collider2D target)
        {
            CapsuleCollider2D collider = (CapsuleCollider2D)target;

            collider.size = this.Size;
            collider.direction = this.Direction;
        }
    }
}
