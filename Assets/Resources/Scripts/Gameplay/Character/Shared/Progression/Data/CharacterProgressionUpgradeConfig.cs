using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "PrgUpgrade_Character", menuName = "Data/FlameOfTheLost/Character/Shared/Progression/CharacterProgressionUpgradeConfig")]
    public class CharacterProgressionUpgradeConfig : ScriptableObject
    {
        [Header("Upgrade Costs")]
        [SerializeField] private List<CharacterProgressionAttributeUpgradeCost> _upgradeCosts = new();
        public IReadOnlyList<CharacterProgressionAttributeUpgradeCost> UpgradeCosts => _upgradeCosts;
    }
}