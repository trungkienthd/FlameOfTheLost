using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class StatusEffectFlagHandler : MonoBehaviour
    {
        // --- References
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

        // --- Aggregated Dictionary: Flag => IsActive (boolean)
        private readonly Dictionary<StatusEffectFlagType, bool> _flagActiveMap = new();
        private Dictionary<StatusEffectFlagType, bool> FlagActiveMap => _flagActiveMap;

        // --- Events
        public event Action OnFlagsRecomputed;

        // --- Unity Methods
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

        // --- Public APIs
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

            this.Recompute();
        }

        public bool HasFlag(StatusEffectFlagType flag)
        {
            if (flag == StatusEffectFlagType.None) return false;
            return this.FlagActiveMap.TryGetValue(flag, out bool isActive) && isActive;
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

        // --- Private helpers
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
                foreach (var kvp in this.FlagActiveMap)
                {
                    var flagType = kvp.Key;
                    var isActive = kvp.Value;
                    consumer.SetStatusEffectFlag(flagType, isActive);
                }
            }
        }
    }
}
