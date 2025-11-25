using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public struct MeleeHitboxProfile
    {
        [Header("Polygon Collider Shape per facing status")]
        [SerializeField] private PolygonColliderShape _facingDownShape;
        [SerializeField] private PolygonColliderShape _facingSideShape;
        [SerializeField] private PolygonColliderShape _facingUpShape;

        public PolygonColliderShape FacingDownShape => _facingDownShape;
        public PolygonColliderShape FacingSideShape => _facingSideShape;
        public PolygonColliderShape FacingUpShape => _facingUpShape;

        [Header("Mirror Side Facing Shape")]
        [SerializeField] private bool _mirrorSideOffsetX;
        public bool MirrorSideOffsetX => _mirrorSideOffsetX;
    }
}
