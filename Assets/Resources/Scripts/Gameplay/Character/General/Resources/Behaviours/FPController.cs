using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class FPController : CharacterResourcePoolController
    {
        // --- States
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

        // Status Effect Stat Periodic
        public override StatusEffectStatPeriodicTargetType PeriodicTarget => StatusEffectStatPeriodicTargetType.FP;

        // --- Events
        public event Action<bool> OnSilenceStateChanged; // (isSilenced)

        // --- Unity methods
        protected override void Awake()
        {
            base.Awake();
        }

        // --- Public APIs
        //  1. Silence State Setter
        public void SetSilenced(bool isSilenced)
        {
            this.IsSilenced = isSilenced;
        }

        //  2. Status Effect Stat Modifier Handlers
        public override void SetStatusEffectStatModifierMultiplier(StatusEffectStatModifierTargetType target, float multiplier)
        {
            if (target == StatusEffectStatModifierTargetType.FPCost)
            {
                this.CostMultiplier = multiplier;
            }
        }
        public override void SetStatusEffectStatModifierAdditiveFlat(StatusEffectStatModifierTargetType target, float additiveFlat)
        {
            if (target == StatusEffectStatModifierTargetType.FPCost)
            {
                this.CostAdditiveFlat = additiveFlat;
            }
        }

        // --- Protected Helpers
        protected override void ApplyStatPeriodicTick(StatusEffectStatPeriodicContribution periodic, int tickAmount, RuntimeStatusEffect source)
        {
            if (periodic == null) return;
            if (tickAmount <= 0) return;
            if (periodic.Target != StatusEffectStatPeriodicTargetType.FP) return;

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