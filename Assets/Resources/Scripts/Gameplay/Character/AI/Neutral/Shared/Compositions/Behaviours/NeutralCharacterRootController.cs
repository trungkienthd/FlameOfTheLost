using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class NeutralCharacterRootController : CharacterAIRootController
    {
        // --- Data / Configs ---
        public abstract NeutralCharacterProfile NeutralCharacterProfile { get; }

        // --- References ---
        [Header("References (Neutral Character-only)")]
        [SerializeField] private CharacterRewardDropProvider _rewardDropProvider;
        public CharacterRewardDropProvider RewardDropProvider => _rewardDropProvider;
        public NeutralCharacterInputHandler NeutralCharacterInputHandler => this.InputHandler as NeutralCharacterInputHandler;

        // --- Helpers ---
        protected virtual void CacheNeutralCharacterConfigs()
        {
            if (this.NeutralCharacterProfile == null) return;

            this.RewardDropProvider?.CacheConfigIfNull(this.NeutralCharacterProfile.RewardDropProfile);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.RewardDropProvider == null) _rewardDropProvider = this.GetComponentInChildren<CharacterRewardDropProvider>(true);

            this.CacheNeutralCharacterConfigs();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.NeutralCharacterInputHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(this.InputHandler)} must be a {nameof(FlameOfTheLost.NeutralCharacterInputHandler)}.", this);
            }
        }
    }
}