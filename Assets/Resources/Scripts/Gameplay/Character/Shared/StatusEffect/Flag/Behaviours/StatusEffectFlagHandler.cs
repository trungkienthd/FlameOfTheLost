using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class StatusEffectFlagHandler : FOTLMonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private StatusEffectController _statusEffectController;
        private List<IStatusEffectFlagConsumer> _flagConsumers = new();
        private StatusEffectController StatusEffectController
        {
            get => _statusEffectController;
            set => _statusEffectController = value;
        }
        private List<IStatusEffectFlagConsumer> FlagConsumers
        {
            get => _flagConsumers;
            set => _flagConsumers = value;
        }

        //  2. Aggregated Dictionary: Flag => IsActive (boolean)
        [Header("SE Flag Map (Runtime)")]
        [SerializeField] private StatusEffectFlagActiveMap _flagActiveMap = new();

        // --- Events ---
        public event Action OnFlagsRecomputed;

        // --- Unity Callbacks ---
        private void OnDisable()
        {
            if (this.StatusEffectController == null) return;
            this.StatusEffectController.OnRuntimeListChanged -= this.HandleRuntimeChanged;
            this.StatusEffectController.OnRuntimeValuesChanged -= this.HandleRuntimeChanged;
        }
        private void OnDestroy()
        {
            if (this.StatusEffectController == null) return;
            this.StatusEffectController.OnRuntimeListChanged -= this.HandleRuntimeChanged;
            this.StatusEffectController.OnRuntimeValuesChanged -= this.HandleRuntimeChanged;
        }

        // --- Public APIs ---
        public void Bind(StatusEffectController controller)
        {
            if (controller == null) return;

            // Unbind previous (if any)
            if (this.StatusEffectController != null)
            {
                this.StatusEffectController.OnRuntimeListChanged -= this.HandleRuntimeChanged;
                this.StatusEffectController.OnRuntimeValuesChanged -= this.HandleRuntimeChanged;
            }

            this.StatusEffectController = controller;
            this.StatusEffectController.OnRuntimeListChanged += this.HandleRuntimeChanged;
            this.StatusEffectController.OnRuntimeValuesChanged += this.HandleRuntimeChanged;

            this.RegisterConsumers();
            this.Recompute();
        }

        public bool HasFlag(StatusEffectFlagTarget flag)
        {
            if (flag == StatusEffectFlagTarget.None) return false;
            return _flagActiveMap.TryGetValue(flag, out bool isActive) && isActive;
        }

        public void ClearAndApply()
        {
            _flagActiveMap.Clear();

            this.ApplyAllKnownFlagsAsInactiveToConsumers();

            this.OnFlagsRecomputed?.Invoke();
        }

        public void Recompute()
        {
            _flagActiveMap.Clear();

            if (this.StatusEffectController == null)
            {
                this.OnFlagsRecomputed?.Invoke();
                return;
            }

            var runtimeStatusEffects = this.StatusEffectController.RuntimeStatusEffects;
            if (runtimeStatusEffects == null || runtimeStatusEffects.Count <= 0)
            {
                this.ApplyAllKnownFlagsAsInactiveToConsumers();
                this.OnFlagsRecomputed?.Invoke();
                return;
            }

            foreach (var runtimeStatusEffect in runtimeStatusEffects)
            {
                if (runtimeStatusEffect.Profile == null) continue;
                if (runtimeStatusEffect.RemainingDuration <= 0f) continue;

                var flags = runtimeStatusEffect.RuntimeFlags;
                if (flags == null || flags.Count <= 0) continue;

                foreach (var flag in flags)
                {
                    if (flag == null) continue;
                    if (!flag.IsEnabled) continue;
                    if (flag.Flag == StatusEffectFlagTarget.None) continue;

                    _flagActiveMap[flag.Flag] = true;
                }
            }

            this.ApplyFlagsToConsumers();
            this.OnFlagsRecomputed?.Invoke();
        }

        // --- Helpers ---
        private void RegisterConsumers()
        {
            this.FlagConsumers = new List<IStatusEffectFlagConsumer>();

            if (this.StatusEffectController == null) return;

            var behaviours = this.StatusEffectController.GetStatusEffectConsumerBehaviours();
            foreach (var behaviour in behaviours)
            {
                if (behaviour is IStatusEffectFlagConsumer consumer) this.FlagConsumers.Add(consumer);
            }
        }
        private void HandleRuntimeChanged()
        {
            this.Recompute();
        }

        private void ApplyFlagsToConsumers()
        {
            if (this.FlagConsumers == null || this.FlagConsumers.Count == 0) return;

            foreach (var consumer in this.FlagConsumers)
            {
                foreach (StatusEffectFlagTarget flagType in Enum.GetValues(typeof(StatusEffectFlagTarget)))
                {
                    if (flagType == StatusEffectFlagTarget.None) continue;

                    bool isActive = _flagActiveMap.TryGetValue(flagType, out bool active) && active;
                    consumer.SetStatusEffectFlag(flagType, isActive);
                }
            }
        }

        private void ApplyAllKnownFlagsAsInactiveToConsumers()
        {
            if (this.FlagConsumers == null || this.FlagConsumers.Count == 0) return;

            foreach (var consumer in this.FlagConsumers)
            {
                foreach (StatusEffectFlagTarget flagType in Enum.GetValues(typeof(StatusEffectFlagTarget)))
                {
                    if (flagType == StatusEffectFlagTarget.None) continue;

                    consumer.SetStatusEffectFlag(flagType, false);
                }
            }
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.StatusEffectController == null) this.StatusEffectController = this.GetComponent<StatusEffectController>();
        }
    }
}
