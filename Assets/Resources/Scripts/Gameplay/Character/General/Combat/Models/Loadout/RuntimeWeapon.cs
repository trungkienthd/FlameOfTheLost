using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class RuntimeWeapon
    {
        // --- Base Weapon
        [Header("Base Weapon)")]
        [SerializeField] private WeaponProfile _profile;
        public WeaponProfile Profile
        {
            get => _profile;
            private set => _profile = value;
        }

        // --- Active Skills
        [Header("Active Skills (Runtime)")]
        [SerializeField] private SkillProfile _activeLeftSpecialSkill;
        [SerializeField] private SkillProfile _activeRightSpecialSkill;
        public SkillProfile ActiveLeftSpecialSkill
        {
            get => _activeLeftSpecialSkill;
            set => _activeLeftSpecialSkill = value;
        }
        public SkillProfile ActiveRightSpecialSkill
        {
            get => _activeRightSpecialSkill;
            set => _activeRightSpecialSkill = value;
        }

        // --- Non-serialized runtime data
        [NonSerialized] private GameObject _owner;
        public GameObject Owner
        {
            get => _owner;
            private set => _owner = value;
        }

        // --- Constructors
        public RuntimeWeapon(WeaponProfile profile, GameObject owner)
        {
            this.Profile = profile;
            this.ActiveLeftSpecialSkill = this.Profile.LeftInnateSpecialSkill;
            this.ActiveRightSpecialSkill = this.Profile.RightInnateSpecialSkill;

            this.Owner = owner;
        }
    }
}
