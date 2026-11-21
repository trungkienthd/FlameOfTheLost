using UnityEngine;

namespace FlameOfTheLost
{
    public static class CharacterRewardDistributionUtility
    {
        public static bool TryDistributeRewardFromDefeatedCharacter(CharacterRootController receiverCharacter, CharacterRootController defeatedCharacter)
        {
            if (receiverCharacter == null) return false;
            if (defeatedCharacter == null) return false;

            if (defeatedCharacter is not CharacterAIRootController) return false;
            if (defeatedCharacter is not ICharacterRewardDropProviderSource rewardDropSource) return false;

            CharacterRewardDropProvider defeatedRewardDropProvider = rewardDropSource.RewardDropProvider;
            if (defeatedRewardDropProvider == null) return false;

            bool distributedAnything = false;

            // a. Lost Embers
            CharacterRewardWalletController receiverRewardWallet = receiverCharacter.RewardWalletController;

            if (receiverRewardWallet != null && CharacterRewardDistributionUtility.TryDistributeReward(defeatedRewardDropProvider, receiverRewardWallet))
            {
                distributedAnything = true;
            }

            // b. Item Drops
            if (defeatedRewardDropProvider.TryDropItems())
            {
                distributedAnything = true;
            }

            // c. Reward VFX
            if (distributedAnything)
            {
                receiverCharacter.RewardVFXHandler?.PlayGainVFX();
                defeatedCharacter.RewardVFXHandler?.PlayReleaseVFX();
            }

            return distributedAnything;
        }

        public static bool TryDistributeReward(CharacterRewardDropProvider dropProvider, CharacterRewardWalletController walletController)
        {
            if (dropProvider == null) return false;
            if (walletController == null) return false;

            int rewardAmount = dropProvider.RewardAmount;
            if (rewardAmount <= 0) return false;

            walletController.Add(rewardAmount);

            return true;
        }
    }
}
