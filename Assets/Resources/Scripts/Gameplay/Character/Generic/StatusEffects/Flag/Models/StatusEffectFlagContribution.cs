using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class StatusEffectFlagContribution
    {
        [SerializeField] private StatusEffectFlagType _flag = StatusEffectFlagType.None;
        [SerializeField] private bool _isEnabled = true;

        public StatusEffectFlagType Flag
        {
            get => _flag;
            set => _flag = value;
        }
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }
    }
}