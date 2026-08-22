using UnityEngine;

namespace FlameOfTheLost
{
    public class FOTLWorldObjectProfileBehaviour<TProfile> : FOTLMonoBehaviour, IIdentityProvider, IIconProvider
        where TProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        [Header("Data / Configs")]
        //  0. Profile
        [SerializeField] private TProfile _profile;
        public TProfile Profile => _profile;

        //  1. Identity & Icon
        public int Id => this.Profile != null
            ? this.Profile.Id
            : 0;

        public string Key => this.Profile != null
            ? this.Profile.Key
            : string.Empty;

        public string DisplayName => this.Profile != null
            ? this.Profile.DisplayName
            : string.Empty;

        public string Description => this.Profile != null
            ? this.Profile.Description
            : string.Empty;

        public Sprite Icon => this.Profile != null
            ? this.Profile.Icon
            : null;

        // --- References ---
        // [Header($"References (Base - {nameof(FOTLWorldObjectProfile)})")]
        [Header("References (Base - FOTLWorldObjectProfile)")]
        [SerializeField] private Transform _visualTransform;
        [SerializeField] private SpriteRenderer _visualSpriteRenderer;
        private Transform VisualTransform => _visualTransform;
        private SpriteRenderer VisualSpriteRenderer => _visualSpriteRenderer;

        // --- Helpers ---
        //  1. Configure Initial Transform(s)
        private void ConfigureInitialTransform()
        {
            if (this.Profile == null) return;
            if (!this.Profile.ConfigureInitialTransformAtStart) return;

            this.ConfigureInitialRootTransform();
            this.ConfigureInitialVisualTransform();
        }
        private void ConfigureInitialRootTransform()
        {
            this.transform.SetPositionAndRotation
            (
                position: this.Profile.InitialPosition,
                rotation: Quaternion.Euler(this.Profile.InitialEulerAngles)
            );

            this.transform.localScale = this.Profile.InitialLocalScale;
        }
        private void ConfigureInitialVisualTransform()
        {
            if (this.VisualTransform == null) return;

            this.VisualTransform.SetLocalPositionAndRotation
            (
                localPosition: this.Profile.InitialVisualLocalPosition,
                localRotation: Quaternion.Euler(this.Profile.InitialVisualLocalEulerAngles)
            );

            this.VisualTransform.localScale = this.Profile.InitialVisualLocalScale;
        }

        //  2. Configure Initial Visual
        private void ConfigureInitialVisual()
        {
            if (this.Profile == null) return;
            if (!this.Profile.ConfigureIconAtStart) return;
            if (this.VisualSpriteRenderer == null) return;
            if (this.VisualSpriteRenderer.sprite != null) return;
            if (this.Icon == null) return;

            this.VisualSpriteRenderer.sprite = this.Icon;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.VisualTransform == null) _visualTransform = this.transform.Find("Visual");
            if (this.VisualSpriteRenderer == null && this.VisualTransform != null)
            {
                _visualSpriteRenderer = this.VisualTransform.GetComponentInChildren<SpriteRenderer>(true);
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Profile == null)
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}.{this.GetType().Name}] " +
                    $"{typeof(TProfile).Name} reference is not assigned.",
                    this
                );
            }
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.ConfigureInitialTransform();
            this.ConfigureInitialVisual();
        }
    }
}
