using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "TerrainProfile", menuName = "Data/FlameOfTheLost/Environment/TerrainProfile")]
    public class TerrainProfile : ScriptableObject, IIdentityProvider
    {
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _key;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(5, 5)] private string _description;
        [SerializeField] private TerrainType _terrainType = TerrainType.Path;
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;
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