using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "PrgUpgrade_Character", menuName = "Data/FlameOfTheLost/Character/Shared/Progression/CharacterProgressionUpgradeConfig")]
    public class CharacterProgressionUpgradeConfig : ScriptableObject
    {
        [Header("Upgrade Costs")]
        [SerializeField] private List<CharacterProgressionAttributeUpgradeCostEntry> _upgradeCosts = new();
        public IReadOnlyList<CharacterProgressionAttributeUpgradeCostEntry> UpgradeCosts => _upgradeCosts;
    }
}