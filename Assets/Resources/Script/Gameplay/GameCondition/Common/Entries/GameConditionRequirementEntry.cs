using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class GameConditionRequirementEntry
    {
        [SerializeField] private GameConditionKey _conditionKey = GameConditionKey.None;
        [SerializeField] private bool _requiredValue = true;
        public GameConditionKey ConditionKey => _conditionKey;
        public bool RequiredValue => _requiredValue;
    }
}
