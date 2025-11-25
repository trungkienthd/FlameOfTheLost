using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "RangedSkillProfile", menuName = "Data/FlameOfTheLost/Combat/Ranged/RangedSkillProfile")]
    public class RangedSkillProfile : SkillProfile
    {
        [Header("Projectile")]
        public GameObject ArrowPrefab;
        [Min(0f)] public float ArrowSpeed = 15f;
        [Min(0f)] public float ArrowLifetime = 3f;
    }
}
