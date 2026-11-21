using UnityEngine;

namespace FlameOfTheLost
{
    public class HarvestableObjectGroupSpawnerController : EnvironmentObjectGroupSpawnerController<HarvestableObjectGroupSpawnerProfile, HarvestableObjectSpawnEntry>
    {
        // --- Hooks ---
        //  1. Spawned Harvestable Configuration
        protected override void ConfigureSpawnedObject(GameObject spawnedObject, HarvestableObjectSpawnEntry spawnEntry)
        {
            base.ConfigureSpawnedObject(spawnedObject, spawnEntry);

            if (spawnedObject == null) return;

            HarvestableObjectController harvestableController = spawnedObject.GetComponent<HarvestableObjectController>();
            if (harvestableController == null)
            {
                Debug.LogWarning($"[{spawnedObject.name}.{nameof(HarvestableObjectGroupSpawnerController)}] Spawned object has no {nameof(HarvestableObjectController)}.", spawnedObject);
            }
        }
    }
}