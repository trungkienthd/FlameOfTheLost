using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public struct MeleeHitboxShape
    {
        [Header("Polygon Collider Shape per facing status")]
        [SerializeField] private PolygonColliderShape _facingDownShape;
        [SerializeField] private PolygonColliderShape _facingSideShape;
        [SerializeField] private PolygonColliderShape _facingUpShape;

        public PolygonColliderShape FacingDownShape => _facingDownShape;
        public PolygonColliderShape FacingSideShape => _facingSideShape;
        public PolygonColliderShape FacingUpShape => _facingUpShape;

        public MeleeHitboxShape(PolygonColliderShape facingDownShape, PolygonColliderShape facingSideShape, PolygonColliderShape facingUpShape)
        {
            _facingDownShape = facingDownShape;
            _facingSideShape = facingSideShape;
            _facingUpShape = facingUpShape;
        }
    }
}
