using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Collider2D))]
    public class RespawnTriggerController : FOTLMonoBehaviour, IIdentityProvider
    {
        // --- States ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id = 1;
        [SerializeField] private string _key = "RESPAWN_TRIGGER";
        [SerializeField] private string _displayName = "Respawn Trigger";
        [SerializeField, TextArea(5, 5)] private string _description = "A trigger that causes characters to respawn when entered.";
        public int Id  => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Trigger Settings
        [Header("Trigger Settings")]
        [SerializeField] private bool _triggerOnceUntilExit = true;
        private bool TriggerOnceUntilExit => _triggerOnceUntilExit;

        private readonly HashSet<CharacterRespawnHandler> _triggeredCharacters = new();

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
            CharacterRespawnHandler characterRespawnHandler = other.GetComponentInParent<CharacterRespawnHandler>();
            if (characterRespawnHandler == null) return;

            if (this.TriggerOnceUntilExit && _triggeredCharacters.Contains(characterRespawnHandler)) return;

            _triggeredCharacters.Add(characterRespawnHandler);

            characterRespawnHandler.Respawn();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            CharacterRespawnHandler characterRespawnHandler = other.GetComponentInParent<CharacterRespawnHandler>();
            if (characterRespawnHandler == null) return;

            _triggeredCharacters.Remove(characterRespawnHandler);
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
