using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerDefaultSpawnPointController : CharacterDefaultSpawnPointController
    {
        // --- Data / Configs ---
        //  1. Profile
        public PlayerDefaultSpawnPointProfile PlayerProfile => this.Profile as PlayerDefaultSpawnPointProfile;

        //  2. Activation Settings
        private bool ActivateOnPlayerEnter => this.PlayerProfile != null && this.PlayerProfile.ActivateOnPlayerEnter;

        // --- References --- 
        [Header("References (Player-related)")]
        [SerializeField] private PlayerSpawnerController _playerSpawnerController;
        private PlayerSpawnerController PlayerSpawnerController => _playerSpawnerController;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!this.ActivateOnPlayerEnter) return;

            PlayerRootController playerRootController = other.GetComponentInParent<PlayerRootController>();
            if (playerRootController == null) return;

            if (this.PlayerSpawnerController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerDefaultSpawnPointController)}] No {nameof(this.PlayerSpawnerController)} assigned.", this);
                return;
            }

            this.PlayerSpawnerController.ActivateSpawnPoint(this);
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PlayerSpawnerController == null) _playerSpawnerController = UnityEngine.Object.FindFirstObjectByType<PlayerSpawnerController>();
        }

        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Profile != null && this.PlayerProfile == null)
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}.{nameof(PlayerDefaultSpawnPointController)}] " +
                    $"requires its Profile to be a {nameof(PlayerDefaultSpawnPointProfile)}.",
                    this
                );
            }

            if (this.PlayerSpawnerController == null)
            {
                Debug.LogError
                (
                    $"[{this.gameObject.name}.{nameof(PlayerDefaultSpawnPointController)}] " +
                    $"Missing reference to {nameof(PlayerSpawnerController)}.",
                    this
                );
            }
        }
    }
}
