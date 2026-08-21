using UnityEngine;

namespace FlameOfTheLost
{
    public class FOTLWorldObjectProfileBehaviour<TProfile> : FOTLMonoBehaviour, IIdentityProvider, IIconProvider
        where TProfile : FOTLWorldObjectProfile
    {
        // --- Data / Configs ---
        [Header("Data / Configs")]
        [SerializeField] private TProfile _profile;
        public TProfile Profile => _profile;

        // --- Identity & Icon ---
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

        // --- Helpers ---
        private void ConfigureInitialTransform()
        {
            if (this.Profile == null) return;
            if (!this.Profile.ConfigureInitialTransformAtStart) return;

            this.transform.SetPositionAndRotation
            (
                position: this.Profile.InitialPosition,
                rotation: Quaternion.Euler(this.Profile.InitialEulerAngles)
            );

            this.transform.localScale = this.Profile.InitialLocalScale;
        }

        // --- FOTLMonoBehaviour Overrides ---
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
        }
    }
}
