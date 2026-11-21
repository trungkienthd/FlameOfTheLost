using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "PrgUpgrade_Character", menuName = "Data/FlameOfTheLost/Character/Shared/Progression/CharacterProgressionUpgradeConfig")]
    public class CharacterProgressionUpgradeConfig : CharacterData
    {
        [Header("Upgrade Costs")]
        [SerializeField] private List<CharacterProgressionAttributeUpgradeCostEntry> _upgradeCosts = new();
        public IReadOnlyList<CharacterProgressionAttributeUpgradeCostEntry> UpgradeCosts => _upgradeCosts;

        // --- Public APIs ---
        public CharacterProgressionAttributeUpgradeCostEntry GetUpgradeCostEntry(CharacterProgressionAttributeType attributeType)
        {
            return this.UpgradeCosts.FirstOrDefault
            (
                entry => entry != null && entry.AttributeType == attributeType
            );
        }
    }
}