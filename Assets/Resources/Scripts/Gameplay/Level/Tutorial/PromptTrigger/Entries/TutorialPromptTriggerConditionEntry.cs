using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class TutorialPromptTriggerConditionEntry
    {
        [SerializeField] private TutorialPromptTriggerConditionKey _conditionKey = TutorialPromptTriggerConditionKey.None;
        [SerializeField] private bool _requiredValue = true;
        [SerializeField] private bool _startValue = false;
        public TutorialPromptTriggerConditionKey ConditionKey => _conditionKey;
        public bool RequiredValue => _requiredValue;
        public bool StartValue => _startValue;
    }
}
