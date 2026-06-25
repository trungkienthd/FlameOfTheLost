using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class InputPromptTriggerConditionEntry
    {
        [SerializeField] private InputPromptTriggerConditionKey _conditionKey = InputPromptTriggerConditionKey.None;
        [SerializeField] private bool _requiredValue = true;
        [SerializeField] private bool _startValue = false;
        public InputPromptTriggerConditionKey ConditionKey => _conditionKey;
        public bool RequiredValue => _requiredValue;
        public bool StartValue => _startValue;
    }
}
