using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public sealed class TerrainStatusEffectRuleEntry
    {
        [SerializeField] private CharacterTraversalType _targetTraversalType;
        [SerializeField] private bool _canEnter = true;
        [SerializeField] private List<StatusEffectApplication> _applications = new();

        public CharacterTraversalType TargetTraversalType => _targetTraversalType;
        public bool CanEnter => _canEnter;
        public IReadOnlyList<StatusEffectApplication> Applications => _applications;
    }
}
