using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class ColliderConfigurationEntry
    {
        // --- Configs ---
        [SerializeField] private ColliderType _colliderType = ColliderType.Box;

        [SerializeField] private BoxColliderConfigurationRecord _boxConfiguration = new();
        [SerializeField] private CircleColliderConfigurationRecord _circleConfiguration = new();
        [SerializeField] private CapsuleColliderConfigurationRecord _capsuleConfiguration = new();
        [SerializeField] private PolygonColliderConfigurationRecord _polygonConfiguration = new();

        public ColliderType ColliderType => _colliderType;

        // --- Public APIs ---
        public Collider2D ConfigureCollider(GameObject targetGameObject)
        {
            if (targetGameObject == null) return null;

            this.DisableUnexpectedColliders(targetGameObject);

            Collider2D collider = this.ColliderType switch
            {
                ColliderType.Box => _boxConfiguration.ConfigureCollider(targetGameObject),
                ColliderType.Circle => _circleConfiguration.ConfigureCollider(targetGameObject),
                ColliderType.Capsule => _capsuleConfiguration.ConfigureCollider(targetGameObject),
                ColliderType.Polygon => _polygonConfiguration.ConfigureCollider(targetGameObject),
                _ => null
            };

            if (collider != null)
            {
                collider.enabled = true;
            }

            this.DestroyUnexpectedColliders(targetGameObject, collider);

            return collider;
        }

        // --- Helpers ---
        private void DisableUnexpectedColliders(GameObject targetGameObject)
        {
            Collider2D[] colliders = targetGameObject.GetComponents<Collider2D>();

            for (int i = 0; i < colliders.Length; i++)
            {
                Collider2D collider = colliders[i];

                if (this.IsExpectedColliderType(collider)) continue;

                collider.enabled = false;
            }
        }

        private void DestroyUnexpectedColliders( GameObject targetGameObject, Collider2D configuredCollider)
        {
            Collider2D[] colliders = targetGameObject.GetComponents<Collider2D>();

            for (int i = 0; i < colliders.Length; i++)
            {
                Collider2D collider = colliders[i];

                if (collider == configuredCollider) continue;

                UnityEngine.Object.Destroy(collider);
            }
        }

        private bool IsExpectedColliderType(Collider2D collider)
        {
            return this.ColliderType switch
            {
                ColliderType.Box => collider is BoxCollider2D,
                ColliderType.Circle => collider is CircleCollider2D,
                ColliderType.Capsule => collider is CapsuleCollider2D,
                ColliderType.Polygon => collider is PolygonCollider2D,
                _ => false
            };
        }
    }
}
