using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class StatusEffectUtility
    {
        public static List<StatusEffectStatModifierContribution> BuildScaledContributions(IReadOnlyList<StatusEffectStatModifierContribution> rawStatModifiers, float potency, int stacks)
        {
            if (rawStatModifiers == null) return new List<StatusEffectStatModifierContribution>(0);

            float scale = Mathf.Max(0f, potency) * Mathf.Max(1, stacks);

            var list = new List<StatusEffectStatModifierContribution>(rawStatModifiers.Count);
            foreach (var rawStatModifier in rawStatModifiers)
            {
                list.Add
                (
                    new StatusEffectStatModifierContribution
                    {
                        Target = rawStatModifier.Target,
                        Operation = rawStatModifier.Operation,
                        Value = rawStatModifier.Operation == StatusEffectStatModifierOperationType.Multiply ? StatusEffectUtility.ScaleMultiplier(rawStatModifier.Value, scale) : StatusEffectUtility.ScaleFlatAdded(rawStatModifier.Value, scale),
                    }
                );
            }

            return list;
        }

        public static List<StatusEffectStatPeriodicContribution> BuildScaledContributions(IReadOnlyList<StatusEffectStatPeriodicContribution> rawStatPeriodics, float potency, int stacks)
        {
            if (rawStatPeriodics == null || rawStatPeriodics.Count == 0) return new List<StatusEffectStatPeriodicContribution>(0);

            float scale = Mathf.Max(0f, potency) * Mathf.Max(1, stacks);

            var list = new List<StatusEffectStatPeriodicContribution>(rawStatPeriodics.Count);
            foreach (var rawStatPeriodic in rawStatPeriodics)    
            {
                if (rawStatPeriodic == null || !rawStatPeriodic.IsEnabled) continue;

                var scaledPeriodic = rawStatPeriodic.Clone();
                scaledPeriodic.AmountPerSecond = rawStatPeriodic.AmountPerSecond * scale;

                list.Add(scaledPeriodic);
            }

            return list;
        }

        private static float ScaleMultiplier(float baseMultiplier, float scale)
        {
            return 1f + ((baseMultiplier - 1f) * scale);
        }
        private static float ScaleFlatAdded(float baseMultiplier, float scale)
        {
            return baseMultiplier * scale;
        }
        public static float ClampMovementSpeedScale(float value, float minScale, float maxScale)
        {
            return Mathf.Clamp(value, minScale, maxScale);
        }
    }
}
