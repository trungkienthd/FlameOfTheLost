using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "AClipAtkHb_New", menuName = "Data/FlameOfTheLost/Character/Shared/Visual/Animation/Attack/CharacterAttackAnimationClipHitboxSetProfile")]
    public class CharacterAttackAnimationClipHitboxSetProfile : ScriptableObject
    {
        // --- States ---
        //  1. Match Keys
        [Header("Match Keys")]
        [SerializeField] private List<SkillProfile> _skills = new();
        [SerializeField] private WeaponProfile _weapon;
        [SerializeField] private CharacterType _characterType;
        public IReadOnlyList<SkillProfile> Skills => _skills;
        public WeaponProfile Weapon => _weapon;
        public CharacterType CharacterType => _characterType;

        //  2. Left / Right attack data
        [Header("Attack Animation + Hitbox Sets")]
        [SerializeField] private CharacterAttackAnimationClipHitboxSetEntry _left;
        [SerializeField] private CharacterAttackAnimationClipHitboxSetEntry _right;

        public CharacterAttackAnimationClipHitboxSetEntry LeftSet => _left;
        public CharacterAttackAnimationClipHitboxSetEntry RightSet => _right;

        //  3. Attack Window
        [Header("Attack Window")]
        [SerializeField, Min(0f)] private float _attackWindowDuration = 0.5f;
        public float AttackWindowDuration => _attackWindowDuration;

        // --- Unity Methods ---
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.CharacterType == CharacterType.Default) return;

            if (this.Skills == null || this.Skills.Count == 0)
            {
                Debug.LogWarning($"[{this.name}.SkillAnimationProfile] Missing Skills list.", this);
                return;
            }

            if (this.Weapon == null)
            {
                Debug.LogWarning($"[{this.name}.SkillAnimationProfile] Missing Weapon reference.", this);
            }
            else if (this.Weapon.Id <= 0)
            {
                Debug.LogWarning($"[{this.name}.SkillAnimationProfile] Weapon '{this.Weapon.name}' has invalid ID.", this);
            }

            bool hasValidSkill = false;
            HashSet<int> ids = new();

            for (int i = 0; i < this.Skills.Count; i++)
            {
                SkillProfile skill = this.Skills[i];
                if (skill == null)
                {
                    Debug.LogWarning($"[{this.name}.SkillAnimationProfile] Null Skill reference found in Skills list.", this);
                    continue;
                }

                if (skill.Id <= 0)
                {
                    Debug.LogWarning($"[{this.name}.SkillAnimationProfile] Skill '{skill.name}' has invalid ID.", this);
                    continue;
                }

                hasValidSkill = true;

                if (!ids.Add(skill.Id))
                {
                    Debug.LogWarning($"[{this.name}.SkillAnimationProfile] Duplicate Skill '{skill.name}' (ID: {skill.Id}) found in Skills list.", this);
                }
            }

            if (!hasValidSkill)
            {
                Debug.LogWarning($"[{this.name}.SkillAnimationProfile] Skills list contains no valid Skill references.", this);
            }

            this.ValidateEntryPair(this.LeftSet.Down,   this.RightSet.Down, "Down");
            this.ValidateEntryPair(this.LeftSet.Side,   this.RightSet.Side, "Side");
            this.ValidateEntryPair(this.LeftSet.Up,     this.RightSet.Up,   "Up");
        }
#endif

        // --- Public APIs ---
        //  1. Get this Animation Clip Set (Facing) for the given side
        public CharacterFacingAnimationClipSetEntry GetFacingAnimationClipSet(AttackSide attackSide = AttackSide.Left)
        {
            CharacterAttackAnimationClipHitboxSetEntry set = this.GetAnimationClipHitboxSet(attackSide);

            CharacterFacingAnimationClipSetEntry result = new CharacterFacingAnimationClipSetEntry
            (
                set.Down.AnimationClip,
                set.Side.AnimationClip,
                set.Up.AnimationClip
            );

            return result;
        }

        //  2. Gets the MeleeHitboxShape for the given side.
        public MeleeHitboxShape GetMeleeHitboxShape(AttackSide attackSide = AttackSide.Left)
        {
            CharacterAttackAnimationClipHitboxSetEntry set = this.GetAnimationClipHitboxSet(attackSide);

            MeleeHitboxShape result = new MeleeHitboxShape
            (
                set.Down.HitboxShape,
                set.Side.HitboxShape,
                set.Up.HitboxShape
            );

            return result;
        }

        //  3. Checks if this entry matches the given skill and weapon.
        public bool Matches(SkillProfile skill, WeaponProfile weapon)
        {
            if (skill == null || weapon == null) return false;
            if (this.Weapon == null) return false;

            bool weaponMatch = UnityEngine.Object.ReferenceEquals(this.Weapon, weapon) || (this.Weapon.Id > 0 && weapon.Id > 0 && this.Weapon.Id == weapon.Id);

            if (!weaponMatch) return false;

            if (this.Skills == null || this.Skills.Count == 0) return false;

            for (int i = 0; i < this.Skills.Count; i++)
            {
                SkillProfile current = _skills[i];
                if (current == null) continue;

                if (UnityEngine.Object.ReferenceEquals(current, skill)) return true;
                if (current.Id > 0 && skill.Id > 0 && current.Id == skill.Id) return true;
            }

            return false;
        }

        // --- Helpers ---
        //  1. Validation for Left/Right entry pairs (Down, Side, Up)
        private void ValidateEntryPair(CharacterAttackAnimationClipHitboxEntry leftEntry, CharacterAttackAnimationClipHitboxEntry rightEntry, string facingName)
        {
            this.ValidateClipPair(leftEntry.AnimationClip, rightEntry.AnimationClip, $"{facingName} AnimationClip");

            this.ValidatePolygonShapePair(leftEntry.HitboxShape, rightEntry.HitboxShape, $"{facingName} PolygonColliderShape");
        }
        private void ValidateClipPair(AnimationClip leftClip, AnimationClip rightClip, string label)
        {
            if (leftClip == null && rightClip == null)
            {
                Debug.LogWarning($"[{this.name}.SkillAnimationProfile] Both Left and Right '{label}' are null. At least one must be assigned.", this);
            }
        }
        private void ValidatePolygonShapePair(PolygonColliderShape leftShape, PolygonColliderShape rightShape, string label)
        {
            if (!leftShape.IsValid && !rightShape.IsValid)
            {
                Debug.LogWarning($"[{this.name}.SkillAnimationProfile] Both Left and Right '{label}' are null. At least one must be assigned.", this);
            }
        }

        //  2. Get the AttackAnimationHitboxSet for the given side, with fallback to the opposite side if needed.
        private CharacterAttackAnimationClipHitboxSetEntry GetAnimationClipHitboxSet(AttackSide attackSide = AttackSide.Left)
        {
            if (attackSide == AttackSide.Right)
            {
                return this.RightSet.GetResolvedWithFallback(this.LeftSet);
            }

            return this.LeftSet.GetResolvedWithFallback(this.RightSet);
        }
    }
}
