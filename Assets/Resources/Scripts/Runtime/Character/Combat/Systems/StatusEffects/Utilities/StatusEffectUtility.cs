using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class StatusEffectUtility
    {
        public static List<StatusEffectModifierContribution> BuildScaledContribution(IReadOnlyList<StatusEffectModifierContribution> rawModifiers, float potency, int stacks)
        {
            if (rawModifiers == null) return new List<StatusEffectModifierContribution>(0);

            var list = new List<StatusEffectModifierContribution>(rawModifiers.Count);
            float scale = Mathf.Max(0f, potency) * Mathf.Max(1, stacks);
            foreach (var rm in rawModifiers)
            {
                list.Add
                (
                    new StatusEffectModifierContribution
                    {
                        Target = rm.Target,
                        Operation = rm.Operation,
                        Value = rm.Operation == StatusEffectModifierOperationType.Multiply ? StatusEffectUtility.ScaleMultiplier(rm.Value, scale) : rm.Value * scale,
                    }
                );
            }

            return list;
        }

        public static StatusEffectPeriodicContribution BuildScaledContribution(StatusEffectPeriodicContribution rawPeriodic, float potency, int stacks)
        {
            if (rawPeriodic == null || !rawPeriodic.IsEnabled) return new StatusEffectPeriodicContribution { IsEnabled = false };

            float scale = Mathf.Max(0f, potency) * Mathf.Max(1, stacks);
            return new StatusEffectPeriodicContribution
            {
                IsEnabled = true,
                TickInterval = Mathf.Max(0.01f, rawPeriodic.TickInterval),
                TickAmount = rawPeriodic.TickAmount * scale,
                TreatAsIncomingDamage = rawPeriodic.TreatAsIncomingDamage,
            };
        }

        private static float ScaleMultiplier(float baseMultiplier, float scale)
        {
            return 1f + ((baseMultiplier - 1f) * scale);
        }

        public static float CombineMultipliers(float a, float b)
        {
            return a * b;
        }

        public static float ClampCooldown(float seconds, float minCooldown)
        {
            return Mathf.Max(seconds, minCooldown);
        }

        public static float ClampMovementSpeedScale(float value, float minScale, float maxScale)
        {
            return Mathf.Clamp(value, minScale, maxScale);
        }
    }
}
