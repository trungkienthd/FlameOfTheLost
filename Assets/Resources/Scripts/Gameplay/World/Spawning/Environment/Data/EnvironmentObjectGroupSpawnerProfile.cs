using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class EnvironmentObjectGroupSpawnerProfile<TEntry> : FOTLWorldObjectProfile where TEntry : EnvironmentObjectSpawnEntry
    {
        // --- Data / Configs ---
        //  1. Spawn Entries
        [Header("Spawn Entries")]
        [SerializeField] private List<TEntry> _spawnEntries = new();
        public IReadOnlyList<TEntry> SpawnEntries => _spawnEntries;

        //  2. Spawn Settings
        [Header("Spawn Settings")]
        [SerializeField] private bool _spawnObjectsOnStart = true;
        public bool SpawnObjectsOnStart => _spawnObjectsOnStart;
    }
}