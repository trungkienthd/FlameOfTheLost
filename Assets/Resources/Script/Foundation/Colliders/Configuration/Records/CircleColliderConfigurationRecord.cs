using System;
using UnityEngine;
namespace FlameOfTheLost
{
    [Serializable]
    public sealed class CircleColliderConfigurationRecord : ColliderConfigurationRecord
    {
        // --- Configs ---
        [SerializeField] private float _radius = 0.5f;
        public float Radius => _radius;

        // --- Public APIs ---
        public override Collider2D ConfigureCollider(GameObject targetGameObject)
        {
            CircleCollider2D collider = ColliderConfigurationRecord.EnsureCollider<CircleCollider2D>(targetGameObject);

            this.ApplyBaseConfiguration(collider);

            collider.radius = this.Radius;

            return collider;
        }
    }
}
