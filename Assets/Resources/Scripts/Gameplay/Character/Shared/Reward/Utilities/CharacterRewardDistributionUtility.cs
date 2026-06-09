using UnityEngine;

namespace FlameOfTheLost
{
    public static class CharacterRewardDistributionUtility
    {
        public static bool TryDistributeRewardFromDefeatedCharacter(CharacterRootController receiverCharacter, CharacterRootController defeatedCharacter)
        {
            if (receiverCharacter == null) return false;
            if (defeatedCharacter == null) return false;

            CharacterRewardDropProvider defeatedRewardDropProvider = defeatedCharacter.RewardDropProvider;
            if (defeatedRewardDropProvider == null) return false;

            CharacterRewardWalletController receiverRewardWallet = receiverCharacter.RewardWalletController;
            if (receiverRewardWallet == null) return false;

            CharacterRewardAmount rewardAmount = defeatedRewardDropProvider.CreateRewardAmount();
            if (rewardAmount == null || rewardAmount.Amount <= 0) return false;

            receiverRewardWallet.Add(rewardAmount.Amount);

            return true;
        }

        public static bool TryDistributeReward(CharacterRewardDropProvider dropProvider, CharacterRewardWalletController walletController)
        {
            if (dropProvider == null) return false;
            if (walletController == null) return false;

            CharacterRewardAmount rewardAmount = dropProvider.CreateRewardAmount();
            if (rewardAmount == null || rewardAmount.Amount <= 0) return false;

            walletController.Add(rewardAmount.Amount);

            return true;
        }
    }
}
