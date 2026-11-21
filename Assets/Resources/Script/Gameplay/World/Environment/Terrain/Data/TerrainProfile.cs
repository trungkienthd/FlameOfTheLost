using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Trn_New", menuName = "Data/FlameOfTheLost/World/Environment/TerrainProfile")]
    public class TerrainProfile : FOTLWorldObjectProfile
    {
        [Header("Classification")]
        [SerializeField] private TerrainType _terrainType = TerrainType.Path;
        public TerrainType TerrainType => _terrainType;

        [Header("Priority")]
        [SerializeField] private int _priority = 0;
        public int Priority => _priority;

        [Header("Rules")]
        [SerializeField] private List<TerrainStatusEffectRuleEntry> _rules = new();
        public IReadOnlyList<TerrainStatusEffectRuleEntry> Rules => _rules;

        public bool TryGetRule(CharacterTraversalType traversalType, out TerrainStatusEffectRuleEntry rule)
        {
            for (int i = 0; i < this.Rules.Count; i++)
            {
                TerrainStatusEffectRuleEntry current = this.Rules[i];
                if (current == null) continue;

                if (current.TargetTraversalType == traversalType)
                {
                    rule = current;
                    return true;
                }
            }

            rule = null;
            return false;
        }
    }
}