using UnityEngine;

namespace FlameOfTheLost
{
    public class KeyItemInteractableVisualHandler : KeyItemInteractableSubHandlerBase
    {
        // --- References ---
        //  1. Persistent Visual
        [Header("References - Visual")]
        [SerializeField] private GameObject _visualRoot;
        [SerializeField] private Animator _visualAnimator;
        private GameObject VisualRoot => _visualRoot;
        private Animator VisualAnimator => _visualAnimator;

        private SpriteRenderer _spriteRenderer;
        private SpriteRenderer SpriteRenderer => _spriteRenderer;

        //  2. Success Effect
        [Header("References - Success Effect")]
        [SerializeField] private ParticleSystem _successEffect;
        private ParticleSystem SuccessEffect => _successEffect;

        // --- Runtime ---
        //  1. Initialization
        private bool _isInitialized;

        //  2. Initial Visual Snapshot
        private Sprite _initialSprite;
        private bool _initialVisualRootActive;
        private bool _initialAnimatorEnabled;

        //  3. Applied Presentation State
        private bool _hasAppliedVisualState;
        private bool _lastAppliedCompletedState;

        // --- Public APIs ---
        //  1. Initialize Visual State
        public bool TryInitializeVisualState(SpriteRenderer spriteRenderer)
        {
            if (!this.EnsureBound()) return false;
            if (this.Profile == null) return false;

            if (_isInitialized) return true;

            // Reuse the reference resolved by the base World Object Behaviour.
            _spriteRenderer = spriteRenderer;

            if (!this.IsVisualConfigurationValid()) return false;

            // Capture the original visual configuration after the Brain has initialized its Profile.
            _initialSprite = this.SpriteRenderer != null
                ? this.SpriteRenderer.sprite
                : null;

            _initialVisualRootActive = this.VisualRoot != null && this.VisualRoot.activeSelf;

            _initialAnimatorEnabled = this.VisualAnimator != null && this.VisualAnimator.enabled;

            _hasAppliedVisualState = false;
            _lastAppliedCompletedState = false;

            this.StopSuccessEffect();

            _isInitialized = true;

            return true;
        }

        //  2. Apply Visual State
        public void ApplyVisualState(bool isCompleted, bool playSuccessEffect = false)
        {
            if (!this.EnsureBound()) return;
            if (!_isInitialized) return;
            if (this.Profile == null) return;

            // a. Only reconfigure the persistent visual when the completion state actually changes.
            bool didStateChange = !_hasAppliedVisualState || _lastAppliedCompletedState != isCompleted;

            if (didStateChange)
            {
                if (isCompleted)
                {
                    this.ApplyCompletedVisualState();
                }
                else
                {
                    this.ApplyInitialVisualState();
                }

                _lastAppliedCompletedState = isCompleted;
                _hasAppliedVisualState = true;
            }

            // b. Success effects are independent from the persistent completed-state presentation.
            if (playSuccessEffect)
            {
                this.PlaySuccessEffect();
            }
            else if (didStateChange)
            {
                this.StopSuccessEffect();
            }
        }

        // --- Helpers ---
        //  1. Initial Visual State
        private void ApplyInitialVisualState()
        {
            if (this.Profile.CompletionVisualType == KeyItemInteractableCompletionVisualType.Preserve) return;

            // Restore the original sprite when returning from ReplaceSprite completion mode.
            if (this.Profile.CompletionVisualType == KeyItemInteractableCompletionVisualType.ReplaceSprite)
            {
                if (this.SpriteRenderer != null)
                {
                    this.SpriteRenderer.sprite = _initialSprite;
                }

                if (this.VisualAnimator != null)
                {
                    this.VisualAnimator.enabled =  _initialAnimatorEnabled;
                }
            }

            this.SetVisualRootActive(_initialVisualRootActive);
        }

        //  2. Completed Visual State
        private void ApplyCompletedVisualState()
        {
            switch (this.Profile.CompletionVisualType)
            {
                case KeyItemInteractableCompletionVisualType.Preserve:      // Keep the existing visual and animation.
                    break;

                case KeyItemInteractableCompletionVisualType.Hide:
                    this.SetVisualRootActive(false);

                    break;

                case KeyItemInteractableCompletionVisualType.ReplaceSprite: // Prevent an idle Animator from overriding the explicitly configured completed sprite.
                    if (this.VisualAnimator != null)
                    {
                        this.VisualAnimator.enabled = false;
                    }

                    if (this.SpriteRenderer != null)
                    {
                        this.SpriteRenderer.sprite = this.Profile.CompletedSprite;
                    }

                    this.SetVisualRootActive(true);

                    break;
            }
        }
        private void SetVisualRootActive(bool isActive)
        {
            if (this.VisualRoot == null) return;
            if (this.VisualRoot.activeSelf == isActive) return;

            this.VisualRoot.SetActive(isActive);
        }

        //  3. Success Effect
        private void PlaySuccessEffect()
        {
            if (this.SuccessEffect == null) return;
            if (!this.SuccessEffect.gameObject.activeInHierarchy) return;

            this.SuccessEffect.Stop
            (
                withChildren: true,
                stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear
            );

            this.SuccessEffect.Play(withChildren: true);
        }
        private void StopSuccessEffect()
        {
            if (this.SuccessEffect == null) return;

            this.SuccessEffect.Stop
            (
                withChildren: true,
                stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }

        //  4. Configuration Validation
        private bool IsVisualConfigurationValid()
        {
            if (this.Profile == null) return false;

            Transform ownerTransform = this.KeyItemInteractableController.transform;

            if (this.VisualRoot != null)
            {
                if (this.VisualRoot.transform == ownerTransform || !this.VisualRoot.transform.IsChildOf(ownerTransform))
                {
                    return this.ReportVisualConfigurationError("Visual Root must be a separate child GameObject.");
                }
            }

            if (this.VisualAnimator != null)
            {
                if (this.VisualRoot == null || (this.VisualAnimator.transform != this.VisualRoot.transform && !this.VisualAnimator.transform.IsChildOf(this.VisualRoot.transform)))
                {
                    return this.ReportVisualConfigurationError("Visual Animator must belong to the Visual Root.");
                }
            }

            if (this.SuccessEffect != null)
            {
                Transform effectTransform = this.SuccessEffect.transform;

                if (!effectTransform.IsChildOf(ownerTransform) || effectTransform == ownerTransform)
                {
                    return this.ReportVisualConfigurationError("Success Effect must be a child of the World Object.");
                }

                if (this.VisualRoot != null && (effectTransform == this.VisualRoot.transform || effectTransform.IsChildOf(this.VisualRoot.transform)))
                {
                    return this.ReportVisualConfigurationError("Success Effect must exist outside the Visual Root.");
                }
            }

            switch (this.Profile.CompletionVisualType)
            {
                case KeyItemInteractableCompletionVisualType.Preserve:
                    return true;

                case KeyItemInteractableCompletionVisualType.Hide:
                    if (this.VisualRoot == null)
                    {
                        return this.ReportVisualConfigurationError("Hide mode requires a Visual Root.");
                    }

                    return true;

                case KeyItemInteractableCompletionVisualType.ReplaceSprite:
                    if (this.VisualRoot == null || this.SpriteRenderer == null || this.Profile.CompletedSprite == null)
                    {
                        return this.ReportVisualConfigurationError("ReplaceSprite requires a Visual Root, SpriteRenderer and Completed Sprite.");
                    }

                    if (this.SpriteRenderer.transform != this.VisualRoot.transform && !this.SpriteRenderer.transform.IsChildOf(this.VisualRoot.transform))
                    {
                        return this.ReportVisualConfigurationError("SpriteRenderer must belong to the Visual Root.");
                    }

                    return true;

                default:

                    return this.ReportVisualConfigurationError("Invalid Completion Visual Type.");
            }
        }
        private bool ReportVisualConfigurationError(string message)
        {
            Debug.LogError($"[{this.gameObject.name}.{this.GetType().Name}] {message}", this);

            return false;
        }

        // --- FOTLSubBehaviour Overrides ---
        protected override void OnUnbinding(KeyItemInteractableController brainBehaviour)
        {
            this.StopSuccessEffect();

            _isInitialized = false;
            _hasAppliedVisualState = false;
            _lastAppliedCompletedState = false;

            _spriteRenderer = null;
            _initialSprite = null;

            base.OnUnbinding(brainBehaviour);
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.VisualRoot == null)
            {
                Transform visual = this.transform.Find("Visual");
                if (visual != null) _visualRoot = visual.gameObject;
            }

            if (this.VisualAnimator == null && this.VisualRoot != null) _visualAnimator = this.VisualRoot.GetComponentInChildren<Animator>(true);
        }
    }
}