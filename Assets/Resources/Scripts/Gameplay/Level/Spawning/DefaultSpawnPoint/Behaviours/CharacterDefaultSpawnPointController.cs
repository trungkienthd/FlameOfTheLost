using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterDefaultSpawnPointController : FOTLMonoBehaviour
    {
        // --- Data / Configs ---
        //  1. Profile
        [Header("Data / Configs")]
        [SerializeField] private CharacterDefaultSpawnPointProfile _profile;
        public CharacterDefaultSpawnPointProfile Profile => _profile;

        //  2. Identity
        public int Id => this.Profile != null ? this.Profile.Id : 0;

        //  3. Spawn Settings
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

        //  2. Visual
        [SerializeField] private SpriteRenderer _spriteRenderer;
        protected SpriteRenderer SpriteRenderer => _spriteRenderer;

        // --- Unity Callbacks ---
        protected virtual void Reset()
        {
            this.CacheInternalReferences();
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

            if (this.SpriteRenderer == null) _spriteRenderer = this.GetComponentInChildren<SpriteRenderer>(true);
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Profile == null)
            {
                Debug.LogError($"[{this.name}.{nameof(CharacterDefaultSpawnPointController)}] " + $"requires a {nameof(CharacterDefaultSpawnPointProfile)}.", this);
            }

            if (this.SpawnAnchor == null)
            {
                Debug.LogWarning($"[{this.name}.{nameof(CharacterDefaultSpawnPointController)}] " + "No spawn anchor found for {this.name}. Defaulting to self transform.");
            }

            if (this.SpriteRenderer == null)
            {
                Debug.LogWarning($"[{this.name}.{nameof(CharacterDefaultSpawnPointController)}] " + "No SpriteRenderer found under Visual.", this);
            }
        }

        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            if (this.SpriteRenderer == null) return;
            if (this.SpriteRenderer.sprite != null) return;
            if (this.Profile.Icon == null) return;

            this.SpriteRenderer.sprite = this.Profile.Icon;
        }
    }
}
