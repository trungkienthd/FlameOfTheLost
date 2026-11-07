using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class NPCDialogueActionEntry
    {
        // --- Data ---
        [SerializeField] private NPCDialogueActionType _actionType;

        [Header("Condition")]
        [SerializeField] private GameConditionKey _conditionKey;

        [SerializeField] private bool _conditionValue = true;

        [Header("Quest")]
        [SerializeField] private NPCQuestProfile _quest;

        [Header("Reward")]
        [SerializeField] private NPCRewardProfile _reward;

        // --- Public APIs ---
        public bool Execute(NPCRootController npc, PlayerRootController player)
        {
            if (npc == null || player == null) return false;

            switch (_actionType)
            {
                case NPCDialogueActionType.SetCondition:
                    {
                        GameConditionController conditions = UnityEngine.Object.FindFirstObjectByType<GameConditionController>(FindObjectsInactive.Include);
                        if (conditions == null) return false;

                        conditions.SetConditionState(_conditionKey, _conditionValue);

                        return true;
                    }

                case NPCDialogueActionType.AcceptQuest:
                    return npc.QuestHandler != null && npc.QuestHandler.TryAcceptQuest(_quest, player);

                case NPCDialogueActionType.CompleteQuest:
                    return npc.QuestHandler != null && npc.QuestHandler.TryCompleteQuest(_quest, player);

                case NPCDialogueActionType.GrantReward:
                    return npc.RewardHandler != null && npc.RewardHandler.TryGrantRewards(player, _reward);
            }

            return false;
        }
    }
}
