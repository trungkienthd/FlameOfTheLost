using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "NPCQstRwd_New", menuName = "Data/FlameOfTheLost/Character/NPC/Quest/NPCQuestRewardProfile")]
    public class NPCQuestRewardProfile : CharacterData
    {
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _key;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(3, 6)] private string _description;
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Reward Entry
        [Header("Reward")]
        [SerializeField] private List<NPCQuestRewardEntry> _entries = new();
        public IReadOnlyList<NPCQuestRewardEntry> Entries => _entries;
    }
}
