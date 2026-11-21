using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Prg_Character", menuName = "Data/FlameOfTheLost/Character/Shared/Progression/CharacterProgressionConfig")]
    public class CharacterProgressionConfig : CharacterData
    {
        [Header("Attribute Display")]
        [SerializeField] private List<CharacterProgressionAttributeDisplayEntry> _attributeDisplayEntries = new();
        public IReadOnlyList<CharacterProgressionAttributeDisplayEntry> AttributeDisplayEntries => _attributeDisplayEntries;

        [Header("Initial Attributes")]
        [SerializeField] private List<CharacterProgressionBaseAttributeEntry> _baseAttributeEntries = new();
        public IReadOnlyList<CharacterProgressionBaseAttributeEntry> BaseAttributeEntries => _baseAttributeEntries;

        [Header("Derived Stats")]
        [SerializeField] private List<CharacterProgressionDerivedStatEntry> _derivedStatEntries = new();
        public IReadOnlyList<CharacterProgressionDerivedStatEntry> DerivedStatEntries => _derivedStatEntries;

        [Header("Upgrade")]
        [SerializeField] private CharacterProgressionUpgradeConfig _upgradeConfig;
        public CharacterProgressionUpgradeConfig UpgradeConfig => _upgradeConfig;

        // --- Public APIs ---
        public CharacterProgressionAttributeDisplayEntry GetAttributeDisplayEntry(CharacterProgressionAttributeType attributeType)
        {
            return this.AttributeDisplayEntries.FirstOrDefault
            (
                entry => entry != null && entry.AttributeType == attributeType
            );
        }
    }
}
