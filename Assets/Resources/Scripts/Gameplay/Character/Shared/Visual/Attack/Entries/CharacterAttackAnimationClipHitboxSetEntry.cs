using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public struct CharacterAttackAnimationClipHitboxSetEntry
    {
        [SerializeField] private CharacterAttackAnimationClipHitboxEntry _down;
        [SerializeField] private CharacterAttackAnimationClipHitboxEntry _side;
        [SerializeField] private CharacterAttackAnimationClipHitboxEntry _up;

        public CharacterAttackAnimationClipHitboxEntry Down => _down;
        public CharacterAttackAnimationClipHitboxEntry Side => _side;
        public CharacterAttackAnimationClipHitboxEntry Up => _up;

        public CharacterAttackAnimationClipHitboxSetEntry(CharacterAttackAnimationClipHitboxEntry down, CharacterAttackAnimationClipHitboxEntry side, CharacterAttackAnimationClipHitboxEntry up)
        {
            _down = down;
            _side = side;
            _up = up;
        }

        public CharacterAttackAnimationClipHitboxEntry GetEntry(CharacterFacingStatus facing)
        {
            return facing switch
            {
                CharacterFacingStatus.Down => _down,
                CharacterFacingStatus.Side => _side,
                CharacterFacingStatus.Up => _up,
                _ => _down
            };
        }

        public AnimationClip GetAttackClip(CharacterFacingStatus facing)
        {
            return this.GetEntry(facing).AnimationClip;
        }

        public CharacterAttackAnimationClipHitboxSetEntry GetResolvedWithFallback(CharacterAttackAnimationClipHitboxSetEntry fallback)
        {
            return new CharacterAttackAnimationClipHitboxSetEntry
            (
                _down.GetResolvedWithFallback(fallback.Down),
                _side.GetResolvedWithFallback(fallback.Side),
                _up.GetResolvedWithFallback(fallback.Up)
            );
        }
    }
}
