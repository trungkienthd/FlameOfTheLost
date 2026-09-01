using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class GameConditionInitialStateEntry
    {
        [SerializeField] private GameConditionKey _conditionKey = GameConditionKey.None;
        [SerializeField] private bool _initialValue = false;
        public GameConditionKey ConditionKey => _conditionKey;
        public bool InitialValue => _initialValue;
    }
}