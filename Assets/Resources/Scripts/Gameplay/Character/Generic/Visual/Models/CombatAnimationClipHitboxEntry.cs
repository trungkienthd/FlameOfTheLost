using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public struct CombatAnimationClipHitboxEntry
    {
        [SerializeField] private AnimationClip _animationClip;
        [SerializeField] private PolygonColliderShape _hitboxShape;

        public AnimationClip AnimationClip => _animationClip;
        public PolygonColliderShape HitboxShape => _hitboxShape;

        public CombatAnimationClipHitboxEntry(AnimationClip animationClip, PolygonColliderShape hitboxShape)
        {
            _animationClip = animationClip;
            _hitboxShape = hitboxShape;
        }

        public CombatAnimationClipHitboxEntry GetResolvedWithFallback(CombatAnimationClipHitboxEntry fallback)
        {
            return new CombatAnimationClipHitboxEntry
            (
                this.AnimationClip != null  ? this.AnimationClip    : fallback.AnimationClip,
                this.HitboxShape.IsValid    ? this.HitboxShape      : fallback.HitboxShape
            );
        }
    }
}