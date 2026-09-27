using UnityEngine;

namespace FlameOfTheLost
{
    public class AnimalGroupSpawnerController : NeutralCharacterGroupSpawnerController<AnimalSpawnEntry>
    {
        protected override void ConfigureSpawnedNeutralCharacter(GameObject spawnedCharacter, NeutralCharacterDefaultSpawnPointController spawnPoint, AnimalSpawnEntry spawnEntry)
        {
            base.ConfigureSpawnedNeutralCharacter(spawnedCharacter, spawnPoint, spawnEntry);

            if (spawnedCharacter == null) return;
            if (spawnPoint is not AnimalDefaultSpawnPointController)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(AnimalGroupSpawnerController)}] Spawn point is not an {nameof(AnimalDefaultSpawnPointController)}.", this);
                return;
            }
            if (spawnEntry == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(AnimalGroupSpawnerController)}] Spawn entry is null.", this);
                return;
            }
            if (!spawnEntry.HomeAreaEntry.HasValidArea())
            {
                Debug.LogWarning($"[{spawnedCharacter.name}.{nameof(AnimalGroupSpawnerController)}] {nameof(AnimalHomeAreaEntry)} is not valid.", spawnedCharacter);
                return;
            }
            if (!spawnEntry.HasValidAnimalAIConfig)
            {
                Debug.LogWarning($"[{spawnedCharacter.name}.{nameof(AnimalGroupSpawnerController)}] Animal AI config is not valid.", spawnedCharacter);
                return;
            }

            AnimalBrainController animalBrainController = spawnedCharacter.GetComponentInChildren<AnimalBrainController>(true);
            if (animalBrainController == null)
            {
                Debug.LogWarning($"[{spawnedCharacter.name}.{nameof(AnimalGroupSpawnerController)}] Spawned animal has no {nameof(AnimalBrainController)}.", spawnedCharacter);
                return;
            }

            animalBrainController.ConfigureAnimalAI(spawnEntry.ResolvedRoamingConfig, spawnEntry.ResolvedFleeConfig, spawnEntry.HomeAreaEntry);
        }
    }
}