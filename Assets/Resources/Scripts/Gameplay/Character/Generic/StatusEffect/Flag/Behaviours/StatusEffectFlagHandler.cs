using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class StatusEffectFlagHandler : MonoBehaviour
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
        private readonly Dictionary<StatusEffectFlagType, bool> _flagActiveMap = new();
        private Dictionary<StatusEffectFlagType, bool> FlagActiveMap => _flagActiveMap;

        //  3. Events
        public event Action OnFlagsRecomputed;

        // --- Unity Callbacks ---
        private void Awake()
        {
            if (this.StatusEffectController == null) this.StatusEffectController = this.GetComponent<StatusEffectController>();

            this.CacheConsumers();
        }
        private void OnDisable()
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

            this.CacheConsumers();
            this.Recompute();
        }

        public bool HasFlag(StatusEffectFlagType flag)
        {
            if (flag == StatusEffectFlagType.None) return false;
            return this.FlagActiveMap.TryGetValue(flag, out bool isActive) && isActive;
        }

        public void ClearAndApply()
        {
            this.FlagActiveMap.Clear();

            this.ApplyAllKnownFlagsAsInactiveToConsumers();

            this.OnFlagsRecomputed?.Invoke();
        }

        public void Recompute()
        {
            this.FlagActiveMap.Clear();

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
                    if (flag.Flag == StatusEffectFlagType.None) continue;

                    this.FlagActiveMap[flag.Flag] = true;
                }
            }

            this.ApplyFlagsToConsumers();
            this.OnFlagsRecomputed?.Invoke();
        }

        // --- Helpers ---
        private void CacheConsumers()
        {
            this.FlagConsumers = new List<IStatusEffectFlagConsumer>();

            // Scan only once through MonoBehaviours
            var behaviours = this.GetComponentsInChildren<MonoBehaviour>(true);
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
                foreach (StatusEffectFlagType flagType in Enum.GetValues(typeof(StatusEffectFlagType)))
                {
                    if (flagType == StatusEffectFlagType.None) continue;

                    bool isActive = this.FlagActiveMap.TryGetValue(flagType, out bool active) && active;
                    consumer.SetStatusEffectFlag(flagType, isActive);
                }
            }
        }

        private void ApplyAllKnownFlagsAsInactiveToConsumers()
        {
            if (this.FlagConsumers == null || this.FlagConsumers.Count == 0) return;

            foreach (var consumer in this.FlagConsumers)
            {
                foreach (StatusEffectFlagType flagType in Enum.GetValues(typeof(StatusEffectFlagType)))
                {
                    if (flagType == StatusEffectFlagType.None) continue;

                    consumer.SetStatusEffectFlag(flagType, false);
                }
            }
        }
    }
}
