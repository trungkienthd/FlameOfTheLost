using System;
using System.Collections;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterVisualHitDeathAnimationHandler : FOTLMonoBehaviour
    {
        // --- States ---
        //  1. Data
        [Header("Hit / Death Animation Data")]
        [SerializeField] private CharacterHitDeathAnimationProfile _placeholderProfile;
        [SerializeField] private CharacterHitDeathAnimationProfile _activeProfile;
        public CharacterHitDeathAnimationProfile PlaceholderProfile => _placeholderProfile;
        public CharacterHitDeathAnimationProfile ActiveProfile => _activeProfile;

        //  2. Configs
        [Header("Timing")]
        [SerializeField] private float _hitCompletionFallbackDelay = 0.3f;
        [SerializeField] private float _hitCompletionPadding = 0.03f;
        [SerializeField] private float _hitToDeathDelay = 0.15f;
        [SerializeField] private float _deathCompletionFallbackDelay = 1.0f;
        [SerializeField] private float _deathCompletionPadding = 0.03f;
        public float HitCompletionFallbackDelay => _hitCompletionFallbackDelay;
        public float HitCompletionPadding => _hitCompletionPadding;
        public float HitToDeathAnimationDelay => _hitToDeathDelay;
        public float DeathCompletionFallbackDelay => _deathCompletionFallbackDelay;
        public float DeathCompletionPadding => _deathCompletionPadding;

        //  3. References
        private CharacterVisualController _visualController;
        private CharacterVisualController VisualController => _visualController;
        private CharacterInputHandler InputHandler => this.VisualController != null
            ? this.VisualController.GetCharacter().InputHandler
            : null;

        //  4. Coroutine 
        private Coroutine _hitDeathSequenceCoroutine;

        //  5. Input Block state
        private bool _isBlockingInputForCurrentSequence;
        private bool IsBlockingInputForCurrentSequence
        {
            get => _isBlockingInputForCurrentSequence;
            set => _isBlockingInputForCurrentSequence = value;
        }

        //  6. Event-related state
        private bool _hasNotifiedDeathAnimationFinished = false;
        private bool HasNotifiedDeathAnimationFinished
        {
            get => _hasNotifiedDeathAnimationFinished;
            set => _hasNotifiedDeathAnimationFinished = value;
        }

        // --- Events ---
        public event Action OnDeathAnimationFinished;

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
                placeholderSet: this.PlaceholderProfile.HitAnimationClipSet,
                targetSet: this.ActiveProfile.HitAnimationClipSet
            );

            CharacterAnimationOverrideUtility.ApplyFacingClipSetOverride
            (
                overrideController: overrideController,
                placeholderSet: this.PlaceholderProfile.DeathAnimationClipSet,
                targetSet: this.ActiveProfile.DeathAnimationClipSet
            );
        }

        public void PlayHitSequence(bool shouldPlayDeathAfterHit)
        {
            this.RestartSequence(this.PlayHitSequenceCoroutine(shouldPlayDeathAfterHit));
        }

        public void PlaySelfCostSequence(bool shouldPlayDeathAfterSelfCost)
        {
            // For now, self-cost reuses Hit visual.
            // Later this can be changed to a different flash/VFX without touching HPController.
            this.RestartSequence(this.PlayHitSequenceCoroutine(shouldPlayDeathAfterSelfCost));
        }

        public void TriggerHit()
        {
            if (this.VisualController == null) return;
            if (this.VisualController.Animator == null) return;

            this.VisualController.Animator.SetTrigger(CharacterVisualController.HitID);
        }

        public void TriggerDeath()
        {
            if (this.VisualController == null) return;
            if (this.VisualController.Animator == null) return;

            this.ResetDeathAnimationFinishedNotification();

            Animator animator = this.VisualController.Animator;

            animator.ResetTrigger(CharacterVisualController.AttackID);
            animator.ResetTrigger(CharacterVisualController.HitID);
            animator.ResetTrigger(CharacterVisualController.DieID);

            animator.SetFloat(CharacterVisualController.SpeedID, 0f);
            animator.SetBool(CharacterVisualController.DeadID, true);
            animator.SetTrigger(CharacterVisualController.DieID);
        }

        public void NotifyDeathAnimationFinished()
        {
            if (this.HasNotifiedDeathAnimationFinished) return;

            this.HasNotifiedDeathAnimationFinished = true;
            this.OnDeathAnimationFinished?.Invoke();
        }

        public void ResetSequenceState()
        {
            this.StopSequence();
            this.ResetDeathAnimationFinishedNotification();
            this.EndHitSequenceInputBlock();
        }

        // --- Helpers ---
        private void RestartSequence(IEnumerator sequence)
        {
            if (this.VisualController == null) return;
            if (this.VisualController.Animator == null) return;

            this.StopSequence();
            _hitDeathSequenceCoroutine = this.StartCoroutine(sequence);
        }
        private void StopSequence()
        {
            if (_hitDeathSequenceCoroutine != null)
            {
                this.StopCoroutine(_hitDeathSequenceCoroutine);
                _hitDeathSequenceCoroutine = null;
            }

            this.EndHitSequenceInputBlock();
        }

        private IEnumerator PlayHitSequenceCoroutine(bool shouldPlayDeathAfterHit)
        {
            this.BeginHitSequenceInputBlock();

            this.TriggerHit();

            if (!shouldPlayDeathAfterHit)
            {
                float hitCompletionDelay = this.GetHitCompletionDelay();
                yield return new WaitForSeconds(hitCompletionDelay);

                this.EndHitSequenceInputBlock();

                _hitDeathSequenceCoroutine = null;
                yield break;
            }

            yield return new WaitForSeconds(this.HitToDeathAnimationDelay);

            this.TriggerDeath();

            float deathCompletionDelay = this.GetDeathCompletionDelay();
            yield return new WaitForSeconds(deathCompletionDelay);

            this.NotifyDeathAnimationFinished();

            // Safe to call even after death.
            // CharacterInputHandler.IsDeathDisabled will keep input blocked permanently after death.
            this.EndHitSequenceInputBlock();

            _hitDeathSequenceCoroutine = null;
        }

        private void BeginHitSequenceInputBlock()
        {
            if (this.InputHandler == null) return;

            this.InputHandler.BlockInputForHitSequence();
            this.IsBlockingInputForCurrentSequence = true;
        }
        private void EndHitSequenceInputBlock()
        {
            if (!this.IsBlockingInputForCurrentSequence) return;

            this.InputHandler?.ClearHitSequenceInputBlock();
            this.IsBlockingInputForCurrentSequence = false;
        }

        private void ResetDeathAnimationFinishedNotification()
        {
            this.HasNotifiedDeathAnimationFinished = false;
        }

        private float GetHitCompletionDelay()
        {
            return this.GetAnimationCompletionDelay
            (
                clipSet: this.ActiveProfile.HitAnimationClipSet,
                fallbackDelay: this.HitCompletionFallbackDelay,
                padding: this.HitCompletionPadding
            );
        }
        private float GetDeathCompletionDelay()
        {
            return this.GetAnimationCompletionDelay
            (
                clipSet: this.ActiveProfile.DeathAnimationClipSet,
                fallbackDelay: this.DeathCompletionFallbackDelay,
                padding: this.DeathCompletionPadding
            );
        }
        private float GetAnimationCompletionDelay(CharacterFacingAnimationClipSetEntry clipSet, float fallbackDelay, float padding)
        {
            if (this.ActiveProfile == null) return 0f;

            AnimationClip animationClip = this.GetCurrentFacingAnimationClip(clipSet);

            float clipLength = animationClip != null
                ? animationClip.length
                : fallbackDelay;

            float animatorSpeed = this.GetSafeAnimatorSpeed();

            return Mathf.Max(0.01f, clipLength / animatorSpeed + padding);
        }
        private AnimationClip GetCurrentFacingAnimationClip(CharacterFacingAnimationClipSetEntry clipSet)
        {
            if (!clipSet.IsValid) return null;
            if (this.VisualController == null) return null;

            CharacterFacingStatus facingStatus = this.VisualController.FacingStatus;

            return clipSet.GetClip(facingStatus);
        }
        private float GetSafeAnimatorSpeed()
        {
            if (this.VisualController == null) return 1f;
            if (this.VisualController.Animator == null) return 1f;

            float animatorSpeed = Mathf.Abs(this.VisualController.Animator.speed);

            return animatorSpeed > 0f
                ? animatorSpeed
                : 1f;
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PlaceholderProfile == null) _placeholderProfile = Resources.Load<CharacterHitDeathAnimationProfile>("Data/Animations/Characters/HitDeath/Generic/AniHD_Generic");
        }
    }
}
