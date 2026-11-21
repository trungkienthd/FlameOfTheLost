using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCRewardHandler : NPCBehaviourControllerBase
    {
        // --- References ---
        [Header("References - Reward Grant Service")]
        [SerializeField] private MonoBehaviour _rewardGrantServiceBehaviour;
        private INPCRewardGrantService RewardGrantService => _rewardGrantServiceBehaviour as INPCRewardGrantService;

        // --- Events ---
        public event Action<PlayerRootController, NPCQuestRewardEntry> RewardGranted;

        // --- Public APIs ---
        public bool TryGrantRewards(PlayerRootController player, NPCQuestRewardProfile reward)
        {
            if (player == null) return false;

            if (reward == null) return true;

            GameConditionController conditions = this.GetGameConditionController();
            if (conditions == null) return false;

            var entries = reward.Entries;
            if (entries == null) return true;

            // Ensure required dependencies are present before attempting any delivery.
            foreach (NPCQuestRewardEntry entry in entries)
            {
                if (entry == null) return false;
                if (entry.Amount <= 0) return false;

                if (conditions.GetConditionState(entry.ClaimedCondition)) continue;

                if (this.RewardGrantService == null)
                {
                    Debug.LogError($"[{this.gameObject.name}.{nameof(NPCRewardHandler)}] Reward grant service is missing.", this);

                    return false;
                }
            }

            foreach (NPCQuestRewardEntry entry in entries)
            {
                if (conditions.GetConditionState(entry.ClaimedCondition)) continue;

                bool granted = this.RewardGrantService.TryGrantReward(player, entry);
                if (!granted) return false;

                conditions.SatisfyCondition(entry.ClaimedCondition);

                this.RewardGranted?.Invoke(player, entry);
            }

            return true;
        }

        // --- Helpers ---
        private GameConditionController GetGameConditionController()
        {
            return FindFirstObjectByType<GameConditionController>(FindObjectsInactive.Include);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (_rewardGrantServiceBehaviour != null && this.RewardGrantService == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(NPCRewardHandler)}] Assigned reward service does not implement {nameof(INPCRewardGrantService)}.", this);
            }
        }
    }
}
