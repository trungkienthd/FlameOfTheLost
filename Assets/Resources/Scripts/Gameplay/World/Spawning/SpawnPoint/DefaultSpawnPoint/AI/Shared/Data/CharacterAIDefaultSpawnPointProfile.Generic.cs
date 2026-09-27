using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterAIDefaultSpawnPointProfile<T> : CharacterAIDefaultSpawnPointProfile
        where T : CharacterAISpawnEntry
    {
        [SerializeField] private List<T> _characterAISpawnEntries = new();
        public IReadOnlyList<T> CharacterAISpawnEntries => _characterAISpawnEntries;
    }
}