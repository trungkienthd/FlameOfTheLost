using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterRewardVFXHandler : CharacterBehaviourControllerBase
    {
        private static readonly int GainTrigger = Animator.StringToHash("Gain");
        private static readonly int ReleaseTrigger = Animator.StringToHash("Release");

        [Header("References")]
        [SerializeField] private Animator _animator;
        private Animator Animator => _animator;
        private CharacterVisualController VisualController => this.CharacterRootController.VisualController;

        [Header("Gain Visual")]
        [SerializeField] private Color _gainColor = Color.yellow;
        private Color GainColor => _gainColor;

        public void PlayGainVFX()
        {
            if (this.Animator == null) return;

            this.VisualController?.SetVisualColor(this.GainColor);

            this.Animator.ResetTrigger(CharacterRewardVFXHandler.ReleaseTrigger);
            this.Animator.SetTrigger(CharacterRewardVFXHandler.GainTrigger);
        }

        public void HandleRewardGainVFXEnded()
        {
            this.VisualController?.ResetVisualColor();
        }

        public void PlayReleaseVFX()
        {
            if (this.Animator == null) return;

            this.Animator.ResetTrigger(CharacterRewardVFXHandler.GainTrigger);
            this.Animator.SetTrigger(CharacterRewardVFXHandler.ReleaseTrigger);
        }

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.Animator == null) _animator = this.GetComponent<Animator>();
        }
    }
}