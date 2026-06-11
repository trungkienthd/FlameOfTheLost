using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterRewardDropProvider : CharacterBehaviourControllerBase
    {
        [Header("Reward")]
        [SerializeField] private CharacterRewardDropProfile _dropProfile;
        public CharacterRewardDropProfile DropProfile => _dropProfile;

        public int RewardAmount => this.DropProfile != null ? this.DropProfile.Amount : 0;

        public bool HasReward => this.RewardAmount > 0;

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.DropProfile == null)
            {
                Debug.LogWarning($"[{this.CharacterName}.{this.GetType().Name}] No Drop Profile assigned. Using default profile.", this);
                _dropProfile = Resources.Load<CharacterRewardDropProfile>("Data/FlameOfTheLost/Character/Shared/Reward/RwdDrop_Generic");
            }
        }

        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.DropProfile == null) Debug.LogError($"[{this.CharacterName}.{this.GetType().Name}] No Drop Profile assigned. Please assign a valid Drop Profile.", this);
        }
    }
}
