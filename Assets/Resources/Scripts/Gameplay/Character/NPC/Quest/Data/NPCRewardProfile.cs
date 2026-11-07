using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "NPCQstRwd_New", menuName = "Data/FlameOfTheLost/Characters/NPC/Quest/NPCRewardProfile")]
    public class NPCRewardProfile : CharacterData
    {
        [SerializeField] private List<NPCRewardEntry> _entries = new();
        public IReadOnlyList<NPCRewardEntry> Entries => _entries;
    }
}
