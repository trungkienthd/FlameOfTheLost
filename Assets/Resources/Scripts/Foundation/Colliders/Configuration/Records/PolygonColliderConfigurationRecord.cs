using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class PolygonColliderConfigurationRecord : ColliderConfigurationRecord
    {
        // --- Configs ---
        [SerializeField] private List<PolygonColliderPathConfigurationEntry> _paths = new();
        public IReadOnlyList<PolygonColliderPathConfigurationEntry> Paths => _paths;

        // --- Public APIs ---
        public override Collider2D ConfigureCollider(GameObject targetGameObject)
        {
            PolygonCollider2D collider = ColliderConfigurationRecord.EnsureCollider<PolygonCollider2D>( targetGameObject);

            this.ApplyBaseConfiguration(collider);

            collider.pathCount = this.Paths.Count;

            for (int i = 0; i < this.Paths.Count; i++)
            {
                PolygonColliderPathConfigurationEntry path = this.Paths[i];

                Vector2[] points = new Vector2[path.Points.Count];

                for (int j = 0; j < path.Points.Count; j++)
                {
                    points[j] = path.Points[j];
                }

                collider.SetPath(i, points);
            }

            return collider;
        }
    }
}
