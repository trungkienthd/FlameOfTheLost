using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterRewardDropProvider : FOTLMonoBehaviour
    {
        [Header("Reward")]
        [SerializeField, Min(0)] private int _rewardAmount = 1;
        public int RewardAmount => _rewardAmount;

        public CharacterRewardAmount CreateRewardAmount()
        {
            return new CharacterRewardAmount(this.RewardAmount);
        }
    }
}
