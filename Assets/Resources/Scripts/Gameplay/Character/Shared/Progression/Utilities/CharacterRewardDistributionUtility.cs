using UnityEngine;

namespace FlameOfTheLost
{
    public static class CharacterRewardDistributionUtility
    {
        public static bool TryGiveReward(CharacterRootController receiver, CharacterRootController defeatedCharacter)
        {
            if (receiver == null) return false;
            if (defeatedCharacter == null) return false;

            CharacterRewardWalletController receiverWallet = receiver.RewardWalletController;
            CharacterRewardDropProvider defeatedRewardDropProvider = defeatedCharacter.RewardDropProvider;

            if (receiverWallet == null) return false;
            if (defeatedRewardDropProvider == null) return false;

            int rewardAmount = defeatedRewardDropProvider.RewardAmount;
            if (rewardAmount <= 0) return false;

            receiverWallet.Add(rewardAmount);
            return true;
        }
    }
}
