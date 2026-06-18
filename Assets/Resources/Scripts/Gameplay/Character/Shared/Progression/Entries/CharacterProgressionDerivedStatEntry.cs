using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class CharacterProgressionDerivedStatEntry
    {
        [SerializeField] private CharacterProgressionDerivedStatType _derivedStatType;
        public CharacterProgressionDerivedStatType DerivedStatType => _derivedStatType;

        [Header("Value")]
        [SerializeField] private int _baseValue;
        [SerializeField] private bool _hasMaximumValue;
        [SerializeField] private int _maximumValue;
        public int BaseValue => _baseValue;
        public bool HasMaximumValue => _hasMaximumValue;
        public int MaximumValue => _maximumValue;

        [Header("Attribute Contributions")]
        [SerializeField] private List<CharacterProgressionDerivedStatContributionEntry> _contributions = new();
        public IReadOnlyList<CharacterProgressionDerivedStatContributionEntry> Contributions => _contributions;

        public int CalculateValue(CharacterProgressionController progressionController)
        {
            int value = this.BaseValue;

            foreach (CharacterProgressionDerivedStatContributionEntry contribution in this.Contributions)
            {
                if (contribution == null) continue;

                int attributeValue = progressionController.GetAttributeValue(contribution.AttributeType);
                value += contribution.CalculateValue(attributeValue);
            }

            if (this.HasMaximumValue)
            {
                value = Mathf.Min(value, this.MaximumValue);
            }

            return Mathf.Max(0, value);
        }
    }
}
