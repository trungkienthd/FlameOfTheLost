using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class StatusEffectModifiersUtility
    {
        public static float SelectExtreme(List<float> values, StatusEffectModifierOperationType operation, bool selectHighest)
        {
            if (values.Count == 0) return operation == StatusEffectModifierOperationType.Multiply ? 1f : 0f;

            // Compare by |mult - 1| for multipliers, |value| for add-flats
            float bestValue = values[0];
            float bestScore = StatusEffectModifiersUtility.Score(values[0], operation);

            for (int i = 1; i < values.Count; i++)
            {
                float currentScore = StatusEffectModifiersUtility.Score(values[i], operation);
                bool isBetter = selectHighest ? (currentScore > bestScore) : (currentScore < bestScore);
                if (isBetter)
                {
                    bestValue = values[i];
                    bestScore = currentScore;
                }
            }

            return bestValue;
        }

        private static float Score(float value, StatusEffectModifierOperationType operation)
        {
            return operation == StatusEffectModifierOperationType.Multiply ? Mathf.Abs(value - 1f) : Mathf.Abs(value);
        }
    }
}
