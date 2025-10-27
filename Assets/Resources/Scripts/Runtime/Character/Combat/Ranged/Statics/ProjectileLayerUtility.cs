using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class ProjectileLayerUtility
    {
        // Cache once
        private static readonly int LAYER_PLAYER    = LayerMask.NameToLayer("Player");
        private static readonly int LAYER_ALLY      = LayerMask.NameToLayer("Ally");
        private static readonly int LAYER_ENEMY     = LayerMask.NameToLayer("Enemy");

        private static readonly int LAYER_PROJECTILE_PLAYER = LayerMask.NameToLayer("ProjectilePlayer");
        private static readonly int LAYER_PROJECTILE_ALLY   = LayerMask.NameToLayer("ProjectileAlly");
        private static readonly int LAYER_PROJECTILE_ENEMY  = LayerMask.NameToLayer("ProjectileEnemy");

        // Preferred: pass the owner GameObject so we can also ignore collisions with it.
        public static void ApplyOwnerMeta(GameObject projectile, GameObject owner)
        {
            if (!projectile || !owner) return;

            // Map owner layer -> projectile layer
            int target = ProjectileLayerUtility.OwnerToProjectileLayer(owner.layer);
            ProjectileLayerUtility.SetLayerRecursively(projectile, target);

            // Set faction on All damagers that live anywhere on the Prefab
            var faction = ProjectileLayerUtility.LayerToFaction(owner.layer);
            var damageConfigurators = projectile.GetComponentsInChildren<MonoBehaviour>(true).OfType<IDamageConfigurator>(); // interface, not concrete
            foreach (var cfg in damageConfigurators)
            {
                cfg.SetSourceCharacterFaction(faction);
            }

            // 3) Prevent immediate self-collision/deflection
            ProjectileLayerUtility.IgnoreOwnerCollisions(projectile, owner);
        }

        // Minimal: set layer & faction from an owner layer integer.
        public static void SetProjectileLayerFromOwner(GameObject projectile, int ownerLayer)
        {
            if (!projectile) return;

            int target = ProjectileLayerUtility.OwnerToProjectileLayer(ownerLayer);
            ProjectileLayerUtility.SetLayerRecursively(projectile, target);

            var faction = ProjectileLayerUtility.LayerToFaction(ownerLayer);
            var damageConfigurators = projectile.GetComponentsInChildren<MonoBehaviour>(true).OfType<IDamageConfigurator>(); // interface, not concrete
            foreach (var cfg in damageConfigurators)
            {
                cfg.SetSourceCharacterFaction(faction);
            }
        }

        private static int OwnerToProjectileLayer(int ownerLayer)
        {
            if (ownerLayer == LAYER_PLAYER) return LAYER_PROJECTILE_PLAYER;
            if (ownerLayer == LAYER_ALLY)   return LAYER_PROJECTILE_ALLY;
            if (ownerLayer == LAYER_ENEMY)  return LAYER_PROJECTILE_ENEMY;

            // Sensible Default
            Debug.LogWarning("[ProjectileLayerUtility] Unsupported owner layer. Defaulting to Player's Projectile");
            return LAYER_PROJECTILE_PLAYER;
        }

        private static CharacterFaction LayerToFaction(int ownerLayer)
        {
            if (ownerLayer == LAYER_PLAYER) return CharacterFaction.Player;
            if (ownerLayer == LAYER_ALLY)   return CharacterFaction.Ally;
            if (ownerLayer == LAYER_ENEMY)  return CharacterFaction.Enemy;

            // Sensible Default
            Debug.LogWarning("[ProjectileLayerUtility] Unsupported owner layer. Defaulting to Player's faction");
            return CharacterFaction.Player;
        }

        public static void SetLayerRecursively(GameObject go, int layer)
        {
            if (!go || layer < 0) return;

            go.layer = layer;

            var t = go.transform;
            for (int i = 0, c = t.childCount; i < c; i++)
            {
                ProjectileLayerUtility.SetLayerRecursively(t.GetChild(i).gameObject, layer);
            }
        }

        private static void IgnoreOwnerCollisions(GameObject projectile, GameObject owner)
        {
            var projectileCollisions = projectile.GetComponentsInChildren<Collider2D>(true);
            var ownerCollisions = owner.GetComponentsInChildren<Collider2D>(true);
            for (int i = 0; i < projectileCollisions.Length; i++)
            {
                for (int j = 0; j < ownerCollisions.Length; j++)
                {
                    if (projectileCollisions[i] && ownerCollisions[j])
                    {
                        Physics2D.IgnoreCollision(projectileCollisions[i], ownerCollisions[j]);
                    }
                }
            }
        }
    }
}
