using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class CharacterPlacementUtility
    {
        public static void PlaceAndRefreshCharacter(GameObject character, Vector3 position, Quaternion rotation, CharacterFacingStatus facing, CharacterPlacementSourceType spawnSourceType)
        {
            if (character == null) return;

            CharacterPlacementContext context = new CharacterPlacementContext
            (
                character: character,
                placementPosition: position,
                placementRotation: rotation,
                placementFacing: facing,
                placementSourceType: spawnSourceType
            );

            CharacterPlacementUtility.ApplyTransform(context);
            CharacterPlacementUtility.RefreshCharacterSystems(context);
        }

        private static void ApplyTransform(CharacterPlacementContext context)
        {
            Rigidbody2D rb = context.Character.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.position = context.PlacementPosition;
                rb.rotation = context.PlacementRotation.eulerAngles.z;
            }
            else
            {
                context.Character.transform.SetPositionAndRotation(context.PlacementPosition, context.PlacementRotation);
            }

            //Debug.Log($"[{context.Character.name}.{nameof(CharacterPlacementHandler)}] Placed at position {context.PlacementPosition} with rotation {context.PlacementRotation.eulerAngles.z}.");
        }

        private static void RefreshCharacterSystems(CharacterPlacementContext context)
        {
            IEnumerable<ICharacterPlacementRefreshable> refreshableSystems = context.Character
                .GetComponentsInChildren<MonoBehaviour>(true)
                .OfType<ICharacterPlacementRefreshable>()
                .OrderBy(system => system.PlacementConfigurePriority);

            foreach (ICharacterPlacementRefreshable refreshableSystem in refreshableSystems)
            {
                refreshableSystem.RefreshAfterCharacterPlacement(context);
            }
        }
    }
}
