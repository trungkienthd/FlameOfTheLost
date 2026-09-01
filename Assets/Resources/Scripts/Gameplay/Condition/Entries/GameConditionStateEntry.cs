using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class GameConditionStateEntry
    {
        [SerializeField] private GameConditionKey _conditionKey = GameConditionKey.None;
        [SerializeField] private bool _value;
        public GameConditionKey ConditionKey
        {
            get => _conditionKey;
            private set => _conditionKey = value;
        }
        public bool Value
        {
            get => _value;
            set => _value = value;
        }

        public GameConditionStateEntry(GameConditionKey conditionKey, bool value)
        {
            this.ConditionKey = conditionKey;
            this.Value = value;
        }
    }
}