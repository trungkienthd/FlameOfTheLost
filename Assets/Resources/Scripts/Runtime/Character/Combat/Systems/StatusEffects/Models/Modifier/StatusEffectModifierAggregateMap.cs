using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    internal class StatusEffectModifierAggregateMap
    {
        private readonly Dictionary<StatusEffectModifierTargetType, StatusEffectModifierAccumulator> _map = new();
        public Dictionary<StatusEffectModifierTargetType, StatusEffectModifierAccumulator> Map => _map;

        public void Clear()
        {
            this.Map.Clear();
        }

        public void Accumulate(StatusEffectModifierTargetType target, StatusEffectModifierOperationType operation, float value)
        {
            if (!this.Map.TryGetValue(target, out var channel))
            {
                channel = new StatusEffectModifierAccumulator();
                this.Map[target] = channel;
            }

            if (operation == StatusEffectModifierOperationType.Multiply)
            {
                channel.TotalMultiplier *= Mathf.Max(0f, value);
            }
            else if (operation == StatusEffectModifierOperationType.AddFlat)
            {
                channel.TotalAdditiveFlat += value;
            }
        }

        public float GetMultiplier(StatusEffectModifierTargetType target)
        {
            if (this.Map.TryGetValue(target, out var channel))
            {
                return channel.TotalMultiplier;
            }

            return 1f;
        }

        public float GetAdditiveFlat(StatusEffectModifierTargetType target)
        {
            if (this.Map.TryGetValue(target, out var channel))
            {
                return channel.TotalAdditiveFlat;
            }

            return 0f;
        }
    }
}
