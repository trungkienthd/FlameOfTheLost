using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class CharacterCapabilityRequirement
    {
        [SerializeField] private CharacterProgressionAttributeType _attributeType;
        [SerializeField, Min(0)] private int _requiredValue;
        public CharacterProgressionAttributeType AttributeType => _attributeType;
        public int RequiredValue => _requiredValue;

        public bool IsSatisfiedBy(CharacterProgressionController progressionController)
        {
            if (progressionController == null) return false;

            int currentValue = progressionController.GetAttributeValue(this.AttributeType);
            return currentValue >= this.RequiredValue;
        }
    }
}
