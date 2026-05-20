using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Collider2D))]
    public class RespawnTriggerController : MonoBehaviour
    {
        // --- States ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private string _id = "001_MAIN_ENTRANCE";
        public string RespawnTriggerId
        {
            get => _id;
            private set => _id = value;
        }

        //  2. Trigger Settings
        [Header("Trigger Settings")]
        [SerializeField] private bool _triggerOnceUntilExit = true;
        private bool TriggerOnceUntilExit => _triggerOnceUntilExit;

        private readonly HashSet<PlayerRespawnHandler> _triggeredPlayers = new();

        // --- Unity Callbacks ---
        private void Reset()
        {
            this.ConfigureColliderAsTrigger();
        }

        private void OnValidate()
        {
            this.ConfigureColliderAsTrigger();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerRespawnHandler playerRespawnHandler = other.GetComponentInParent<PlayerRespawnHandler>();
            if (playerRespawnHandler == null) return;

            if (this.TriggerOnceUntilExit && _triggeredPlayers.Contains(playerRespawnHandler)) return;

            _triggeredPlayers.Add(playerRespawnHandler);

            playerRespawnHandler.Respawn();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            PlayerRespawnHandler playerRespawnHandler = other.GetComponentInParent<PlayerRespawnHandler>();
            if (playerRespawnHandler == null) return;

            _triggeredPlayers.Remove(playerRespawnHandler);
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
