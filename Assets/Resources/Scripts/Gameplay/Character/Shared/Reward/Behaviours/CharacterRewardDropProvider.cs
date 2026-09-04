using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterRewardDropProvider : CharacterConfiguredBehaviourBase<CharacterRewardDropProfile>
    {
        // --- Data / Configs ---
        public CharacterRewardDropProfile DropProfile => this.Config;

        public int RewardAmount => this.DropProfile != null ? this.DropProfile.Amount : 0;
        public bool HasReward => this.RewardAmount > 0;

        // --- FOTLMonoBehavior Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.DropProfile == null) Debug.LogError($"[{this.CharacterName}.{this.GetType().Name}] No Drop Profile assigned. Please assign a valid Drop Profile.", this);
        }
    }
}
