using System;
using System.Collections.Generic;

namespace FlameOfTheLost
{
    [Serializable]
    public class PlayerCombatLoadoutSaveData
    {
        //  1. Weapons
        public List<string> EquippedWeaponEntryIds = new();
        public string ActiveWeaponEntryId;

        //      1.1. Weapons' Special Skills
        public List<PlayerCombatLoadoutWeaponSkillSaveEntry> WeaponActiveSpecialSkills = new();

        //  2. Projectiles
        public List<string> EquippedProjectileEntryIds = new();
        public string ActiveProjectileEntryId;

        //  3. Consumables
        public List<string> EquippedConsumableEntryIds = new();
        public string ActiveConsumableEntryId;
    }
}