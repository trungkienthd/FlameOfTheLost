using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class CharacterProgressionDerivedStatContributionEntry
    {
        [SerializeField] private CharacterProgressionAttributeType _attributeType;

        [Header("Scaling")]
        [SerializeField] private int _valuePerStep = 1;
        [SerializeField, Min(1)] private int _attributePointsPerStep = 1;

        public CharacterProgressionAttributeType AttributeType => _attributeType;
        public int ValuePerStep => _valuePerStep;
        public int AttributePointsPerStep => _attributePointsPerStep;

        public int CalculateValue(int attributeValue)
        {
            if (attributeValue <= 0) return 0;

            int steps = attributeValue / this.AttributePointsPerStep;
            return steps * this.ValuePerStep;
        }
    }
}
