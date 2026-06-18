using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class FPController : CharacterResourcePoolController
    {
        // --- States ---
        //  1. FP-related states
        [Header("States")]
        [SerializeField] private bool _isSilenced = false;
        public bool IsSilenced
        {
            get => _isSilenced;
            private set
            {
                if (_isSilenced == value) return;

                _isSilenced = value;
                this.OnSilenceStateChanged?.Invoke(_isSilenced);
            }
        }

        //  2. Status Effect Stat Periodic
        public override StatusEffectStatPeriodicTarget Target => StatusEffectStatPeriodicTarget.FP;

        //  3. Events
        public event Action<bool> OnSilenceStateChanged; // (isSilenced)

        // --- Unity Callbacks ---
        protected override void Awake()
        {
            base.Awake();
        }

        // --- Public APIs ---
        //  1. Silence State Setter
        public void SetSilenced(bool isSilenced)
        {
            this.IsSilenced = isSilenced;
        }

        //  2. Status Effect Stat Modifier Handlers
        public override void SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTarget target, float multiplier)
        {
            if (target == StatusEffectStatModifierTarget.FPCost)
            {
                this.CostMultiplier = multiplier;
            }
        }
        public override void SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTarget target, float additiveFlat)
        {
            if (target == StatusEffectStatModifierTarget.FPCost)
            {
                this.CostAdditiveFlat = additiveFlat;
            }
        }

        //  3. Refresh Pool for Respawning
        public override void RefreshPoolAfterPlace(CharacterRefreshContext context)
        {
            base.RefreshPoolAfterPlace(context);

            this.SetSilenced(false);
        }

        // --- Helpers ---
        protected override void ApplyStatPeriodicTick(StatusEffectStatPeriodicContributionEntry periodic, int tickAmount, RuntimeStatusEffect source)
        {
            if (periodic == null) return;
            if (tickAmount <= 0) return;
            if (periodic.Target != StatusEffectStatPeriodicTarget.FP) return;

            switch (periodic.Method)
            {
                case StatusEffectStatPeriodicMethodType.Drain:
                    this.ApplyRawDelta(-tickAmount);
                    break;

                case StatusEffectStatPeriodicMethodType.Restore:
                    this.ApplyRawDelta(+tickAmount);
                    break;

                case StatusEffectStatPeriodicMethodType.Damage:
                    this.ApplyRawDelta(-tickAmount);
                    Debug.LogWarning($"[{this.gameObject.name}.FPController] FP periodic Damage used. Prefer Drain.");
                    break;

                case StatusEffectStatPeriodicMethodType.Heal:
                    this.ApplyRawDelta(+tickAmount);
                    Debug.LogWarning($"[{this.gameObject.name}.FPController] FP periodic Heal used. Prefer Restore.");
                    break;
            }
        }
    }
}