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
        [SerializeField] private List<StatusEffectApplicationEntry> _applications = new();

        public CharacterTraversalType TargetTraversalType => _targetTraversalType;
        public bool CanEnter => _canEnter;
        public IReadOnlyList<StatusEffectApplicationEntry> Applications => _applications;
    }
}
