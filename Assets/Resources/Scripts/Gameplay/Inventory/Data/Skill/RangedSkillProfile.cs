using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "RangedSkillProfile", menuName = "Data/FlameOfTheLost/Combat/Skills/RangedSkill")]
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

        public GameObject ArrowPrefab => _arrowPrefab;
        public float ArrowSpeed => _arrowSpeed;
        public float ArrowLifetime => _arrowLifetime;
        public bool DestroyArrowOnHit => _destroyArrowOnHit;

        public int ProjectileCount => Mathf.Max(1, _projectileCount);
        public float ProjectileAngleStep => Mathf.Max(0f, _projectileAngleStep);
    }
}
