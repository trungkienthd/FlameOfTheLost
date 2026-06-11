using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class CharacterProgressionAttributeRuntimeEntry
    {
        // --- States ---
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
        public int CurrentValue => this.BaseValue + this.BonusValue;

        // --- Constructors ---
        public CharacterProgressionAttributeRuntimeEntry(CharacterProgressionAttributeType attributeType, int baseValue)
        {
            this.AttributeType = attributeType;
            this.BaseValue = Mathf.Max(0, baseValue);
            this.BonusValue = 0;
        }
        public CharacterProgressionAttributeRuntimeEntry(CharacterProgressionBaseAttributeConfigEntry configEntry)
        {
            if (configEntry == null)
            {
                this.AttributeType = default;
                this.BaseValue = 0;
                this.BonusValue = 0;
                return;
            }

            this.AttributeType = configEntry.AttributeType;
            this.BaseValue = Mathf.Max(0, configEntry.BaseValue);

            // Important:
            // Bonus value is always runtime-only.
            this.BonusValue = 0;
        }

        // --- Public APIs ---
        //public void SetBaseValue(int value)
        //{
        //    this.BaseValue = Mathf.Max(0, value);
        //}

        public void AddBonus(int amount)
        {
            if (amount <= 0) return;

            this.BonusValue += amount;
        }

        public void SetBonusValue(int value)
        {
            this.BonusValue = Mathf.Max(0, value);
        }
    }
}
