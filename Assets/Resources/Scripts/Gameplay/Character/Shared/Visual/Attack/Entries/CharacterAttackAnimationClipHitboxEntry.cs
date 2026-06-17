using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public struct CharacterAttackAnimationClipHitboxEntry
    {
        [SerializeField] private AnimationClip _animationClip;
        [SerializeField] private PolygonColliderShape _hitboxShape;

        public AnimationClip AnimationClip => _animationClip;
        public PolygonColliderShape HitboxShape => _hitboxShape;

        public CharacterAttackAnimationClipHitboxEntry(AnimationClip animationClip, PolygonColliderShape hitboxShape)
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