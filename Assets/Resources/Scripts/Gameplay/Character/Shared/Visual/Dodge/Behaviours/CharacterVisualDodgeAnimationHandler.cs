using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterVisualDodgeAnimationHandler : CharacterVisualSubBehaviourBase
    {
        // --- Data / Configs ---
        [Header("Placeholder Animation Data")]
        [SerializeField] private CharacterDodgeAnimationProfile _placeholderProfile;
        private CharacterDodgeAnimationProfile PlaceholderProfile => _placeholderProfile;

        private CharacterDodgeAnimationProfile ActiveProfile => this.VisualConfig != null
            ? this.VisualConfig.DodgeAnimationProfile
            : null;

        // --- Public APIs ---
        public void ApplyOverridesTo(AnimatorOverrideController overrideController)
        {
            if (!this.EnsureBound()) return;
            if (overrideController == null) return;
            if (this.PlaceholderProfile == null) return;
            if (this.ActiveProfile == null) return;

            CharacterAnimationOverrideUtility.ApplyFacingClipSetOverride
            (
                overrideController: overrideController,
                placeholderSet: this.PlaceholderProfile.DodgeAnimationClipSet,
                targetSet: this.ActiveProfile.DodgeAnimationClipSet
            );
        }

        public void TriggerDodge()
        {
            if (!this.EnsureBound()) return;
            if (this.VisualController.Animator == null) return;

            Animator animator = this.VisualController.Animator;

            animator.ResetTrigger(CharacterVisualController.AttackID);
            animator.ResetTrigger(CharacterVisualController.HitID);
            animator.ResetTrigger(CharacterVisualController.DieID);
            animator.ResetTrigger(CharacterVisualController.DodgeID);

            animator.SetTrigger(CharacterVisualController.DodgeID);
        }

        public void TriggerDodge(Vector2 dodgeDirection)
        {
            if (!this.EnsureBound()) return;

            if (dodgeDirection.sqrMagnitude > 0.0001f)
            {
                this.VisualController.ForceFacing(dodgeDirection);
            }

            this.TriggerDodge();
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PlaceholderProfile == null) _placeholderProfile = Resources.Load<CharacterDodgeAnimationProfile>("Data/Character/Shared/Animation/Dodge/Generic/AniDdg_Generic");
        }
    }
}
