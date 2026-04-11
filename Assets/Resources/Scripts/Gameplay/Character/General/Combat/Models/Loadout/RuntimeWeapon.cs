using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class RuntimeWeapon
    {
        // --- States ---
        //  1. Base Weapon
        [Header("Base Weapon)")]
        [SerializeField] private WeaponProfile _profile;
        public WeaponProfile Profile
        {
            get => _profile;
            private set => _profile = value;
        }

        //  2. Active Skills
        [Header("Active Skills (Runtime)")]
        [SerializeField] private SkillProfile _normalSkill;
        [SerializeField] private SkillProfile _activeSpecialSkill;
        public SkillProfile NormalSkill => this.Profile.NormalSkill;
        public SkillProfile ActiveSpecialSkill
        {
            get => _activeSpecialSkill;
            set => _activeSpecialSkill = value;
        }

        //  3. Non-serialized runtime data
        [NonSerialized] private GameObject _owner;
        public GameObject Owner
        {
            get => _owner;
            private set => _owner = value;
        }

        // --- Constructors ---
        public RuntimeWeapon(WeaponProfile profile, GameObject owner)
        {
            this.Profile = profile;
            this.ActiveSpecialSkill = this.Profile.InnateSpecialSkill;

            this.Owner = owner;
        }
    }
}
