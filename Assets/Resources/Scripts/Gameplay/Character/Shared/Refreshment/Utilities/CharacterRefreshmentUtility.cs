using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class CharacterRefreshmentUtility
    {
        public static void RefreshCharacter(CharacterRootController character, Vector3 position, Quaternion rotation, CharacterFacingStatus facing, CharacterPlacementSource placementSource)
        {
            if (character == null) return;

            CharacterRefreshmentContext context = new CharacterRefreshmentContext
            (
                character: character,
                refreshmentPosition: position,
                refreshmentRotation: rotation,
                refreshmentFacing: facing,
                placementSource: placementSource
            );

            CharacterRefreshmentUtility.ApplyTransform(context);
            CharacterRefreshmentUtility.RefreshCharacterSystems(context);
        }

        private static void ApplyTransform(CharacterRefreshmentContext context)
        {
            Rigidbody2D rb = context.Character.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.position = context.RefreshmentPosition;
                rb.rotation = context.RefreshmentRotation.eulerAngles.z;
            }
            else
            {
                context.Character.transform.SetPositionAndRotation(context.RefreshmentPosition, context.RefreshmentRotation);
            }

            //Debug.Log($"[{context.Character.name}.{nameof(CharacterRefreshmentHandler)}] Placed at position {context.PlacementPosition} with rotation {context.PlacementRotation.eulerAngles.z}.");
        }

        private static void RefreshCharacterSystems(CharacterRefreshmentContext context)
        {
            IEnumerable<ICharacterRefreshable> refreshableSystems = context.Character
                .GetComponentsInChildren<MonoBehaviour>(true)
                .OfType<ICharacterRefreshable>()
                .OrderBy(system => system.RefreshmentConfigurePriority);

            //Debug.Log($"[{context.Character.name}.{nameof(CharacterRefreshmentHandler)}] Found { refreshableSystems.Count()} refreshable systems to refresh");

            foreach (ICharacterRefreshable refreshableSystem in refreshableSystems)
            {
                refreshableSystem.RefreshCharacter(context);
            }
        }
    }
}
