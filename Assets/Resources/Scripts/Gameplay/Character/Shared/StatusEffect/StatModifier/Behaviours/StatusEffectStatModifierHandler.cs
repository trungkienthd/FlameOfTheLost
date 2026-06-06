using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FlameOfTheLost
{
    public class StatusEffectStatModifierHandler : FOTLMonoBehaviour
    {
        // --- States ---
        //  1. References
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

        //  2. Aggregate Map
        [Header("SE Modifier Aggregate Map (Runtime)")]
        [SerializeField] private StatusEffectStatModifierTargetToAccumulatorDictionary _modifierAggregateMap = new StatusEffectStatModifierTargetToAccumulatorDictionary();

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

            this.ClearAggregateMap();

            // (Target, Operation) -> accumulated bucket state
            var buckets = new Dictionary<(StatusEffectStatModifierTarget target, StatusEffectStatModifierOperationType operation), StatusEffectStatModifierBucketState>();

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

        public void ClearAndApply()
        {
            this.ClearAggregateMap();
            this.ApplyAllAggregates();
        }

        // --- Helpers ---
        //  1. Cache all modifier consumers on this GameObject and its children
        private void RegisterConsumers()
        {
            this.MultiplierConsumers = new List<IStatusEffectStatModifierMultiplierConsumer>();
            this.AdditiveConsumers = new List<IStatusEffectStatModifierAdditiveConsumer>();
            this.VirtualHPGrantedConsumer = null;

            if (this.StatusEffectController == null) return;

            var behaviours = this.StatusEffectController.GetStatusEffectConsumerBehaviours();
            foreach (var behaviour in behaviours)
            {
                if (behaviour is IStatusEffectStatModifierMultiplierConsumer    multiplierConsumer) this.MultiplierConsumers.Add(multiplierConsumer);
                if (behaviour is IStatusEffectStatModifierAdditiveConsumer      additiveConsumer)   this.AdditiveConsumers.Add(additiveConsumer);
                if (behaviour is IVirtualHPGrantedConsumer                      virtualHPConsumer)  this.VirtualHPGrantedConsumer = virtualHPConsumer;
            }
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
            _modifierAggregateMap.Clear();
        }
        //      3.2. Accumulate new modifier into aggregate map
        private void Accumulate(StatusEffectStatModifierTarget target, StatusEffectStatModifierOperationType operation, float value)
        {
            if (!_modifierAggregateMap.TryGetValue(target, out var channel))
            {
                channel = new StatusEffectStatModifierAccumulator();
                _modifierAggregateMap[target] = channel;
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
            float rawScale = this.GetMultiplier(StatusEffectStatModifierTarget.MovementSpeedScale);

            // Clamp using system config if available
            float clampedScale = this.ClampMultiplier(StatusEffectStatModifierTarget.MovementSpeedScale, rawScale);

            // Push to all multiplier consumers (MovementHandler will pick it up via target filter)
            for (int i = 0; i < this.MultiplierConsumers.Count; i++)
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTarget.MovementSpeedScale, clampedScale);
        }
        //     4.2. Individual Apply Methods: Damage/Knockback Dealt
        private void ApplyDamageAndKnockbackDealtModifiers()
        {
            float rawDmgMul = this.GetMultiplier(StatusEffectStatModifierTarget.DamageDealt);
            float rawDmgAdd = this.GetAdditiveFlat(StatusEffectStatModifierTarget.DamageDealt);
            float rawKbMul = this.GetMultiplier(StatusEffectStatModifierTarget.KnockbackDealt);
            float rawKbAdd = this.GetAdditiveFlat(StatusEffectStatModifierTarget.KnockbackDealt);

            float dmgMul = this.ClampMultiplier(StatusEffectStatModifierTarget.DamageDealt, rawDmgMul);
            float dmgAdd = this.ClampAddFlat(StatusEffectStatModifierTarget.DamageDealt, rawDmgAdd);
            float kbMul = this.ClampMultiplier(StatusEffectStatModifierTarget.KnockbackDealt, rawKbMul);
            float kbAdd = this.ClampAddFlat(StatusEffectStatModifierTarget.KnockbackDealt, rawKbAdd);

            for (int i = 0; i < this.MultiplierConsumers.Count; i++)
            {
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTarget.DamageDealt, dmgMul);
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTarget.KnockbackDealt, kbMul);
            }
            for (int i = 0; i < this.AdditiveConsumers.Count; i++)
            {
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTarget.DamageDealt, dmgAdd);
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTarget.KnockbackDealt, kbAdd);
            }
        }
        //     4.3. Individual Apply Methods: Damage/Knockback Taken
        private void ApplyDamageAndKnockbackTakenModifiers()
        {
            float rawDmgMul = this.GetMultiplier(StatusEffectStatModifierTarget.DamageReceived);
            float rawDmgAdd = this.GetAdditiveFlat(StatusEffectStatModifierTarget.DamageReceived);
            float rawKbMul = this.GetMultiplier(StatusEffectStatModifierTarget.KnockbackReceived);
            float rawKbAdd = this.GetAdditiveFlat(StatusEffectStatModifierTarget.KnockbackReceived);

            float dmgMul = this.ClampMultiplier(StatusEffectStatModifierTarget.DamageReceived, rawDmgMul);
            float dmgAdd = this.ClampAddFlat(StatusEffectStatModifierTarget.DamageReceived, rawDmgAdd);
            float kbMul = this.ClampMultiplier(StatusEffectStatModifierTarget.KnockbackReceived, rawKbMul);
            float kbAdd = this.ClampAddFlat(StatusEffectStatModifierTarget.KnockbackReceived, rawKbAdd);

            for (int i = 0; i < this.MultiplierConsumers.Count; i++)
            {
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTarget.DamageReceived, dmgMul);
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTarget.KnockbackReceived, kbMul);
            }

            for (int i = 0; i < this.AdditiveConsumers.Count; i++)
            {
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTarget.DamageReceived, dmgAdd);
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTarget.KnockbackReceived, kbAdd);
            }
        }
        //     4.4. Individual Apply Methods: Virtual HP Granted
        private void ApplyVirtualHPGrantedModifiers()
        {
            if (this.VirtualHPGrantedConsumer != null)
            {
                float rawVirtualHPGranted = this.GetAdditiveFlat(StatusEffectStatModifierTarget.VirtualHPGranted);

                // Clamp and convert to int
                int clampedVirtualHPGranted = Mathf.Max(0, Mathf.RoundToInt(rawVirtualHPGranted));

                this.VirtualHPGrantedConsumer?.SetVirtualHPGranted(clampedVirtualHPGranted);
            }
        }
        //     4.5. Individual Apply Methods: FP Cost & Regen
        private void ApplyFPCostModifiers()
        {
            float rawCostMul = this.GetMultiplier(StatusEffectStatModifierTarget.FPCost);
            float costMul = this.ClampMultiplier(StatusEffectStatModifierTarget.FPCost, rawCostMul);
            float costAdd = this.GetAdditiveFlat(StatusEffectStatModifierTarget.FPCost);
            for (int i = 0; i < this.MultiplierConsumers.Count; i++)
            {
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTarget.FPCost, costMul);
            }
            for (int i = 0; i < this.AdditiveConsumers.Count; i++)
            {
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTarget.FPCost, costAdd);
            }
        }
        //    4.6. Individual Apply Methods: Stamina Cost & Regen
        private void ApplyStaminaCostModifiers()
        {
            float rawCostMul = this.GetMultiplier(StatusEffectStatModifierTarget.StaminaCost);
            float costMul = this.ClampMultiplier(StatusEffectStatModifierTarget.StaminaCost, rawCostMul);
            float costAdd = this.GetAdditiveFlat(StatusEffectStatModifierTarget.StaminaCost);
            for (int i = 0; i < this.MultiplierConsumers.Count; i++)
            {
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTarget.StaminaCost, costMul);
            }
            for (int i = 0; i < this.AdditiveConsumers.Count; i++)
            {
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTarget.StaminaCost, costAdd);
            }
        }
        //    4.7. Individual Apply Methods: Stamina Regen Rate & Delay
        private void ApplyStaminaRegenRateModifiers()
        {
            float regenMul = this.GetMultiplier(StatusEffectStatModifierTarget.StaminaRegenRate);
            float rawRegenAdd = this.GetAdditiveFlat(StatusEffectStatModifierTarget.StaminaRegenRate);
            float regenAdd = this.ClampAddFlat(StatusEffectStatModifierTarget.StaminaRegenRate, rawRegenAdd);
            for (int i = 0; i < this.MultiplierConsumers.Count; i++)
            {
                this.MultiplierConsumers[i]?.SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTarget.StaminaRegenRate, regenMul);
            }
            for (int i = 0; i < this.AdditiveConsumers.Count; i++)
            {
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTarget.StaminaRegenRate, regenAdd);
            }
        }
        //    4.8. Individual Apply Methods: Stamina Regen Delay Delta
        private void ApplyStaminaRegenDelayDelta()
        {
            float rawDelayAdd = this.GetAdditiveFlat(StatusEffectStatModifierTarget.StaminaRegenDelayDelta);
            float delayAdd = this.ClampAddFlat(StatusEffectStatModifierTarget.StaminaRegenDelayDelta, rawDelayAdd);
            for (int i = 0; i < this.AdditiveConsumers.Count; i++)
            {
                this.AdditiveConsumers[i]?.SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTarget.StaminaRegenDelayDelta, delayAdd);
            }
        }

        //  5. Aggregate read helpers
        //    5.1. Multiplier Helper
        private float GetMultiplier(StatusEffectStatModifierTarget target)
        {
            if (!StatusEffectStatModifierUtility.ValidateOperation(target, StatusEffectStatModifierOperationType.Multiply))
            {
                Debug.LogWarning($"[StatusEffectModifiersHandler] GetMultiplier called with invalid target {target} that does not support Multiply operation.");
                return 1f;
            }

            return _modifierAggregateMap.TryGetValue(target, out var channel) ? channel.TotalMultiplier : 1f;
        }
        //    5.2. Additive Helper
        private float GetAdditiveFlat(StatusEffectStatModifierTarget target)
        {
            if (!StatusEffectStatModifierUtility.ValidateOperation(target, StatusEffectStatModifierOperationType.AddFlat))
            {
                Debug.LogWarning($"[StatusEffectModifiersHandler] GetAdditiveFlat called with invalid target {target} that does not support AddFlat operation.");
                return 0f;
            }

            return _modifierAggregateMap.TryGetValue(target, out var channel) ? channel.TotalAdditiveFlat : 0f;
        }
        //     5.3. Clamp Multiplier Helper
        private float ClampMultiplier(StatusEffectStatModifierTarget target, float raw)
        {
            var guardConfig = this.StatusEffectController?.GuardConfig;
            if (guardConfig == null) return Mathf.Max(0f, raw);

            switch (target)
            {
                case StatusEffectStatModifierTarget.MovementSpeedScale:
                    return Mathf.Clamp(raw, guardConfig.MinMovementSpeedScale, guardConfig.MaxMovementSpeedScale);

                case StatusEffectStatModifierTarget.DamageReceived:
                    return Mathf.Max(guardConfig.MinDamageTakenScale, raw);

                case StatusEffectStatModifierTarget.DamageDealt:
                    return Mathf.Max(guardConfig.MinDamageDealtScale, raw);

                case StatusEffectStatModifierTarget.KnockbackReceived:
                    return Mathf.Max(guardConfig.MinKnockbackTakenScale, raw);

                case StatusEffectStatModifierTarget.KnockbackDealt:
                    return Mathf.Max(guardConfig.MinKnockbackDealtScale, raw);

                case StatusEffectStatModifierTarget.FPCost:
                case StatusEffectStatModifierTarget.StaminaCost:
                    return Mathf.Max(guardConfig.MinCostScale, raw);

                default:
                    return Mathf.Max(0f, raw);
            }
        }
        //     5.4. Clamp Additive Helper
        private float ClampAddFlat(StatusEffectStatModifierTarget target, float raw)
        {
            var guardConfig = this.StatusEffectController?.GuardConfig;
            if (guardConfig == null) return raw;

            // Regen channels: optionally disallow negative
            if (!guardConfig.AllowNegativeRegen)
            {
                if (target == StatusEffectStatModifierTarget.StaminaRegenRate && raw < 0f) raw = 0f;
                if (target == StatusEffectStatModifierTarget.StaminaRegenDelayDelta && raw < 0f) raw = 0f;
            }
            return raw;
        }
    }
}
