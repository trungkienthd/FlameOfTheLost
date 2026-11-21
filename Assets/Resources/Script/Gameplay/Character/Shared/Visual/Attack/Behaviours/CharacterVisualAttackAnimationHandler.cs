using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterVisualAttackAnimationHandler : CharacterVisualSubBehaviourBase
    {
        // --- Data ---
        [Header("Placeholder Animation Data")]
        [SerializeField] private CharacterAttackAnimationProfile _placeholderProfile;
        private CharacterAttackAnimationProfile PlaceholderProfile => _placeholderProfile;

        private CharacterAttackAnimationProfile ActiveProfile => this.VisualConfig != null
            ? this.VisualConfig.AttackAnimationProfile
            : null;

        // --- Public APIs ---
        public void ApplyAttackOverridesTo(AnimatorOverrideController overrideController, RuntimeSkill runtimeSkill, WeaponProfile weapon, AttackSide attackSideType)
        {
            if (!this.EnsureBound()) return;
            if (overrideController == null) return;
            if (runtimeSkill == null) return;
            if (this.PlaceholderProfile == null) return;
            if (this.ActiveProfile == null) return;

            if (!this.ActiveProfile.TryGetElement(runtimeSkill.Profile, weapon, out CharacterAttackAnimationClipHitboxSetProfile element) || element == null)
            {
                string skillName = runtimeSkill.Profile != null ? runtimeSkill.Profile.name : "NULL";
                string weaponName = weapon != null ? weapon.name : "NULL";
                Debug.LogWarning($"[{this.name}.{nameof(CharacterVisualAttackAnimationHandler)}] No combat animation entry found for skill '{skillName}' and weapon '{weaponName}'.", this);
                return;
            }

            CharacterFacingAnimationClipSetEntry animationClipSet = element.GetFacingAnimationClipSet(attackSideType);
            if (!animationClipSet.IsValid)
            {
                string skillName = runtimeSkill.Profile != null ? runtimeSkill.Profile.name : "NULL";
                string weaponName = weapon != null ? weapon.name : "NULL";
                Debug.LogWarning($"[{this.name}.{nameof(CharacterVisualAttackAnimationHandler)}] Combat animation clip set is invalid for skill '{skillName}' and weapon '{weaponName}'.", this);
                return;
            }

            if (this.PlaceholderProfile.Elements == null || this.PlaceholderProfile.Elements.Count == 0) return;

            CharacterAttackAnimationClipHitboxSetProfile placeholderElement = this.PlaceholderProfile.Elements.FirstOrDefault();
            if (placeholderElement == null) return;

            CharacterAnimationOverrideUtility.ApplyFacingClipSetOverride
            (
                overrideController: overrideController,
                placeholderSet: placeholderElement.GetFacingAnimationClipSet(),
                targetSet: animationClipSet
            );
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PlaceholderProfile == null) _placeholderProfile = Resources.Load<CharacterAttackAnimationProfile>("Data/Animations/Characters/Attack/Generic/AniAtk_Generic");
        }
    }
}
