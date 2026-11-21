using UnityEngine;

namespace FlameOfTheLost
{
    public class SkirmisherEnemyGroupSpawnerController : EnemyGroupSpawnerController<SkirmisherEnemySpawnEntry>
    {
        protected override void ConfigureSpawnedEnemy(GameObject spawnedEnemy, EnemyDefaultSpawnPointController spawnPoint, SkirmisherEnemySpawnEntry spawnEntry)
        {
            base.ConfigureSpawnedEnemy(spawnedEnemy, spawnPoint, spawnEntry);

            if (spawnedEnemy == null) return;

            if (spawnPoint is not SkirmisherEnemyDefaultSpawnPointController)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(SkirmisherEnemyGroupSpawnerController)}] Spawn point is not a {nameof(SkirmisherEnemyDefaultSpawnPointController)}.", this);
                return;
            }

            if (spawnEntry == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(SkirmisherEnemyGroupSpawnerController)}] Spawn entry is null.", this);
                return;
            }
            if (!spawnEntry.HomeAreaEntry.HasValidArea())
            {
                Debug.LogWarning($"[{spawnedEnemy.name}.{nameof(SkirmisherEnemyGroupSpawnerController)}] {nameof(TerritorialEnemyHomeAreaEntry)} is not valid.", spawnedEnemy);
                return;
            }
            if (!spawnEntry.HasValidSkirmisherAIConfig)
            {
                Debug.LogWarning($"[{spawnedEnemy.name}.{nameof(SkirmisherEnemyGroupSpawnerController)}] Skirmisher AI config is not valid.", spawnedEnemy);
                return;
            }

            SkirmisherEnemyBrainController skirmisherBrainController = spawnedEnemy.GetComponentInChildren<SkirmisherEnemyBrainController>(true);
            if (skirmisherBrainController == null)
            {
                Debug.LogWarning($"[{spawnedEnemy.name}.{nameof(SkirmisherEnemyGroupSpawnerController)}] Spawned enemy has no {nameof(SkirmisherEnemyBrainController)}.", spawnedEnemy);
                return;
            }

            skirmisherBrainController.ConfigureSkirmisherAI(spawnEntry.ResolvedRoamingConfig, spawnEntry.EngagementConfig, spawnEntry.PositioningConfig, spawnEntry.CombatConfig, spawnEntry.RetreatConfig, spawnEntry.HomeAreaEntry);
        }
    }
}