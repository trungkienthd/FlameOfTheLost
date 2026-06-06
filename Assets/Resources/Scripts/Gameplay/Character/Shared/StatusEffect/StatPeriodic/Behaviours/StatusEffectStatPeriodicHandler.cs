using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class StatusEffectStatPeriodicHandler : MonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private StatusEffectController _statusEffectController;
        private StatusEffectController StatusEffectController
        {
            get => _statusEffectController;
            set => _statusEffectController = value;
        }

        //  2. Consumers discovered in children (HP/FP/Stamina etc.)
        [Header("SE Periodic Map (Runtime)")]
        [SerializeField] private StatusEffectStatPeriodicTargetToConsumerDictionary _consumersByTarget = new();
        private IReadOnlyDictionary<StatusEffectStatPeriodicTarget, List<IStatusEffectStatPeriodicConsumer>> PeriodicConsumersByTarget => _consumersByTarget;

        // --- Unity Callbacks ---
        private void OnDisable()
        {
            if (this.StatusEffectController == null) return;
            this.StatusEffectController.OnRuntimeListChanged    -= this.HandleRuntimeChanged;
            this.StatusEffectController.OnRuntimeValuesChanged  -= this.HandleRuntimeChanged;
        }
        private void OnDestroy()
        {
            if (this.StatusEffectController == null) return;
            this.StatusEffectController.OnRuntimeListChanged    -= this.HandleRuntimeChanged;
            this.StatusEffectController.OnRuntimeValuesChanged  -= this.HandleRuntimeChanged;
        }

        // --- Public APIs ---
        public void Bind(StatusEffectController controller)
        {
            this.StatusEffectController = controller;

            if (this.StatusEffectController != null)
            {
                this.StatusEffectController.OnRuntimeListChanged    -= this.HandleRuntimeChanged;
                this.StatusEffectController.OnRuntimeValuesChanged  -= this.HandleRuntimeChanged;

                this.StatusEffectController.OnRuntimeListChanged    += this.HandleRuntimeChanged;
                this.StatusEffectController.OnRuntimeValuesChanged  += this.HandleRuntimeChanged;
            }

            this.RegisterConsumers();
            this.Recompute();
        }

        public void Recompute()
        {
            if (this.StatusEffectController == null) return;

            // Build streams per target
            var streamsByTarget = new Dictionary<StatusEffectStatPeriodicTarget, List<StatusEffectStatPeriodicStream>>();

            var runtimeStatusEffects = this.StatusEffectController.RuntimeStatusEffects;
            if (runtimeStatusEffects != null)
            {
                foreach (var runtimeStatusEffect in runtimeStatusEffects)
                {
                    if (runtimeStatusEffect == null) continue;

                    var statPeriodics = runtimeStatusEffect.RuntimeScaledStatPeriodics;
                    if (statPeriodics == null || statPeriodics.Count <= 0) continue;

                    foreach (var statPeriodic in statPeriodics)
                    {
                        if (statPeriodic == null) continue;
                        if (!statPeriodic.IsEnabled) continue;

                        // Debug.Log($"Processing SE '{runtimeStatusEffect.Profile.DisplayName}' stat periodic contribution: Target={statPeriodic.Target}, Method={statPeriodic.Method}, AmountPerSecond={statPeriodic.AmountPerSecond}, TickInterval={statPeriodic.TickInterval}");

                        float interval = this.ClampTickInterval(statPeriodic.TickInterval);
                        float amountPerSecond = this.ClampAmountPerSecond(statPeriodic.Target, statPeriodic.AmountPerSecond);

                        if (interval <= 0f) continue;
                        if (amountPerSecond <= 0f) continue;

                        if (!streamsByTarget.TryGetValue(statPeriodic.Target, out var list))
                        {
                            list = new List<StatusEffectStatPeriodicStream>();
                            streamsByTarget[statPeriodic.Target] = list;
                        }

                        list.Add(new StatusEffectStatPeriodicStream(statPeriodic, runtimeStatusEffect, interval, amountPerSecond));
                    }
                }
            }

            // Push streams to consumers
            foreach (var kvp in this.PeriodicConsumersByTarget)
            {
                var target = kvp.Key;
                var consumers = kvp.Value;

                streamsByTarget.TryGetValue(target, out var streams);
                streams ??= new List<StatusEffectStatPeriodicStream>(0);

                foreach (var consumer in consumers)
                {
                    consumer?.SetActivePeriodicStreams(streams);
                }
            }
        }

        public void ClearAndApply()
        {
            foreach (var kvp in this.PeriodicConsumersByTarget)
            {
                var consumers = kvp.Value;

                foreach (var consumer in consumers)
                {
                    consumer?.SetActivePeriodicStreams(new List<StatusEffectStatPeriodicStream>(0));
                }
            }
        }

        // --- Helpers ---
        private void HandleRuntimeChanged()
        {
            this.Recompute();
        }
        private void RegisterConsumers()
        {
            _consumersByTarget.Clear();

            if (this.StatusEffectController == null) return;

            var allBehaviours = this.StatusEffectController.GetStatusEffectConsumerBehaviours();
            for (int i = 0; i < allBehaviours.Length; i++)
            {
                var behaviour = allBehaviours[i];
                if (behaviour is not IStatusEffectStatPeriodicConsumer consumer) continue;

                // NEW: direct mapping via consumer.PeriodicTarget
                this.RegisterConsumer(consumer.Target, consumer);
            }
        }
        private void RegisterConsumer(StatusEffectStatPeriodicTarget target, IStatusEffectStatPeriodicConsumer consumer)
        {
            if (!this.PeriodicConsumersByTarget.TryGetValue(target, out var list))
            {
                list = new List<IStatusEffectStatPeriodicConsumer>();
                _consumersByTarget[target] = list;
            }

            // avoid duplicates if CacheConsumers() is called redundantly (still cheap, but safe)
            if (!list.Contains(consumer)) list.Add(consumer);
        }

        private float ClampTickInterval(float raw)
        {
            var guard = this.StatusEffectController?.GuardConfig;
            if (guard == null) return Mathf.Max(0.01f, raw);
            return Mathf.Clamp(raw, guard.MinTickInterval, guard.MaxTickInterval);
        }
        private float ClampAmountPerSecond(StatusEffectStatPeriodicTarget target, float rawValue)
        {
            var guard = this.StatusEffectController?.GuardConfig;
            if (guard == null) return rawValue;

            return target switch
            {
                StatusEffectStatPeriodicTarget.HP       => Mathf.Clamp(rawValue, float.MinValue, guard.MaxHPPerSecond),
                StatusEffectStatPeriodicTarget.FP       => Mathf.Clamp(rawValue, float.MinValue, guard.MaxFPPerSecond),
                StatusEffectStatPeriodicTarget.Stamina  => Mathf.Clamp(rawValue, float.MinValue, guard.MaxStaminaPerSecond),
                _ => rawValue,
            };
        }
    }
}
