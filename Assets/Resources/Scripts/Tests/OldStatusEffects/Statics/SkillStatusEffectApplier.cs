using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class SkillStatusEffectApplier
    {
        // Call this from your hit/impact events.
        public static void ApplyToTargets(GameObject source, IEnumerable<StatusEffectController> targets, StatusEffectApplication application, bool wasCritical = false)
        {
            if (application?.StatusEffectProfile == null) return;

            foreach (var target in targets)
            {
                if (!target) continue;
                if (Random.value > application.ApplicationChance) continue;

                float potencyScale = Mathf.Max(0f, application.PotencyScale);
                float durationInSecs = application.DurationOverrideInSeconds > 0f ? application.DurationOverrideInSeconds : application.StatusEffectProfile.BaseDurationInSeconds;
                int stacks = Mathf.Max(0, application.StacksToApply);

                if (wasCritical && application.BonusOnCritical)
                {
                    potencyScale += Mathf.Max(0f, application.BonusPotencyOnCritical);
                    durationInSecs += Mathf.Max(0f, application.BonusDurationOnCriticalInSeconds);
                    stacks += Mathf.Max(0, application.BonusStacksOnCritical);
                }

                // Apply "stacks" times (lets MaxStacks logic inside controller manage it).
                for (int i = 0; i < stacks; i++)
                {
                    target.TryApply(application.StatusEffectProfile, source, potencyScale, durationInSecs);
                }
            }
        }
    }
}
