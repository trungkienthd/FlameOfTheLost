using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class StatusEffectStatPeriodicContributionEntry
    {
        // --- States
        //  1. General
        [Header("General")]
        [SerializeField] private bool _isEnabled = false;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        //  2. Periodic Specific
        [Header("Specific")]
        [SerializeField] private StatusEffectStatPeriodicTarget _target;
        [SerializeField] private StatusEffectStatPeriodicMethodType _method;
        public StatusEffectStatPeriodicTarget Target
        {
            get => _target;
            set => _target = value;
        }
        public StatusEffectStatPeriodicMethodType Method
        {
            get => _method;
            set => _method = value;
        }

        //  3. Tick (value) Settings
        [Header("Tick settings")]
        [SerializeField, Min(0f)] private float _amountPerSecond = 1f;
        [SerializeField, Min(0.01f)] private float _tickInterval = 1f;
        public float AmountPerSecond
        {
            get => _amountPerSecond;
            set => _amountPerSecond = Mathf.Max(0f, value);
        }
        public float TickInterval
        {
            get => _tickInterval;
            set => _tickInterval = Mathf.Max(0.01f, value);
        }

        //  4. HP Application Policies
        [SerializeField] private bool _treatHPDamageAsTrueDamage = true;            // If true, this will call TakeDamage instead of HP Cost
        [SerializeField] private bool _trueDamageBypassVirtualHP = false;           // If true, this will ignore Virtual HP when applying damage. This is only relevant if TreatHPDamageAsTrueDamage is true.
        public bool TreatHPDamageAsTrueDamage
        {
            get => _treatHPDamageAsTrueDamage;
            set => _treatHPDamageAsTrueDamage = value;
        }
        public bool TrueDamageBypassVirtualHP
        {
            get => _trueDamageBypassVirtualHP;
            set => _trueDamageBypassVirtualHP = value;
        }

        //  5. Stamina Application Policy
        [SerializeField] private bool _staminaDrainTriggersRegenDelay = true; // If true, draining stamina will trigger the stamina regen delay (in StaminaController)
        public bool StaminaDrainTriggersRegenDelay
        {
            get => _staminaDrainTriggersRegenDelay;
            set => _staminaDrainTriggersRegenDelay = value;
        }

        public bool IsDamageOrDrain => this.Method == StatusEffectStatPeriodicMethodType.Damage || this.Method == StatusEffectStatPeriodicMethodType.Drain;

        // --- Public APIs ---
        public StatusEffectStatPeriodicContributionEntry Clone()
        {
            return new StatusEffectStatPeriodicContributionEntry
            {
                IsEnabled = this.IsEnabled,

                Target = this.Target,
                Method = this.Method,

                AmountPerSecond = this.AmountPerSecond,
                TickInterval = this.TickInterval,

                TreatHPDamageAsTrueDamage = this.TreatHPDamageAsTrueDamage,
                TrueDamageBypassVirtualHP = this.TrueDamageBypassVirtualHP,
                StaminaDrainTriggersRegenDelay = this.StaminaDrainTriggersRegenDelay,
            };
        }
    }
}
