using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class RuntimeStatusEffect       // A runtime state of the corresponding StatusEffectProfile
    {
        private StatusEffectProfile _profile;
        public StatusEffectProfile Profile
        {
            get => _profile;
            internal set => _profile = value;
        }

        public uint RuntimeID
        {
            get;
            internal set;
        }
        public GameObject Source
        {
            get;
            internal set;
        }
        private float _potency = 1f;
        public float Potency            // Scales contribution(s) of status effect
        {
            get => _potency;
            internal set
            {
                _potency = Mathf.Max(0f, value); // Clamp to non-negative
                this.InvalidateCache();
            }
        }

        private int _currentStacks = 1;
        public int CurrentStacks               // Number of stacks applied
        {
            get => _currentStacks;
            internal set
            {
                _currentStacks = Mathf.Max(1, value);
                this.InvalidateCache();
            }
        }
        public float RemainingDuration  // Time remaining until expiration
        {
            get;
            internal set;
        }

        private List<StatusEffectFlagContribution> _flags;
        private List<StatusEffectStatModifierContribution> _scaledStatModifiers;
        private List<StatusEffectStatPeriodicContribution> _scaledStatPeriodic;
        public List<StatusEffectFlagContribution> RuntimeFlags
        {
            get
            {
                if (_flags == null)
                {
                    var rawFlags = this.Profile?.Flags;
                    _flags = rawFlags != null ? new List<StatusEffectFlagContribution>(rawFlags) : new List<StatusEffectFlagContribution>(0);
                }
                return _flags;
            }
            private set => _flags = value;
        }
        public List<StatusEffectStatModifierContribution> RuntimeScaledStatModifiers
        {
            get
            {
                if (_scaledStatModifiers == null) _scaledStatModifiers = StatusEffectUtility.BuildScaledContributions(this.Profile?.StatModifiers, this.Potency, this.CurrentStacks);
                return _scaledStatModifiers;
            }
            private set => _scaledStatModifiers = value;
        }
        public List<StatusEffectStatPeriodicContribution> RuntimeScaledStatPeriodics
        {
            get
            {
                if (_scaledStatPeriodic == null) _scaledStatPeriodic = StatusEffectUtility.BuildScaledContributions(this.Profile?.StatPeriodics, this.Potency, this.CurrentStacks);
                return _scaledStatPeriodic;
            }
            private set => _scaledStatPeriodic = value;
        }

        // --- Tick accumulator for periodic status effects
        //  One accumulator per ScaledPeriodics index.
        //  Handler uses these to tick deterministically across FPS.
        private List<float> _periodicTickAccumulators;
        public List<float> PeriodicTickAccumulators
        {
            get
            {
                int count = RuntimeScaledStatPeriodics?.Count ?? 0;

                if (_periodicTickAccumulators == null || _periodicTickAccumulators.Count != count)  
                {
                    _periodicTickAccumulators = new List<float>(count);
                    for (int i = 0; i < count; i++) _periodicTickAccumulators.Add(0f);
                }
                return _periodicTickAccumulators;
            }
            private set => _periodicTickAccumulators = value;
        }

        internal void InvalidateCache()
        {
            this.RuntimeFlags = null;
            this.RuntimeScaledStatModifiers = null;
            this.RuntimeScaledStatPeriodics = null;

            // Tick accumulators must reset when scaling structure changes
            this.PeriodicTickAccumulators = null;
        }
    }
}
