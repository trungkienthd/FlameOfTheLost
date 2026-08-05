using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class PolygonColliderSnapshotRecord : ColliderSnapshotRecord
    {
        // --- States ---
        private Vector2[][] Paths { get; }

        // --- Constructors ---
        public PolygonColliderSnapshotRecord(PolygonCollider2D collider) : base(collider)
        {
            this.Paths = PolygonColliderSnapshotRecord.CapturePaths(collider);
        }

        // --- Helpers ---
        protected override bool CanApplyTo(Collider2D target)
        {
            return target is PolygonCollider2D;
        }
        protected override void ApplyShapeTo(Collider2D target)
        {
            PolygonCollider2D collider = (PolygonCollider2D)target;

            collider.pathCount = this.Paths.Length;

            for (int i = 0; i < this.Paths.Length; i++)
            {
                collider.SetPath(i, this.Paths[i]);
            }
        }

        // --- Statics ---
        private static Vector2[][] CapturePaths(PolygonCollider2D collider)
        {
            Vector2[][] paths = new Vector2[collider.pathCount][];
            for (int i = 0; i < collider.pathCount; i++)
            {
                paths[i] = collider.GetPath(i);
            }

            return paths;
        }
    }
}
