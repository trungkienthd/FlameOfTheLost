using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "MeleeSkillProfile", menuName = "FlameOfTheLost/Combat/Melee/MeleeSkillProfile")]
    public class MeleeSkillProfile : SkillProfile
    {
        [Header("Hitbox Shapes")]
        [SerializeField] private MeleeHitboxProfile _meleeHitboxProfile;
        public MeleeHitboxProfile MeleeHitboxProfile => _meleeHitboxProfile;
    }
}
