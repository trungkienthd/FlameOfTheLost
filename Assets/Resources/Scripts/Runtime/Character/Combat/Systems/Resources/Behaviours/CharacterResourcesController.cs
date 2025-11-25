using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterResourcesController : MonoBehaviour
    {
        [Header("Stamina")]
        [SerializeField] private int _currentStamina;
        [SerializeField] private int _maxStamina = 100;
        [SerializeField] private float _staminaRegenPerSec = 10f;
        [SerializeField] private float _staminaRegenDelay = 1.0f;   // In seconds
        [SerializeField] private int _minStaminaReserve = 0;

        public int CurrentStamina
        {
            get => _currentStamina;
            private set => _currentStamina = Mathf.Clamp(value, 0, this.MaxStamina);
        }
        public int MaxStamina => _maxStamina;
        public float StaminaRegenPerSecond => _staminaRegenPerSec;
        public float StaminaRegenDelay => _staminaRegenDelay;
        public int MinStaminaReserve
        {
            get => _minStaminaReserve;
            set => _minStaminaReserve = Mathf.Max(0, value);
        }

        [Header("FP")]
        [SerializeField] private int _currentFP;
        [SerializeField] private int _maxFP = 100;
        [SerializeField] private float _fpRegenPerSec = 5f;
        [SerializeField] private float _fpRegenDelay = 1.25f;

        public int CurrentFP
        {
            get => _currentFP;
            private set => _currentFP = Mathf.Clamp(value, 0, this.MaxFP);
        }
        public int MaxFP => _maxFP;
        public float FPRegenPerSecond => _fpRegenPerSec;
        public float FPRegenDelay => _fpRegenDelay;

        [Header("Scales & States")]
        [SerializeField] private float _staminaCostScale = 1f;
        [SerializeField] private float _fpCostScale = 1f;
        [SerializeField] private float _exhaustedThresholdRatio = 0.1f; // 10% => exhausted
        [SerializeField] private bool _isSilenced = false;
        [SerializeField] private bool _isFatigued = false;

        public float StaminaCostScale
        {
            get => _staminaCostScale;
            private set => _staminaCostScale = value;
        }
        public float FPCostScale
        {
            get => _fpCostScale;
            private set => _fpCostScale = value;
        }
        public float ExhaustedThresholdRatio => _exhaustedThresholdRatio;
        public bool IsSilenced
        {
            get => _isSilenced;
            private set => _isSilenced = value;
        }
        public bool IsFatigued
        {
            get => _isFatigued;
            private set => _isFatigued = value;
        }
        public bool IsExhausted => this.CurrentStamina <= (this.MaxStamina * this.ExhaustedThresholdRatio);

        [Header("Status Effects Modifiers")]
        [NonSerialized] private float _staminaCostPercentAdd = 0f;          // e.g., +0.20 = +20%
        [NonSerialized] private float _fpCostPercentAdd = 0f;
        private float StaminaCostPercentAdd
        {
            get => _staminaCostPercentAdd;
            set => _staminaCostPercentAdd = value;
        }
        private float FPCostPercentAdd
        {
            get => _fpCostPercentAdd;
            set => _fpCostPercentAdd = value;
        }

        [NonSerialized] private float _staminaRegenPerSecBonus = 0f;        // e.g., +5 = +5 per second
        [NonSerialized] private float _fpRegenPerSecBonus = 0f;
        private float StaminaRegenPerSecondBonus
        {
            get => _staminaRegenPerSecBonus;
            set => _staminaRegenPerSecBonus = value;
        }
        private float FPRegenPerSecondBonus
        {
            get => _fpRegenPerSecBonus;
            set => _fpRegenPerSecBonus = value;
        }

        [NonSerialized] private float _staminaRegenDelayBonus = 0f;       // e.g., -0.5 = -0.5 seconds
        [NonSerialized] private float _fpRegenDelayBonus = 0f;
        private float StaminaRegenDelayBonus
        {
            get => _staminaRegenDelayBonus;
            set => _staminaRegenDelayBonus = value;
        }
        private float FPRegenDelayBonus
        {
            get => _fpRegenDelayBonus;
            set => _fpRegenDelayBonus = value;
        }

        [NonSerialized] private int _minStaminaReserveBonus = 0;        
        private int MinStaminaReserveBonus
        {
            get => _minStaminaReserveBonus;
            set => _minStaminaReserveBonus = value;
        }

        // Effective properties with bonuses applied
        private float EffectiveStaminaCostScale => Mathf.Max(1f + this.StaminaCostPercentAdd, 0f);
        private float EffectiveFPCostScale      => Mathf.Max(1f + this.FPCostPercentAdd, 0f);
        private float EffectiveStaminaRegenPerSecond    => Mathf.Max(this.StaminaRegenPerSecond + this.StaminaRegenPerSecondBonus, 0f);
        private float EffectiveFPRegenPerSecond         => Mathf.Max(this.FPRegenPerSecond + this.FPRegenPerSecondBonus, 0f);
        private float EffectiveStaminaRegenDelay    => Mathf.Max(this.StaminaRegenDelay + this.StaminaRegenDelayBonus, 0f);
        private float EffectiveFPRegenDelay         => Mathf.Max(this.FPRegenDelay + this.FPRegenDelayBonus, 0f);
        private int EffectiveMinStaminaReserve => Mathf.Max(this.MinStaminaReserve + this.MinStaminaReserveBonus, 0);

        // Events for HUD
        public event Action<int, int> OnStaminaChanged; // (current, max)
        public event Action<int, int> OnFPChanged;      // (current, max)

        private float _staminaRegenResumeTime;
        private float _fpRegenResumeTime;

        public float StaminaRegenResumeTime
        {
            get => _staminaRegenResumeTime;
            private set => _staminaRegenResumeTime = value;
        }
        public float FPRegenResumeTime
        {
            get => _fpRegenResumeTime;
            private set => _fpRegenResumeTime = value;
        }

        // Regen accumulators (carry fractional regen until it becomes whole)
        private float _staminaRegenAcc;
        private float _fpRegenAcc;

        private float StaminaRegenAccumulator
        {
            get => _staminaRegenAcc;
            set => _staminaRegenAcc = value;
        }
        private float FPRegenAccumulator
        {
            get => _fpRegenAcc;
            set => _fpRegenAcc = value;
        }

        // Per-second-drain accumulators for channels
        private float _staminaPayAcc;
        private float _fpPayAcc;
        private float StaminaPayAccumulator
        {
            get => _staminaPayAcc;
            set => _staminaPayAcc = value;
        }
        private float FPPayAccumulator
        {
            get => _fpPayAcc;
            set => _fpPayAcc = value;
        }

        private void Awake()
        {
            this.CurrentStamina = this.MaxStamina;
            this.CurrentFP      = this.MaxFP;

            this.StaminaRegenResumeTime = 0f;
            this.FPRegenResumeTime      = 0f;

            this.StaminaRegenAccumulator    = 0f;
            this.FPRegenAccumulator         = 0f;
            this.StaminaPayAccumulator      = 0f;
            this.FPPayAccumulator           = 0f;

            // Trigger initial events
            this.OnStaminaChanged?.Invoke(this.CurrentStamina, this.MaxStamina);
            this.OnFPChanged?.Invoke(this.CurrentFP, this.MaxFP);
        }

        private void Update()
        {
            this.TickRegen(Time.deltaTime);
        }

        // Clears all temporary status-effect overrides (does not change base stats)
        public void ResetStatusEffectResourcesOverrides()
        {
            this.StaminaCostPercentAdd      = 0f;
            this.FPCostPercentAdd           = 0f;
            this.StaminaRegenPerSecondBonus = 0f;
            this.FPRegenPerSecondBonus      = 0f;
            this.StaminaRegenDelayBonus     = 0f;
            this.FPRegenDelayBonus          = 0f;
            this.MinStaminaReserveBonus     = 0;

            // Keep legacy scales coherant
            this.StaminaCostScale   = 1f;
            this.FPCostScale        = 1f;
        }

        // Percent-add deltas from status effects. Example: +0.20 => +20%.
        public void SetCostScales(float staminaPercentAdd, float fpPercentAdd)
        {
            // Treat NaN value(s) as 0
            if (float.IsNaN(staminaPercentAdd)) staminaPercentAdd = 0f;
            if (float.IsNaN(fpPercentAdd))      fpPercentAdd = 0f;

            this.StaminaCostPercentAdd  = staminaPercentAdd;
            this.FPCostPercentAdd       = fpPercentAdd;

            // Keep legacy fields in sync for any code still using them
            this.StaminaCostScale   = this.EffectiveStaminaCostScale;
            this.FPCostScale        = this.EffectiveFPCostScale;
        }

        // Flat bonuses to regen per second and regen delay (seconds) from status effects.
        public void SetRegenAdjustments(float staminaRegenPerSecDelta, float fpRegenPerSecDelta, float staminaRegenDelayDelta, float fpRegenDelayDelta)
        {
            this.StaminaRegenPerSecondBonus = staminaRegenPerSecDelta;
            this.FPRegenPerSecondBonus      = fpRegenPerSecDelta;

            this.StaminaRegenDelayBonus = staminaRegenDelayDelta;
            this.FPRegenDelayBonus      = fpRegenDelayDelta;
        }

        // Flat bonus added to the minimum stamina reserve gate.
        public void SetMinStaminaReserveBonus(int flatBonus)
        {
            this.MinStaminaReserveBonus = flatBonus;
        }
        

        // Gate FP-using Skills
        public void SetSilenced(bool value)
        {
            this.IsSilenced = value;
        }

        // Optional: reduce stamina efficiency/regen behavior
        public void SetFatigued(bool value)
        {
            this.IsFatigued = value;
        }

        public void TickRegen(float dt)
        {
            // Optional activity multipliers (keep 1.0 for now; expose later):
            float staminaMultiplier = this.IsFatigued ? 0.6f : 1.0f;
            float fpMultiplier = 1.0f;

            if (Time.time >= this.StaminaRegenResumeTime && this.EffectiveStaminaRegenPerSecond > 0f && this.CurrentStamina < this.MaxStamina)
            {
                this.StaminaRegenAccumulator += this.EffectiveStaminaRegenPerSecond * staminaMultiplier * dt;
                int delta = Mathf.FloorToInt(this.StaminaRegenAccumulator);
                if (delta > 0)
                {
                    var staminaBefore = this.CurrentStamina;
                    this.CurrentStamina = Mathf.Min(this.CurrentStamina + delta, this.MaxStamina);
                    this.StaminaRegenAccumulator -= delta;

                    if (this.CurrentStamina != staminaBefore)
                    {
                        this.OnStaminaChanged?.Invoke(this.CurrentStamina, this.MaxStamina);
                    }
                }
            }

            if (Time.time >= this.FPRegenResumeTime && this.EffectiveFPRegenPerSecond > 0f)
            {
                this.FPRegenAccumulator += this.EffectiveFPRegenPerSecond * fpMultiplier * dt;
                int delta = Mathf.FloorToInt(this.FPRegenAccumulator);
                if (delta > 0)
                {
                    var fpBefore = this.CurrentFP;
                    this.CurrentFP = Mathf.Min(this.CurrentFP + delta, this.MaxFP);
                    this.FPRegenAccumulator -= delta;

                    if (this.CurrentFP != fpBefore)
                    {
                        this.OnFPChanged?.Invoke(this.CurrentFP, this.MaxFP);
                    }
                }
            }
        }

        public EffectiveResourceCost PreviewEffectiveResourceCost(ResourceCost cost)
        {
            float rawStamina    = cost.Stamina  + cost.StaminaPercentOfMax  * this.MaxStamina;
            float rawFP         = cost.FP       + cost.FPPercentOfMax       * this.MaxFP;

            // Apply scales (floats), then round to int to avoid 0 costs
            int finalStamina    = Mathf.Max(0, Mathf.CeilToInt(rawStamina   * this.EffectiveStaminaCostScale));
            int finalFP         = Mathf.Max(0, Mathf.CeilToInt(rawFP        * this.EffectiveFPCostScale));

            return new EffectiveResourceCost
            {
                Stamina = finalStamina,
                FP      = finalFP,
                CostMode = cost.CostMode,
                RequiresFP = cost.RequiresFP,
                PerSecondStamina    = cost.PerSecondStamina * this.EffectiveStaminaCostScale,
                PerSecondFP         = cost.PerSecondFP      * this.EffectiveFPCostScale,
            };
        }

        public bool CanAffordResourceCost(EffectiveResourceCost effectiveCost, bool enforceReserve)
        {
            if (effectiveCost.RequiresFP && this.IsSilenced)
            {
                return false;
            }

            int freeStamina = enforceReserve ? (this.CurrentStamina - this.EffectiveMinStaminaReserve) : this.CurrentStamina;
            if (freeStamina < effectiveCost.Stamina) return false;
            if (this.CurrentFP < effectiveCost.FP) return false;

            return true;
        }

        // We implement "Strict + Reserve" model:
        // - For OnStart: spend now.
        // - For OnHit/OnEnd: RESERVE now (we subtract now), then finalize later (no second charge).
        // - If canceled before finalize: refund 100% for now.
        public bool TryReserveOrSpend(EffectiveResourceCost effectiveCost)
        {
            if (!this.CanAffordResourceCost(effectiveCost, true))
            {
                return false;
            }

            if (effectiveCost.Stamina > 0)
            {
                this.CurrentStamina -= effectiveCost.Stamina;
                this.OnStaminaChanged?.Invoke(this.CurrentStamina, this.MaxStamina);
                this.StaminaRegenResumeTime = Time.time + this.EffectiveStaminaRegenDelay;
            }
            if (effectiveCost.FP > 0)
            {
                this.CurrentFP -= effectiveCost.FP;
                this.OnFPChanged?.Invoke(this.CurrentFP, this.MaxFP);
                this.FPRegenResumeTime = Time.time + this.EffectiveFPRegenDelay;
            }

            return true;
        }

        public void Refund(EffectiveResourceCost effectiveCost, float refundRatio)
        {
            refundRatio = Mathf.Clamp01(refundRatio);
            if (refundRatio <= 0f) return;

            if (effectiveCost.Stamina > 0f)
            {
                int refundAmount = Mathf.RoundToInt(effectiveCost.Stamina * refundRatio);
                if (refundAmount > 0)
                {
                    this.CurrentStamina = Mathf.Min(this.CurrentStamina + refundAmount, this.MaxStamina);
                    this.OnStaminaChanged?.Invoke(this.CurrentStamina, this.MaxStamina);
                }
            }
            if (effectiveCost.FP > 0f)
            {
                int refundAmount = Mathf.RoundToInt(effectiveCost.FP * refundRatio);
                if (refundAmount > 0)
                {
                    this.CurrentFP = Mathf.Min(this.CurrentFP + refundAmount, this.MaxFP);
                    this.OnFPChanged?.Invoke(this.CurrentFP, this.MaxFP);
                }
            }
        }

        public bool TryPayPerSecond(float dt, float staminaPerSecond, float fpPerSecond)
        {
            // Accumulate fractional costs over time
            this.StaminaPayAccumulator  += Mathf.Max(0f, staminaPerSecond)  * dt;
            this.FPPayAccumulator       += Mathf.Max(0f, fpPerSecond)       * dt;

            int staminaToPay    = Mathf.FloorToInt(this.StaminaPayAccumulator);
            int fpToPay         = Mathf.FloorToInt(this.FPPayAccumulator);

            // Nothing to pay
            if (staminaToPay == 0 && fpToPay == 0) return true;

            // Enforce reserve for stamina here
            int freeStamina = this.CurrentStamina - this.EffectiveMinStaminaReserve;
            if (freeStamina < staminaToPay || this.CurrentFP < fpToPay) return false;

            if (staminaToPay > 0)
            {
                this.CurrentStamina -= staminaToPay;
                this.StaminaPayAccumulator -= staminaToPay;
                this.OnStaminaChanged?.Invoke(this.CurrentStamina, this.MaxStamina);
                this.StaminaRegenResumeTime = Time.time + this.EffectiveStaminaRegenDelay;
            }

            if (fpToPay > 0)
            {
                this.CurrentFP -= fpToPay;
                this.FPPayAccumulator -= fpToPay;
                this.OnFPChanged?.Invoke(this.CurrentFP, this.MaxFP);
                this.FPRegenResumeTime = Time.time + this.EffectiveFPRegenDelay;
            }

            return true;
        }

        public bool IsCurrentlyAffordable(ResourceCost cost)
        {
            var effectiveCost = this.PreviewEffectiveResourceCost(cost);
            if (effectiveCost.RequiresFP && this.IsSilenced)
            {
                return false;
            }

            return this.CanAffordResourceCost(effectiveCost, true);
        }
    }
}
