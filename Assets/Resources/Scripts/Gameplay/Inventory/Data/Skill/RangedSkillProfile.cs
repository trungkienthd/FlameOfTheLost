using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "SKR_RangedSkill", menuName = "Data/FlameOfTheLost/Inventory/Skill/RangedSkill")]
    public class RangedSkillProfile : SkillProfile
    {
        // --- Style
        public override SkillStyleType Style => SkillStyleType.Ranged;

        // --- Projectile Config
        [Header("Projectile Config")]
        [SerializeField] private GameObject _arrowPrefab;
        [SerializeField, Min(0f)] private float _arrowSpeed = 15f;
        [SerializeField, Min(0f)] private float _arrowLifetime = 3f;
        [SerializeField] private bool _destroyArrowOnHit = false;

        [Header("Multi Projectile Config")]
        [SerializeField, Min(1)] private int _projectileCount = 1;

        [Tooltip("Angle gap in degrees between adjacent projectiles.")]
        [SerializeField, Min(0f)] private float _projectileAngleStep = 0f;

        public GameObject ProjectilePrefab => _arrowPrefab;
        public float ProjectileSpeed => _arrowSpeed;
        public float ProjectileLifetime => _arrowLifetime;
        public bool DestroyProjectileOnHit => _destroyArrowOnHit;

        public int ProjectileCount => Mathf.Max(1, _projectileCount);
        public float ProjectileAngleStep => Mathf.Max(0f, _projectileAngleStep);
    }
}
