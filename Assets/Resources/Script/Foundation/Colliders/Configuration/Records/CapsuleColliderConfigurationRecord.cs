using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class CapsuleColliderConfigurationRecord : ColliderConfigurationRecord
    {
        // --- Configs ---
        [SerializeField] private Vector2 _size = Vector2.one;
        [SerializeField] private CapsuleDirection2D _direction = CapsuleDirection2D.Vertical;
        public Vector2 Size => _size;
        public CapsuleDirection2D Direction => _direction;

        // --- Public APIs ---
        public override Collider2D ConfigureCollider(GameObject targetGameObject)
        {
            CapsuleCollider2D collider = ColliderConfigurationRecord.EnsureCollider<CapsuleCollider2D>(targetGameObject);

            this.ApplyBaseConfiguration(collider);

            collider.size = this.Size;
            collider.direction = this.Direction;

            return collider;
        }
    }
}
