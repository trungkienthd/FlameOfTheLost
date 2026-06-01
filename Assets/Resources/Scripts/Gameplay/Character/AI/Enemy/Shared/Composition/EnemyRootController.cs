using UnityEngine;

namespace FlameOfTheLost
{
    public class EnemyRootController : CharacterAIRootController
    {
        [Header("References (Enemy-only)")]
        [SerializeField] private EnemyBrainController _enemyBrainController;
        public EnemyBrainController EnemyBrainController => _enemyBrainController;

        public EnemyInputHandler EnemyInputHandler => this.InputHandler as EnemyInputHandler;
        public EnemyRespawnHandler EnemyRespawnHandler => this.RespawnHandler as EnemyRespawnHandler;

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.EnemyBrainController == null) _enemyBrainController = this.GetComponentInChildren<EnemyBrainController>(true);
        }

        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.EnemyInputHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] InputHandler must be an EnemyInputHandler.", this);
            }

            if (this.EnemyRespawnHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] RespawnHandler must be an EnemyRespawnHandler.", this);
            }

            if (this.EnemyBrainController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{this.GetType().Name}] No EnemyBrainController assigned.", this);
            }
        }
    }
}
