using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class ColliderSnapshotRecord
    {
        // --- States ---
        private Vector2 Offset { get; }
        private bool IsTrigger { get; }
        private bool Enabled { get; }

        // --- Constructors ---
        protected ColliderSnapshotRecord(Collider2D collider)
        {
            this.Offset = collider.offset;
            this.IsTrigger = collider.isTrigger;
            //this.Enabled = collider.enabled;
        }

        // --- Public APIs ---
        public void ApplyTo(Collider2D target, UnityEngine.Object logContext)
        {
            if (target == null) return;
            if (!this.CanApplyTo(target))
            {
                Debug.LogError
                (
                    $"[{target.gameObject.name}." +
                    $"{nameof(HarvestableBrokenStateHandler)}] " +
                    $"Cannot apply collider state " +
                    $"'{this.GetType().Name}' to collider " +
                    $"type '{target.GetType().Name}'.",
                    logContext
                );

                return;
            }

            target.offset = this.Offset;
            target.isTrigger = this.IsTrigger;

            this.ApplyShapeTo(target);

            //target.enabled = this.Enabled;
        }

        // --- Helpers ---
        protected abstract bool CanApplyTo(Collider2D target);

        protected abstract void ApplyShapeTo(Collider2D target);

        // --- Statics ---
        public static bool TryCapture(Collider2D collider, out ColliderSnapshotRecord state)
        {
            state = collider switch
            {
                BoxCollider2D       boxCollider     => new BoxColliderSnapshotRecord(boxCollider),
                CircleCollider2D    circleCollider  => new CircleColliderSnapshotRecord(circleCollider),
                CapsuleCollider2D   capsuleCollider => new CapsuleColliderSnapshotRecord(capsuleCollider),
                PolygonCollider2D   polygonCollider => new PolygonColliderSnapshotRecord(polygonCollider),
                _ => null
            };

            if (state != null) return true;

            Debug.LogError
            (
                $"[{collider.gameObject.name}." +
                $"{nameof(HarvestableBrokenStateHandler)}] " +
                $"Unsupported collider type: " +
                $"{collider.GetType().Name}.",
                collider
            );

            return false;
        }
    }
}