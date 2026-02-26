using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class StaminaController : CharacterResourcePoolController
    {
        // --- Regeneration Configuration
        [Header("Regeneration")]
        [SerializeField] private float _staminaRegenRate = 5f;      // Stamina points per second
        [SerializeField] private float _staminaRegenDelay = 1.0f;   // seconds after spending before regen resumes
        public float StaminaRegenRate
        {
            get => _staminaRegenRate;
            private set => _staminaRegenRate = value;
        }
        public float StaminaRegenDelay
        {
            get => _staminaRegenDelay;
            private set => _staminaRegenDelay = value;
        }

        // --- Runtime regen state
        private float _regenResumeTime;
        private float _regenAccumulator;
        public float RegenResumeTime
        {
            get => _regenResumeTime;
            private set => _regenResumeTime = value;
        }
        public float RegenAccumulator
        {
            get => _regenAccumulator;
            private set => _regenAccumulator = value;
        }

        // --- Fatigue Configuration
        [Header("Reserve / Exhaustion")]
        [SerializeField] private float _minStaminaReserve = 0;
        [SerializeField] private float _exhaustedThresholdRatio = 0.1f; // 10% => exhausted

        public float MinStaminaReserve
        {
            get => _minStaminaReserve;
            private set => _minStaminaReserve = Mathf.Max(0f, value);
        }

        public float ExhaustedThresholdRatio
        {
            get => _exhaustedThresholdRatio;
            private set => _exhaustedThresholdRatio = Mathf.Clamp01(value);
        }

        // --- States
        [Header("States")]
        [SerializeField] private bool _isFatigued = false;
        public bool IsFatigued
        {
            get => _isFatigued;
            private set
            {
                if (_isFatigued == value) return;

                _isFatigued = value;
                this.OnFatigueStateChanged?.Invoke(_isFatigued);
            }
        }

        // --- Status Effects Stat Modifiers
        [Header("Status Effects Stat Modifier (Runtime)")]
        [SerializeField] private float _staminaRegenRateAdditiveFlat = 1f;
        [SerializeField] private float _staminaRegenDelayAdditiveFlat = 1f;
        public float StaminaRegenRateAdditiveFlat
        {
            get => _staminaRegenRateAdditiveFlat;
            private set => _staminaRegenRateAdditiveFlat = value;
        }
        public float StaminaRegenDelayAdditiveFlat
        {
            get => _staminaRegenDelayAdditiveFlat;
            private set => _staminaRegenDelayAdditiveFlat = value;
        }

        // Status Effect Stat Periodic
        public override StatusEffectStatPeriodicTargetType PeriodicTarget => StatusEffectStatPeriodicTargetType.Stamina;

        // --- Unity methods
        protected override void Awake()
        {
            base.Awake();

            // Start with regen immediately available
            this.RegenResumeTime = 0f;
            this.RegenAccumulator = 0f;

            this.EvaluateFatigueState();
        }
        protected override void Update()
        {
            base.Update();

            this.TickRegen(Time.deltaTime);
        }

        // --- Events
        public event Action<bool> OnFatigueStateChanged; // (isFatigued)

        // --- Public APIs
        //  1.1 Computes the "effective" regen parameters given base values + status effect modifiers
        public float EffectiveStaminaRegenRate => Mathf.Max(0f, this.StaminaRegenRate + this.StaminaRegenRateAdditiveFlat);
        //  1.2. Computes the "effective" regen delay given base value + status effect modifiers
        public float EffectiveRegenDelay => Mathf.Max(0f, this.StaminaRegenDelay + this.StaminaRegenDelayAdditiveFlat);

        //  2. Computes the "effective" stamina cost given the base cost: Cost = ceil( baseCost * multiplier + additiveFlat ), clamped to >= 0. True if spending cost would not violate reserve.
        public override bool CanSpendCost(int baseCostValue)
        {
            int effectiveCostValue = this.ComputeEffectiveCostValue(baseCostValue);

            // Reserve can be fractional, stamina is int
            int reserveInt = Mathf.FloorToInt(this.MinStaminaReserve);
            int after = this.CurrentValue - effectiveCostValue;

            return after >= reserveInt;
        }

        //  3. Attempts to spend stamina (with SE scaling) while respecting reserve. Regen delay timer is automatically applied.
        public override bool TrySpendCost(int baseCostValue)
        {
            if (baseCostValue < 0) return false;
            if (baseCostValue == 0) return true;
            if (!this.CanSpendCost(baseCostValue)) return false;

            int effectiveCostValue = this.ComputeEffectiveCostValue(baseCostValue);
            this.CurrentValue -= effectiveCostValue;

            // Trigger regen delay
            // spending pushes regen out
            this.RegenResumeTime = Time.time + this.EffectiveRegenDelay;
            this.RegenAccumulator = 0f; // reset accumulator

            this.EvaluateFatigueState();

            return true;
        }

        //  4. Direct Setters Overrides
        //      4.1. Current Value Setter  
        public override void SetCurrentValue(int amount)
        {
            base.SetCurrentValue(amount);

            // Regen state is automatically reset on direct set
            this.RegenResumeTime = 0f;
            this.RegenAccumulator = 0f;

            this.EvaluateFatigueState();
        }
        //     4.2. Max Value Setter
        public override void SetMaxValue(int amount, bool keepRatio = true)
        {
            base.SetMaxValue(amount, keepRatio);
            this.EvaluateFatigueState();
        }
        //     4.3. Min Stamina Reserve Setter
        public void SetMinStaminaReserve(float reserve)
        {
            this.MinStaminaReserve = reserve;
        }

        //  5. Status Effect Stat Modifier Handlers
        public override void SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType target, float multiplier)
        {
            switch (target)
            {
                case StatusEffectStatModifierTargetType.StaminaCost:
                    this.CostMultiplier = Mathf.Max(0f, multiplier);
                    break;
            }
        }
        public override void SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType target, float additiveFlat)
        {
            switch (target)
            {
                case StatusEffectStatModifierTargetType.StaminaCost:
                    this.CostAdditiveFlat = additiveFlat;
                    break;
                case StatusEffectStatModifierTargetType.StaminaRegenRate:
                    this.StaminaRegenRateAdditiveFlat = additiveFlat;
                    break;
                case StatusEffectStatModifierTargetType.StaminaRegenDelayDelta:
                    this.StaminaRegenDelayAdditiveFlat = additiveFlat;
                    break;
            }
        }

        // --- Private/Protected Helper
        private void TickRegen(float dt)
        {
            if (dt <= 0) return;
            if (this.CurrentValue >= this.MaxValue) return;

            // Wait for regen delay after spending
            if (Time.time < this.RegenResumeTime) return;

            float rate = this.EffectiveStaminaRegenRate;
            if (rate <= 0f) return;

            this.RegenAccumulator += rate * dt;

            // Convert float accumulation -> int points
            int delta = Mathf.FloorToInt(this.RegenAccumulator);
            if (delta <= 0) return;

            this.RegenAccumulator -= delta;
            this.ApplyRawDelta(+delta);
        }
        private void EvaluateFatigueState()
        {
            float threshold = this.MaxValue * this.ExhaustedThresholdRatio;
            bool newFatigueState = this.CurrentValue <= Mathf.FloorToInt(threshold);

            if (newFatigueState != this.IsFatigued)
            {
                this.IsFatigued = newFatigueState;
            }
        }
        private void ApplyRegenDelayAfterDrain()
        {
            this.RegenResumeTime = Time.time + this.EffectiveRegenDelay;
            this.RegenAccumulator = 0f;
        }

        protected override void ApplyStatPeriodicTick(StatusEffectStatPeriodicContribution periodic, int tickAmount, RuntimeStatusEffect source)
        {
            if (periodic == null) return;
            if (tickAmount <= 0) return;
            if (periodic.Target != StatusEffectStatPeriodicTargetType.Stamina) return;

            switch (periodic.Method)
            {
                case StatusEffectStatPeriodicMethodType.Drain:
                    this.ApplyRawDelta(-tickAmount);
                    if (periodic.StaminaDrainTriggersRegenDelay) this.ApplyRegenDelayAfterDrain();
                    break;

                case StatusEffectStatPeriodicMethodType.Restore:
                    this.ApplyRawDelta(+tickAmount);
                    break;

                // Not recommended, but predictable fallback:
                case StatusEffectStatPeriodicMethodType.Damage:
                    this.ApplyRawDelta(-tickAmount);
                    if (periodic.StaminaDrainTriggersRegenDelay) this.ApplyRegenDelayAfterDrain();
                    Debug.LogWarning($"[{this.gameObject.name}.StaminaController] Stamina periodic Damage used. Prefer Drain.");
                    break;

                case StatusEffectStatPeriodicMethodType.Heal:
                    this.ApplyRawDelta(+tickAmount);
                    Debug.LogWarning($"[{this.gameObject.name}.StaminaController] Stamina periodic Heal used. Prefer Restore.");
                    break;
            }

            this.EvaluateFatigueState();
        }
    }
}