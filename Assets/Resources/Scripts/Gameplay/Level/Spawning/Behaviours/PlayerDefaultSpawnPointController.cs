using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerDefaultSpawnPointController : MonoBehaviour
    {
        // --- States ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private string _id = "001_MAIN_ENTRANCE";
        public string SpawnPointId
        {
            get => _id;
            private set => _id = value;
        }

        //  2. Anchor
        [Header("Anchor")]
        [SerializeField] private Transform _spawnAnchor;
        private Transform SpawnAnchor => _spawnAnchor;

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

        //  4. Activation Settings
        [Header("Activation Settings")]
        [SerializeField] private bool _activateOnPlayerEnter = true;
        private bool ActivateOnPlayerEnter => _activateOnPlayerEnter;

        // --- Unity Callbacks ---
        private void Reset()
        {
            this.CacheReferences();
            this.ConfigureColliderAsTrigger();
        }

        private void OnValidate()
        {
            this.CacheReferences();
            this.ConfigureColliderAsTrigger();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!this.ActivateOnPlayerEnter) return;

            PlayerRespawnHandler playerRespawnHandler = other.GetComponentInParent<PlayerRespawnHandler>();
            if (playerRespawnHandler == null) return;

            playerRespawnHandler.RegisterVisitedSpawnPoint(this);
        }

        // --- Helpers ---
        private void CacheReferences()
        {
            if (this.SpawnAnchor != null) return;

            Transform anchor = this.transform.Find("SpawnAnchor");

            if (anchor != null)
            {
                _spawnAnchor = anchor;
            }
        }

        private void ConfigureColliderAsTrigger()
        {
            Collider2D triggerCollider = this.GetComponent<Collider2D>();

            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }
        }
    }
}
