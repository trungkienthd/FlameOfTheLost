using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class BoxColliderConfigurationRecord : ColliderConfigurationRecord
    {
        // --- Configs ---
        [SerializeField] private Vector2 _size = Vector2.one;
        [SerializeField] private float _edgeRadius;
        [SerializeField] private bool _autoTiling;
        public Vector2 Size => _size;
        public float EdgeRadius => _edgeRadius;
        public bool AutoTiling => _autoTiling;

        // --- Public APIs ---
        public override Collider2D ConfigureCollider(GameObject targetGameObject)
        {
            BoxCollider2D collider = ColliderConfigurationRecord.EnsureCollider<BoxCollider2D>(targetGameObject);

            this.ApplyBaseConfiguration(collider);

            collider.size = this.Size;
            collider.edgeRadius = this.EdgeRadius;
            collider.autoTiling = this.AutoTiling;

            return collider;
        }
    }
}
