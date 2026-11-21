using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public struct CharacterFacingAnimationClipSetEntry
    {
        [SerializeField] private AnimationClip _down;
        [SerializeField] private AnimationClip _side;
        [SerializeField] private AnimationClip _up;
        public AnimationClip Down => _down;
        public AnimationClip Side => _side;
        public AnimationClip Up => _up;

        public CharacterFacingAnimationClipSetEntry(AnimationClip down, AnimationClip side, AnimationClip up)
        {
            _down = down;
            _side = side;
            _up = up;
        } 

        public AnimationClip GetClip(CharacterFacingStatus facingStatusType)
        {
            return facingStatusType switch
            {
                CharacterFacingStatus.Down => this.Down,
                CharacterFacingStatus.Side => this.Side,
                CharacterFacingStatus.Up => this.Up,
                _ => null,
            };
        }

        public bool IsValid => this.Down != null && this.Side != null && this.Up != null;
    }
}
