using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class StatusEffectAggregates
    {
        [Header("Flags")]
        [SerializeField] private bool _silenced;
        [SerializeField] private bool _fatigued;
        [SerializeField] private bool _stunned;

        public bool Silenced
        {
            get => _silenced;
            set => _silenced = value;
        }
        public bool Fatigued
        {
            get => _fatigued;
            set => _fatigued = value;
        }
        public bool Stunned
        {
            get => _stunned;
            set => _stunned = value;
        }

        [Header("Scalars")]
        // Percent-add scales for costs (interpret as +X where X is fractional, e.g., 0.20 = +20%)
        [SerializeField] private float _staminaCostScale;           
        [SerializeField] private float _fpCostScale;

        // Flat floor bonus to minimum stamina reserve before spending
        [SerializeField] private int _minStaminaReserveFlat; 

        // Flat regen/delay deltas (+= value)
        [SerializeField] private float _staminaRegenPerSecondDelta;
        [SerializeField] private float _fpRegenPerSecondDelta;
        [SerializeField] private float _staminaRegenDelayDelta;
        [SerializeField] private float _fpRegenDelayDelta;
        public float StaminaCostScale
        {
            get => _staminaCostScale;
            set => _staminaCostScale = value;
        }
        public float FPCostScale
        {
            get => _fpCostScale;
            set => _fpCostScale = value;
        }
        public int MinStaminaReserveFlat
        {
            get => _minStaminaReserveFlat;   
            set => _minStaminaReserveFlat = value;
        }
        public float StaminaRegenPerSecondDelta
        {
            get => _staminaRegenPerSecondDelta;
            set => _staminaRegenPerSecondDelta = value;
        }
        public float FPRegenPerSecondDelta
        {
            get => _fpRegenPerSecondDelta;
            set => _fpRegenPerSecondDelta = value;
        }

        public float StaminaRegenDelayDelta
        {
            get => _staminaRegenDelayDelta;
            set => _staminaRegenDelayDelta = value;
        }
        public float FPRegenDelayDelta
        {
            get => _fpRegenDelayDelta;
            set => _fpRegenDelayDelta = value;
        }

        // -------- Combat math multipliers --------
        [Header("Damage/Knockback/Health")]
        // Multipliers must default to 1
        [SerializeField] private float _damageTakenMultiplier       = 1f; 
        [SerializeField] private float _damageDealtMultiplier       = 1f;
        [SerializeField] private float _knockbackTakenMultiplier    = 1f;
        public float DamageTakenMultiplier
        {
            get => _damageTakenMultiplier;
            set => _damageTakenMultiplier = value;
        }
        public float DamageDealtMultiplier
        {
            get => _damageDealtMultiplier;
            set => _damageDealtMultiplier = value;
        }
        public float KnockbackTakenMultiplier
        {
            get => _knockbackTakenMultiplier;
            set => _knockbackTakenMultiplier = value;
        }

        // Flat max HP delta (+= value). If used, clamp current HP externally.
        [SerializeField] private float _maxHealthDelta; // Flat value, applied as (+X)
        public float MaxHealthDelta
        {
            get => _maxHealthDelta;
            set => _maxHealthDelta = value;
        }

        [Header("Movement/Poise")]
        [SerializeField] private float _moveSpeedMultiplier         = 1f;
        [SerializeField] private float _attackSpeedMultiplier       = 1f;
        [SerializeField] private float _poiseRegenDelta;
        [SerializeField] private float _poiseDamageTakenMultiplier = 1f;

        public float MoveSpeedMultiplier 
        {
            get => _moveSpeedMultiplier; 
            set => _moveSpeedMultiplier = value; 
        }
        public float AttackSpeedMultiplier 
        {
            get => _attackSpeedMultiplier; 
            set => _attackSpeedMultiplier = value; 
        }
        public float PoiseRegenDelta 
        {
            get => _poiseRegenDelta; 
            set => _poiseRegenDelta = value; 
        }
        public float PoiseDamageTakenMultiplier 
        {
            get => _poiseDamageTakenMultiplier; 
            set => _poiseDamageTakenMultiplier = value; 
        }

        public StatusEffectAggregates Clone()
        {
            return (StatusEffectAggregates)this.MemberwiseClone();
        }

        public void ResetToDefaults()
        {
            this.Silenced = this.Fatigued = this.Stunned = false;

            this.StaminaCostScale = 0f;
            this.FPCostScale = 0f;
            this.MinStaminaReserveFlat = 0;
            this.StaminaRegenPerSecondDelta = 0f;
            this.FPRegenPerSecondDelta = 0f;
            this.StaminaRegenDelayDelta = 0f;
            this.FPRegenDelayDelta = 0f;

            this.DamageTakenMultiplier = 1f;
            this.DamageDealtMultiplier = 1f;
            this.KnockbackTakenMultiplier = 1f;
            this.MaxHealthDelta = 0f;

            this.MoveSpeedMultiplier = 1f;
            this.AttackSpeedMultiplier = 1f;
            this.PoiseRegenDelta = 0f;
            this.PoiseDamageTakenMultiplier = 1f;
        }

        public static StatusEffectAggregates Default()
        {
            return new StatusEffectAggregates();
        }
    }
}
