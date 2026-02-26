using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class StatusEffectStatModifierHandler : MonoBehaviour
    {
        // --- References
        [Header("References")]
        [SerializeField] private StatusEffectController _statusEffectController;
        private List<IStatusEffectStatModifierMultiplierConsumer> _multiplierConsumers;
        private List<IStatusEffectStatModifierAdditiveConsumer> _additiveConsumers;
        private IVirtualHPGrantedConsumer _virtualHPConsumer;
        private StatusEffectController StatusEffectController
        {
            get => _statusEffectController;
            set => _statusEffectController = value;
        }
        private List<IStatusEffectStatModifierMultiplierConsumer> MultiplierConsumers
        {
            get => _multiplierConsumers;
            set => _multiplierConsumers = value;
        }
        private List<IStatusEffectStatModifierAdditiveConsumer> AdditiveConsumers
        {
            get => _additiveConsumers;
            set => _additiveConsumers = value;
        }
        private IVirtualHPGrantedConsumer VirtualHPGrantedConsumer
        {
            get => _virtualHPConsumer;
            set => _virtualHPConsumer = value;
        }

        // --- Aggregate Map
        [SerializeField] private StatModifierTargetTypeToAccumulatorDictionary _aggregateMap = new StatModifierTargetTypeToAccumulatorDictionary();
        private StatModifierTargetTypeToAccumulatorDictionary AggregateMap 
        {
            get => _aggregateMap;
        }

        // --- Unity Methods
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

        // --- Public APIs

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

            this.ClearAggregateMap();

            // (Target, Operation) -> accumulated bucket state
            var buckets = new Dictionary<(StatusEffectStatModifierTargetType target, StatusEffectStatModifierOperationType operation), StatusEffectStatModifierBucketState>();

            foreach (var runtimeStatusEffect in this.StatusEffectController.RuntimeStatusEffects)
            {
                var modifiers = runtimeStatusEffect.RuntimeScaledStatModifiers;
                if (modifiers == null) continue;

                var combinationType = runtimeStatusEffect.Profile.StatModifierCombinationType;

                foreach (var modifier in modifiers)
                {
                    // Safe-guard: skip invalid modifiers
                    if (!StatusEffectStatModifierUtility.ValidateOperation(modifier.Target, modifier.Operation)) continue;

                    var key = (modifier.Target, modifier.Operation);
                    if (!buckets.TryGetValue(key, out var state))
                    {
                        state = new StatusEffectStatModifierBucketState()
                        {
                            // Baseline depends on operation for stacking:
                            //  Multiply -> neutral 1;  AddFlat -> neutral 0
                            StackedValue = modifier.Operation == StatusEffectStatModifierOperationType.Multiply ? 1f : 0f,
                            HasAbsoluteOverridden = false,
                            AbsoluteOverriddenValue = 0f,
                        };
                    }

                    // Apply according to combination type
                    switch (combinationType)
                    {
                        default:
                        case StatusEffectStatModifierCombinationType.AddStacks:
                            if      (modifier.Operation == StatusEffectStatModifierOperationType.Multiply)  state.StackedValue *= modifier.Value;
                            else if (modifier.Operation == StatusEffectStatModifierOperationType.AddFlat)   state.StackedValue += modifier.Value;
                            break;

                        case StatusEffectStatModifierCombinationType.AbsoluteOverride:
                            state.HasAbsoluteOverridden = true;
                            state.AbsoluteOverriddenValue = modifier.Value;
                            break;
                    }  
                    
                    buckets[key] = state;
                }
            }

            // Finalize buckets into AggregateMap
            foreach (var bucket in buckets)
            {
                var (target, operation) = bucket.Key; 
                var state = bucket.Value;

                // If any AbsoluteOverride exists, we ignore stacking and use the override value.
                float combinedValue = state.HasAbsoluteOverridden ? state.AbsoluteOverriddenValue : state.StackedValue;

                this.Accumulate(target, operation, combinedValue);
            }

            this.ApplyAllAggregates();
        }

        // --- Private Helpers
        //  1. Cache all modifier consumers on this GameObject and its children
        private void CacheConsumers()
        {
            this.MultiplierConsumers = new List<IStatusEffectStatModifierMultiplierConsumer>();
            this.AdditiveConsumers = new List<IStatusEffectStatModifierAdditiveConsumer>();

            // Scan only once through MonoBehaviours
            var behaviours = this.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var behaviour in behaviours)
            {
                if (behaviour is IStatusEffectStatModifierMultiplierConsumer    multiplierConsumer) this.MultiplierConsumers.Add(multiplierConsumer);
                if (behaviour is IStatusEffectStatModifierAdditiveConsumer      additiveConsumer)   this.AdditiveConsumers.Add(additiveConsumer);
            }

            this.VirtualHPGrantedConsumer = this.GetComponentInChildren<IVirtualHPGrantedConsumer>(true);
        }

        //  2. Event Handlers
        private void HandleRuntimeChanged()
        {
            this.Recompute();
        }

        //  3. Aggregate Map Management
        //      3.1. Clear existing aggregates
        private void ClearAggregateMap()
        {
            this.AggregateMap.Clear();
        }
        //      3.2. Accumulate new modifier into aggregate map
        private void Accumulate(StatusEffectStatModifierTargetType target, StatusEffectStatModifierOperationType operation, float value)
        {
            if (!this.AggregateMap.TryGetValue(target, out var channel))
            {
                channel = new StatusEffectStatModifierAccumulator();
                this.AggregateMap[target] = channel;
            }

            if      (operation == StatusEffectStatModifierOperationType.Multiply)   channel.TotalMultiplier     *= Mathf.Max(0f, value);
            else if (operation == StatusEffectStatModifierOperationType.AddFlat)    channel.TotalAdditiveFlat   += value;
        }

        //  4. Apply all aggregates to consumers
        private void ApplyAllAggregates()
        {
            // Assumes AggregateMap is already up-to-date
            this.ApplyMovementSpeedScaleModifiers();
            this.ApplyDamageAndKnockbackDealtModifiers();
            this.ApplyDamageAndKnockbackTakenModifiers();
            this.ApplyVirtualHPGrantedModifiers();
            this.ApplyFPCostModifiers();
            this.ApplyStaminaCostModifiers();
            this.ApplyStaminaRegenRateModifiers();
            this.ApplyStaminaRegenDelayDelta();
        }
        //     4.1. Individual Apply Methods: Movement Speed
        private void ApplyMovementSpeedScaleModifiers()
        {
            float rawScale = this.GetMultiplier(StatusEffectStatModifierTargetType.MovementSpeedScale);

            // Clamp using system config if available
            float clampedScale = this.ClampMultiplier(StatusEffectStatModifierTargetType.MovementSpeedScale, rawScale);

            // Push to all multiplier consumers (MovementHandler will pick it up via target filter)
            for (int i = 0; i < this.MultiplierConsumers.Count; i++)
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType.MovementSpeedScale, clampedScale);
        }
        //     4.2. Individual Apply Methods: Damage/Knockback Dealt
        private void ApplyDamageAndKnockbackDealtModifiers()
        {
            float rawDmgMul = this.GetMultiplier(StatusEffectStatModifierTargetType.DamageDealt);
            float rawDmgAdd = this.GetAdditiveFlat(StatusEffectStatModifierTargetType.DamageDealt);
            float rawKbMul = this.GetMultiplier(StatusEffectStatModifierTargetType.KnockbackDealt);
            float rawKbAdd = this.GetAdditiveFlat(StatusEffectStatModifierTargetType.KnockbackDealt);

            float dmgMul = this.ClampMultiplier(StatusEffectStatModifierTargetType.DamageDealt, rawDmgMul);
            float dmgAdd = this.ClampAddFlat(StatusEffectStatModifierTargetType.DamageDealt, rawDmgAdd);
            float kbMul = this.ClampMultiplier(StatusEffectStatModifierTargetType.KnockbackDealt, rawKbMul);
            float kbAdd = this.ClampAddFlat(StatusEffectStatModifierTargetType.KnockbackDealt, rawKbAdd);

            for (int i = 0; i < this.MultiplierConsumers.Count; i++)
            {
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType.DamageDealt, dmgMul);
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType.KnockbackDealt, kbMul);
            }
            for (int i = 0; i < this.AdditiveConsumers.Count; i++)
            {
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType.DamageDealt, dmgAdd);
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType.KnockbackDealt, kbAdd);
            }
        }
        //     4.3. Individual Apply Methods: Damage/Knockback Taken
        private void ApplyDamageAndKnockbackTakenModifiers()
        {
            float rawDmgMul = this.GetMultiplier(StatusEffectStatModifierTargetType.DamageReceived);
            float rawDmgAdd = this.GetAdditiveFlat(StatusEffectStatModifierTargetType.DamageReceived);
            float rawKbMul = this.GetMultiplier(StatusEffectStatModifierTargetType.KnockbackReceived);
            float rawKbAdd = this.GetAdditiveFlat(StatusEffectStatModifierTargetType.KnockbackReceived);

            float dmgMul = this.ClampMultiplier(StatusEffectStatModifierTargetType.DamageReceived, rawDmgMul);
            float dmgAdd = this.ClampAddFlat(StatusEffectStatModifierTargetType.DamageReceived, rawDmgAdd);
            float kbMul = this.ClampMultiplier(StatusEffectStatModifierTargetType.KnockbackReceived, rawKbMul);
            float kbAdd = this.ClampAddFlat(StatusEffectStatModifierTargetType.KnockbackReceived, rawKbAdd);

            for (int i = 0; i < this.MultiplierConsumers.Count; i++)
            {
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType.DamageReceived, dmgMul);
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType.KnockbackReceived, kbMul);
            }

            for (int i = 0; i < this.AdditiveConsumers.Count; i++)
            {
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType.DamageReceived, dmgAdd);
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType.KnockbackReceived, kbAdd);
            }
        }
        //     4.4. Individual Apply Methods: Virtual HP Granted
        private void ApplyVirtualHPGrantedModifiers()
        {
            if (this.VirtualHPGrantedConsumer != null)
            {
                float rawVirtualHPGranted = this.GetAdditiveFlat(StatusEffectStatModifierTargetType.VirtualHPGranted);

                // Clamp and convert to int
                int clampedVirtualHPGranted = Mathf.Max(0, Mathf.RoundToInt(rawVirtualHPGranted));

                this.VirtualHPGrantedConsumer?.SetVirtualHPGranted(clampedVirtualHPGranted);
            }
        }
        //     4.5. Individual Apply Methods: FP Cost & Regen
        private void ApplyFPCostModifiers()
        {
            float rawCostMul = this.GetMultiplier(StatusEffectStatModifierTargetType.FPCost);
            float costMul = this.ClampMultiplier(StatusEffectStatModifierTargetType.FPCost, rawCostMul);
            float costAdd = this.GetAdditiveFlat(StatusEffectStatModifierTargetType.FPCost);
            for (int i = 0; i < this.MultiplierConsumers.Count; i++)
            {
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType.FPCost, costMul);
            }
            for (int i = 0; i < this.AdditiveConsumers.Count; i++)
            {
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType.FPCost, costAdd);
            }
        }
        //    4.6. Individual Apply Methods: Stamina Cost & Regen
        private void ApplyStaminaCostModifiers()
        {
            float rawCostMul = this.GetMultiplier(StatusEffectStatModifierTargetType.StaminaCost);
            float costMul = this.ClampMultiplier(StatusEffectStatModifierTargetType.StaminaCost, rawCostMul);
            float costAdd = this.GetAdditiveFlat(StatusEffectStatModifierTargetType.StaminaCost);
            for (int i = 0; i < this.MultiplierConsumers.Count; i++)
            {
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType.StaminaCost, costMul);
            }
            for (int i = 0; i < this.AdditiveConsumers.Count; i++)
            {
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType.StaminaCost, costAdd);
            }
        }
        //    4.7. Individual Apply Methods: Stamina Regen Rate & Delay
        private void ApplyStaminaRegenRateModifiers()
        {
            float regenMul = this.GetMultiplier(StatusEffectStatModifierTargetType.StaminaRegenRate);
            float rawRegenAdd = this.GetAdditiveFlat(StatusEffectStatModifierTargetType.StaminaRegenRate);
            float regenAdd = this.ClampAddFlat(StatusEffectStatModifierTargetType.StaminaRegenRate, rawRegenAdd);
            for (int i = 0; i < this.MultiplierConsumers.Count; i++)
            {
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType.StaminaRegenRate, regenMul);
            }
            for (int i = 0; i < this.AdditiveConsumers.Count; i++)
            {
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType.StaminaRegenRate, regenAdd);
            }
        }
        //    4.8. Individual Apply Methods: Stamina Regen Delay Delta
        private void ApplyStaminaRegenDelayDelta()
        {
            float rawDelayAdd = this.GetAdditiveFlat(StatusEffectStatModifierTargetType.StaminaRegenDelayDelta);
            float delayAdd = this.ClampAddFlat(StatusEffectStatModifierTargetType.StaminaRegenDelayDelta, rawDelayAdd);
            for (int i = 0; i < this.AdditiveConsumers.Count; i++)
            {
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType.StaminaRegenDelayDelta, delayAdd);
            }
        }

        //  5. Aggregate read helpers
        //    5.1. Multiplier Helper
        private float GetMultiplier(StatusEffectStatModifierTargetType target)
        {
            if (!StatusEffectStatModifierUtility.ValidateOperation(target, StatusEffectStatModifierOperationType.Multiply))
            {
                Debug.LogWarning($"[StatusEffectModifiersHandler] GetMultiplier called with invalid target {target} that does not support Multiply operation.");
                return 1f;
            }

            return this.AggregateMap.TryGetValue(target, out var channel) ? channel.TotalMultiplier : 1f;
        }
        //    5.2. Additive Helper
        private float GetAdditiveFlat(StatusEffectStatModifierTargetType target)
        {
            if (!StatusEffectStatModifierUtility.ValidateOperation(target, StatusEffectStatModifierOperationType.AddFlat))
            {
                Debug.LogWarning($"[StatusEffectModifiersHandler] GetAdditiveFlat called with invalid target {target} that does not support AddFlat operation.");
                return 0f;
            }

            return this.AggregateMap.TryGetValue(target, out var channel) ? channel.TotalAdditiveFlat : 0f;
        }
        //     5.3. Clamp Multiplier Helper
        private float ClampMultiplier(StatusEffectStatModifierTargetType target, float raw)
        {
            var guardConfig = this.StatusEffectController?.GuardConfig;
            if (guardConfig == null) return Mathf.Max(0f, raw);

            switch (target)
            {
                case StatusEffectStatModifierTargetType.MovementSpeedScale:
                    return Mathf.Clamp(raw, guardConfig.MinMovementSpeedScale, guardConfig.MaxMovementSpeedScale);

                case StatusEffectStatModifierTargetType.DamageReceived:
                    return Mathf.Max(guardConfig.MinDamageTakenScale, raw);

                case StatusEffectStatModifierTargetType.DamageDealt:
                    return Mathf.Max(guardConfig.MinDamageDealtScale, raw);

                case StatusEffectStatModifierTargetType.KnockbackReceived:
                    return Mathf.Max(guardConfig.MinKnockbackTakenScale, raw);

                case StatusEffectStatModifierTargetType.KnockbackDealt:
                    return Mathf.Max(guardConfig.MinKnockbackDealtScale, raw);

                case StatusEffectStatModifierTargetType.FPCost:
                case StatusEffectStatModifierTargetType.StaminaCost:
                    return Mathf.Max(guardConfig.MinCostScale, raw);

                default:
                    return Mathf.Max(0f, raw);
            }
        }
        //     5.4. Clamp Additive Helper
        private float ClampAddFlat(StatusEffectStatModifierTargetType target, float raw)
        {
            var guardConfig = this.StatusEffectController?.GuardConfig;
            if (guardConfig == null) return raw;

            // Regen channels: optionally disallow negative
            if (!guardConfig.AllowNegativeRegen)
            {
                if (target == StatusEffectStatModifierTargetType.StaminaRegenRate && raw < 0f) raw = 0f;
                if (target == StatusEffectStatModifierTargetType.StaminaRegenDelayDelta && raw < 0f) raw = 0f;
            }
            return raw;
        }
    }
}
