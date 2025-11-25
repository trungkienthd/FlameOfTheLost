using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class StatusEffectModifierContribution
    {
        [SerializeField] private StatusEffectModifierTargetType _target;
        [SerializeField] private StatusEffectModifierOperationType _operation;
        [SerializeField] private float _value;                          // Multiply: x1.2, AddFlat: +3

        public StatusEffectModifierTargetType Target
        {
            get => _target;
            set => _target = value;
        }
        public StatusEffectModifierOperationType Operation
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
