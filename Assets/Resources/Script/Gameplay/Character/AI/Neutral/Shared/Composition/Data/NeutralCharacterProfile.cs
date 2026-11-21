using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class NeutralCharacterProfile : CharacterAIProfile
    {
        // 1. Reward
        [Header("Reward")]
        [SerializeField] private CharacterRewardDropProfile _rewardDropProfile;
        public CharacterRewardDropProfile RewardDropProfile => _rewardDropProfile;
    }
}
