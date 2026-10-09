using UnityEngine;

namespace FlameOfTheLost
{
    public class ItemContainerGroupSpawnerController : EnvironmentObjectGroupSpawnerController<ItemContainerSpawnEntry>
    {
        // --- Hooks ---
        //  1. Spawned Item Container Configuration
        protected override void ConfigureSpawnedObject(GameObject spawnedObject, ItemContainerSpawnEntry spawnEntry)
        {
            base.ConfigureSpawnedObject(spawnedObject, spawnEntry);

            if (spawnedObject == null) return;

            ItemContainerController itemContainerController = spawnedObject.GetComponent<ItemContainerController>();
            if (itemContainerController == null)
            {
                Debug.LogWarning($"[{spawnedObject.name}.{nameof(ItemContainerGroupSpawnerController)}] Spawned object has no {nameof(ItemContainerController)}.", spawnedObject);
                return;
            }

            if (itemContainerController.Profile == null)
            {
                Debug.LogWarning($"[{spawnedObject.name}.{nameof(ItemContainerGroupSpawnerController)}] {nameof(ItemContainerProfile)} is not assigned.", spawnedObject);
            }
        }
    }
}