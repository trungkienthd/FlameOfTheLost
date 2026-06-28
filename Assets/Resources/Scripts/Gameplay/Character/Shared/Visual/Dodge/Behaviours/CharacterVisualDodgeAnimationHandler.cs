using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterVisualDodgeAnimationHandler : FOTLMonoBehaviour
    {
        // --- States ---
        //  1. Data
        [Header("Dodge Animation Data")]
        [SerializeField] private CharacterDodgeAnimationProfile _placeholderProfile;
        [SerializeField] private CharacterDodgeAnimationProfile _activeProfile;
        public CharacterDodgeAnimationProfile PlaceholderProfile => _placeholderProfile;
        public CharacterDodgeAnimationProfile ActiveProfile => _activeProfile;

        //  2. References
        private CharacterVisualController _visualController;
        private CharacterVisualController VisualController => _visualController;

        // --- Public APIs ---
        public void Bind(CharacterVisualController visualController)
        {
            _visualController = visualController;
            this.CacheInternalReferences();
        }

        public void ApplyOverridesTo(AnimatorOverrideController overrideController)
        {
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
            if (this.VisualController == null) return;
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
            if (this.VisualController == null) return;

            if (dodgeDirection.sqrMagnitude > 0.0001f)
            {
                this.VisualController.ForceFacing(dodgeDirection);
            }

            this.TriggerDodge();
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PlaceholderProfile == null) _placeholderProfile = Resources.Load<CharacterDodgeAnimationProfile>("Data/Character/Shared/Animation/Dodge/Generic/AniDdg_Generic");
        }
    }
}
