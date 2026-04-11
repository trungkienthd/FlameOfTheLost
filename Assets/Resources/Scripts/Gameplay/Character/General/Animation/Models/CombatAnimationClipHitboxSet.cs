using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public struct CombatAnimationClipHitboxSet
    {
        [SerializeField] private CombatAnimationClipHitboxEntry _down;
        [SerializeField] private CombatAnimationClipHitboxEntry _side;
        [SerializeField] private CombatAnimationClipHitboxEntry _up;

        public CombatAnimationClipHitboxEntry Down => _down;
        public CombatAnimationClipHitboxEntry Side => _side;
        public CombatAnimationClipHitboxEntry Up => _up;

        public CombatAnimationClipHitboxSet(CombatAnimationClipHitboxEntry down, CombatAnimationClipHitboxEntry side, CombatAnimationClipHitboxEntry up)
        {
            _down = down;
            _side = side;
            _up = up;
        }

        public CombatAnimationClipHitboxEntry GetEntry(FacingStatusType facing)
        {
            return facing switch
            {
                FacingStatusType.Down => _down,
                FacingStatusType.Side => _side,
                FacingStatusType.Up => _up,
                _ => _down
            };
        }

        public AnimationClip GetAttackClip(FacingStatusType facing)
        {
            return this.GetEntry(facing).AnimationClip;
        }

        public CombatAnimationClipHitboxSet GetResolvedWithFallback(CombatAnimationClipHitboxSet fallback)
        {
            return new CombatAnimationClipHitboxSet
            (
                _down.GetResolvedWithFallback(fallback.Down),
                _side.GetResolvedWithFallback(fallback.Side),
                _up.GetResolvedWithFallback(fallback.Up)
            );
        }
    }
}
