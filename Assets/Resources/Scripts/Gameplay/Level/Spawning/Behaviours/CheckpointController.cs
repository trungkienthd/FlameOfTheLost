using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class CheckpointController : MonoBehaviour
    {
        // --- States ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private string _id = "101_LOST_FLAME";
        public string CheckpointId => _id;

        //  2. Anchor
        [Header("Anchor")]
        [SerializeField] private Transform _spawnAnchor;
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
        [SerializeField] private CharacterFacingStatusType _defaultFacing = CharacterFacingStatusType.Down;
        public Transform SpawnAnchor => _spawnAnchor;
        public CharacterFacingStatusType DefaultFacing => _defaultFacing;

        [Header("State")]
        [SerializeField] private bool _isActivated = false;
        public bool IsActivated
        {
            get => _isActivated;
            private set => _isActivated = value;
        }

        // --- Unity Callbacks ---
        private void Reset()
        {
            this.CacheReferences();
        }

        private void OnValidate()
        {
            this.CacheReferences();
        }

        // --- Public APIs ---
        public void Activate()
        {
            if (this.IsActivated) return;

            this.IsActivated = true;

            // Later:
            // - play activation VFX
            // - change flame sprite/animation
            // - play sound
            // - show UI message
        }

        public void Deactivate()
        {
            this.IsActivated = false;

            // Later:
            // - update visuals
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
    }
}