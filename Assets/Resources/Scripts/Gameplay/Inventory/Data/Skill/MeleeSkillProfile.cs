using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "MeleeSkillProfile", menuName = "Data/FlameOfTheLost/Combat/Melee/MeleeSkillProfile")]
    public class MeleeSkillProfile : SkillProfile
    {
        // --- Style
        public override SkillStyleType Style => SkillStyleType.Melee;

        // --- Hitbox Shape
        [Header("Hitbox Shapes")]
        [SerializeField] private MeleeHitboxShape _meleeHitboxShape;
        public MeleeHitboxShape MeleeHitboxShape => _meleeHitboxShape;
    }
}
