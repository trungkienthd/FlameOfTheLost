using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class StatusEffectStatModifierContribution
    {
        [SerializeField] private StatusEffectStatModifierTargetType _target;
        [SerializeField] private StatusEffectStatModifierOperationType _operation;
        [SerializeField] private float _value;                          // Multiply: x1.2, AddFlat: +3

        public StatusEffectStatModifierTargetType Target
        {
            get => _target;
            set => _target = value;
        }
        public StatusEffectStatModifierOperationType Operation
        {
            get => _operation;
            set => _operation = value;
        }
        public float Value
        {
            get => _value;
            set => _value = value;
        }
    }
}
