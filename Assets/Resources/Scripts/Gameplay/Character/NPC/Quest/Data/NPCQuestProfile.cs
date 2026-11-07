using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "NPCQst_New", menuName = "Data/FlameOfTheLost/Characters/NPC/Quest/NPCQuestProfile")]
    public class NPCQuestProfile : CharacterData
    {
        // --- Data / Configs ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _questID;
        [SerializeField] private string _questName;
        [SerializeField, TextArea(3, 6)] private string _description;
        public int QuestID => _questID;
        public string QuestName => _questName;
        public string Description => _description;

        //  2. Progression
        [Header("Progression")]
        [SerializeField] private GameConditionKey _acceptedCondition;
        [SerializeField] private GameConditionKey _completedCondition;
        public GameConditionKey AcceptedCondition => _acceptedCondition;
        public GameConditionKey CompletedCondition => _completedCondition;

        //  3. Requirements
        [Header("Requirements - Availability")]
        [SerializeField]
        private List<NPCConditionRequirementEntry> _availabilityRequirements = new();
        public IReadOnlyList<NPCConditionRequirementEntry> AvailabilityRequirements => _availabilityRequirements;

        [Header("Requirements - Completion")]
        [SerializeField] private List<NPCConditionRequirementEntry> _completionRequirements = new();
        public IReadOnlyList<NPCConditionRequirementEntry> CompletionRequirements => _completionRequirements;

        //  4. Rewards
        [Header("Rewards")]
        [SerializeField] private NPCRewardProfile _acceptanceReward;
        [SerializeField] private NPCRewardProfile _completionReward;
        public NPCRewardProfile AcceptanceReward => _acceptanceReward;
        public NPCRewardProfile CompletionReward => _completionReward;
    }
}