using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class StatusEffectModifiersHandler : MonoBehaviour
    {
        private readonly StatusEffectModifierAggregateMap _aggregates = new();
        private StatusEffectModifierAggregateMap ModifierAggregatesMap => _aggregates;

        // Cache of active instances injected by the controller
        private readonly List<RuntimeStatusEffect> _runtimeStatusEffects = new();
        private List<RuntimeStatusEffect> RuntimeStatusEffects => _runtimeStatusEffects;

        internal void SetActiveStatusEffects(List<RuntimeStatusEffect> list)
        {
            this.RuntimeStatusEffects.Clear();
            this.RuntimeStatusEffects.AddRange(list);

            this.Rebuild();
        }

        internal void Rebuild()
        {
            this.ModifierAggregatesMap.Clear();

            // bucket by (Target, Operation) for HighestOnly/LowestOnly selection
            var buckets = new Dictionary<(StatusEffectModifierTargetType target, StatusEffectModifierOperationType operation), List<float>>();

            foreach (var statusEffect in this.RuntimeStatusEffects)
            {
                var modifiers = statusEffect.ScaledModifiers;
                if (modifiers == null) continue;

                foreach (var modifier in modifiers)
                {
                    var key = (modifier.Target, modifier.Operation);
                    if (!buckets.TryGetValue(key, out var list))
                    {
                        list = new List<float>();
                        buckets[key] = list;
                    }
                    list.Add(modifier.Value);
                }
            }

            // Combine per bucket according to the "dominant" profile policy
            // For simplicity, we treat selection policy as:
            //  If multiple instances contribute to the same (Target, Operation), and any of them prefers Selecting Extreme (KeepStrongest/KeepHighest),
            //  we apply that selection (Strongest/Weakest respectively) across all contributions for that (Target, Operation)
            foreach (var bucket in buckets)
            {
                var (target, operation) = bucket.Key; 
                var values = bucket.Value;

                // Check only relevant runtime status effects
                var relevantStatusEffects = this.RuntimeStatusEffects.Where(rse => rse.ScaledModifiers != null && rse.ScaledModifiers.Any(m => m.Target == target && m.Operation == operation));

                // We need to determine the only combination type for this bucket based on the following priority:
                // KeepStrongest > KeepWeakest > StackAll
                bool hasKeepStrongest = false;
                bool hasKeepWeakest = false;
                foreach (var rse in relevantStatusEffects)
                {
                    switch (rse.Profile.CombinationType)
                    {
                        default:
                        case StatusEffectCombinationType.StackAll:
                            break;
                        case StatusEffectCombinationType.KeepStrongest:
                            hasKeepStrongest = true;
                            break;
                        case StatusEffectCombinationType.KeepWeakest:
                            hasKeepWeakest = true;
                            break;
                    }
                }

                StatusEffectCombinationType finalCombinationType;
                if (hasKeepStrongest && hasKeepWeakest)
                {
                    Debug.LogWarning($"[StatusEffectModifiersHandler] Conflict on {target}/{operation}: both KeepStrongest and KeepWeakest found. Defaulting to KeepStrongest.");
                    finalCombinationType = StatusEffectCombinationType.KeepStrongest;
                }
                else if (hasKeepStrongest)
                {
                    finalCombinationType = StatusEffectCombinationType.KeepStrongest;
                }
                else if (hasKeepWeakest)
                {
                    finalCombinationType = StatusEffectCombinationType.KeepWeakest;
                }
                else
                {
                    finalCombinationType = StatusEffectCombinationType.StackAll;
                }

                // Default Policy: Stack all contributions
                float combinedValue = operation == StatusEffectModifierOperationType.Multiply ? 1f : 0f;

                // Combine the values for the target based on the corresponding operation
                switch (finalCombinationType)
                {
                    case StatusEffectCombinationType.StackAll:
                        if (operation == StatusEffectModifierOperationType.Multiply)
                        {
                            foreach (var val in values)
                            {
                                combinedValue *= val;
                            }
                        }
                        else if (operation == StatusEffectModifierOperationType.AddFlat)
                        {
                            foreach (var val in values)
                            {
                                combinedValue += val;
                            }
                        }
                        break;

                    case StatusEffectCombinationType.KeepStrongest:
                        combinedValue = StatusEffectModifiersUtility.SelectExtreme(values, operation, selectHighest: true);
                        break;

                    case StatusEffectCombinationType.KeepWeakest:
                        combinedValue = StatusEffectModifiersUtility.SelectExtreme(values, operation, selectHighest: false);
                        break;
                }

                // Guard rails for known single-operation targets
                if (target == StatusEffectModifierTargetType.MovementSpeedScale && operation == StatusEffectModifierOperationType.Multiply)
                {
                    //combinedValue = Mathf.Clamp(combinedValue, 0f, float.MaxValue);
                }

                this.ModifierAggregatesMap.Accumulate(target, operation, combinedValue);
            }
        }

        // Public Readonly APIs
        public float GetMultiplier(StatusEffectModifierTargetType target)
        {
            return this.ModifierAggregatesMap.GetMultiplier(target);
        }

        public float GetAdditiveFlat(StatusEffectModifierTargetType target)
        {
            return this.ModifierAggregatesMap.GetAdditiveFlat(target);
        }
    }
}
