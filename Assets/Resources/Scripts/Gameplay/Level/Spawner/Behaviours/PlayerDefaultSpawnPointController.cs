using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerDefaultSpawnPointController : CharacterDefaultSpawnPointController
    {
        // --- States ---
        //  1. Activation Settings
        [Header("Activation Settings")]
        [SerializeField] private bool _activateOnPlayerEnter = true;
        private bool ActivateOnPlayerEnter => _activateOnPlayerEnter;

        // --- Unity Callbacks ---
        protected override void Reset()
        {
            base.Reset();
            this.ConfigureColliderAsTrigger();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
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
