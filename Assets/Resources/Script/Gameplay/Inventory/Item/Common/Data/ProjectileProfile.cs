using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Prjt_New", menuName = "Data/FlameOfTheLost/Inventory/Item/Projectile")]
    public class ProjectileProfile : ItemProfile
    {
        // --- States ---
        //  1. Item Type (Override)
        public override ItemType ItemType => ItemType.Projectile;

        //  2. Projectile Type  
        [Header("Projectile (Classification)")]
        [SerializeField] private ProjectileType _projectileType;
        public ProjectileType ProjectileType => _projectileType;

        //  3. Projectile Prefab 
        [Header("Projectile (Prefab)")]
        [SerializeField] private GameObject _projectilePrefab;
        public GameObject ProjectilePrefab => _projectilePrefab;

        //  4. Stackable (Override): Projectile items should normally be stackable.
        public override bool IsStackable => true;
    }
}
