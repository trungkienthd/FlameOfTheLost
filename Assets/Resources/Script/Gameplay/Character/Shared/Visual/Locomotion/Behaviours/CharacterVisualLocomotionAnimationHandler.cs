using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterVisualLocomotionAnimationHandler : CharacterVisualSubBehaviourBase
    {
        // --- Data / Configs ---
        [Header("Placeholder Animation Data")]
        [SerializeField] private CharacterLocomotionAnimationProfile _placeholderProfile;
        private CharacterLocomotionAnimationProfile PlaceholderProfile => _placeholderProfile;

        private CharacterLocomotionAnimationProfile ActiveProfile => this.VisualConfig != null
            ? this.VisualConfig.LocomotionAnimationProfile
            : null;

        // --- References ---
        [Header("Optional Mode Provider")]
        [SerializeField] private MonoBehaviour _locomotionModeProviderBehaviour;
        private ICharacterLocomotionVisualModeProvider ConfiguredLocomotionModeProvider => _locomotionModeProviderBehaviour as ICharacterLocomotionVisualModeProvider;
        private ICharacterLocomotionVisualModeProvider _boundProvider;
        private ICharacterLocomotionVisualModeProvider LocomotionModeProvider => _boundProvider;

        // --- Runtime ---
        [Header("Manual Runtime State")]
        [SerializeField] private CharacterLocomotionMode _manualLocomotionMode = CharacterLocomotionMode.Default;
        private CharacterLocomotionMode ManualLocomotionMode => _manualLocomotionMode;

        private CharacterLocomotionMode CurrentLocomotionMode => this.LocomotionModeProvider != null
            ? this.LocomotionModeProvider.GetLocomotionMode()
            : this.ManualLocomotionMode;

        private CharacterCarryItemType CurrentCarryItemType => this.LocomotionModeProvider != null
            ? this.LocomotionModeProvider.GetCarryItemType()
            : CharacterCarryItemType.None;

        private LightSourceType CurrentLightSourceType => this.LocomotionModeProvider != null
            ? this.LocomotionModeProvider.GetLightSourceType()
            : LightSourceType.None;

        // --- Public APIs ---
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
            if (!this.EnsureBound()) return;
            if (overrideController == null) return;
            if (this.PlaceholderProfile == null) return;
            if (this.ActiveProfile == null) return;

            CharacterLocomotionMode locomotionMode = this.CurrentLocomotionMode;
            CharacterCarryItemType carryItemType = this.CurrentCarryItemType;
            LightSourceType lightSourceType = this.CurrentLightSourceType;

            this.LogDebug
            (
                $"Applying locomotion overrides. " +
                $"Mode: {locomotionMode}, " +
                $"Carry: {carryItemType}, " +
                $"Light: {lightSourceType}."
            );

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

            _boundProvider = this.ConfiguredLocomotionModeProvider;
            if (_boundProvider == null)
            {
                this.LogDebug("No locomotion visual mode provider resolved.");
                return;
            }

            _boundProvider.OnLocomotionVisualModeChanged += this.OnProviderModeChanged;

            this.LogDebug($"Bound locomotion visual mode provider: {this.GetObjectName(this._locomotionModeProviderBehaviour)}.");
        }
        private void UnbindProvider()
        {
            if (_boundProvider == null) return;

            _boundProvider.OnLocomotionVisualModeChanged -= this.OnProviderModeChanged;
            _boundProvider = null;
        }

        private void OnProviderModeChanged()
        {
            CharacterLocomotionMode resolvedMode = this.LocomotionModeProvider != null
                ? this.LocomotionModeProvider.GetLocomotionMode()
                : this.ManualLocomotionMode;

            this.LogDebug($"Provider locomotion mode changed. Resolved Mode: {resolvedMode}.");

            this.RequestRebuild();
        }

        private void RequestRebuild()
        {
            if (!this.EnsureBound()) return;

            this.LogDebug("Requesting animation override rebuild.");

            this.VisualController.RebuildBaseAnimationOverride();
        }

        // --- FOTLSubBehaviour Overrides ---
        protected override void OnBound(CharacterVisualController visualController)
        {
            base.OnBound(visualController);

            this.ResolveProvider();
        }
        protected override void OnUnbinding(CharacterVisualController visualController)
        {
            this.UnbindProvider();

            base.OnUnbinding(visualController);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PlaceholderProfile == null) _placeholderProfile = Resources.Load<CharacterLocomotionAnimationProfile>("Data/Animations/Locomotion/Generic/AniLcm_Generic");
        }
    }
}
