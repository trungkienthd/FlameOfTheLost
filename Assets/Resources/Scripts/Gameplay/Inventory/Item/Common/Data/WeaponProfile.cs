using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Wp_New", menuName = "Data/FlameOfTheLost/Inventory/Item/Weapon")]
    public class WeaponProfile : ItemProfile
    {
        // --- States ---
        //  0. Item type
        public override bool IsStackable => false;
        public override int MaxStackSize => 1;
        public override ItemType ItemType => ItemType.Weapon;

        //  1. Weapon Type
        [Header("Weapon Type")]
        [SerializeField] private WeaponType _type;
        public WeaponType Type => _type;

        //  2. Skills (Definition-only, immutable at runtime)
        [Header("Skills (Definition)")]
        [SerializeField] private SkillProfile _normalSkill;             // non-null
        [SerializeField] private SkillProfile _innateSpecialSkill;      // nullable
        public SkillProfile NormalSkill => _normalSkill;
        public SkillProfile InnateSpecialSkill => _innateSpecialSkill;

        //  3. Capability Requirements
        [Header("Capability Requirements")]
        [SerializeField] private List<CharacterCapabilityRequirementEntry> _requirements = new();
        public IReadOnlyList<CharacterCapabilityRequirementEntry> Requirements => _requirements;

        // --- Unity Callbacks ---
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (this.NormalSkill == null)
            {
                Debug.LogWarning($"[{this.name}.WeaponProfile] Normal skill should be non-null.", this);
            }
        }
#endif

        // --- Public APIs
        //  1. Resolves the special skill to use for the given side.
        //      Active comes first, then Innate, then Normal.
        //      Active specials are per-owner state, so they are passed in (do NOT store in this ScriptableObject).
        public SkillProfile ResolveSpecialSkill(SkillProfile activeSpecialSkill)
        {
            SkillProfile active = activeSpecialSkill;
            if (active != null) return active;

            SkillProfile innate = this.InnateSpecialSkill;
            if (innate != null) return innate;

            return this.NormalSkill;
        }

        //  2. Checks if the given character progression can satisfy the requirements to equip this weapon.
        public bool CanBeEquippedBy(CharacterProgressionController progressionController)
        {
            return CharacterCapabilityRequirementUtility.AreRequirementsSatisfied(progressionController, this.Requirements);
        }
    }
}
