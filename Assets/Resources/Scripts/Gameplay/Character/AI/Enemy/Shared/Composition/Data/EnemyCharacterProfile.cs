using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Chr_Enemy_New", menuName = "Data/FlameOfTheLost/Character/Enemy/EnemyCharacterProfile")]
    public class EnemyCharacterProfile : CharacterAIProfile
    {
        // 1. Reward
        [Header("Reward")]
        [SerializeField] private CharacterRewardDropProfile _rewardDropProfile;
        public CharacterRewardDropProfile RewardDropProfile => _rewardDropProfile;
    }
}