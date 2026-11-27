using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class GameConditionOperationEntry
    {
        // --- Data / Configs ---
        [SerializeField] private GameConditionKey _conditionKey = GameConditionKey.None;
        [SerializeField] private bool _value = true;
        public GameConditionKey ConditionKey => _conditionKey;
        public bool Value => _value;
    }
}