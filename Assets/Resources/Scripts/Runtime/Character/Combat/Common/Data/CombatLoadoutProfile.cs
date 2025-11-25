using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "CombatLoadoutProfile", menuName = "Data/FlameOfTheLost/Combat/Common/Loadout/CombatLoadoutProfile")]
    public class CombatLoadoutProfile : ScriptableObject
    {
        [Header("Melee Weapons")]
        [SerializeField] private List<MeleeWeaponProfile> _meleeWeapons = new();
        public List<MeleeWeaponProfile> MeleeWeapons => _meleeWeapons;

        [Header("Ranged Weapons")]
        [SerializeField] private List<RangedWeaponProfile> _rangedWeapons = new();
        public List<RangedWeaponProfile> RangedWeapons => _rangedWeapons;

        public bool TryGetWeaponList<TSkill>(out List<WeaponProfile<TSkill>> weaponList) where TSkill : SkillProfile
        {
            weaponList = null;

            switch (typeof(TSkill))
            {
                case var t when t == typeof(MeleeSkillProfile):
                    if (this.MeleeWeapons != null && this.MeleeWeapons.Count > 0)
                    {
                        // Upcast each element to the generic base
                        weaponList = this.MeleeWeapons.ConvertAll(w => (WeaponProfile<TSkill>)(object)w);
                    }
                    break;

                case var t when t == typeof(RangedSkillProfile):
                    if (this.RangedWeapons != null && this.RangedWeapons.Count > 0)
                    {
                        weaponList = this.RangedWeapons.ConvertAll(w => (WeaponProfile<TSkill>)(object)w);
                    }
                    break;
            }

            return weaponList != null && weaponList.Count > 0;
        }
    }
}
