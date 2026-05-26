using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "CharacterLocomotionAnimationProfile", menuName = "Data/FlameOfTheLost/Animations/Locomotion/CharacterLocomotionAnimationProfile")]
    public class CharacterLocomotionAnimationProfile : ScriptableObject
    {
        [Header("Default Locomotion")]
        [SerializeField] private CharacterLocomotionAnimationClipSetProfile _defaultLocomotion;
        public CharacterLocomotionAnimationClipSetProfile DefaultLocomotion => _defaultLocomotion;

        [Header("Locomotion Variants")]
        [SerializeField] private List<CharacterLocomotionAnimationClipSetProfile> _locomotionVariants = new();
        public IReadOnlyList<CharacterLocomotionAnimationClipSetProfile> LocomotionVariants => _locomotionVariants;

        public CharacterLocomotionAnimationClipSetProfile GetLocomotionSet(CharacterLocomotionModeType locomotionMode, CharacterCarryItemType carryItemType, LightSourceType lightSourceType)
        {
            if (locomotionMode == CharacterLocomotionModeType.Default)
            {
                return this.DefaultLocomotion;
            }

            if (locomotionMode == CharacterLocomotionModeType.Carry)
            {
                if (this.LocomotionVariants != null)
                {
                    for (int i = 0; i < this.LocomotionVariants.Count; i++)
                    {
                        CharacterLocomotionAnimationClipSetProfile entry = this.LocomotionVariants[i];
                        if (entry == null) continue;

                        if (entry.Matches(carryItemType, lightSourceType))
                        {
                            return entry;
                        }
                    }
                }

                // fallback
                return this.DefaultLocomotion;
            }

            return this.DefaultLocomotion;
        }

        public AnimationClip GetIdleClip(CharacterFacingStatusType facingStatus, CharacterLocomotionModeType locomotionMode = CharacterLocomotionModeType.Default, CharacterCarryItemType carryItemType = CharacterCarryItemType.None, LightSourceType lightSourceType = LightSourceType.None)
        {
            CharacterLocomotionAnimationClipSetProfile set = this.GetLocomotionSet(locomotionMode, carryItemType, lightSourceType);

            return set != null
                ? set.GetIdleClip(facingStatus)
                : null;
        }

        public AnimationClip GetWalkClip(CharacterFacingStatusType facingStatus, CharacterLocomotionModeType locomotionMode = CharacterLocomotionModeType.Default, CharacterCarryItemType carryItemType = CharacterCarryItemType.None, LightSourceType lightSourceType = LightSourceType.None)
        {
            CharacterLocomotionAnimationClipSetProfile set = this.GetLocomotionSet(locomotionMode, carryItemType, lightSourceType);

            return set != null
                ? set.GetWalkClip(facingStatus)
                : null;
        }
    }
}
