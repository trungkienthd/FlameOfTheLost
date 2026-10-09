using UnityEngine;

namespace FlameOfTheLost
{
    public class CollectibleObjectGroupSpawnerController : EnvironmentObjectGroupSpawnerController<CollectibleObjectSpawnEntry>
    {
        // --- Hooks ---
        //  1. Spawned Collectible Configuration
        protected override void ConfigureSpawnedObject(GameObject spawnedObject, CollectibleObjectSpawnEntry spawnEntry)
        {
            base.ConfigureSpawnedObject(spawnedObject, spawnEntry);

            if (spawnedObject == null) return;
            if (spawnEntry == null) return;

            CollectibleObjectController collectibleController = spawnedObject.GetComponent<CollectibleObjectController>();

            if (collectibleController == null)
            {
                Debug.LogWarning($"[{spawnedObject.name}.{nameof(CollectibleObjectGroupSpawnerController)}] Spawned object has no {nameof(CollectibleObjectController)}.", spawnedObject);
                return;
            }

            // Apply the initial quantity configured in the spawn entry.
            collectibleController.ConfigureQuantity(spawnEntry.InitialQuantity);
        }
    }
}
