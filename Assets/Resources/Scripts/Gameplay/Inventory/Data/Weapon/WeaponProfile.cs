using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "WeaponProfile", menuName = "Data/FlameOfTheLost/Combat/WeaponProfile")]
    public class WeaponProfile : ScriptableObject, IIdentityProvider, IIconProvider
    {
        // --- Identity
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(2, 5)] private string _description;
        public int ID => _id;
        public string DisplayName => _displayName;
        public string Description => _description;

        // --- Icon
        [Header("UI (Icon)")]
        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;


        // --- Weapon Type
        [Header("Weapon Type")]
        [SerializeField] private WeaponType _type;
        public WeaponType Type => _type;

        // --- Skills (Definition-only, immutable at runtime)
        [Header("Skills (Definition)")]
        [SerializeField] private SkillProfile _leftNormalSkill;             // non-null
        [SerializeField] private SkillProfile _rightNormalSkill;            // non-null
        [SerializeField] private SkillProfile _leftInnateSpecialSkill;      // nullable
        [SerializeField] private SkillProfile _rightInnateSpecialSkill;     // nullable
        public SkillProfile LeftNormalSkill => _leftNormalSkill;
        public SkillProfile RightNormalSkill => _rightNormalSkill;
        public SkillProfile LeftInnateSpecialSkill => _leftInnateSpecialSkill;
        public SkillProfile RightInnateSpecialSkill => _rightInnateSpecialSkill;

        // --- Unity Methods
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (this.LeftNormalSkill == null || this.RightNormalSkill == null)
            {
                Debug.LogWarning($"[{this.name}.WeaponProfile] Normal skills should be non-null.", this);
            }
        }
#endif

        // --- Public APIs
        //  1. Gets the normal skill for the given side.
        public SkillProfile GetNormalSkill(AttackSideType side)
        {
            return side == AttackSideType.Left ? this.LeftNormalSkill : this.RightNormalSkill;
        }

        //  2. Gets the innate special skill for the given side (may be null).
        public SkillProfile GetInnateSpecialSkill(AttackSideType side)
        {
            return side == AttackSideType.Left ? this.LeftInnateSpecialSkill : this.RightInnateSpecialSkill;
        }

        //  3. Resolves the special skill to use for the given side.
        //      Active comes first, then Innate, then Normal.
        //      Active specials are per-owner state, so they are passed in (do NOT store in this ScriptableObject).
        public SkillProfile ResolveSpecialSkill(AttackSideType side, SkillProfile activeLeftSpecial, SkillProfile activeRightSpecial)
        {
            SkillProfile active = side == AttackSideType.Left ? activeLeftSpecial : activeRightSpecial;
            if (active != null) return active;

            SkillProfile innate = this.GetInnateSpecialSkill(side);
            if (innate != null) return innate;

            return this.GetNormalSkill(side);
        }
    }
}
