using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public struct MeleeHitboxShapeEntry
    {
        [Header("Polygon Collider Shape per facing status")]
        [SerializeField] private PolygonColliderShapeEntry _facingDownShape;
        [SerializeField] private PolygonColliderShapeEntry _facingSideShape;
        [SerializeField] private PolygonColliderShapeEntry _facingUpShape;

        public PolygonColliderShapeEntry FacingDownShape => _facingDownShape;
        public PolygonColliderShapeEntry FacingSideShape => _facingSideShape;
        public PolygonColliderShapeEntry FacingUpShape => _facingUpShape;

        public MeleeHitboxShapeEntry(PolygonColliderShapeEntry facingDownShape, PolygonColliderShapeEntry facingSideShape, PolygonColliderShapeEntry facingUpShape)
        {
            _facingDownShape = facingDownShape;
            _facingSideShape = facingSideShape;
            _facingUpShape = facingUpShape;
        }
    }
}
