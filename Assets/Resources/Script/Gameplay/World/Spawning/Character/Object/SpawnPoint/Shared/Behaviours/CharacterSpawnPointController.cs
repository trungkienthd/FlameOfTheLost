using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterSpawnPointController : FOTLWorldObjectProfileBehaviour<CharacterSpawnPointProfile>
    {
        // --- Data / Configs ---
        //  1. Spawn Settings
        public CharacterFacingStatus DefaultFacing => this.Profile != null
            ? this.Profile.DefaultFacing
            : CharacterFacingStatus.Down;

        // --- References ---
        //  1. Spawn Anchor
        [Header("References")]
        [SerializeField] private Transform _spawnAnchor;
        protected Transform SpawnAnchor => _spawnAnchor;
        public Vector3 SpawnPosition => this.SpawnAnchor != null
            ? this.SpawnAnchor.position
            : this.transform.position;
        public Quaternion SpawnRotation => this.SpawnAnchor != null
            ? this.SpawnAnchor.rotation
            : this.transform.rotation;

        // --- Runtime ---
        [Header("Runtime - Spawn Anchor")]
        [SerializeField] private bool _isSpawnAnchorInitialized;
        public bool IsSpawnAnchorInitialized => _isSpawnAnchorInitialized;

        // --- Public APIs ---
        //  1. Spawn Anchor Initialization
        public void EnsureSpawnAnchorInitialized()
        {
            if (this.IsSpawnAnchorInitialized) return;

            this.ResolveSpawnAnchorReference();

            if (this.Profile == null) return;

            this.ConfigureSpawnAnchorTransform();

            _isSpawnAnchorInitialized = true;
        }


        // --- Helpers ---
        //  1. Spawn Anchor Reference
        private void ResolveSpawnAnchorReference()
        {
            if (this.SpawnAnchor != null) return;

            Transform spawnAnchor = this.transform.Find("SpawnAnchor");
            if (spawnAnchor != null) _spawnAnchor = spawnAnchor;
        }

        //  2. Spawn Anchor Configuration
        private void ConfigureSpawnAnchorTransform()
        {
            if (this.Profile == null) return;
            if (!this.Profile.ConfigureSpawnAnchorTransformAtStart) return;
            if (this.SpawnAnchor == null) return;

            this.SpawnAnchor.SetLocalPositionAndRotation
            (
                localPosition: this.Profile.InitialSpawnAnchorLocalPosition,
                localRotation: Quaternion.Euler(this.Profile.InitialSpawnAnchorLocalEulerAngles)
            );

            this.SpawnAnchor.localScale = this.Profile.InitialSpawnAnchorLocalScale;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            this.ResolveSpawnAnchorReference();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.SpawnAnchor == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(CharacterSpawnPointController)}] No SpawnAnchor found. Using root transform as the spawn transform.", this);
            }
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.EnsureSpawnAnchorInitialized();
        }
    }
}