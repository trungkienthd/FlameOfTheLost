using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class StatusEffectPeriodicContribution
    {
        [SerializeField] private bool _isEnabled = false;
        [SerializeField] private float _tickInterval = 1f;                  // seconds
        [SerializeField] private float _tickAmount = 0f;                    // damage ("-") or healing ("+") per tick
        [SerializeField] private bool _treatAsIncomingDamage = false;       // if true, pass through DamageTaken pipeline

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }
        public float TickInterval
        {
            get => _tickInterval;
            set => _tickInterval = Mathf.Max(0.01f, value);
        }
        public float TickAmount
        {
            get => _tickAmount;
            set => _tickAmount = value;
        }
        public bool TreatAsIncomingDamage
        {
            get => _treatAsIncomingDamage;
            set => _treatAsIncomingDamage = value;
        }
    }
}
