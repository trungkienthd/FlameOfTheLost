using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class StatusEffectStatPeriodicStream
    {
        [SerializeField] private StatusEffectStatPeriodicContribution _periodicContribution;
        [SerializeField] private RuntimeStatusEffect _source;
        public StatusEffectStatPeriodicContribution StatPeriodicContribution
        {
            get => _periodicContribution;
            private set => _periodicContribution = value;
        }
        public RuntimeStatusEffect Source
        {
            get => _source;
            private set => _source = value;
        }

        // Pre-clamped values (handler clamps; consumer just uses them)
        [SerializeField] private float _tickInterval;
        [SerializeField] private float _tickAmountPerSecond;
        public float TickInterval
        {
            get => _tickInterval;
            private set => _tickInterval = value;
        }
        public float TickAmountPerSecond
        {
            get => _tickAmountPerSecond;
            private set => _tickAmountPerSecond = value;
        }

        // Consumer-side accumulator
        [SerializeField] private float _accumulator;
        public float Accumulator
        {
            get => _accumulator;
            set => _accumulator = value;
        }

        public StatusEffectStatPeriodicStream(StatusEffectStatPeriodicContribution periodic, RuntimeStatusEffect source, float tickInterval, float amountPerSecond)
        {
            this.StatPeriodicContribution = periodic;
            this.Source = source;
            this.TickInterval = tickInterval;
            this.TickAmountPerSecond = amountPerSecond;
            this.Accumulator = 0f;
        }
    }
}