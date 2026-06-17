using UnityEngine;

namespace FlameOfTheLost
{
    public static class CharacterAnimationOverrideUtility
    {
        public static void ApplyFacingClipSetOverride(AnimatorOverrideController overrideController, CharacterFacingAnimationClipSetEntry placeholderSet, CharacterFacingAnimationClipSetEntry targetSet)
        {
            if (overrideController == null) return;
            if (!placeholderSet.IsValid) return;
            if (!targetSet.IsValid) return;

            if (placeholderSet.Down != null && targetSet.Down != null)
            {
                overrideController[placeholderSet.Down] = targetSet.Down;
            }

            if (placeholderSet.Side != null && targetSet.Side != null)
            {
                overrideController[placeholderSet.Side] = targetSet.Side;
            }

            if (placeholderSet.Up != null && targetSet.Up != null)
            {
                overrideController[placeholderSet.Up] = targetSet.Up;
            }
        }
    }
}
