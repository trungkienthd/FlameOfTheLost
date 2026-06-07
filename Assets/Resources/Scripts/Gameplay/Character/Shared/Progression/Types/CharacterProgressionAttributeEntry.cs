using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class CharacterProgressionAttributeEntry
    {
        [SerializeField] private CharacterProgressionAttributeType _attributeType;
        public CharacterProgressionAttributeType AttributeType => _attributeType;

        [Header("Value")]
        [SerializeField, Min(0)] private int _baseValue = 1;
        [SerializeField, Min(0)] private int _valuePerLevel = 1;
        public int BaseValue => _baseValue;
        public int ValuePerLevel => _valuePerLevel;
        public int EffectiveValue => this.BaseValue + (this.ValuePerLevel * this.Level);

        [Header("Upgrade")]
        [SerializeField, Min(0)] private int _level = 0;
        [SerializeField, Min(0)] private int _maxLevel = 10;
        public int Level
        {
            get => _level;
            private set => _level = Mathf.Clamp(value, 0, this.MaxLevel);
        }
        public int MaxLevel => _maxLevel;
        public bool IsMaxLevel => this.Level >= this.MaxLevel;

        [Header("Cost")]
        [SerializeField, Min(0)] private int _baseUpgradeCost = 10;
        [SerializeField, Min(0)] private int _upgradeCostPerLevel = 5;
        public int BaseUpgradeCost => _baseUpgradeCost;
        public int UpgradeCostPerLevel => _upgradeCostPerLevel;
        public int NextLevelUpgradeCost => (!this.IsMaxLevel) ? this.BaseUpgradeCost + (this.UpgradeCostPerLevel * this.Level) : 0;

        public bool TryIncreaseLevel()
        {
            if (this.IsMaxLevel) return false;

            this.Level++;
            return true;
        }

        public void SetLevel(int level)
        {
            this.Level = level;
        }
    }
}
