using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterDefaultSpawnPointController : FOTLMonoBehaviour, IIdentityProvider
    {
        // --- States ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id = 1;
        [SerializeField] private string _key = "MAIN_ENTRANCE";
        [SerializeField] private string _displayName = "Main Entrance";
        [SerializeField, TextArea(5, 5)] private string _description = "Spawn point for the main entrance of the level.";
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Anchor
        [Header("Anchor")]
        [SerializeField] private Transform _spawnAnchor;
        protected Transform SpawnAnchor => _spawnAnchor;

        public Vector3 SpawnPosition
        {
            get
            {
                return this.SpawnAnchor != null ? this.SpawnAnchor.position : this.transform.position;
            }
        }
        public Quaternion SpawnRotation
        {
            get
            {
                return this.SpawnAnchor != null ? this.SpawnAnchor.rotation : this.transform.rotation;
            }
        }

        //  3. Spawn Settings
        [Header("Spawn Settings")]
        [SerializeField] private CharacterFacingStatus _defaultFacing = CharacterFacingStatus.Down;
        public CharacterFacingStatus DefaultFacing => _defaultFacing;

        // --- Unity Callbacks ---
        protected virtual void Reset()
        {
            this.CacheInternalReferences();
        }

        //protected virtual void OnValidate()
        //{
        //    this.CacheReferences();
        //}

        // --- Helpers ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.SpawnAnchor != null) return;

            Transform anchor = this.transform.Find("SpawnAnchor");

            if (anchor != null)
            {
                _spawnAnchor = anchor;
            }
        }

        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.SpawnAnchor == null)
            {
                Debug.LogWarning($"No spawn anchor found for {this.name}. Defaulting to self transform.");
            }
        }
    }
}
