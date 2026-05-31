using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterVisualCombatAnimationHandler : MonoBehaviour
    {
        // --- States ---
        [Header("Combat Animation Data")]
        [SerializeField] private CharacterCombatAnimationProfile _placeholderProfile;
        [SerializeField] private CharacterCombatAnimationProfile _activeProfile;
        public CharacterCombatAnimationProfile PlaceholderProfile => _placeholderProfile;
        public CharacterCombatAnimationProfile ActiveProfile => _activeProfile;

        private CharacterVisualController _visualController;
        private CharacterVisualController VisualController => _visualController;

        // --- Public APIs ---
        public void Initialize(CharacterVisualController visualController)
        {
            _visualController = visualController;
            this.CacheReferences();
        }

        public void ApplyHitAndDieOverridesTo(AnimatorOverrideController overrideController)
        {
            if (overrideController == null) return;
            if (this.PlaceholderProfile == null) return;
            if (this.ActiveProfile == null) return;

            this.ApplyFacingClipSetOverride
            (
                overrideController,
                this.PlaceholderProfile.HitAnimationClipSet,
                this.ActiveProfile.HitAnimationClipSet
            );

            this.ApplyFacingClipSetOverride
            (
                overrideController,
                this.PlaceholderProfile.DieAnimationClipSet,
                this.ActiveProfile.DieAnimationClipSet
            );
        }

        public void ApplyAttackOverridesTo(AnimatorOverrideController overrideController, RuntimeSkill runtimeSkill, WeaponProfile weapon, AttackSide attackSideType)
        {
            if (overrideController == null) return;
            if (runtimeSkill == null) return;
            if (this.PlaceholderProfile == null) return;
            if (this.ActiveProfile == null) return;

            if (!this.ActiveProfile.TryGetElement(runtimeSkill.Profile, weapon, out CharacterCombatAnimationClipHitboxSetProfile element) || element == null)
            {
                string skillName = runtimeSkill.Profile != null ? runtimeSkill.Profile.name : "NULL";
                string weaponName = weapon != null ? weapon.name : "NULL";
                Debug.LogWarning($"[{this.name}.CharacterVisualCombatAnimationHandler] No combat animation entry found for skill '{skillName}' and weapon '{weaponName}'.", this);

                return;
            }

            FacingAnimationClipSet animationClipSet = element.GetFacingAnimationClipSet(attackSideType);
            if (!animationClipSet.IsValid)
            {
                string skillName = runtimeSkill.Profile != null ? runtimeSkill.Profile.name : "NULL";
                string weaponName = weapon != null ? weapon.name : "NULL";
                Debug.LogWarning($"[{this.name}.CharacterVisualCombatAnimationHandler] Combat animation clip set is invalid for skill '{skillName}' and weapon '{weaponName}'.", this);

                return;
            }

            this.ApplyFacingClipSetOverride
            (
                overrideController: overrideController,
                placeholderSet: this.PlaceholderProfile.Elements.FirstOrDefault().GetFacingAnimationClipSet(),
                targetSet: animationClipSet
            );
        }

        // --- Helpers ---
        private void ApplyFacingClipSetOverride(AnimatorOverrideController overrideController, FacingAnimationClipSet placeholderSet, FacingAnimationClipSet targetSet)
        {
            if (overrideController == null) return;
            if (!placeholderSet.IsValid) return;
            if (!targetSet.IsValid) return;

            if (placeholderSet.Down != null && targetSet.Down != null)
            {
                overrideController[placeholderSet.Down] = targetSet.Down;
            }

            if (placeholderSet.Side != null && targetSet.Side != null)
            {
                overrideController[placeholderSet.Side] = targetSet.Side;
            }

            if (placeholderSet.Up != null && targetSet.Up != null)
            {
                overrideController[placeholderSet.Up] = targetSet.Up;
            }
        }

        private void CacheReferences()
        {
            if (this.PlaceholderProfile == null) _placeholderProfile = Resources.Load<CharacterCombatAnimationProfile>("Data/Animations/Characters/Combat/Generic/CAni_Generic");
        }
    }
}
