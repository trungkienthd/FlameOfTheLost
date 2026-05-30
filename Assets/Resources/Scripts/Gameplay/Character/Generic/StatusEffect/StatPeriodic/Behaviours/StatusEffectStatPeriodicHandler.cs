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
        private readonly Dictionary<StatusEffectStatPeriodicTargetType, List<IStatusEffectStatPeriodicConsumer>> _consumersByTarget = new();
        private Dictionary<StatusEffectStatPeriodicTargetType, List<IStatusEffectStatPeriodicConsumer>> ConsumersByTarget => _consumersByTarget;

        // --- Unity Callbacks ---
        private void Awake()
        {
            if (this.StatusEffectController == null) this.StatusEffectController = this.GetComponent<StatusEffectController>();
            this.CacheConsumers();
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

            this.CacheConsumers();
            this.Recompute();
        }

        public void Recompute()
        {
            if (this.StatusEffectController == null) return;

            // Build streams per target
            var streamsByTarget = new Dictionary<StatusEffectStatPeriodicTargetType, List<StatusEffectStatPeriodicStream>>();

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
            foreach (var kvp in this.ConsumersByTarget)
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
            foreach (var kvp in this.ConsumersByTarget)
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
        private void CacheConsumers()
        {
            this.ConsumersByTarget.Clear();

            var allBehaviours = this.GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < allBehaviours.Length; i++)
            {
                var behaviour = allBehaviours[i];
                if (behaviour is not IStatusEffectStatPeriodicConsumer consumer) continue;

                // NEW: direct mapping via consumer.PeriodicTarget
                this.RegisterConsumer(consumer.PeriodicTarget, consumer);
            }
        }
        private void RegisterConsumer(StatusEffectStatPeriodicTargetType target, IStatusEffectStatPeriodicConsumer consumer)
        {
            if (!this.ConsumersByTarget.TryGetValue(target, out var list))
            {
                list = new List<IStatusEffectStatPeriodicConsumer>();
                this.ConsumersByTarget[target] = list;
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
        private float ClampAmountPerSecond(StatusEffectStatPeriodicTargetType target, float rawValue)
        {
            var guard = this.StatusEffectController?.GuardConfig;
            if (guard == null) return rawValue;

            return target switch
            {
                StatusEffectStatPeriodicTargetType.HP       => Mathf.Clamp(rawValue, float.MinValue, guard.MaxHPPerSecond),
                StatusEffectStatPeriodicTargetType.FP       => Mathf.Clamp(rawValue, float.MinValue, guard.MaxFPPerSecond),
                StatusEffectStatPeriodicTargetType.Stamina  => Mathf.Clamp(rawValue, float.MinValue, guard.MaxStaminaPerSecond),
                _ => rawValue,
            };
        }
    }
}
