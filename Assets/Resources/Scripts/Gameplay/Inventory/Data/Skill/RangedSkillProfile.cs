using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "RangedSkillProfile", menuName = "Data/FlameOfTheLost/Combat/Ranged/RangedSkillProfile")]
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
        public GameObject ArrowPrefab => _arrowPrefab;
        public float ArrowSpeed => _arrowSpeed;
        public float ArrowLifetime => _arrowLifetime;
        public bool DestroyArrowOnHit => _destroyArrowOnHit;
    }
}
