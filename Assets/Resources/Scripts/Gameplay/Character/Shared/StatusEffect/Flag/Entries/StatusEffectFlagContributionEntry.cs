using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class StatusEffectFlagContributionEntry
    {
        [SerializeField] private StatusEffectFlagTarget _flag = StatusEffectFlagTarget.None;
        [SerializeField] private bool _isEnabled = true;

        public StatusEffectFlagTarget Flag
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