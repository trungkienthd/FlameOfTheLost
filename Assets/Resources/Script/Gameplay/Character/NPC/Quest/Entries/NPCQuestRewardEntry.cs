using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class NPCQuestRewardEntry
    {
        [SerializeField] private NPCQuestRewardType _rewardType;
        [SerializeField] private ItemProfile _item; 
        [SerializeField, Min(1)] private int _amount = 1;
        [SerializeField] private GameConditionKey _claimedCondition;
        public NPCQuestRewardType RewardType => _rewardType;
        public ItemProfile Item => _item;
        public int Amount => _amount;
        public GameConditionKey ClaimedCondition => _claimedCondition;
    }
}
