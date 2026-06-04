using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "LAni_New", menuName = "Data/FlameOfTheLost/Character/Shared/Visual/Animation/Locomotion/CharacterLocomotionAnimationProfile")]
    public class CharacterLocomotionAnimationProfile : ScriptableObject
    {
        [Header("Default Locomotion")]
        [SerializeField] private CharacterLocomotionAnimationClipSetProfile _defaultLocomotion;
        public CharacterLocomotionAnimationClipSetProfile DefaultLocomotion => _defaultLocomotion;

        [Header("Locomotion Variants")]
        [SerializeField] private List<CharacterLocomotionAnimationClipSetProfile> _locomotionVariants = new();
        public IReadOnlyList<CharacterLocomotionAnimationClipSetProfile> LocomotionVariants => _locomotionVariants;

        public CharacterLocomotionAnimationClipSetProfile GetLocomotionSet(CharacterLocomotionMode locomotionMode, CharacterCarryItemType carryItemType, LightSourceType lightSourceType)
        {
            if (locomotionMode == CharacterLocomotionMode.Default)
            {
                return this.DefaultLocomotion;
            }

            if (this.LocomotionVariants != null)
            {
                for (int i = 0; i < this.LocomotionVariants.Count; i++)
                {
                    CharacterLocomotionAnimationClipSetProfile entry = this.LocomotionVariants[i];
                    if (entry == null) continue;

                    if (entry.Matches(locomotionMode, carryItemType, lightSourceType))
                    {
                        return entry;
                    }
                }
            }

            return this.DefaultLocomotion;
        }

        public AnimationClip GetIdleClip(CharacterFacingStatus facingStatus, CharacterLocomotionMode locomotionMode = CharacterLocomotionMode.Default, CharacterCarryItemType carryItemType = CharacterCarryItemType.None, LightSourceType lightSourceType = LightSourceType.None)
        {
            CharacterLocomotionAnimationClipSetProfile set = this.GetLocomotionSet(locomotionMode, carryItemType, lightSourceType);

            return set != null
                ? set.GetIdleClip(facingStatus)
                : null;
        }

        public AnimationClip GetWalkClip(CharacterFacingStatus facingStatus, CharacterLocomotionMode locomotionMode = CharacterLocomotionMode.Default, CharacterCarryItemType carryItemType = CharacterCarryItemType.None, LightSourceType lightSourceType = LightSourceType.None)
        {
            CharacterLocomotionAnimationClipSetProfile set = this.GetLocomotionSet(locomotionMode, carryItemType, lightSourceType);

            return set != null
                ? set.GetWalkClip(facingStatus)
                : null;
        }
    }
}
