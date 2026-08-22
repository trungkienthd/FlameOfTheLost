using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterDefaultSpawnPointController : FOTLWorldObjectProfileBehaviour<CharacterDefaultSpawnPointProfile>
    {
        // --- Data / Configs ---
        //  1. Spawn Settings
        public CharacterFacingStatus DefaultFacing => this.Profile != null
            ? this.Profile.DefaultFacing
            : CharacterFacingStatus.Down;

        // --- References ---
        //  1. Anchor
        [Header("References")]
        [SerializeField] private Transform _spawnAnchor;
        protected Transform SpawnAnchor => _spawnAnchor;

        public Vector3 SpawnPosition => this.SpawnAnchor != null 
            ? this.SpawnAnchor.position 
            : this.transform.position;
        public Quaternion SpawnRotation => this.SpawnAnchor != null 
            ? this.SpawnAnchor.rotation 
            : this.transform.rotation;

        // --- Helpers ---
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

            if (this.SpawnAnchor == null)
            {
                Transform anchor = this.transform.Find("SpawnAnchor");
                if (anchor != null) _spawnAnchor = anchor;
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.SpawnAnchor == null)
            {
                Debug.LogWarning($"[{this.name}.{nameof(CharacterDefaultSpawnPointController)}] " + $"No spawn anchor found for {this.name}. Defaulting to self transform.");
            }
        }

        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.ConfigureSpawnAnchorTransform();
        }
    }
}
