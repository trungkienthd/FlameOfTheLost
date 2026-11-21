using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "SKR_RangedSkill", menuName = "Data/FlameOfTheLost/Inventory/Skill/RangedSkill")]
    public class RangedSkillProfile : SkillProfile
    {
        // --- States ---
        //  1. Style (Override)
        public override SkillStyle Style => SkillStyle.Ranged;

        //  2. Projectile Requirement
        [Header("Projectile (Requirement)")]
        [SerializeField] private ProjectileType _requiredProjectileType;
        [SerializeField] private bool _costProjectile = false;
        public ProjectileType RequiredProjectileType => _requiredProjectileType;
        public bool CostProjectile => _costProjectile;

        //  3. Free / Default Projectile
        [Header("Default Projectile")]
        [Tooltip("Used when CostProjectile is false. This lets the skill fire even when the projectile slot is empty or mismatched.")]
        [SerializeField] private ProjectileProfile _defaultProjectile;
        public ProjectileProfile DefaultProjectile => _defaultProjectile;
        public GameObject DefaultProjectilePrefab => this.DefaultProjectile != null
            ? this.DefaultProjectile.ProjectilePrefab
            : null;

        //  4. Projectile Config
        [Header("Projectile Config")]
        [SerializeField, Min(0f)] private float _projectileSpeed = 15f;
        [SerializeField, Min(0f)] private float _projectileLifetime = 3f;
        [SerializeField] private bool _destroyProjectileOnHit = true;
        public float ProjectileSpeed => _projectileSpeed;
        public float ProjectileLifetime => _projectileLifetime;
        public bool DestroyProjectileOnHit => _destroyProjectileOnHit;

        [Header("Multi Projectile Config")]
        [SerializeField, Min(1)] private int _projectileCount = 1;
        public int ProjectileCount => Mathf.Max(1, _projectileCount);

        [Tooltip("Angle gap in degrees between adjacent projectiles.")]
        [SerializeField, Min(0f)] private float _projectileAngleStep = 0f;
        public float ProjectileAngleStep => Mathf.Max(0f, _projectileAngleStep);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!this.CostProjectile && this.DefaultProjectile == null)
            {
                Debug.LogWarning($"[{this.name}.{nameof(RangedSkillProfile)}] '{this.name}' does not cost projectile item but has no default projectile item.", this);
            }

            if (this.DefaultProjectile != null && this.DefaultProjectile.ProjectileType != this.RequiredProjectileType)
            {
                Debug.LogWarning($"[{this.name}.{nameof(RangedSkillProfile)}] '{this.name}' default projectile classification does not match required projectile classification.", this);
            }
        }
#endif
    }
}
