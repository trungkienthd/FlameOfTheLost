using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class PromptTriggerConditionEntry
    {
        [SerializeField] private PromptTriggerConditionKey _conditionKey = PromptTriggerConditionKey.None;
        [SerializeField] private bool _requiredValue = true;
        [SerializeField] private bool _startValue = false;
        public PromptTriggerConditionKey ConditionKey => _conditionKey;
        public bool RequiredValue => _requiredValue;
        public bool StartValue => _startValue;
    }
}
