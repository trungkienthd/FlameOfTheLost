using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "DSP_Chr_NPC_New", menuName = "Data/FlameOfTheLost/World/Spawning/SpawnPoints/DefaultSpawnPoints/NPC/NPCDefaultSpawnPointProfile")]
    public class NPCDefaultSpawnPointProfile : CharacterDefaultSpawnPointProfile
    {
        // --- Data / Configs ---
        //  1. NPC Spawn Entries
        [Header("Configs - NPC Spawn Entries")]
        [SerializeField] private List<NPCSpawnEntry> _npcSpawnEntries = new();
        public IReadOnlyList<NPCSpawnEntry> NPCSpawnEntries => _npcSpawnEntries;
    }
}
