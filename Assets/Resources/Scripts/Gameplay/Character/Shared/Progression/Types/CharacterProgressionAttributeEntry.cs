using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class CharacterProgressionAttributeEntry
    {
        [SerializeField] private CharacterProgressionAttributeType _attributeType;
        public CharacterProgressionAttributeType AttributeType
        {
            get => _attributeType;
            private set => _attributeType = value;
        }

        [Header("Value")]
        [SerializeField, Min(0)] private int _baseValue = 1;
        [SerializeField, Min(0)] private int _bonusValue;
        public int BaseValue
        {
            get => _baseValue;
            private set => _baseValue = value;
        }
        public int BonusValue
        {
            get => _bonusValue;
            private set => _bonusValue = value;
        }
        public int CurrentValue => _baseValue + _bonusValue;

        public CharacterProgressionAttributeEntry(CharacterProgressionAttributeType attributeType, int baseValue)
        {
            this.AttributeType = attributeType;
            this.BaseValue = Mathf.Max(0, baseValue);
            this.BonusValue = 0;
        }

        public void AddBonus(int amount)
        {
            if (amount <= 0) return;

            this.BonusValue += amount;
        }

        public void SetBaseValue(int value)
        {
            this.BaseValue = Mathf.Max(0, value);
        }

        public void SetBonusValue(int value)
        {
            this.BonusValue = Mathf.Max(0, value);
        }
    }
}
