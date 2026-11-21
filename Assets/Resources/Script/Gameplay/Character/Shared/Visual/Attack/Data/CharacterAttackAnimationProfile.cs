using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "AniAtk_New", menuName = "Data/FlameOfTheLost/Character/Shared/Visual/Animation/Attack/CharacterAttackAnimationProfile")]
    public class CharacterAttackAnimationProfile : ScriptableObject
    {
        // --- States ---
        [Header("Character")]
        [SerializeField] private CharacterType _characterType = CharacterType.Default;
        public CharacterType CharacterType => _characterType;

        [Header("(Override-At-Runtime) Combat Animation Elements")]
        [SerializeField] private List<CharacterAttackAnimationClipHitboxSetProfile> _elements = new();
        public IReadOnlyList<CharacterAttackAnimationClipHitboxSetProfile> Elements => _elements;

        // --- Unity Callbacks ---
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.CharacterType == CharacterType.Default) return;
            if (this.Elements == null || this.Elements.Count == 0) return;

            for (int i = 0; i < this.Elements.Count; i++)
            {
                var element = this.Elements[i];
                if (element == null)
                {
                    Debug.LogError($"[{this.name}] Entry at index {i} is NULL.", this);
                    continue;
                }

                // 1. CharacterTag consistency check
                if (element.CharacterType != this.CharacterType)
                {
                    Debug.LogError($"[{this.name}] CharacterType mismatch in entry '{element.name}'. " + $"Entry type = {element.CharacterType}, Profile type = {this.CharacterType}.", this);
                }

                // 2. Basic validity checks
                if (element.Weapon == null)
                {
                    Debug.LogError($"[{this.name}] Entry '{element.name}' has NULL Weapon.", this);
                }

                if (element.Skills == null || element.Skills.Count == 0)
                {
                    Debug.LogError($"[{this.name}] Entry '{element.name}' has EMPTY Skills list.", this);
                }
            }

            // 3. Duplicate detection (same weapon + overlapping skills)
            for (int i = 0; i < this.Elements.Count; i++)
            {
                var a = this.Elements[i];
                if (a == null) continue;

                for (int j = i + 1; j < this.Elements.Count; j++)
                {
                    var b = this.Elements[j];
                    if (b == null) continue;

                    if (a.Weapon == null || b.Weapon == null) continue;

                    // Same weapon?
                    bool sameWeapon = UnityEngine.Object.ReferenceEquals(a.Weapon, b.Weapon) || (a.Weapon.Id > 0 && b.Weapon.Id > 0 && a.Weapon.Id == b.Weapon.Id);
                    if (!sameWeapon) continue;

                    // Check overlapping skills
                    if (a.Skills == null || b.Skills == null) continue;

                    for (int s1 = 0; s1 < a.Skills.Count; s1++)
                    {
                        var skillA = a.Skills[s1];
                        if (skillA == null) continue;

                        for (int s2 = 0; s2 < b.Skills.Count; s2++)
                        {
                            var skillB = b.Skills[s2];
                            if (skillB == null) continue;

                            bool sameSkill = UnityEngine.Object.ReferenceEquals(skillA, skillB) ||(skillA.Id > 0 && skillB.Id > 0 && skillA.Id == skillB.Id);

                            if (sameSkill)
                            {
                                Debug.LogError
                                (
                                    $"[{this.name}] Duplicate mapping detected:\n" +
                                    $"- Skill: {skillA.name}\n" +
                                    $"- Weapon: {a.Weapon.name}\n" +
                                    $"- Entries: '{a.name}' and '{b.name}'",
                                    this
                                );
                            }
                        }
                    }
                }
            }
        }
#endif

        // --- Public APIs ---
        public bool TryGetElement(SkillProfile skill, WeaponProfile weapon, out CharacterAttackAnimationClipHitboxSetProfile element)
        {
            element = null;
            if (skill == null || weapon == null || this.Elements == null || this.Elements.Count == 0) return false;

            for (int i = 0; i < this.Elements.Count; i++)
            {
                CharacterAttackAnimationClipHitboxSetProfile current = this.Elements[i];
                if (current == null) continue;

                if (current.Matches(skill, weapon))
                {
                    element = current;
                    return true;
                }
            }

            return false;
        }

        public CharacterAttackAnimationClipHitboxSetProfile GetOrNull(SkillProfile skill, WeaponProfile weapon)
        {
            this.TryGetElement(skill, weapon, out CharacterAttackAnimationClipHitboxSetProfile entry);
            return entry;
        }
    }
}