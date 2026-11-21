using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public abstract class ColliderConfigurationRecord
    {
        // --- Configs ---
        [SerializeField] private Vector2 _offset;
        [SerializeField] private bool _isTrigger;
        public Vector2 Offset => _offset;
        public bool IsTrigger => _isTrigger;

        // --- Public APIs ---
        public abstract Collider2D ConfigureCollider(GameObject targetGameObject);

        // --- Helpers ---
        protected void ApplyBaseConfiguration(Collider2D collider)
        {
            collider.offset = this.Offset;
            collider.isTrigger = this.IsTrigger;
        }

        protected static T EnsureCollider<T>(GameObject targetGameObject) where T : Collider2D
        {
            if (targetGameObject.TryGetComponent(out T collider))
            {
                return collider;
            }

            return targetGameObject.AddComponent<T>();
        }
    }
}
