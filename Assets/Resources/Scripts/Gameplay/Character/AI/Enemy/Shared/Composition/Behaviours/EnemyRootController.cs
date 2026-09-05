using UnityEngine;

namespace FlameOfTheLost
{
    public class EnemyRootController : CharacterAIRootController<EnemyCharacterProfile>
    {
        // --- Data / Configs ---
        public EnemyCharacterProfile EnemyProfile => this.TypedProfile;

        // --- References ---
        [Header("References (Enemy-only)")]
        [SerializeField] private EnemyBrainController _enemyBrainController;
        [SerializeField] private CharacterRewardDropProvider _rewardDropProvider;
        public EnemyBrainController EnemyBrainController => _enemyBrainController;
        public CharacterRewardDropProvider RewardDropProvider => _rewardDropProvider;

        public EnemyInputHandler EnemyInputHandler => this.InputHandler as EnemyInputHandler;

        // --- Helpers ---
        private void CacheEnemyConfigs()
        {
            if (this.EnemyProfile == null) return;

            this.RewardDropProvider?.CacheConfigIfNull( this.EnemyProfile.RewardDropProfile);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.EnemyBrainController   == null) _enemyBrainController  = this.GetComponentInChildren<EnemyBrainController>(true);
            if (this.RewardDropProvider     == null) _rewardDropProvider    = this.GetComponentInChildren<CharacterRewardDropProvider>(true);

            this.CacheEnemyConfigs();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.EnemyInputHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {nameof(InputHandler)} must be an {nameof(EnemyInputHandler)}.", this);
            }

            if (this.EnemyBrainController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] No {nameof(EnemyBrainController)} assigned.", this);
            }

            if (this.RewardDropProvider == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] No {nameof(CharacterRewardDropProvider)} assigned.", this);
            }
        }
    }
}
