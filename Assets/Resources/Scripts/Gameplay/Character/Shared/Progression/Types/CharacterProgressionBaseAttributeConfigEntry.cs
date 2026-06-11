using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class CharacterProgressionBaseAttributeConfigEntry
    {
        [SerializeField] private CharacterProgressionAttributeType _attributeType;
        public CharacterProgressionAttributeType AttributeType
        {
            get => _attributeType;
            private set => _attributeType = value;
        }

        [Header("Base Value")]
        [SerializeField, Min(0)] private int _baseValue;
        public int BaseValue
        {
            get => _baseValue;
            private set => _baseValue = Mathf.Max(0, value);
        }

        public CharacterProgressionBaseAttributeConfigEntry(CharacterProgressionAttributeType attributeType, int baseValue)
        {
            this.AttributeType = attributeType;
            this.BaseValue = baseValue;
        }
    }
}