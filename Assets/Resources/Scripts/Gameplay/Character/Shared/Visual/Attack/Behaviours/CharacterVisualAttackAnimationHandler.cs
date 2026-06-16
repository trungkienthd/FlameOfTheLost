using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterVisualAttackAnimationHandler : FOTLMonoBehaviour
    {
        // --- States ---
        [Header("Combat Animation Data")]
        [SerializeField] private CharacterAttackAnimationProfile _placeholderProfile;
        [SerializeField] private CharacterAttackAnimationProfile _activeProfile;
        public CharacterAttackAnimationProfile PlaceholderProfile => _placeholderProfile;
        public CharacterAttackAnimationProfile ActiveProfile => _activeProfile;

        private CharacterVisualController _visualController;
        private CharacterVisualController VisualController => _visualController;

        // --- Public APIs ---
        public void Initialize(CharacterVisualController visualController)
        {
            _visualController = visualController;
            this.CacheReferences();
        }

        public void ApplyAttackOverridesTo(AnimatorOverrideController overrideController, RuntimeSkill runtimeSkill, WeaponProfile weapon, AttackSide attackSideType)
        {
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

            FacingAnimationClipSet animationClipSet = element.GetFacingAnimationClipSet(attackSideType);
            if (!animationClipSet.IsValid)
            {
                string skillName = runtimeSkill.Profile != null ? runtimeSkill.Profile.name : "NULL";
                string weaponName = weapon != null ? weapon.name : "NULL";
                Debug.LogWarning($"[{this.name}.{nameof(CharacterVisualAttackAnimationHandler)}] Combat animation clip set is invalid for skill '{skillName}' and weapon '{weaponName}'.", this);

                return;
            }

            CharacterAnimationOverrideUtility.ApplyFacingClipSetOverride
            (
                overrideController: overrideController,
                placeholderSet: this.PlaceholderProfile.Elements.FirstOrDefault().GetFacingAnimationClipSet(),
                targetSet: animationClipSet
            );
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.PlaceholderProfile == null) _placeholderProfile = Resources.Load<CharacterAttackAnimationProfile>("Data/Animations/Characters/Attack/Generic/AniAtk_Generic");
        }
    }
}
