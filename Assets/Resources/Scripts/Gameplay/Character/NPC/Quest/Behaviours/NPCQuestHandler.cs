using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCQuestHandler : NPCBehaviourControllerBase
    {
        // --- References ---
        [Header("References - Quest Progression")]
        [SerializeField]
        private QuestProgressController _questProgressController;
        private QuestProgressController QuestProgressController => _questProgressController;

        // --- Public APIs ---
        public NPCQuestStatus GetQuestStatus(NPCQuestProfile quest)
        {
            if (this.QuestProgressController == null) return NPCQuestStatus.Locked;

            return this.QuestProgressController.GetQuestStatus(quest);
        }

        public bool TryAcceptQuest(NPCQuestProfile quest, PlayerRootController player)
        {
            if (quest == null || player == null) return false;

            if (this.QuestProgressController == null) return false;
            if (!this.QuestProgressController.CanAcceptQuest(quest)) return false;

            NPCRewardHandler rewardHandler = this.NPCRootController?.RewardHandler;
            if (rewardHandler == null) return false;

            // Deliver the acceptance reward before marking the quest as accepted.
            if (!rewardHandler.TryGrantRewards(player, quest.AcceptanceReward)) return false;

            return this.QuestProgressController.TryAcceptQuest(quest);
        }

        public bool TryCompleteQuest(NPCQuestProfile quest, PlayerRootController player)
        {
            if (quest == null || player == null) return false;

            if (this.QuestProgressController == null) return false;
            if (!this.QuestProgressController.CanCompleteQuest(quest)) return false;

            NPCRewardHandler rewardHandler = this.NPCRootController?.RewardHandler;
            if (rewardHandler == null) return false;

            // Only resolve progression after the reward delivery has succeeded.
            if (!rewardHandler.TryGrantRewards(player, quest.CompletionReward)) return false;

            return this.QuestProgressController.TryCompleteQuest(quest);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.QuestProgressController == null)
            {
                _questProgressController = UnityEngine.Object.FindFirstObjectByType<QuestProgressController>(FindObjectsInactive.Include);
            }
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.QuestProgressController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCQuestHandler)}] {nameof(QuestProgressController)} reference is missing.", this);
            }
        }
    }
}
