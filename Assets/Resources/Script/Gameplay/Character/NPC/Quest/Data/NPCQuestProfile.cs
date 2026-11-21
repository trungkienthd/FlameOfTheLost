using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "NPCQst_New", menuName = "Data/FlameOfTheLost/Character/NPC/Quest/NPCQuestProfile")]
    public class NPCQuestProfile : CharacterData, IIdentityProvider
    {
        // --- Data / Configs ---
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
        [SerializeField] private NPCQuestRewardProfile _acceptanceReward;
        [SerializeField] private NPCQuestRewardProfile _completionReward;
        public NPCQuestRewardProfile AcceptanceReward => _acceptanceReward;
        public NPCQuestRewardProfile CompletionReward => _completionReward;
    }
}