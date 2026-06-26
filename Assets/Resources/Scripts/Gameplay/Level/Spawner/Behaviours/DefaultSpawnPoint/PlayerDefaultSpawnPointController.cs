using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerDefaultSpawnPointController : CharacterDefaultSpawnPointController
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private PlayerSpawnerController _playerSpawnerController;
        private PlayerSpawnerController PlayerSpawnerController => _playerSpawnerController;

        //  2. Activation Settings
        [Header("Activation Settings")]
        [SerializeField] private bool _activateOnPlayerEnter = true;
        private bool ActivateOnPlayerEnter => _activateOnPlayerEnter;

        // --- Unity Callbacks ---
        protected override void Reset()
        {
            base.Reset();
            this.ConfigureColliderAsTrigger();
            this.CacheInternalReferences();
        }

        //protected override void OnValidate()
        //{
        //    base.OnValidate();
        //    this.ConfigureColliderAsTrigger();
        //    this.CacheReferences();
        //}

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!this.ActivateOnPlayerEnter) return;

            PlayerRootController playerRootController = other.GetComponentInParent<PlayerRootController>();
            if (playerRootController == null) return;

            if (this.PlayerSpawnerController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(PlayerDefaultSpawnPointController)}] No PlayerSpawnerController assigned.", this);
                return;
            }

            this.PlayerSpawnerController.ActivateSpawnPoint(this);
        }

        // --- Helpers ---
        private void ConfigureColliderAsTrigger()
        {
            Collider2D triggerCollider = this.GetComponent<Collider2D>();

            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.PlayerSpawnerController != null) return;

            _playerSpawnerController = Object.FindFirstObjectByType<PlayerSpawnerController>();
        }

        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.PlayerSpawnerController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(PlayerDefaultSpawnPointController)}] Missing reference to {nameof(PlayerSpawnerController)}.", this);
            }
        }
    }
}
