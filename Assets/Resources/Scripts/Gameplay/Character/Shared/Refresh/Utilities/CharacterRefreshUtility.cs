using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class CharacterRefreshUtility
    {
        public static void RefreshCharacter(CharacterRootController character, Vector3 position, Quaternion rotation, CharacterFacingStatus facing, CharacterRefreshOrigin refreshOrigin)
        {
            if (character == null) return;

            CharacterRefreshTarget target = new CharacterRefreshTarget
            (
                position: position,
                rotation: rotation,
                facing: facing
            );

            CharacterRefreshContext context = new CharacterRefreshContext
            (
                character: character,
                refreshTarget: target,
                refreshOrigin: refreshOrigin
            );

            CharacterRefreshUtility.ApplyTransform(context);
            CharacterRefreshUtility.RefreshCharacterSystems(context);
        }

        private static void ApplyTransform(CharacterRefreshContext context)
        {
            Rigidbody2D rb = context.Character.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.position = context.RefreshTarget.Position;
                rb.rotation = context.RefreshTarget.Rotation.eulerAngles.z;
            }
            else
            {
                context.Character.transform.SetPositionAndRotation(context.RefreshTarget.Position, context.RefreshTarget.Rotation);
            }

            //Debug.Log($"[{context.Character.name}.{nameof(CharacterRefreshmentHandler)}] Placed at position {context.PlacementPosition} with rotation {context.PlacementRotation.eulerAngles.z}.");
        }

        private static void RefreshCharacterSystems(CharacterRefreshContext context)
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
