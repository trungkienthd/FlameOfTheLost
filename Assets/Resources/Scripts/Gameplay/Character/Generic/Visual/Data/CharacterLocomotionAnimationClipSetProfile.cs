using UnityEngine;

namespace FlameOfTheLost
{


    [CreateAssetMenu(fileName = "CharacterLocomotionAnimationClipSetProfile", menuName = "Data/FlameOfTheLost/Animations/Locomotion/CharacterLocomotionAnimationClipSetProfile")]
    public class CharacterLocomotionAnimationClipSetProfile : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private CharacterLocomotionMode _locomotionMode = CharacterLocomotionMode.Default;
        [SerializeField] private CharacterCarryItemType _carryItemType = CharacterCarryItemType.None;
        [SerializeField] private LightSourceType _lightSourceType = LightSourceType.None;
        public CharacterLocomotionMode LocomotionMode => _locomotionMode;
        public CharacterCarryItemType CarryItemType => _carryItemType;
        public LightSourceType LightSourceType => _lightSourceType;

        [Header("Animation Sets")]
        [SerializeField] private FacingAnimationClipSet _idleClipSet;
        [SerializeField] private FacingAnimationClipSet _walkClipSet;
        public FacingAnimationClipSet IdleClipSet => _idleClipSet;
        public FacingAnimationClipSet WalkClipSet => _walkClipSet;

        public AnimationClip GetIdleClip(CharacterFacingStatus facingStatus)
        {
            return this.IdleClipSet.IsValid
                ? this.IdleClipSet.GetClip(facingStatus)
                : null;
        }

        public AnimationClip GetWalkClip(CharacterFacingStatus facingStatus)
        {
            return this.WalkClipSet.IsValid
                ? this.WalkClipSet.GetClip(facingStatus)
                : null;
        }

        public bool Matches(CharacterLocomotionMode locomotionMode, CharacterCarryItemType carryItemType, LightSourceType lightSourceType)
        {
            if (this.LocomotionMode != locomotionMode)
            {
                return false;
            }

            if (locomotionMode == CharacterLocomotionMode.Carry)
            {
                if (this.CarryItemType != carryItemType)
                {
                    return false;
                }

                if (carryItemType == CharacterCarryItemType.LightSource)
                {
                    return this.LightSourceType == lightSourceType;
                }
            }

            return true;
        }
    }
}
