using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public struct CharacterAttackAnimationClipHitboxEntry
    {
        [SerializeField] private AnimationClip _animationClip;
        [SerializeField] private PolygonColliderShapeEntry _hitboxShape;

        public AnimationClip AnimationClip => _animationClip;
        public PolygonColliderShapeEntry HitboxShape => _hitboxShape;

        public CharacterAttackAnimationClipHitboxEntry(AnimationClip animationClip, PolygonColliderShapeEntry hitboxShape)
        {
            _animationClip = animationClip;
            _hitboxShape = hitboxShape;
        }

        public CharacterAttackAnimationClipHitboxEntry GetResolvedWithFallback(CharacterAttackAnimationClipHitboxEntry fallback)
        {
            return new CharacterAttackAnimationClipHitboxEntry
            (
                this.AnimationClip != null ? this.AnimationClip : fallback.AnimationClip,
                this.HitboxShape.IsValid ? this.HitboxShape : fallback.HitboxShape
            );
        }
    }
}