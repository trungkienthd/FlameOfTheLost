using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class NPCSpawnEntry
    {
        // --- Data / Configs ---
        //  1. NPC Profile
        [Header("Configs - NPC Profile")]
        [SerializeField] private NPCProfile _npcProfile;
        public NPCProfile NPCProfile => _npcProfile;

        //  2. Spawn Settings
        [Header("Configs - Spawn Settings")]
        [SerializeField] private bool _spawnOnStart = true;
        [SerializeField] private bool _spawnWhenPlayerRespawns = true;

        public bool SpawnOnStart => _spawnOnStart;
        public bool SpawnWhenPlayerRespawns => _spawnWhenPlayerRespawns;

        // --- Public APIs ---
        //  1. Validation
        public bool IsValid => this.NPCProfile != null && this.NPCProfile.Prefab != null;

        //  2. Spawn Result
        public CharacterRefreshTarget ResolveSpawnResult(CharacterDefaultSpawnPointController spawnPoint)
        {
            return new CharacterRefreshTarget
            (
                spawnPoint.SpawnPosition,
                spawnPoint.SpawnRotation,
                spawnPoint.DefaultFacing
            );
        }
    }
}
