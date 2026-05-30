using UnityEngine;

namespace FlameOfTheLost
{
    public class EnemyRootController : CharacterRootController
    {
        [Header("References (Enemy-only)")]
        [SerializeField] private EnemyBrainController _enemyBrainController;
        [SerializeField] private EnemyAIInputHandler _enemyAIInputHandler;
        [SerializeField] private EnemyTargetDetectionHandler _enemyTargetDetectionHandler;
        public EnemyBrainController EnemyBrainController => _enemyBrainController;
        public EnemyAIInputHandler EnemyAIInputHandler => _enemyAIInputHandler;
        public EnemyTargetDetectionHandler EnemyTargetDetectionHandler => _enemyTargetDetectionHandler;

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.EnemyBrainController == null) _enemyBrainController = this.GetComponentInChildren<EnemyBrainController>(true);
            if (this.EnemyAIInputHandler == null) _enemyAIInputHandler = this.GetComponentInChildren<EnemyAIInputHandler>(true);
            if (this.EnemyTargetDetectionHandler == null) _enemyTargetDetectionHandler = this.GetComponentInChildren<EnemyTargetDetectionHandler>(true);
        }
    }
}
