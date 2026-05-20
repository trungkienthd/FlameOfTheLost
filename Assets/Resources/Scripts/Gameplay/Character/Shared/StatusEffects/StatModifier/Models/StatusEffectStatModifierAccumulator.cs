using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class StatusEffectStatModifierAccumulator
    {
        [SerializeField] private float _totalMultiplier = 1f;
        [SerializeField] private float _totalAdditiveFlat = 0f;

        public float TotalMultiplier
        {
            get => _totalMultiplier;
            set => _totalMultiplier = value;
        }
        public float TotalAdditiveFlat
        {
            get => _totalAdditiveFlat;
            set => _totalAdditiveFlat = value;
        }
    }
}