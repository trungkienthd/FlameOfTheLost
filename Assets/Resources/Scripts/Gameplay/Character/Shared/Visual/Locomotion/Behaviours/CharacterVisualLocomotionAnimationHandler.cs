using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterVisualLocomotionAnimationHandler : FOTLMonoBehaviour
    {
        // --- States ---
        [Header("Locomotion Animation Data")]
        [SerializeField] private CharacterLocomotionAnimationProfile _placeholderProfile;
        [SerializeField] private CharacterLocomotionAnimationProfile _activeProfile;
        public CharacterLocomotionAnimationProfile PlaceholderProfile => _placeholderProfile;
        public CharacterLocomotionAnimationProfile ActiveProfile => _activeProfile;

        [Header("Optional Mode Provider")]
        [SerializeField] private MonoBehaviour _locomotionModeProviderBehaviour;
        private MonoBehaviour LocomotionModeProviderBehavior => _locomotionModeProviderBehaviour;
        private ICharacterLocomotionVisualModeProvider _boundProvider;
        private ICharacterLocomotionVisualModeProvider LocomotionModeProvider => this.LocomotionModeProviderBehavior as ICharacterLocomotionVisualModeProvider;

        [Header("Manual Runtime State")]
        [SerializeField] private CharacterLocomotionMode _manualLocomotionMode = CharacterLocomotionMode.Default;
        private CharacterLocomotionMode ManualLocomotionMode => _manualLocomotionMode;

        private CharacterVisualController _visualController;
        private CharacterVisualController VisualController => _visualController;

        private CharacterLocomotionMode CurrentLocomotionMode => this.LocomotionModeProvider != null
            ? this.LocomotionModeProvider.GetLocomotionMode()
            : this.ManualLocomotionMode;

        private CharacterCarryItemType CurrentCarryItemType => this.LocomotionModeProvider != null
            ? this.LocomotionModeProvider.GetCarryItemType()
            : CharacterCarryItemType.None;

        private LightSourceType CurrentLightSourceType => this.LocomotionModeProvider != null
            ? this.LocomotionModeProvider.GetLightSourceType()
            : LightSourceType.None;

        // --- Unity Methods ---
        private void OnDisable()
        {
            this.UnbindProvider();
        }

        // --- Public APIs ---
        public void Bind(CharacterVisualController visualController)
        {
            _visualController = visualController;

            this.ResolveProvider();
        }

        public void SetManualLocomotionMode(CharacterLocomotionMode locomotionMode)
        {
            if (_manualLocomotionMode == locomotionMode) return;

            _manualLocomotionMode = locomotionMode;
            this.RequestRebuild();
        }

        public void RefreshResolvedLocomotionMode()
        {
            this.RequestRebuild();
        }

        public void ApplyOverridesTo(AnimatorOverrideController overrideController)
        {
            if (overrideController == null) return;
            if (this.PlaceholderProfile == null) return;
            if (this.ActiveProfile == null) return;

            CharacterLocomotionMode locomotionMode = this.CurrentLocomotionMode;
            CharacterCarryItemType carryItemType = this.CurrentCarryItemType;
            LightSourceType lightSourceType = this.CurrentLightSourceType;

            AnimationClip realIdleDown  = this.ActiveProfile.GetIdleClip(CharacterFacingStatus.Down, locomotionMode, carryItemType, lightSourceType);
            AnimationClip realIdleSide  = this.ActiveProfile.GetIdleClip(CharacterFacingStatus.Side, locomotionMode, carryItemType, lightSourceType);
            AnimationClip realIdleUp    = this.ActiveProfile.GetIdleClip(CharacterFacingStatus.Up,   locomotionMode, carryItemType, lightSourceType);

            AnimationClip realWalkDown  = this.ActiveProfile.GetWalkClip(CharacterFacingStatus.Down, locomotionMode, carryItemType, lightSourceType);
            AnimationClip realWalkSide  = this.ActiveProfile.GetWalkClip(CharacterFacingStatus.Side, locomotionMode, carryItemType, lightSourceType);
            AnimationClip realWalkUp    = this.ActiveProfile.GetWalkClip(CharacterFacingStatus.Up,   locomotionMode, carryItemType, lightSourceType);

            AnimationClip placeholderIdleDown   = this.PlaceholderProfile.GetIdleClip(CharacterFacingStatus.Down);
            AnimationClip placeholderIdleSide   = this.PlaceholderProfile.GetIdleClip(CharacterFacingStatus.Side);
            AnimationClip placeholderIdleUp     = this.PlaceholderProfile.GetIdleClip(CharacterFacingStatus.Up);

            AnimationClip placeholderWalkDown   = this.PlaceholderProfile.GetWalkClip(CharacterFacingStatus.Down);
            AnimationClip placeholderWalkSide   = this.PlaceholderProfile.GetWalkClip(CharacterFacingStatus.Side);
            AnimationClip placeholderWalkUp     = this.PlaceholderProfile.GetWalkClip(CharacterFacingStatus.Up);

            if (placeholderIdleDown != null && realIdleDown != null) overrideController[placeholderIdleDown]    = realIdleDown;
            if (placeholderIdleSide != null && realIdleSide != null) overrideController[placeholderIdleSide]    = realIdleSide;
            if (placeholderIdleUp   != null && realIdleUp   != null) overrideController[placeholderIdleUp]      = realIdleUp;

            if (placeholderWalkDown != null && realWalkDown != null) overrideController[placeholderWalkDown]    = realWalkDown;
            if (placeholderWalkSide != null && realWalkSide != null) overrideController[placeholderWalkSide]    = realWalkSide;
            if (placeholderWalkUp   != null && realWalkUp   != null) overrideController[placeholderWalkUp]      = realWalkUp;
        }

        // --- Helpers ---
        private void ResolveProvider()
        {
            this.UnbindProvider();

            _boundProvider = this.LocomotionModeProvider;
            if (_boundProvider != null)
            {
                _boundProvider.OnLocomotionVisualModeChanged += this.OnProviderModeChanged;
            }
        }

        private void UnbindProvider()
        {
            if (_boundProvider == null) return;

            _boundProvider.OnLocomotionVisualModeChanged -= this.OnProviderModeChanged;
            _boundProvider = null;
        }

        private void OnProviderModeChanged()
        {
            this.RequestRebuild();
        }

        private void RequestRebuild()
        {
            if (this.VisualController == null) return;

            this.VisualController.RebuildBaseAnimationOverride();
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PlaceholderProfile == null) _placeholderProfile = Resources.Load<CharacterLocomotionAnimationProfile>("Data/Animations/Locomotion/Generic/AniLcm_Generic");
        }
    }
}
