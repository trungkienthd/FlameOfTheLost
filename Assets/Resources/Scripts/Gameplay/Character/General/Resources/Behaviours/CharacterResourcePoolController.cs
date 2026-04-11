using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterResourcePoolController : MonoBehaviour, IStatusEffectStatModifierMultiplierConsumer, IStatusEffectStatModifierAdditiveConsumer, IStatusEffectStatPeriodicConsumer
    {
        // --- Resource Pool
        [Header("Resource Pool Settings")]
        [SerializeField, Min(0)] private int _currentValue;
        [SerializeField, Min(1)] private int _maxValue = 100;
        public int CurrentValue
        {
            get => _currentValue;
            protected set
            {
                int clampedValue = Mathf.Clamp(value, 0, this.MaxValue);
                if (_currentValue == clampedValue) return;

                _currentValue = clampedValue;
                this.OnValueChanged?.Invoke(_currentValue, this.MaxValue);
            }
        }
        public int MaxValue
        {
            get => _maxValue;
            protected set => _maxValue = Mathf.Max(1, value);
        }

        // --- Status Effect Stat Modifiers
        [Header("Status Effects Stat Modifiers (General Cost)")]
        [SerializeField] private float _costMultiplier = 1.0f;
        [SerializeField] private float _costAdditiveFlat = 0f;
        public float CostMultiplier
        {
            get => _costMultiplier;
            protected set => _costMultiplier = Mathf.Max(0f, value);
        }
        public float CostAdditiveFlat
        {
            get => _costAdditiveFlat;
            protected set => _costAdditiveFlat = value;
        }

        // --- Status Effect Stat Periodic
        // Stat Periodic Target Type
        public abstract StatusEffectStatPeriodicTargetType PeriodicTarget
        {
            get;
        }

        // Stat Periodic streams injected by StatPeriodicHandler.Recompute()
        [SerializeField] private List<StatusEffectStatPeriodicStream> _activePeriodicStreams = new();
        protected List<StatusEffectStatPeriodicStream> ActiveStatPeriodicStreams => _activePeriodicStreams;

        // --- Events
        public event Action<int, int> OnValueChanged;

        // --- Unity
        protected virtual void Awake()
        {
            this.CurrentValue = this.MaxValue;
        }
        protected virtual void Update()
        {
            this.TickStatPeriodic(Time.deltaTime);
        }

        // ---Public APIs
        //  1. Compute effective cost value after modifiers
        public int ComputeEffectiveCostValue(int baseCostValue)
        {
            if (baseCostValue <= 0) return 0;

            float rawValue = baseCostValue * this.CostMultiplier + this.CostAdditiveFlat;
            return Mathf.Max(0, Mathf.CeilToInt(rawValue));
        }

        //  2. Adds immediately (e.g., item, script action). Does not change regen timers.
        public void Restore(int amount)
        {
            if (amount <= 0) return;

            this.ApplyRawDelta(+amount);
        }

        //  3. Can spend check
        public virtual bool CanSpendCost(int baseCostValue)
        {
            int effectiveCostValue = this.ComputeEffectiveCostValue(baseCostValue);
            return this.CurrentValue >= effectiveCostValue;
        }

        //  4. Spend immediately (No regen timers).
        public virtual bool TrySpendCost(int baseCostValue)
        {
            if (baseCostValue < 0) return false;   
            if (baseCostValue == 0) return true;   
            if (!this.CanSpendCost(baseCostValue)) return false;

            int effectiveCostValue = this.ComputeEffectiveCostValue(baseCostValue);
            this.CurrentValue -= effectiveCostValue;

            return true;
        }

        //  5. Direct setters
        public virtual void SetCurrentValue(int amount)
        {
            this.CurrentValue = amount;
        }

        //  6. Direct setters
        public virtual void SetMaxValue(int amount, bool keepRatio = true)
        {
            amount = Mathf.Max(1, amount);
            if (amount == this.MaxValue) return;

            float ratio = (this.MaxValue > 0) ? (float)this.CurrentValue / this.MaxValue : 1f;
            this.MaxValue = amount;

            if (keepRatio)
            {
                this.CurrentValue = Mathf.RoundToInt(ratio * this.MaxValue);
            }
            else
            {
                this.CurrentValue = Mathf.Min(this.CurrentValue, this.MaxValue);
            }
        }

        //  7. SE Stat Modifier APIs
        public abstract void SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType target, float multiplier);
        public abstract void SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType target, float additiveFlat);

        //  8. SE Stat Periodic APIs
        public virtual void SetActivePeriodicStreams(IReadOnlyList<StatusEffectStatPeriodicStream> streams)
        {
            // Store a copy so each consumer owns its own accumulators
            this.ActiveStatPeriodicStreams.Clear();
            if (streams == null) return;

            for (int i = 0; i < streams.Count; i++)
            {
                var s = streams[i];
                if (s == null) continue;

                // Copy stream, accumulator starts at 0 for this consumer
                this.ActiveStatPeriodicStreams.Add(new StatusEffectStatPeriodicStream(s.StatPeriodicContribution, s.Source, s.TickInterval, s.TickAmountPerSecond));
            }
        }

        // --- Protected helpers
        //  1. Subclass-specific application of a stat periodic tick
        protected abstract void ApplyStatPeriodicTick(StatusEffectStatPeriodicContribution periodic, int tickAmount, RuntimeStatusEffect source);

        //  2. Ticking stat periodic streams
        protected virtual void TickStatPeriodic(float dt)
        {
            if (dt <= 0f) return;
            if (this.ActiveStatPeriodicStreams == null || this.ActiveStatPeriodicStreams.Count == 0) return;

            for (int i = 0; i < this.ActiveStatPeriodicStreams.Count; i++)
            {
                var stream = this.ActiveStatPeriodicStreams[i];
                if (stream == null) continue;

                // Accumulate
                stream.Accumulator += dt;

                // Tick as needed
                while (stream.Accumulator >= stream.TickInterval)
                {
                    stream.Accumulator -= stream.TickInterval;

                    float rawTick = stream.TickAmountPerSecond * stream.TickInterval;
                    int tickAmount = Mathf.CeilToInt(rawTick);
                    if (tickAmount <= 0) continue;

                    // Route to subclass-specific application (existing logic)
                    this.ApplyStatPeriodicTick(stream.StatPeriodicContribution, tickAmount, stream.Source);
                }
            }
        }

        //  3. Applies a raw signed delta directly to the resource pool.
        //      Positive = restore/regen, Negative = drain/cost.
        //      No damage or side effects are applied here.
        protected void ApplyRawDelta(int delta)
        {
            if (delta == 0) return;

            int newValue = this.CurrentValue + delta;
            newValue = Mathf.Clamp(newValue, 0, this.MaxValue);

            if (newValue == this.CurrentValue) return;

            this.CurrentValue = newValue;
        }
    }
}
