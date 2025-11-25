using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class RuntimeStatusEffect       // A runtime state of the corresponding StatusEffectProfile
    {
        public uint RuntimeID
        {
            get;
            internal set;
        }
        public StatusEffectProfile Profile
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

        private int _stacks = 1;
        public int CurrentStacks               // Number of stacks applied
        {
            get => _stacks;
            internal set
            {
                _stacks = Mathf.Max(1, value);
                this.InvalidateCache();
            }
        }
        public float RemainingDuration  // Time remaining until expiration
        {
            get;
            internal set;
        }

        // Over-time bookkeeping
        private float _tickTimer = 0f;
        public float TickTimer
        {
            get => _tickTimer;
            internal set => _tickTimer = value;
        }

        private List<StatusEffectModifierContribution> _scaledModifiers;
        private StatusEffectPeriodicContribution _scaledPeriodic;

        public List<StatusEffectModifierContribution> ScaledModifiers
        {
            get => _scaledModifiers ?? StatusEffectUtility.BuildScaledContribution(this.Profile.Modifiers, this.Potency, this.CurrentStacks);
            private set => _scaledModifiers = value;
        }
        public StatusEffectPeriodicContribution ScaledPeriodic
        {
            get => _scaledPeriodic ?? StatusEffectUtility.BuildScaledContribution(this.Profile.Periodic, this.Potency, this.CurrentStacks);
            private set => _scaledPeriodic = value;
        }

        internal void InvalidateCache()
        {
            this.ScaledModifiers    = null;
            this.ScaledPeriodic     = null;
        }
    }
}
