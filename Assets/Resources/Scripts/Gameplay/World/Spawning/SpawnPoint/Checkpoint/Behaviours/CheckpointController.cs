using System;
using UnityEngine;

namespace FlameOfTheLost
{
    public sealed class CheckpointController : CharacterSpawnPointController,
        IPlayerInteractable,
        IPlayerInteractionPromptSource
    {
        // --- Data / Configs ---
        //  1. Profile
        public CheckpointProfile CheckpointProfile => this.Profile as CheckpointProfile;

        //  2. Interaction
        public int InteractionPromptPriority => this.CheckpointProfile != null ? this.CheckpointProfile.InteractionPromptPriority : 0;

        // --- References ---
        //  1. Player Spawner
        [Header("References (Checkpoint) - Player Spawner")]
        [SerializeField] private PlayerSpawnerController _playerSpawnerController;
        private PlayerSpawnerController PlayerSpawnerController => _playerSpawnerController;

        //  2. UI
        [Header("References (Checkpoint) - UI")]
        [SerializeField] private CheckpointPanelController _checkpointPanelController;  
        private CheckpointPanelController CheckpointPanelController => _checkpointPanelController;

        // --- Events ---
        public event Action<CheckpointController, PlayerRootController> CheckpointInteracted;

        // --- Public APIs ---
        //  1. IPlayerInteractable
        public bool CanInteract(GameObject interactor)
        {
            return this.TryGetPlayer(interactor, out _);
        }
        public bool TryInteract(GameObject interactor)
        {
            if (!this.TryGetPlayer(interactor, out PlayerRootController player)) return false;

            if (this.PlayerSpawnerController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(CheckpointController)}] No {nameof(FlameOfTheLost.PlayerSpawnerController)} assigned.", this);
                return false;
            }

            if (this.CheckpointPanelController == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(CheckpointController)}] No {nameof(CheckpointPanelController)} assigned.", this);
                return false;
            }

            // Activate this Checkpoint as the Player's respawn point.
            this.PlayerSpawnerController.ActivateCheckpoint(this);

            // Open the Checkpoint Panel and enter the resting state.
            this.CheckpointPanelController.Open(this, player);

            // Notify listeners.
            this.CheckpointInteracted?.Invoke(this, player);

            return true;
        }

        //  2. IPlayerInteractionPromptSource
        public bool CanShowInteractionPrompt(GameObject interactor)
        {
            return this.CanInteract(interactor);
        }
        public string GetInteractionPromptText(GameObject interactor)
        {
            if (!this.CanShowInteractionPrompt(interactor)) return string.Empty;

            if (this.CheckpointProfile == null) return "Rest";

            return string.IsNullOrWhiteSpace(this.CheckpointProfile.InteractionPromptText)
                ? "Rest"
                : this.CheckpointProfile.InteractionPromptText;
        }

        // --- Helpers ---
        //  1. Player Resolution
        private bool TryGetPlayer(GameObject interactor, out PlayerRootController player)
        {
            player = null;

            if (interactor == null) return false;

            player = interactor.GetComponentInParent<PlayerRootController>();

            return player != null;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.Profile != null && this.CheckpointProfile == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CheckpointController)}] requires its Profile to be a {nameof(CheckpointProfile)}.", this);
            }
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.PlayerSpawnerController == null) _playerSpawnerController = UnityEngine.Object.FindFirstObjectByType<PlayerSpawnerController>();

            if (this.CheckpointPanelController == null) _checkpointPanelController = UnityEngine.Object.FindFirstObjectByType<CheckpointPanelController>(FindObjectsInactive.Include);
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.PlayerSpawnerController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CheckpointController)}] Missing reference to {nameof(PlayerSpawnerController)}.", this);
            }

            if (this.CheckpointPanelController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(CheckpointController)}] Missing reference to {nameof(CheckpointPanelController)}.", this);
            }
        }
    }
}