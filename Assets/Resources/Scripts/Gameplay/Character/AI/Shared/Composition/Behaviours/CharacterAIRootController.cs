using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterAIRootController : CharacterRootController
    {
        [Header("References (Character AI-only)")]
        [SerializeField] private CharacterAITargetDetectionHandler _targetDetectionHandler;
        public CharacterAITargetDetectionHandler TargetDetectionHandler => _targetDetectionHandler;

        [SerializeField] private CharacterRewardDropProvider _rewardDropProvider;
        public CharacterRewardDropProvider RewardDropProvider => _rewardDropProvider;

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.TargetDetectionHandler == null) _targetDetectionHandler = this.GetComponentInChildren<CharacterAITargetDetectionHandler>(true);

            if (this.RewardDropProvider == null) _rewardDropProvider = this.GetComponentInChildren<CharacterRewardDropProvider>(true);
        }

        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.TargetDetectionHandler == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] No CharacterAITargetDetectionHandler assigned.", this);
            }

            if (this.RewardDropProvider == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] No CharacterRewardDropProvider assigned.", this);
            }
        }
    }
}
