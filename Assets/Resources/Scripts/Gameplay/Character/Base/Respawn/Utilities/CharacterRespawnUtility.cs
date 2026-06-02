using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class CharacterRespawnUtility
    {
        public static void RespawnCharacter(GameObject character, Vector3 position, Quaternion rotation, CharacterFacingStatus facing, CharacterSpawnSourceType spawnSourceType)
        {
            if (character == null) return;

            CharacterRespawnUtility.PlaceCharacter(character, position, rotation);

            CharacterRespawnUtility.ApplyFacing(character, facing);

            CharacterRespawnUtility.RefreshCharacterSystems(character, position, rotation, facing, spawnSourceType);
        }

        private static void PlaceCharacter(GameObject character, Vector3 position, Quaternion rotation)
        {
            Rigidbody2D rb = character.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.position = position;
                rb.rotation = rotation.eulerAngles.z;
            }
            else
            {
                character.transform.SetPositionAndRotation(position, rotation);
            }

            Debug.Log($"[{character.name}] Placed at position {position} with rotation {rotation.eulerAngles.z}.");
        }

        private static void ApplyFacing(GameObject character, CharacterFacingStatus facing)
        {
            CharacterVisualController visualController = character.GetComponentInChildren<CharacterVisualController>(true);
            visualController?.SetFacingParameter(facing);
        }

        private static void RefreshCharacterSystems(GameObject character, Vector3 position, Quaternion rotation, CharacterFacingStatus facing, CharacterSpawnSourceType spawnSourceType)
        {
            CharacterRespawnRefreshContext context = new CharacterRespawnRefreshContext
            (
                character: character,
                respawnPosition: position,
                respawnRotation: rotation,
                respawnFacing: facing,
                spawnSourceType: spawnSourceType
            );

            IEnumerable<ICharacterRespawnRefreshable> refreshableSystems = character
                .GetComponentsInChildren<MonoBehaviour>(true)
                .OfType<ICharacterRespawnRefreshable>()
                .OrderBy(system => system.RespawnRefreshPriority);

            foreach (ICharacterRespawnRefreshable refreshableSystem in refreshableSystems)
            {
                refreshableSystem.RefreshAfterRespawn(context);
            }
        }
    }
}
