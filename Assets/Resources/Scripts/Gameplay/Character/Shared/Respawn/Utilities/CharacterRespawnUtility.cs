using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class CharacterRespawnUtility
    {
        public static void RespawnCharacter(GameObject character, Vector3 position, Quaternion rotation, CharacterFacingStatus facing, PlayerSpawnSourceType spawnSourceType)
        {
            if (character == null) return;

            PlaceCharacter(character, position, rotation);

            CharacterVisualController visualController = character.GetComponentInChildren<CharacterVisualController>(true);
            visualController?.SetFacingParameter(facing);

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
        }
    }
}
