using UnityEngine;

namespace FlameOfTheLost
{
    public class TerritorialEnemyGroupSpawnerController : EnemyGroupSpawnerController<TerritorialEnemySpawnEntry>
    {
        protected override void ConfigureSpawnedEnemy(GameObject spawnedEnemy, TerritorialEnemySpawnEntry spawnEntry)
        {
            base.ConfigureSpawnedEnemy(spawnedEnemy, spawnEntry);

            if (spawnedEnemy == null) return;

            if (spawnEntry == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(TerritorialEnemyGroupSpawnerController)}] Spawn entry is null.", this);
                return;
            }

            if (!spawnEntry.HomeAreaEntry.HasValidArea())
            {
                Debug.LogWarning($"[{spawnedEnemy.name}.{nameof(TerritorialEnemyGroupSpawnerController)}] {nameof(TerritorialEnemyHomeAreaEntry)} is not valid.", spawnedEnemy);
                return;
            }

            if (!spawnEntry.HasValidTerritorialAIConfig)
            {
                Debug.LogWarning($"[{spawnedEnemy.name}.{nameof(TerritorialEnemyGroupSpawnerController)}] Territorial AI config is not valid.", spawnedEnemy);
                return;
            }

            TerritorialEnemyBrainController territorialBrainController = spawnedEnemy.GetComponentInChildren<TerritorialEnemyBrainController>(true);
            if (territorialBrainController == null)
            {
                Debug.LogWarning($"[{spawnedEnemy.name}.{nameof(TerritorialEnemyGroupSpawnerController)}] Spawned enemy has no {nameof(TerritorialEnemyBrainController)}.", spawnedEnemy);
                return;
            }

            territorialBrainController.ConfigureTerritorialAI
            (
                spawnEntry.RoamingConfig,
                spawnEntry.AttackConfig,
                spawnEntry.HomeAreaEntry
            );
        }
    }
}
