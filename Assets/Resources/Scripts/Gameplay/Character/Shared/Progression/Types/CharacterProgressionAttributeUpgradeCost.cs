using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class CharacterProgressionAttributeUpgradeCost
    {
        [SerializeField] private CharacterProgressionAttributeType _attributeType;
        public CharacterProgressionAttributeType AttributeType => _attributeType;

        [Header("Cost")]
        [SerializeField, Min(0)] private int _baseCost = 10;
        [SerializeField, Min(0)] private int _costIncreasePerCurrentValue = 5;
        public int BaseCost => _baseCost;
        public int CostIncreasePerCurrentValue => _costIncreasePerCurrentValue;

        public int CalculateCost(int currentAttributeValue)
        {
            return this.BaseCost + Mathf.Max(0, currentAttributeValue) * this.CostIncreasePerCurrentValue;
        }
    }
}
