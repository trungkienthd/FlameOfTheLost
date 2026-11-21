using System;

namespace FlameOfTheLost
{
    [Serializable]
    public class PlayerCombatLoadoutWeaponSkillSaveEntry
    {
        public const int NO_SPECIAL_SKILL_ID = -1;

        public string WeaponEntryId;

        public int ActiveSpecialSkillId = PlayerCombatLoadoutWeaponSkillSaveEntry.NO_SPECIAL_SKILL_ID;
    }
}
