using UnityEngine;

namespace FlameOfTheLost
{
    public static class CharacterRewardDistributionUtility
    {
        public static bool TryDistributeRewardFromDefeatedCharacter(CharacterRootController receiverCharacter, CharacterRootController defeatedCharacter)
        {
            if (receiverCharacter == null) return false;
            if (defeatedCharacter == null) return false;

            CharacterAIRootController defeatedAICharacter = defeatedCharacter as CharacterAIRootController;
            if (defeatedAICharacter == null) return false;

            EnemyRootController defeatedEnemy = defeatedAICharacter as EnemyRootController;

            CharacterRewardDropProvider defeatedRewardDropProvider = defeatedEnemy.RewardDropProvider;
            if (defeatedRewardDropProvider == null) return false;

            CharacterRewardWalletController receiverRewardWallet = receiverCharacter.RewardWalletController;
            if (receiverRewardWallet == null) return false;

            if (CharacterRewardDistributionUtility.TryDistributeReward(defeatedRewardDropProvider, receiverRewardWallet))
            {
                receiverCharacter?.RewardVFXHandler?.PlayGainVFX();
                defeatedCharacter?.RewardVFXHandler?.PlayReleaseVFX();

                return true;
            }

            return false;
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
