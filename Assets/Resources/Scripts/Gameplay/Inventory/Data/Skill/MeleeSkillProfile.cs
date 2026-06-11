using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "SKM_MeleeSkill", menuName = "Data/FlameOfTheLost/Inventory/Skill/MeleeSkill")]
    public class MeleeSkillProfile : SkillProfile
    {
        // --- Style
        public override SkillStyle Style => SkillStyle.Melee;
    }
}
