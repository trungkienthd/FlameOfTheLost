using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class NPCRewardEntry
    {
        [SerializeField] private NPCRewardType _rewardType;
        [SerializeField] private int _itemID; 
        [SerializeField, Min(1)] private int _amount = 1;
        [SerializeField] private GameConditionKey _claimedCondition;
        public NPCRewardType RewardType => _rewardType;
        public int ItemID => _itemID;
        public int Amount => _amount;
        public GameConditionKey ClaimedCondition => _claimedCondition;
    }
}
