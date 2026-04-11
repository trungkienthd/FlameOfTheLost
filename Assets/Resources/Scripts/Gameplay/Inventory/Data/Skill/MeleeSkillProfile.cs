using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "MeleeSkillProfile", menuName = "Data/FlameOfTheLost/Combat/Skills/MeleeSkill")]
    public class MeleeSkillProfile : SkillProfile
    {
        // --- Style
        public override SkillStyleType Style => SkillStyleType.Melee;
    }
}
